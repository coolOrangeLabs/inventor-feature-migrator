namespace FeatureMigratorLib
{
    partial class DetailReportControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetailReportControl));
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.cbFeatureName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbStyles = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAction = new System.Windows.Forms.ComboBox();
            this.lbAction = new System.Windows.Forms.Label();
            this.lbFeatureStatus = new System.Windows.Forms.Label();
            this.panelTop = new System.Windows.Forms.Panel();
            this.splitContainerDown = new System.Windows.Forms.SplitContainer();
            this.cbFeatureAction = new System.Windows.Forms.ComboBox();
            this.lvNewFeatures = new FeatureMigratorLib.ComboListView();
            this.HeaderAction = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.HeaderFeature = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.HeaderStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.HeaderNewDocs = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvUnrefDocs = new System.Windows.Forms.ListView();
            this.HeaderUnrefDocs = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelButtons = new System.Windows.Forms.Panel();
            this.bPrevious = new System.Windows.Forms.Button();
            this.bOk = new System.Windows.Forms.Button();
            this.bNext = new System.Windows.Forms.Button();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.splitContainerDown.Panel1.SuspendLayout();
            this.splitContainerDown.Panel2.SuspendLayout();
            this.splitContainerDown.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.cbFeatureName);
            this.splitContainerMain.Panel1.Controls.Add(this.label2);
            this.splitContainerMain.Panel1.Controls.Add(this.cbStyles);
            this.splitContainerMain.Panel1.Controls.Add(this.label3);
            this.splitContainerMain.Panel1.Controls.Add(this.label1);
            this.splitContainerMain.Panel1.Controls.Add(this.cbAction);
            this.splitContainerMain.Panel1.Controls.Add(this.lbAction);
            this.splitContainerMain.Panel1.Controls.Add(this.lbFeatureStatus);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.panelTop);
            this.splitContainerMain.Size = new System.Drawing.Size(675, 447);
            this.splitContainerMain.SplitterDistance = 141;
            this.splitContainerMain.TabIndex = 0;
            // 
            // cbFeatureName
            // 
            this.cbFeatureName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFeatureName.FormattingEnabled = true;
            this.cbFeatureName.Location = new System.Drawing.Point(157, 14);
            this.cbFeatureName.Name = "cbFeatureName";
            this.cbFeatureName.Size = new System.Drawing.Size(173, 21);
            this.cbFeatureName.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "Part Features Render Style:";
            // 
            // cbStyles
            // 
            this.cbStyles.DropDownHeight = 200;
            this.cbStyles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStyles.FormattingEnabled = true;
            this.cbStyles.IntegralHeight = false;
            this.cbStyles.Location = new System.Drawing.Point(157, 97);
            this.cbStyles.Name = "cbStyles";
            this.cbStyles.Size = new System.Drawing.Size(173, 21);
            this.cbStyles.TabIndex = 19;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Feature Status:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Assembly Feature:";
            // 
            // cbAction
            // 
            this.cbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAction.FormattingEnabled = true;
            this.cbAction.Items.AddRange(new object[] {
            "Suppress",
            "Delete",
            "None"});
            this.cbAction.Location = new System.Drawing.Point(157, 69);
            this.cbAction.Name = "cbAction";
            this.cbAction.Size = new System.Drawing.Size(173, 21);
            this.cbAction.TabIndex = 15;
            // 
            // lbAction
            // 
            this.lbAction.AutoSize = true;
            this.lbAction.Location = new System.Drawing.Point(8, 74);
            this.lbAction.Name = "lbAction";
            this.lbAction.Size = new System.Drawing.Size(141, 13);
            this.lbAction.TabIndex = 14;
            this.lbAction.Text = "Action for Assembly Feature:";
            // 
            // lbFeatureStatus
            // 
            this.lbFeatureStatus.AutoSize = true;
            this.lbFeatureStatus.BackColor = System.Drawing.SystemColors.Menu;
            this.lbFeatureStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFeatureStatus.ForeColor = System.Drawing.Color.Black;
            this.lbFeatureStatus.Location = new System.Drawing.Point(154, 43);
            this.lbFeatureStatus.Name = "lbFeatureStatus";
            this.lbFeatureStatus.Size = new System.Drawing.Size(166, 13);
            this.lbFeatureStatus.TabIndex = 13;
            this.lbFeatureStatus.Text = "FeatureStatusStr                ";
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.splitContainerDown);
            this.panelTop.Controls.Add(this.panelButtons);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(675, 302);
            this.panelTop.TabIndex = 3;
            // 
            // splitContainerDown
            // 
            this.splitContainerDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerDown.Location = new System.Drawing.Point(0, 0);
            this.splitContainerDown.Name = "splitContainerDown";
            this.splitContainerDown.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerDown.Panel1
            // 
            this.splitContainerDown.Panel1.Controls.Add(this.cbFeatureAction);
            this.splitContainerDown.Panel1.Controls.Add(this.lvNewFeatures);
            // 
            // splitContainerDown.Panel2
            // 
            this.splitContainerDown.Panel2.Controls.Add(this.lvUnrefDocs);
            this.splitContainerDown.Size = new System.Drawing.Size(675, 275);
            this.splitContainerDown.SplitterDistance = 135;
            this.splitContainerDown.TabIndex = 2;
            // 
            // cbFeatureAction
            // 
            this.cbFeatureAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFeatureAction.FormattingEnabled = true;
            this.cbFeatureAction.Items.AddRange(new object[] {
            "Suppress",
            "Delete",
            "None"});
            this.cbFeatureAction.Location = new System.Drawing.Point(11, 23);
            this.cbFeatureAction.Name = "cbFeatureAction";
            this.cbFeatureAction.Size = new System.Drawing.Size(70, 21);
            this.cbFeatureAction.TabIndex = 21;
            this.cbFeatureAction.Visible = false;
            // 
            // lvNewFeatures
            // 
            this.lvNewFeatures.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.HeaderAction,
            this.HeaderFeature,
            this.HeaderStatus,
            this.HeaderNewDocs});
            this.lvNewFeatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvNewFeatures.GridLines = true;
            this.lvNewFeatures.Location = new System.Drawing.Point(0, 0);
            this.lvNewFeatures.Name = "lvNewFeatures";
            this.lvNewFeatures.Size = new System.Drawing.Size(675, 135);
            this.lvNewFeatures.TabIndex = 0;
            this.lvNewFeatures.UseCompatibleStateImageBehavior = false;
            this.lvNewFeatures.View = System.Windows.Forms.View.Details;
            // 
            // HeaderAction
            // 
            this.HeaderAction.Text = "Action";
            this.HeaderAction.Width = 82;
            // 
            // HeaderFeature
            // 
            this.HeaderFeature.Text = "Feature";
            this.HeaderFeature.Width = 101;
            // 
            // HeaderStatus
            // 
            this.HeaderStatus.Text = "Status";
            this.HeaderStatus.Width = 89;
            // 
            // HeaderNewDocs
            // 
            this.HeaderNewDocs.Text = "News Documents";
            this.HeaderNewDocs.Width = 399;
            // 
            // lvUnrefDocs
            // 
            this.lvUnrefDocs.CheckBoxes = true;
            this.lvUnrefDocs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.HeaderUnrefDocs});
            this.lvUnrefDocs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvUnrefDocs.GridLines = true;
            this.lvUnrefDocs.Location = new System.Drawing.Point(0, 0);
            this.lvUnrefDocs.Name = "lvUnrefDocs";
            this.lvUnrefDocs.Size = new System.Drawing.Size(675, 136);
            this.lvUnrefDocs.TabIndex = 5;
            this.lvUnrefDocs.UseCompatibleStateImageBehavior = false;
            this.lvUnrefDocs.View = System.Windows.Forms.View.Details;
            // 
            // HeaderUnrefDocs
            // 
            this.HeaderUnrefDocs.Text = "Select unreferenced documents to delete:";
            this.HeaderUnrefDocs.Width = 670;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.bPrevious);
            this.panelButtons.Controls.Add(this.bOk);
            this.panelButtons.Controls.Add(this.bNext);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 275);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(675, 27);
            this.panelButtons.TabIndex = 1;
            // 
            // bPrevious
            // 
            this.bPrevious.Dock = System.Windows.Forms.DockStyle.Right;
            this.bPrevious.Location = new System.Drawing.Point(310, 0);
            this.bPrevious.Name = "bPrevious";
            this.bPrevious.Size = new System.Drawing.Size(110, 27);
            this.bPrevious.TabIndex = 2;
            this.bPrevious.Text = "< Previous Feature";
            this.bPrevious.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bPrevious.UseVisualStyleBackColor = true;
            this.bPrevious.Click += new System.EventHandler(this.bPrevious_Click);
            // 
            // bOk
            // 
            this.bOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.bOk.Location = new System.Drawing.Point(420, 0);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(145, 27);
            this.bOk.TabIndex = 1;
            this.bOk.Text = "OK";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // bNext
            // 
            this.bNext.Dock = System.Windows.Forms.DockStyle.Right;
            this.bNext.Location = new System.Drawing.Point(565, 0);
            this.bNext.Name = "bNext";
            this.bNext.Size = new System.Drawing.Size(110, 27);
            this.bNext.TabIndex = 0;
            this.bNext.Text = "Next Feature >";
            this.bNext.UseVisualStyleBackColor = true;
            this.bNext.Click += new System.EventHandler(this.bNext_Click);
            // 
            // DetailReportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 447);
            this.Controls.Add(this.splitContainerMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DetailReportControl";
            this.Text = "Features Migration - Detailed Report";
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel1.PerformLayout();
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.splitContainerDown.Panel1.ResumeLayout(false);
            this.splitContainerDown.Panel2.ResumeLayout(false);
            this.splitContainerDown.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbStyles;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbAction;
        private System.Windows.Forms.Label lbAction;
        private System.Windows.Forms.Label lbFeatureStatus;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.SplitContainer splitContainerDown;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.ListView lvUnrefDocs;
        private System.Windows.Forms.ColumnHeader HeaderUnrefDocs;
        private System.Windows.Forms.ComboBox cbFeatureAction;
        private ComboListView lvNewFeatures;
        private System.Windows.Forms.ColumnHeader HeaderNewDocs;
        private System.Windows.Forms.ColumnHeader HeaderFeature;
        private System.Windows.Forms.ColumnHeader HeaderStatus;
        private System.Windows.Forms.ColumnHeader HeaderAction;
        private System.Windows.Forms.Button bPrevious;
        private System.Windows.Forms.Button bOk;
        private System.Windows.Forms.Button bNext;
        private System.Windows.Forms.ComboBox cbFeatureName;


    }
}