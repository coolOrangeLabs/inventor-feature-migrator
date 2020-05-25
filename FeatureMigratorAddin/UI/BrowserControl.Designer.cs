namespace FeatureMigratorAddin.UI
{
    partial class BrowserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserControl));
            this.panelMain = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TreeView = new FeatureMigratorLib.TreeViewMultiSelect();
            this.panelTop = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.bRefresh = new System.Windows.Forms.Button();
            this.imageFeatures = new System.Windows.Forms.ImageList(this.components);
            this.panelMain.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panel1);
            this.panelMain.Controls.Add(this.panelTop);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(216, 454);
            this.panelMain.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.TreeView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 23);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(216, 431);
            this.panel1.TabIndex = 2;
            // 
            // TreeView
            // 
            this.TreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeView.Indent = 20;
            this.TreeView.ItemHeight = 20;
            this.TreeView.Location = new System.Drawing.Point(0, 0);
            this.TreeView.Name = "TreeView";
            this.TreeView.Size = new System.Drawing.Size(216, 431);
            this.TreeView.Sorted = true;
            this.TreeView.TabIndex = 0;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Controls.Add(this.bRefresh);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(216, 23);
            this.panelTop.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "featureMigrator Control";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // bRefresh
            // 
            this.bRefresh.Dock = System.Windows.Forms.DockStyle.Right;
            this.bRefresh.FlatAppearance.BorderSize = 0;
            this.bRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bRefresh.Image = ((System.Drawing.Image)(resources.GetObject("bRefresh.Image")));
            this.bRefresh.Location = new System.Drawing.Point(196, 0);
            this.bRefresh.Name = "bRefresh";
            this.bRefresh.Size = new System.Drawing.Size(20, 23);
            this.bRefresh.TabIndex = 3;
            this.bRefresh.UseVisualStyleBackColor = true;
            this.bRefresh.Click += new System.EventHandler(this.bRefresh_Click);
            // 
            // imageFeatures
            // 
            this.imageFeatures.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageFeatures.ImageStream")));
            this.imageFeatures.TransparentColor = System.Drawing.Color.Transparent;
            this.imageFeatures.Images.SetKeyName(0, "folderClose3.ico");
            this.imageFeatures.Images.SetKeyName(1, "folderOpen3.ico");
            this.imageFeatures.Images.SetKeyName(2, "FeatDef1.ico");
            this.imageFeatures.Images.SetKeyName(3, "FeatDef2.ico");
            this.imageFeatures.Images.SetKeyName(4, "INV_Model_Extrude_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(5, "INV_Model_Extrude_Suppressed_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(6, "INV_Model_Hole_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(7, "INV_Model_Hole_Suppressed_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(8, "INV_Model_Revolve_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(9, "INV_Model_Revolve_Suppressed_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(10, "INV_Model_Fillet_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(11, "INV_Model_Fillet_Suppressed_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(12, "INV_Model_Chamfer_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(13, "INV_Model_Chamfer_Suppressed_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(14, "INV_Model_Sweep_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(15, "INV_Model_Sweep_Suppressed_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(16, "INV_Model_MoveFace_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(17, "INV_Model_MoveFace_Suppressed_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(18, "INV_Model_RectangularPattern_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(19, "INV_Model_RectangularPattern_Suppressed_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(20, "INV_Model_CircularPattern_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(21, "INV_Model_CircularPattern_Suppressed_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(22, "INV_Model_Mirror_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(23, "INV_Model_Mirror_Suppressed_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(24, "INV_Assembly_Doc_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(25, "INV_Model_Solidbody_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(26, "INV_Sketch_2DSketch_Active_Browser_16x16.ico");
            this.imageFeatures.Images.SetKeyName(27, "INV_Model_Solidbody_Consumed_Browser_16x16.ico");
            // 
            // BrowserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMain);
            this.Name = "BrowserControl";
            this.Size = new System.Drawing.Size(216, 454);
            this.panelMain.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.ImageList imageFeatures;
        private System.Windows.Forms.Panel panel1;
        protected FeatureMigratorLib.TreeViewMultiSelect TreeView;
        private System.Windows.Forms.Button bRefresh;
        protected System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label label1;
    }
}
