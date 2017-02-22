using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace insertingFeatures
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string sdePath = @"C:\Users\Xinying\AppData\Roaming\ESRI\Desktop10.4\ArcCatalog\Connection to 192.168.220.132 (3).sde";
        string fileGDBPath = Application.StartupPath + "\\test.gdb";
        string sourceFCName = "testPoint_10w";
        string targetFCName = "testPoint_10w_sde";
        string queryClause = "OBJECTID <101";
        private void createFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch myWatch = Stopwatch.StartNew();

            IFeatureClass sourceFeatureClass = getFeatureClass("esriDataSourcesGDB.FileGDBWorkspaceFactory", fileGDBPath, sourceFCName);
            IFeatureClass targetFeatureClass = getFeatureClass("esriDataSourcesGDB.SdeWorkspaceFactory", sdePath, targetFCName);
          
            createFeature(sourceFeatureClass, targetFeatureClass);

            myWatch.Stop();
            string time = myWatch.Elapsed.TotalSeconds.ToString();
            MessageBox.Show(time + " Seconds");   
           
        }

        public void createFeature(IFeatureClass sourceFeatureClass, IFeatureClass targetFeatureClass)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = queryClause;
            IFeatureCursor cursor = sourceFeatureClass.Search(queryFilter, true);
            IFeature sourceFeature = cursor.NextFeature();
            while (sourceFeature != null)
            {
                IFeature targetFeature = targetFeatureClass.CreateFeature();

                //如果是线或面要素类需要执行下Simplify，这里用的点要素类，不做验证了
                targetFeature.Shape = sourceFeature.ShapeCopy;

                for (int i = 0; i < sourceFeature.Fields.FieldCount; i++)
                {
                    IField field = sourceFeature.Fields.get_Field(i);
                    if (field.Type != esriFieldType.esriFieldTypeOID && field.Type != esriFieldType.esriFieldTypeGeometry && field.Type != esriFieldType.esriFieldTypeGlobalID && field.Type != esriFieldType.esriFieldTypeGUID)
                    {
                        string fieldName = field.Name;
                        int index = targetFeature.Fields.FindField(fieldName);
                        if (index > -1 && fieldName != "Shape_Length" && fieldName != "Shape_Area")
                            targetFeature.set_Value(index, sourceFeature.get_Value(i));
                    }

                }
                targetFeature.Store();
                sourceFeature = cursor.NextFeature();

            }
            ComReleaser.ReleaseCOMObject(cursor);

            IFeatureClassManage targetFeatureClassManage = targetFeatureClass as IFeatureClassManage;
            targetFeatureClassManage.UpdateExtent();
        }
        private IFeatureClass getFeatureClass(string WorkspaceFactoryProgID, string GDBPath, string featureClassName)
        {
            IWorkspaceName workspaceName = new WorkspaceNameClass
            {
                WorkspaceFactoryProgID = WorkspaceFactoryProgID,
                PathName = GDBPath
            };

            IName workspaceIName = (IName)workspaceName;
            IWorkspace workspace = (IWorkspace)workspaceIName.Open();

            IFeatureClass featureClass = (workspace as IFeatureWorkspace).OpenFeatureClass(featureClassName);

            return featureClass; 
        }       

        private void insertFeatureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch myWatch = Stopwatch.StartNew();

            IFeatureClass sourceFeatureClass = getFeatureClass("esriDataSourcesGDB.FileGDBWorkspaceFactory", fileGDBPath, sourceFCName);
            IFeatureClass targetFeatureClass = getFeatureClass("esriDataSourcesGDB.SdeWorkspaceFactory", sdePath, targetFCName);
           
            InsertFeaturesUsingCursor(sourceFeatureClass, targetFeatureClass);

            myWatch.Stop();
            string time = myWatch.Elapsed.TotalSeconds.ToString();
            MessageBox.Show(time + " Seconds"); 
        }
        public void InsertFeaturesUsingCursor(IFeatureClass sourceFeatureClass, IFeatureClass targetFeatureClass)
        {
            using (ComReleaser comReleaser = new ComReleaser())
            {
                // Create a feature buffer.
                IFeatureBuffer featureBuffer = targetFeatureClass.CreateFeatureBuffer();
                comReleaser.ManageLifetime(featureBuffer);

                // Create an insert cursor.
                IFeatureCursor insertCursor = targetFeatureClass.Insert(true);
                comReleaser.ManageLifetime(insertCursor);

                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = queryClause;
                IFeatureCursor cursor = sourceFeatureClass.Search(queryFilter, true);

                IFeature sourceFeature = cursor.NextFeature();

                while (sourceFeature != null)
                {
                    //如果是线或面要素类需要执行下Simplify，这里用的点要素类，不做验证了
                    featureBuffer.Shape = sourceFeature.ShapeCopy;

                    for (int i = 0; i < sourceFeature.Fields.FieldCount; i++)
                    {
                        IField field = sourceFeature.Fields.get_Field(i);
                        if (field.Type != esriFieldType.esriFieldTypeOID && field.Type != esriFieldType.esriFieldTypeGeometry && field.Type != esriFieldType.esriFieldTypeGlobalID && field.Type != esriFieldType.esriFieldTypeGUID)
                        {
                            string fieldName = field.Name;
                            int index = featureBuffer.Fields.FindField(fieldName);
                            if (index > -1 && fieldName != "Shape_Length" && fieldName != "Shape_Area")
                                featureBuffer.set_Value(index, sourceFeature.get_Value(i));
                        }

                    }
                    insertCursor.InsertFeature(featureBuffer);
                    sourceFeature = cursor.NextFeature();
                }

                // Flush the buffer to the geodatabase.
                insertCursor.Flush();
                ComReleaser.ReleaseCOMObject(cursor);
                
            }
            IFeatureClassManage targetFeatureClassManage = targetFeatureClass as IFeatureClassManage;
            targetFeatureClassManage.UpdateExtent();
        }

        private void loadOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch myWatch = Stopwatch.StartNew();

            IFeatureClass sourceFeatureClass = getFeatureClass("esriDataSourcesGDB.FileGDBWorkspaceFactory", fileGDBPath, sourceFCName);
            IFeatureClass targetFeatureClass = getFeatureClass("esriDataSourcesGDB.SdeWorkspaceFactory", sdePath, targetFCName);
          
            LoadOnlyModeInsert(sourceFeatureClass, targetFeatureClass);

            myWatch.Stop();
            string time = myWatch.Elapsed.TotalSeconds.ToString();
            MessageBox.Show(time + " Seconds"); 
        }
        public void LoadOnlyModeInsert(IFeatureClass sourceFeatureClass, IFeatureClass targetFeatureClass)
        {
            // Cast the feature class to the IFeatureClassLoad interface.
            IFeatureClassLoad featureClassLoad = (IFeatureClassLoad)targetFeatureClass;

            // Acquire an exclusive schema lock for the class.
            ISchemaLock schemaLock = (ISchemaLock)targetFeatureClass;
            try
            {
                schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);

                // Enable load-only mode on the feature class.
                featureClassLoad.LoadOnlyMode = true;
                using (ComReleaser comReleaser = new ComReleaser())
                {
                    // Create the feature buffer.
                    IFeatureBuffer featureBuffer = targetFeatureClass.CreateFeatureBuffer();
                    comReleaser.ManageLifetime(featureBuffer);

                    // Create an insert cursor.
                    IFeatureCursor insertCursor = targetFeatureClass.Insert(true);
                    comReleaser.ManageLifetime(insertCursor);

                    IQueryFilter queryFilter = new QueryFilterClass();
                    queryFilter.WhereClause = queryClause;
                    IFeatureCursor cursor = sourceFeatureClass.Search(queryFilter, true);

                    IFeature sourceFeature = cursor.NextFeature();

                    while (sourceFeature != null)
                    {
                        //如果是线或面要素类需要执行下Simplify，这里用的点要素类，不做验证了
                        featureBuffer.Shape = sourceFeature.ShapeCopy;

                        for (int i = 0; i < sourceFeature.Fields.FieldCount; i++)
                        {
                            IField field = sourceFeature.Fields.get_Field(i);
                            if (field.Type != esriFieldType.esriFieldTypeOID && field.Type != esriFieldType.esriFieldTypeGeometry && field.Type != esriFieldType.esriFieldTypeGlobalID && field.Type != esriFieldType.esriFieldTypeGUID)
                            {
                                string fieldName = field.Name;
                                int index = featureBuffer.Fields.FindField(fieldName);
                                if (index > -1 && fieldName != "Shape_Length" && fieldName != "Shape_Area")
                                    featureBuffer.set_Value(index, sourceFeature.get_Value(i));
                            }

                        }
                        insertCursor.InsertFeature(featureBuffer);
                        sourceFeature = cursor.NextFeature();
                    }

                    // Flush the buffer to the geodatabase.
                    insertCursor.Flush();
                    ComReleaser.ReleaseCOMObject(cursor);
                    IFeatureClassManage targetFeatureClassManage = targetFeatureClass as IFeatureClassManage;
                    targetFeatureClassManage.UpdateExtent();
                }
            }
            catch (Exception)
            {
                // Handle the failure in a way appropriate to the application.
                MessageBox.Show("无法获取该要素类的排它锁，检查ArcMap是否打开了该要素类，建议关闭！");
            }
            finally
            {
                // Disable load-only mode on the feature class.
                featureClassLoad.LoadOnlyMode = false;

                // Demote the exclusive schema lock to a shared lock.
                schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
            }
            
        }

        private void objectLoaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch myWatch = Stopwatch.StartNew();

            IObjectLoader objectLoader = new ObjectLoaderClass();
            IFeatureClass sourceFeatureClass = getFeatureClass("esriDataSourcesGDB.FileGDBWorkspaceFactory", fileGDBPath, sourceFCName);
            IFeatureClass targetFeatureClass = getFeatureClass("esriDataSourcesGDB.SdeWorkspaceFactory", sdePath, targetFCName);
             
            string sInFieldList = targetFeatureClass.Fields.get_Field(0).Name;
            for (int i = 1; i < targetFeatureClass.Fields.FieldCount; i++)
                sInFieldList = sInFieldList + "," + targetFeatureClass.Fields.get_Field(i).Name;
            
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = queryClause;
            queryFilter.SubFields = sInFieldList;
            
            IEnumInvalidObject invalidObject;
            objectLoader.LoadObjects(null, sourceFeatureClass as ITable, queryFilter, targetFeatureClass as ITable, targetFeatureClass.Fields, false, 0, false, false, 1000, out invalidObject);
            IFeatureClassManage targetFeatureClassManage = targetFeatureClass as IFeatureClassManage;
            targetFeatureClassManage.UpdateExtent();
           
            myWatch.Stop();
            string time = myWatch.Elapsed.TotalSeconds.ToString();
            MessageBox.Show(time + " Seconds"); 
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {            
            queryClause = comboBox1.Text;
        }

        private void featureClassWriteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stopwatch myWatch = Stopwatch.StartNew();

            IFeatureClass sourceFeatureClass = getFeatureClass("esriDataSourcesGDB.FileGDBWorkspaceFactory", fileGDBPath, sourceFCName);
            IFeatureClass targetFeatureClass = getFeatureClass("esriDataSourcesGDB.SdeWorkspaceFactory", sdePath, targetFCName);
           
            featureClassWrite(sourceFeatureClass, targetFeatureClass);

            myWatch.Stop();
            string time = myWatch.Elapsed.TotalSeconds.ToString();
            MessageBox.Show(time + " Seconds"); 
        }
        public void featureClassWrite(IFeatureClass sourceFeatureClass, IFeatureClass targetFeatureClass)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = queryClause;
            IFeatureCursor cursor = sourceFeatureClass.Search(queryFilter, true);
            IFeature sourceFeature = cursor.NextFeature();
            IFeatureClassWrite featureClassWrite = targetFeatureClass as IFeatureClassWrite;
            ISet setAdd = new SetClass(); 
            while (sourceFeature != null)
            {
                IFeature targetFeature = targetFeatureClass.CreateFeature();

                //如果是线或面要素类需要执行下Simplify，这里用的点要素类，不做验证了
                targetFeature.Shape = sourceFeature.ShapeCopy;

                for (int i = 0; i < sourceFeature.Fields.FieldCount; i++)
                {
                    IField field = sourceFeature.Fields.get_Field(i);
                    if (field.Type != esriFieldType.esriFieldTypeOID && field.Type != esriFieldType.esriFieldTypeGeometry && field.Type != esriFieldType.esriFieldTypeGlobalID && field.Type != esriFieldType.esriFieldTypeGUID)
                    {
                        string fieldName = field.Name;
                        int index = targetFeature.Fields.FindField(fieldName);
                        if (index > -1 && fieldName != "Shape_Length" && fieldName != "Shape_Area")
                            targetFeature.set_Value(index, sourceFeature.get_Value(i));
                    }

                }
                //setAdd.Add(targetFeature);
                featureClassWrite.WriteFeature(targetFeature);
                sourceFeature = cursor.NextFeature();

            }
            //featureClassWrite.WriteFeatures(setAdd);//与WriteFeature没啥效率上的区别
            ComReleaser.ReleaseCOMObject(cursor);

            IFeatureClassManage targetFeatureClassManage = targetFeatureClass as IFeatureClassManage;
            targetFeatureClassManage.UpdateExtent();
        }
    }
}
