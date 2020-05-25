namespace FeatureMigratorLib
{
    partial class ReportControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportControl));
            this.label2 = new System.Windows.Forms.Label();
            this.cbStyles = new System.Windows.Forms.ComboBox();
            this.cbAction = new System.Windows.Forms.ComboBox();
            this.lbAction = new System.Windows.Forms.Label();
            this.bOk = new System.Windows.Forms.Button();
            this.cbActionPart = new System.Windows.Forms.ComboBox();
            this.lbFeaturesError = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbBackup = new System.Windows.Forms.CheckBox();
            this.cbEachFeature = new System.Windows.Forms.CheckBox();
            this.gbConstructionSettings = new System.Windows.Forms.GroupBox();
            this.cbConstrucWorkplane = new System.Windows.Forms.CheckBox();
            this.cbConstrucWorkaxes = new System.Windows.Forms.CheckBox();
            this.cbConstrucWorkpoint = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gbConstructionSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Part Features Render Style:";
            // 
            // cbStyles
            // 
            this.cbStyles.DropDownHeight = 200;
            this.cbStyles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStyles.FormattingEnabled = true;
            this.cbStyles.IntegralHeight = false;
            this.cbStyles.Location = new System.Drawing.Point(202, 49);
            this.cbStyles.Name = "cbStyles";
            this.cbStyles.Size = new System.Drawing.Size(172, 21);
            this.cbStyles.TabIndex = 27;
            this.cbStyles.SelectedIndexChanged += new System.EventHandler(this.cbStyles_SelectedIndexChanged);
            // 
            // cbAction
            // 
            this.cbAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAction.FormattingEnabled = true;
            this.cbAction.Items.AddRange(new object[] {
            "Suppress if Succeeded",
            "Suppress always",
            "Delete if Succeeded",
            "Delete always",
            "None"});
            this.cbAction.Location = new System.Drawing.Point(201, 24);
            this.cbAction.Name = "cbAction";
            this.cbAction.Size = new System.Drawing.Size(173, 21);
            this.cbAction.TabIndex = 23;
            this.cbAction.SelectedIndexChanged += new System.EventHandler(this.cbAction_SelectedIndexChanged);
            // 
            // lbAction
            // 
            this.lbAction.AutoSize = true;
            this.lbAction.Location = new System.Drawing.Point(6, 27);
            this.lbAction.Name = "lbAction";
            this.lbAction.Size = new System.Drawing.Size(146, 13);
            this.lbAction.TabIndex = 22;
            this.lbAction.Text = "Action for Assembly Features:";
            // 
            // bOk
            // 
            this.bOk.Location = new System.Drawing.Point(242, 330);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(145, 27);
            this.bOk.TabIndex = 29;
            this.bOk.Text = "OK";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // cbActionPart
            // 
            this.cbActionPart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbActionPart.FormattingEnabled = true;
            this.cbActionPart.Items.AddRange(new object[] {
            "Suppress",
            "Delete",
            "None"});
            this.cbActionPart.Location = new System.Drawing.Point(202, 22);
            this.cbActionPart.Name = "cbActionPart";
            this.cbActionPart.Size = new System.Drawing.Size(172, 21);
            this.cbActionPart.TabIndex = 32;
            this.cbActionPart.SelectedIndexChanged += new System.EventHandler(this.cbActionPart_SelectedIndexChanged);
            // 
            // lbFeaturesError
            // 
            this.lbFeaturesError.AutoSize = true;
            this.lbFeaturesError.Location = new System.Drawing.Point(6, 25);
            this.lbFeaturesError.Name = "lbFeaturesError";
            this.lbFeaturesError.Size = new System.Drawing.Size(157, 13);
            this.lbFeaturesError.TabIndex = 33;
            this.lbFeaturesError.Text = "Action for non healthy Features:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbAction);
            this.groupBox1.Controls.Add(this.cbAction);
            this.groupBox1.Location = new System.Drawing.Point(5, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(382, 58);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Assembly Features:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbActionPart);
            this.groupBox2.Controls.Add(this.lbFeaturesError);
            this.groupBox2.Controls.Add(this.cbStyles);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(5, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(382, 79);
            this.groupBox2.TabIndex = 38;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Part Features:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbBackup);
            this.groupBox3.Controls.Add(this.cbEachFeature);
            this.groupBox3.Location = new System.Drawing.Point(5, 248);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(382, 76);
            this.groupBox3.TabIndex = 39;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "General Settings";
            // 
            // cbBackup
            // 
            this.cbBackup.AutoSize = true;
            this.cbBackup.Location = new System.Drawing.Point(7, 44);
            this.cbBackup.Name = "cbBackup";
            this.cbBackup.Size = new System.Drawing.Size(257, 17);
            this.cbBackup.TabIndex = 1;
            this.cbBackup.Text = "Create backup file before sending feature to part ";
            this.cbBackup.UseVisualStyleBackColor = true;
            this.cbBackup.CheckedChanged += new System.EventHandler(this.cbBackup_CheckedChanged);
            // 
            // cbEachFeature
            // 
            this.cbEachFeature.AutoSize = true;
            this.cbEachFeature.Location = new System.Drawing.Point(7, 20);
            this.cbEachFeature.Name = "cbEachFeature";
            this.cbEachFeature.Size = new System.Drawing.Size(157, 17);
            this.cbEachFeature.TabIndex = 0;
            this.cbEachFeature.Text = "Set options for each feature";
            this.cbEachFeature.UseVisualStyleBackColor = true;
            this.cbEachFeature.CheckedChanged += new System.EventHandler(this.cbEachFeature_CheckedChanged);
            // 
            // gbConstructionSettings
            // 
            this.gbConstructionSettings.Controls.Add(this.cbConstrucWorkplane);
            this.gbConstructionSettings.Controls.Add(this.cbConstrucWorkaxes);
            this.gbConstructionSettings.Controls.Add(this.cbConstrucWorkpoint);
            this.gbConstructionSettings.Location = new System.Drawing.Point(5, 156);
            this.gbConstructionSettings.Name = "gbConstructionSettings";
            this.gbConstructionSettings.Size = new System.Drawing.Size(382, 86);
            this.gbConstructionSettings.TabIndex = 40;
            this.gbConstructionSettings.TabStop = false;
            this.gbConstructionSettings.Text = "Construction Settings";
            // 
            // cbConstrucWorkplane
            // 
            this.cbConstrucWorkplane.AutoSize = true;
            this.cbConstrucWorkplane.Location = new System.Drawing.Point(7, 63);
            this.cbConstrucWorkplane.Name = "cbConstrucWorkplane";
            this.cbConstrucWorkplane.Size = new System.Drawing.Size(179, 17);
            this.cbConstrucWorkplane.TabIndex = 2;
            this.cbConstrucWorkplane.Text = "Create Construction Workplanes";
            this.cbConstrucWorkplane.UseVisualStyleBackColor = true;
            this.cbConstrucWorkplane.CheckedChanged += new System.EventHandler(this.cbConstrucWorkplane_CheckedChanged);
            // 
            // cbConstrucWorkaxes
            // 
            this.cbConstrucWorkaxes.AutoSize = true;
            this.cbConstrucWorkaxes.Location = new System.Drawing.Point(7, 42);
            this.cbConstrucWorkaxes.Name = "cbConstrucWorkaxes";
            this.cbConstrucWorkaxes.Size = new System.Drawing.Size(170, 17);
            this.cbConstrucWorkaxes.TabIndex = 1;
            this.cbConstrucWorkaxes.Text = "Create Construction Workaxes";
            this.cbConstrucWorkaxes.UseVisualStyleBackColor = true;
            this.cbConstrucWorkaxes.CheckedChanged += new System.EventHandler(this.cbConstrucWorkaxes_CheckedChanged);
            // 
            // cbConstrucWorkpoint
            // 
            this.cbConstrucWorkpoint.AutoSize = true;
            this.cbConstrucWorkpoint.Location = new System.Drawing.Point(7, 20);
            this.cbConstrucWorkpoint.Name = "cbConstrucWorkpoint";
            this.cbConstrucWorkpoint.Size = new System.Drawing.Size(176, 17);
            this.cbConstrucWorkpoint.TabIndex = 0;
            this.cbConstrucWorkpoint.Text = "Create Construction Workpoints";
            this.cbConstrucWorkpoint.UseVisualStyleBackColor = true;
            this.cbConstrucWorkpoint.CheckedChanged += new System.EventHandler(this.cbConstrucWorkpoint_CheckedChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(161, 332);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 41;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ReportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 369);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.gbConstructionSettings);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReportControl";
            this.Text = "featureMigrator - Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbConstructionSettings.ResumeLayout(false);
            this.gbConstructionSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbStyles;
        private System.Windows.Forms.ComboBox cbAction;
        private System.Windows.Forms.Label lbAction;
        private System.Windows.Forms.Button bOk;
        private System.Windows.Forms.ComboBox cbActionPart;
        private System.Windows.Forms.Label lbFeaturesError;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbBackup;
        private System.Windows.Forms.CheckBox cbEachFeature;
        private System.Windows.Forms.GroupBox gbConstructionSettings;
        private System.Windows.Forms.CheckBox cbConstrucWorkpoint;
        private System.Windows.Forms.CheckBox cbConstrucWorkaxes;
        private System.Windows.Forms.CheckBox cbConstrucWorkplane;
        private System.Windows.Forms.Button cancelButton;
    }
}