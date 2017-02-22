# Engine中如何往已有要素类中插入数据
您在Engine程序开发过程中是否遇到过要将新获取的数据向已有要素类中插入？对于数据插入的几种方法您是否清楚？各种方法的效率您对比过吗？今天我们讨论的话题为Engine中如何往已有要素类中插入数据，上述问题的答案会为您一一揭晓。


##一、往已有要素类中插入数据的几种方式及其优缺点：

1，**IFeatureClass.CreateFeature配合IFeature.Store使用**。该方法在调用Store时会触发所有与Feature相关的行为，不但会触发IObjectClassEvents，还会触发涉及网络、注记要素，以及参与拓扑的要素等特殊的行为（比如向参加拓扑的要素类中添加要素后会自动创建dirty area）。

该方法在调用CreateFeature方法时，创建要素的OID，执行Store时将要素存入数据库中。

***优点：***代码简单；
***缺点：***效率低，如果仅仅插入几个要素或者进行测试而不考虑性能的话可以考虑。

2，**IFeatureClass.CreateFeatureBuffer配合insert cursor使用**。不会触发事件，该方法常常用来一次性插入大量要素，比第一种方法效率高很多。此外，IFeatureClass.Insert(bool useBuffering)方法中参数useBuffering建议总将其设为true，这样数据会先缓冲在客户端，然后执行Flush时批量写入，以提升效率。

该方法创建的IFeatureBuffer可以进行多次赋值，在执行InsertFeature时，创建要素的OID，但是此时数据并没有写入到数据库中，只有在执行IFeatureCursor.Flush时数据才会真正写入到数据库中。注意Flush方法是需要手动调用的，如果不执行，在程序释放时自动执行，但这样会无法检测错误，比如要素类空间不够了就会报错。

***优点：***效率高，尤其是插入大量数据时。如果仅就性能而言，除非插入一个要素，其余情况都推荐使用该方法；
***缺点：***代码相比第一种，稍显复杂，注意需要释放ICursor和IFeatureBuffer对象。

这里提一下，该方法中涉及到cursor的释放，有两种方法释放游标：
a，	直接使用ComReleaser.ReleaseCOMObject(cursor); 或者Marshal.FinalReleaseComObject(cursor);

b，	使用using(){  }方式，如下：

```
 using(ComReleaser comReleaser = new ComReleaser())
    {
        // Create a feature buffer.
        IFeatureBuffer featureBuffer = featureClass.CreateFeatureBuffer();
        comReleaser.ManageLifetime(featureBuffer);

        // Create an insert cursor.
        IFeatureCursor insertCursor = featureClass.Insert(true);
        comReleaser.ManageLifetime(insertCursor);

      …
    }

```

执行完using后会自动释放insertCursor和featureBuffer，可以避免程序比较复杂时不确定在哪释放或者忘记释放的情况，是比较优秀的编程方式，推荐使用。

3，**使用Load-Only模式插入数据（SDE以及FileGDB都支持）**。这种方法是对第二种方法的升级，主要用来在插入数据量特别大的情况下提高性能。Load-Only模式仅能用来插入新的要素，而不能用于编辑已有要素。原理是开启Load-Only模式后，插入数据时会停止更新空间索引和属性索引，解除Load-Only后才重建索引（*重建索引是重建该要素类所有要素的索引，如果已有要素类中含有大量要素，而插入数据量不太大时，重建索引可能会影响性能*）。开启Load-Only模式时，其它程序不能访问该数据，可以在获取排它锁之后设置Load-Only。如下：

```
// Cast the feature class to the IFeatureClassLoad interface.
    IFeatureClassLoad featureClassLoad = (IFeatureClassLoad)featureClass;

    // Acquire an exclusive schema lock for the class.
    ISchemaLock schemaLock = (ISchemaLock)featureClass;
    try
    {
        schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);

        // Enable load-only mode on the feature class.
        featureClassLoad.LoadOnlyMode = true;
        using(ComReleaser comReleaser = new ComReleaser())
        {
            // Create the feature buffer.
            IFeatureBuffer featureBuffer = featureClass.CreateFeatureBuffer();
            comReleaser.ManageLifetime(featureBuffer);

            // Create an insert cursor.
            IFeatureCursor insertCursor = featureClass.Insert(true);
            comReleaser.ManageLifetime(insertCursor);

            …
        }
    }
    catch (Exception)
    {
        // Handle the failure in a way appropriate to the application.
    }
    finally
    {
        // Disable load-only mode on the feature class.
        featureClassLoad.LoadOnlyMode = false;

        // Demote the exclusive schema lock to a shared lock.
        schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
}
```

插入数据常用的就是以上三种方式，下面两种方式也可以插入数据，但是不常用，简要介绍一下。

4，**IObjectLoader.LoadObjects方法。**该方法***只能在Desktop产品下***使用，也就是Engine许可无法使用，相比之前的方法，该方法代码简单，对于属性值的复制也更为简单，如下：

