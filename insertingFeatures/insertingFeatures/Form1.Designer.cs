namespace insertingFeatures
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.往已有要素类中插入数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createFeatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertFeatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.objectLoaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.featureClassWriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.往已有要素类中插入数据ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(636, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 往已有要素类中插入数据ToolStripMenuItem
            // 
            this.往已有要素类中插入数据ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createFeatureToolStripMenuItem,
            this.insertFeatureToolStripMenuItem,
            this.loadOnlyToolStripMenuItem,
            this.objectLoaderToolStripMenuItem,
            this.featureClassWriteToolStripMenuItem});
            this.往已有要素类中插入数据ToolStripMenuItem.Name = "往已有要素类中插入数据ToolStripMenuItem";
            this.往已有要素类中插入数据ToolStripMenuItem.Size = new System.Drawing.Size(197, 24);
            this.往已有要素类中插入数据ToolStripMenuItem.Text = "往已有要素类中插入数据";
            // 
            // createFeatureToolStripMenuItem
            // 
            this.createFeatureToolStripMenuItem.Name = "createFeatureToolStripMenuItem";
            this.createFeatureToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.createFeatureToolStripMenuItem.Text = "CreateFeature";
            this.createFeatureToolStripMenuItem.Click += new System.EventHandler(this.createFeatureToolStripMenuItem_Click);
            // 
            // insertFeatureToolStripMenuItem
            // 
            this.insertFeatureToolStripMenuItem.Name = "insertFeatureToolStripMenuItem";
            this.insertFeatureToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.insertFeatureToolStripMenuItem.Text = "InsertFeature";
            this.insertFeatureToolStripMenuItem.Click += new System.EventHandler(this.insertFeatureToolStripMenuItem_Click);
            // 
            // loadOnlyToolStripMenuItem
            // 
            this.loadOnlyToolStripMenuItem.Name = "loadOnlyToolStripMenuItem";
            this.loadOnlyToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.loadOnlyToolStripMenuItem.Text = "LoadOnly";
            this.loadOnlyToolStripMenuItem.Click += new System.EventHandler(this.loadOnlyToolStripMenuItem_Click);
            // 
            // objectLoaderToolStripMenuItem
            // 
            this.objectLoaderToolStripMenuItem.Name = "objectLoaderToolStripMenuItem";
            this.objectLoaderToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.objectLoaderToolStripMenuItem.Text = "ObjectLoader";
            this.objectLoaderToolStripMenuItem.Click += new System.EventHandler(this.objectLoaderToolStripMenuItem_Click);
            // 
            // featureClassWriteToolStripMenuItem
            // 
            this.featureClassWriteToolStripMenuItem.Name = "featureClassWriteToolStripMenuItem";
            this.featureClassWriteToolStripMenuItem.Size = new System.Drawing.Size(202, 26);
            this.featureClassWriteToolStripMenuItem.Text = "FeatureClassWrite";
            this.featureClassWriteToolStripMenuItem.Click += new System.EventHandler(this.featureClassWriteToolStripMenuItem_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "OBJECTID <2",
            "OBJECTID <101",
            "OBJECTID <10001",
            "OBJECTID <100001"});
            this.comboBox1.Location = new System.Drawing.Point(434, 31);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(190, 24);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 310);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 往已有要素类中插入数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createFeatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertFeatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadOnlyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem objectLoaderToolStripMenuItem;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ToolStripMenuItem featureClassWriteToolStripMenuItem;
    }
}

