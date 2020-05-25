////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Philippe Leefsma 2009-2010 - ADN/Developer Technical Services
//
// Feedback and questions: Philippe.Leefsma@Autodesk.com
//
// This software is provided as is, without any warranty that it will work. You choose to use this tool at your own risk.
// Neither Autodesk nor the author Philippe Leefsma can be taken as responsible for any damage this tool can cause to 
// your data. Please always make a back up of your data prior to use this tool, as it will modify the documents involved 
// in the feature migration.
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace FeatureMigratorAddin.UI
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // The SettingsDlg exposes the addin/Feature Library setting to the user. 
    // It is triggered by a Right-Click on the BrowserControl Top area.
    // those settings are pretty limited at the moment.
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class SettingsDlg : Form
    {
        private AppSettings _Settings;

        public SettingsDlg()
        {
            InitializeComponent();

            _Settings = new AppSettings();

            _Settings.Load();

            cbConstrucWorkpoint.Checked = _Settings.ConstructionWorkPoint;
            cbConstrucWorkaxes.Checked = _Settings.ConstructionWorkAxis;
            cbConstrucWorkplane.Checked = _Settings.ConstructionWorkPlane;
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            _Settings.Save();

            Close();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbConstrucWorkpoint_CheckedChanged(object sender, EventArgs e)
        {
            _Settings.ConstructionWorkPoint = cbConstrucWorkpoint.Checked;
        }

        private void cbConstrucWorkaxes_CheckedChanged(object sender, EventArgs e)
        {
            _Settings.ConstructionWorkAxis = cbConstrucWorkaxes.Checked;
        }

        private void cbConstrucWorkplane_CheckedChanged(object sender, EventArgs e)
        {
            _Settings.ConstructionWorkPlane = cbConstrucWorkplane.Checked;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // AppSettings holds the settings present in the SettingsDlg
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    class AppSettings
    {
        public bool ConstructionWorkPoint;
        public bool ConstructionWorkAxis;
        public bool ConstructionWorkPlane;
       
        public AppSettings()
        { 
        
        }

        public void Load()
        { 
            ConstructionWorkPoint = FeatureMigratorLib.FeatureUtilities.ConstructionWorkPoint;
            ConstructionWorkAxis = FeatureMigratorLib.FeatureUtilities.ConstructionWorkAxis;
            ConstructionWorkPlane = FeatureMigratorLib.FeatureUtilities.ConstructionWorkPlane;
        }

        public void Save()
        {
            FeatureMigratorLib.FeatureUtilities.ConstructionWorkPoint = ConstructionWorkPoint;
            FeatureMigratorLib.FeatureUtilities.ConstructionWorkAxis = ConstructionWorkAxis;
            FeatureMigratorLib.FeatureUtilities.ConstructionWorkPlane = ConstructionWorkPlane;
        }
    }
}
