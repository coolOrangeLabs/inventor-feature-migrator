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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Inventor;

namespace FeatureMigratorLib
{
    public partial class ReportControl : Form
    {
        private readonly AppSettings _settings;

        private List<FeatureReport> _Reports;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ReportControl(AssemblyDocument document, List<FeatureReport> reports)
        {
            InitializeComponent();

            _Reports = reports;

            cbAction.SelectedIndex = 0;


            int nbNonHealthy = 0;

            foreach (FeatureReport report in _Reports)
            {
                foreach (ReportData data in report.ReportDataList)
                {
                    data.Load();

                    if (data.PartFeature == null) 
                        continue;

                    if (data.PartFeature.HealthStatus != HealthStatusEnum.kUpToDateHealth)
                    {
                        ++nbNonHealthy;
                    }
                }

            }


            cbActionPart.SelectedIndex = 0;

            cbStyles.Tag = document;

            cbStyles.Items.Add("As Part");

            foreach (RenderStyle style in document.RenderStyles)
            {
                cbStyles.Items.Add(style.Name);
            }

            //Set "As Part" as default
            cbStyles.SelectedIndex = 0;

            this.FormClosing += new FormClosingEventHandler(ReportControl_FormClosing);
        }

        public ReportControl(Document document)
        {
            InitializeComponent();

            _settings = new AppSettings();

            _settings.Load();

            cbConstrucWorkpoint.Checked = _settings.ConstructionWorkPoint;
            cbConstrucWorkaxes.Checked = _settings.ConstructionWorkAxis;
            cbConstrucWorkplane.Checked = _settings.ConstructionWorkPlane;

            cbAction.SelectedItem = _settings.ActionForAssemblyFeature;
            cbActionPart.SelectedItem = _settings.ActionForUnhealthyPartFeature;

            cbBackup.Checked = _settings.CreateBackupFile;
            cbEachFeature.Checked = _settings.SingleFeatureOptions;

            cbStyles.Tag = document;

            cbStyles.Items.Add("As Part");

            foreach (RenderStyle style in document.RenderStyles)
            {
                cbStyles.Items.Add(style.Name);
            }
            cbStyles.SelectedItem = _settings.PartFeatureRenderStyle;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ShowAsChildModal()
        {
            ShowDialog(new WindowWrapper((IntPtr)FeatureUtilities.Application.MainFrameHWND));
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ActionPreFinalize()
        {
            foreach (FeatureReport report in _Reports)
            {
                switch (cbAction.SelectedIndex)
                {
                    case 0: //Suppress if succeed

                        if (report.ReportStatus == ReportStatusEnum.kSuccess)
                        {
                            report.FinalizeAction = FinalizeActionEnum.kSuppress;
                        }
                        break;

                    case 1: //Suppress always

                        report.FinalizeAction = FinalizeActionEnum.kSuppress;
                        break;

                    case 2: //Delete if succeed

                        if (report.ReportStatus == ReportStatusEnum.kSuccess)
                        {
                            report.FinalizeAction = FinalizeActionEnum.kDeleteAll;
                        }
                        break;

                    case 3: //Delete always

                        report.FinalizeAction = FinalizeActionEnum.kDeleteAll;
                        break;

                    case 4: //None
                    default:
                        break;
                }

                report.Style = cbStyles.Text;

                foreach (ReportData data in report.ReportDataList)
                {
                    if (data.PartFeature == null) 
                        continue;

                    if (data.PartFeature.HealthStatus != HealthStatusEnum.kUpToDateHealth)
                    {
                        switch (cbActionPart.SelectedIndex)
                        {
                            case 0: //Suppress
                                data.FinalizeAction = FinalizeActionEnum.kSuppress;
                                break;

                            case 1: //delete
                                data.FinalizeAction = FinalizeActionEnum.kDeleteAll;
                                break;

                            case 2: //None
                                data.FinalizeAction = FinalizeActionEnum.kNone;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ActionFinalize()
        {
            foreach (FeatureReport report in _Reports)
            {
                report.ActionFinalize();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void bOk_Click(object sender, EventArgs e)
        {

            _settings.Save();

            Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // User closed the form without clicking Ok, so do not perform any action
        // just close all loaded part documents
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void ReportControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            //foreach (FeatureReport report in _Reports)
            //{
            //    report.UnloadReportData();
            //}
        }

        private void cbAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            _settings.ActionForAssemblyFeature = cbAction.SelectedItem.ToString();
        }

        private void cbActionPart_SelectedIndexChanged(object sender, EventArgs e)
        {
            _settings.ActionForUnhealthyPartFeature = cbActionPart.SelectedItem.ToString();
        }

        private void cbStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            _settings.PartFeatureRenderStyle = cbStyles.SelectedItem.ToString();
        }

        private void cbConstrucWorkpoint_CheckedChanged(object sender, EventArgs e)
        {
            _settings.ConstructionWorkPoint = cbConstrucWorkpoint.Checked;
        }

        private void cbConstrucWorkaxes_CheckedChanged(object sender, EventArgs e)
        {
            _settings.ConstructionWorkAxis = cbConstrucWorkaxes.Checked;
        }

        private void cbConstrucWorkplane_CheckedChanged(object sender, EventArgs e)
        {
            _settings.ConstructionWorkPlane = cbConstrucWorkplane.Checked;
        }

        private void cbEachFeature_CheckedChanged(object sender, EventArgs e)
        {
            _settings.SingleFeatureOptions = cbEachFeature.Checked;
        }

        private void cbBackup_CheckedChanged(object sender, EventArgs e)
        {
            _settings.CreateBackupFile = cbBackup.Checked;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
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
        public string ActionForAssemblyFeature;
        public string ActionForUnhealthyPartFeature;
        public string PartFeatureRenderStyle;
        public bool SingleFeatureOptions;
        public bool CreateBackupFile;



        public AppSettings()
        {

        }

        public void Load()
        {
            ConstructionWorkPoint = FeatureUtilities.ConstructionWorkPoint;
            ConstructionWorkAxis = FeatureUtilities.ConstructionWorkAxis;
            ConstructionWorkPlane = FeatureUtilities.ConstructionWorkPlane;
            ActionForAssemblyFeature = FeatureUtilities.CbAction;
            ActionForUnhealthyPartFeature = FeatureUtilities.CbActionPart;
            PartFeatureRenderStyle = FeatureUtilities.RenderStyle;
            SingleFeatureOptions = FeatureUtilities.SingleFeatureOptions;
            CreateBackupFile = FeatureUtilities.CreateBackupFile;
        }

        public void Save()
        {
            FeatureUtilities.ConstructionWorkPoint = ConstructionWorkPoint;
            FeatureUtilities.ConstructionWorkAxis = ConstructionWorkAxis;
            FeatureUtilities.ConstructionWorkPlane = ConstructionWorkPlane;
            FeatureUtilities.CbAction = ActionForAssemblyFeature;
            FeatureUtilities.CbActionPart = ActionForUnhealthyPartFeature;
            FeatureUtilities.RenderStyle = PartFeatureRenderStyle;
            FeatureUtilities.SingleFeatureOptions = SingleFeatureOptions;
            FeatureUtilities.CreateBackupFile = CreateBackupFile;
        }
    }
}
