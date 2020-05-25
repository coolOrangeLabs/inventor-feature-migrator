namespace FeatureMigratorAddin.UI
{
    partial class SettingsDlg
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
            this.bOk = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.cbConstrucWorkpoint = new System.Windows.Forms.CheckBox();
            this.cbConstrucWorkaxes = new System.Windows.Forms.CheckBox();
            this.cbConstrucWorkplane = new System.Windows.Forms.CheckBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bOk
            // 
            this.bOk.Location = new System.Drawing.Point(130, 270);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(85, 27);
            this.bOk.TabIndex = 0;
            this.bOk.Text = "OK";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(221, 270);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(85, 27);
            this.bCancel.TabIndex = 1;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // cbConstrucWorkpoint
            // 
            this.cbConstrucWorkpoint.AutoSize = true;
            this.cbConstrucWorkpoint.Location = new System.Drawing.Point(8, 13);
            this.cbConstrucWorkpoint.Name = "cbConstrucWorkpoint";
            this.cbConstrucWorkpoint.Size = new System.Drawing.Size(176, 17);
            this.cbConstrucWorkpoint.TabIndex = 2;
            this.cbConstrucWorkpoint.Text = "Create Construction Workpoints";
            this.cbConstrucWorkpoint.UseVisualStyleBackColor = true;
            this.cbConstrucWorkpoint.CheckedChanged += new System.EventHandler(this.cbConstrucWorkpoint_CheckedChanged);
            // 
            // cbConstrucWorkaxes
            // 
            this.cbConstrucWorkaxes.AutoSize = true;
            this.cbConstrucWorkaxes.Location = new System.Drawing.Point(8, 48);
            this.cbConstrucWorkaxes.Name = "cbConstrucWorkaxes";
            this.cbConstrucWorkaxes.Size = new System.Drawing.Size(170, 17);
            this.cbConstrucWorkaxes.TabIndex = 3;
            this.cbConstrucWorkaxes.Text = "Create Construction Workaxes";
            this.cbConstrucWorkaxes.UseVisualStyleBackColor = true;
            this.cbConstrucWorkaxes.CheckedChanged += new System.EventHandler(this.cbConstrucWorkaxes_CheckedChanged);
            // 
            // cbConstrucWorkplane
            // 
            this.cbConstrucWorkplane.AutoSize = true;
            this.cbConstrucWorkplane.Location = new System.Drawing.Point(8, 85);
            this.cbConstrucWorkplane.Name = "cbConstrucWorkplane";
            this.cbConstrucWorkplane.Size = new System.Drawing.Size(179, 17);
            this.cbConstrucWorkplane.TabIndex = 4;
            this.cbConstrucWorkplane.Text = "Create Construction Workplanes";
            this.cbConstrucWorkplane.UseVisualStyleBackColor = true;
            this.cbConstrucWorkplane.CheckedChanged += new System.EventHandler(this.cbConstrucWorkplane_CheckedChanged);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Location = new System.Drawing.Point(6, 7);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(300, 256);
            this.tabControl.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cbConstrucWorkpoint);
            this.tabPage1.Controls.Add(this.cbConstrucWorkplane);
            this.tabPage1.Controls.Add(this.cbConstrucWorkaxes);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(292, 230);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General Settings";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // SettingsDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 303);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SettingsDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Feature Migrator Settings";
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bOk;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.CheckBox cbConstrucWorkpoint;
        private System.Windows.Forms.CheckBox cbConstrucWorkaxes;
        private System.Windows.Forms.CheckBox cbConstrucWorkplane;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
    }
}