```
IObjectLoader objectLoader = new ObjectLoaderClass();
            IFeatureClass sourceFeatureClass = getFeatureClass("esriDataSourcesGDB.FileGDBWorkspaceFactory", fileGDBPath, sourceFCName);
            IFeatureClass targetFeatureClass = getFeatureClass("esriDataSourcesGDB.SdeWorkspaceFactory", sdePath, targetFCName);
             
            string sInFieldList = targetFeatureClass.Fields.get_Field(0).Name;
            for(int i=1;i< targetFeatureClass.Fields.FieldCount;i++)
                sInFieldList = sInFieldList + "," + targetFeatureClass.Fields.get_Field(i).Name;
            
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = queryClause;
            queryFilter.SubFields = sInFieldList;
            
            IEnumInvalidObject invalidObject;
            objectLoader.LoadObjects(null, sourceFeatureClass as ITable, queryFilter, targetFeatureClass as ITable, targetFeatureClass.Fields, false, 0, false, false, 1000, out invalidObject);
```

***优点：***代码简单，属性值复制也更为简单；
***缺点：***只能在Desktop许可下使用。

5，**IFeatureClass.CreateFeature配合IFeatureClassWrite.WriteFeature/WriteFeatures使用**。IFeatureClassWrite是low-level的接口，不会触发任何事件。只是使用WriteFeature/WriteFeatures替换IFeature.Store将要素写入数据库，用的人很少。

以上就是插入数据的几种方式，您是否都清楚了？接下来，在程序的编写过程中有哪些需要注意的呢？

a，	以下数据类型需要EngineGeoDB许可或者Desktop产品的Standard或者Advanced许可：涉及几何网络、拓扑、Dimension要素类、注记类、ArcSDE数据库的。

b，	以下情况需要开启编辑会话：涉及拓扑、几何网络、terrain、representation以及ArcSDE中的版本数据的。

c，	在给要素赋予Shape时，有必要执行一下**ITopologicalOperator.Simplify**来验证几何，尤其是往SDE中插入时，有几何错误会导致失败。

d，	插入数据可能导致要素类的Extent发生变化，插入完后建议调用**IFeatureClassManage. UpdateExtent**更新范围（*需要获取要素类的排他锁，也就是不能有别的用户或程序访问该要素类*），不然有可能执行缩放到图层或全图时数据显示不全。

e，	Geometry的空间参考需要与目标要素类的空间参考相一致，否则会进行动态投影而影响性能。

f，	如果源要素类中有Z值或M值，注意赋Shape时需要使用IZAware或者IMAware进行设置。


##二、效率对比

由于经常碰到用户需要将本地数据库的数据往SDE中导入，所以本文以将FileGDB中的点要素类插入到ArcSDE中已有的字段结构相同的要素类中为例进行测试。

代码可参考：[ArcObjects帮助文档](http://resources.arcgis.com/en/help/arcobjects-net/conceptualhelp/#/Creating_features/00010000049v000000/)

**测试结果：**

1，	将100个点要素从FileGDB中插入SDE要素类中使用五种方法分别用时：2.25秒、0.19秒、0.21秒、0.23秒、2.40秒。

2，	将10000个点要素从FileGDB中插入SDE要素类中使用五种方法分别用时：185秒、14.5秒、11.3秒、13.2秒、204秒。

3，	将100000个点要素从FileGDB中插入SDE要素类中使用五种方法分别用时：1345秒、163秒、127秒、146秒、1409秒。

4，	文中提到使用游标方式插入数据，如果将IFeatureClass.Insert(bool useBuffering)方法中参数useBuffering设为true会提高效率，现在就来测试一下。将100000个点要素插入SDE中，设为false所用时间为：164秒，设为true所用时间为：122秒。

5，	开启编辑会话会不会提高效率呢？将100000个点要素插入SDE中，设为false所用时间为：161秒，设为true所用时间为：117秒。相比不开启编辑会话，几乎没有差别。

   
**从上述测试结果可以看出：**

a，	无论是插入少量数据还是插入大量数据，Insert方式都比CreateFeature方式快；

b，	如果仅插入少量数据，使用LoadOnly模式反而会慢（文中插入100个点），如果插入大量要素使用LoadOnly会快（文中插入1万和10万个点），但是如果该要素类中已有大量要素，而仅插入不是太多数据的话重建索引可能会影响性能；

c，	IFeatureClassWrite.WriteFeature方法比IFeature.Store还要慢，这大概也是该方法用的很少的原因之一吧；

d，	IObjectLoader.LoadObjects方法比Insert稍快，但是比LoadOnly要慢些（文中插入1万和10万个点）。由于测试数据中仅含有两个自定义字段，如果字段个数较多的话，我觉得这种方法会更快；

e，	IFeatureClass.Insert(bool useBuffering)方法中参数useBuffering设为true确实可以提高效率；

f，	开启编辑会话与不开启效率几乎一样，当然如果涉及到拓扑，几何网络等复杂情况时必须开启编辑会话。


##Demo

使用ArcGIS Engine 10.4，Visual Studio 2013编写，汇总了往已有要素类中插入数据的几种方法，右侧的comboBox里可以选择查询条件。读者们可以自行选择合适的方法，界面为：
![程序界面](http://img.blog.csdn.net/20170222153956026?watermark/2/text/aHR0cDovL2Jsb2cuY3Nkbi5uZXQveGlueWluZzE4MA==/font/5a6L5L2T/fontsize/400/fill/I0JBQkFCMA==/dissolve/70/gravity/SouthEast)


###工程下载地址：

[InsertingFeatures](https://github.com/xinying180/InsertingFeatures)
