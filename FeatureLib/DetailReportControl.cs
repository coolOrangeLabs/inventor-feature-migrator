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
   
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // DetailReportControl dialog displays a detailed report of the feature migration operation
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class DetailReportControl : Form
    {
        private List<FeatureReport> _Reports;

        private int _Index;

        private ListViewItem _lvItem;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public DetailReportControl(AssemblyDocument document, List<FeatureReport> reports)
        {
            InitializeComponent();

            _Reports = reports;

            if (reports.Count <= 1)
            {
                bPrevious.Enabled = false;
                bNext.Enabled = false;
            }

            foreach (FeatureReport report in _Reports)
            {
                cbFeatureName.Items.Add(report.AssemblyFeature.Name);
            }

            cbFeatureName.SelectedIndex = 0;

            cbFeatureName.SelectedIndexChanged += new EventHandler(cbFeatureName_SelectedIndexChanged);

            cbStyles.Items.Add("As Part");

            foreach (RenderStyle style in document.RenderStyles)
            {
                cbStyles.Items.Add(style.Name);
            }

            //Set "As Part" as default
            cbStyles.SelectedIndex = 0;

            cbFeatureAction.KeyPress += new KeyPressEventHandler(cbFeatureAction_KeyPress);
            cbFeatureAction.Leave += new EventHandler(cbFeatureAction_Leave);
            cbFeatureAction.SelectedValueChanged += new EventHandler(cbFeatureAction_SelectedValueChanged);

            lvNewFeatures.MouseUp += new MouseEventHandler(lvNewFeatures_MouseUp);

            this.FormClosing += new FormClosingEventHandler(ReportControl_FormClosing);
 
            _Index = 0;

            //Init dialog with first report
            LoadReport(_Index);
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
        private void LoadReport(int index)
        {
            FeatureReport report = _Reports[index];

            switch (report.ReportStatus)
            {
                case ReportStatusEnum.kFailed:
                    lbFeatureStatus.Text = " Send to Parts Failed";
                    lbFeatureStatus.ForeColor = System.Drawing.Color.Red;
                    break;

                case ReportStatusEnum.kPartial:
                    lbFeatureStatus.Text = " Send to Parts Partially Succeeded";
                    lbFeatureStatus.ForeColor = System.Drawing.Color.Orange;
                    break;

                case ReportStatusEnum.kSuccess:
                    lbFeatureStatus.Text = " Send to Parts Succeeded";
                    lbFeatureStatus.ForeColor = System.Drawing.Color.Green;
                    break;
            }

            switch (report.FinalizeAction)
            { 
                case FinalizeActionEnum.kSuppress:

                    cbAction.SelectedIndex = 0;
                    break;

                case FinalizeActionEnum.kDeleteAll:
                case FinalizeActionEnum.kDeleteRetainConsumedSketches:
                case FinalizeActionEnum.kDeleteRetainSketchesFeatAndWorkFeat:

                    cbAction.SelectedIndex = 1;
                    break;

                case FinalizeActionEnum.kNone:

                    cbAction.SelectedIndex = 2;
                    break;

                default:
                    break;
            }
            
            cbStyles.Text = report.Style;

            lvNewFeatures.Items.Clear();

            foreach (ReportData data in report.ReportDataList)
            {
                ListViewItem item = null;

                if (data.PartFeature == null)
                {
                    item = lvNewFeatures.Items.Add(" --- ");
                    item.UseItemStyleForSubItems = false;
                    item.Tag = data;

                    ListViewItem.ListViewSubItem subItemName = item.SubItems.Add(" --- ");

                    ListViewItem.ListViewSubItem subItemHealth = item.SubItems.Add("Copy Failure");
                    subItemHealth.ForeColor = System.Drawing.Color.Red;

                     ListViewItem.ListViewSubItem subItemPath = item.SubItems.Add(data.Document.FullFileName);
                }
                else
                {
                    switch (data.FinalizeAction)
                    {
                        case FinalizeActionEnum.kDeleteAll:
                        case FinalizeActionEnum.kDeleteRetainConsumedSketches:
                        case FinalizeActionEnum.kDeleteRetainSketchesFeatAndWorkFeat:

                            item = lvNewFeatures.Items.Add("Delete");
                            break;

                        case FinalizeActionEnum.kSuppress:

                            item = lvNewFeatures.Items.Add("Suppress");
                            break;

                        case FinalizeActionEnum.kNone:
                        default:

                            item = lvNewFeatures.Items.Add("None");
                            break;
                    }
                
                    item.UseItemStyleForSubItems = false;
                    item.Tag = data;

                    ListViewItem.ListViewSubItem subItemName = item.SubItems.Add(data.FeatureName);
                    
                    ListViewItem.ListViewSubItem subItemHealth = item.SubItems.Add(FeatureUtilities.GetHealthStatusString(data.PartFeature.HealthStatus));

                    if (data.PartFeature.HealthStatus != HealthStatusEnum.kUpToDateHealth)
                    {
                        subItemHealth.ForeColor = System.Drawing.Color.Red;
                    }

                    ListViewItem.ListViewSubItem subItemPath = item.SubItems.Add(data.Document.FullFileName);
                }
            }

            lvUnrefDocs.Items.Clear();

            foreach (string doc in report.UnreferencedDocuments)
            {
                ListViewItem item = lvUnrefDocs.Items.Add(doc);

                item.Checked = report.GetUnrefDocAction(doc);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SaveReport(int index)
        {
            FeatureReport report = _Reports[index];

            switch (cbAction.SelectedIndex)
            {
                case 0: //Suppress

                    report.FinalizeAction = FinalizeActionEnum.kSuppress;
                    break;

                case 1: //Delete 

                    report.FinalizeAction = FinalizeActionEnum.kDeleteAll;
                    break;

                case 2: //None

                    report.FinalizeAction = FinalizeActionEnum.kNone;
                    break;
            }

            report.Style = cbStyles.Text;

            foreach (ListViewItem item in lvNewFeatures.Items)
            {
                ReportData data = item.Tag as ReportData; 

                switch (item.Text)
                {
                    case "Delete":

                        data.FinalizeAction = FinalizeActionEnum.kDeleteAll;
                        break;

                    case "Suppress":

                        data.FinalizeAction = FinalizeActionEnum.kSuppress;
                        break;

                    case "None":
                    default:

                        data.FinalizeAction = FinalizeActionEnum.kNone;
                        break;
                }
            }

            foreach (ListViewItem item in lvUnrefDocs.Items)
            {
                report.SetUnrefDocAction(item.Text, item.Checked);
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
            SaveReport(_Index);

            ActionFinalize();

            Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // User closed the form without clicking Ok, so do not perform any action
        // just close all loaded part documents
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void ReportControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (FeatureReport report in _Reports)
            {
                report.UnloadReportData();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void bPrevious_Click(object sender, EventArgs e)
        {
            int newIndex = _Index-1;

            if (newIndex < 0) 
                newIndex = _Reports.Count - 1;

            cbFeatureName.SelectedIndex = newIndex;
        }

        private void bNext_Click(object sender, EventArgs e)
        {
            int newIndex = _Index+1;

            if (newIndex > 
                _Reports.Count - 1) newIndex = 0;

            cbFeatureName.SelectedIndex = newIndex;
        }

        void cbFeatureName_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveReport(_Index);

            _Index = cbFeatureName.SelectedIndex;

            LoadReport(_Index);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Handles the ListView Combobox functionality
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void cbFeatureAction_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Verify that the user presses ESC.
            switch((int)e.KeyChar)
            {
                case 27: //Escape
                    //Reset the original text value, and then hide the ComboBox.
                    cbFeatureAction.Text = _lvItem.Text;
                    cbFeatureAction.Visible = false;
                    break;
                case 13: //Enter
                    //Hide the ComboBox.
                    cbFeatureAction.Visible = false;
                    break;
                default:
                    break;
            }
        }
       
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Handles the ListView Combobox functionality
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void cbFeatureAction_Leave(object sender, EventArgs e)
        {
            //Set text of ListView item to match the ComboBox.
            _lvItem.Text = cbFeatureAction.Text;

            //Hide the ComboBox.
            cbFeatureAction.Visible = false;
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Handles the ListView Combobox functionality
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void cbFeatureAction_SelectedValueChanged(object sender, EventArgs e)
        {
            //Set text of ListView item to match the ComboBox.
            _lvItem.Text = cbFeatureAction.Text;

            //Hide the ComboBox.
            cbFeatureAction.Visible = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Handles the ListView Combobox functionality
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void lvNewFeatures_MouseUp(object sender, MouseEventArgs e)
        {
            //Get the item on the row that is clicked.
            _lvItem = lvNewFeatures.GetItemAt(e.X, e.Y);

            //Make sure that an item is clicked.
            if (_lvItem == null || _lvItem.Text == " --- ") 
                return;

            
            //Get the bounds of the item that is clicked.
            Rectangle ClickedItem = _lvItem.Bounds;

            //Verify that the column is completely scrolled off to the left.
            if ((ClickedItem.Left + lvNewFeatures.Columns[0].Width) < 0)
            {
                //If the cell is out of view to the left, do nothing.
                return;
            }

            //Verify that the column is partially scrolled off to the left.
            else if (ClickedItem.Left < 0)
            {
                //Determine if column extends beyond right side of ListView.
                if((ClickedItem.Left + lvNewFeatures.Columns[0].Width) > lvNewFeatures.Width)
                {
                    //Set width of column to match width of ListView.
                    ClickedItem.Width = lvNewFeatures.Width;
                    ClickedItem.X = 0;
                }
                else
                {
                    //Right side of cell is in view.
                    ClickedItem.Width = lvNewFeatures.Columns[0].Width + ClickedItem.Left;
                    ClickedItem.X = 2;
                }
            }

            else if (lvNewFeatures.Columns[0].Width > lvNewFeatures.Width)
            {
                ClickedItem.Width = lvNewFeatures.Width;
            }
            else
            {
                ClickedItem.Width = lvNewFeatures.Columns[0].Width;
                ClickedItem.X = 2;
            }

            //Adjust the top to account for the location of the ListView.
            ClickedItem.Y += lvNewFeatures.Top;
            ClickedItem.X += lvNewFeatures.Left;

            //Assign calculated bounds to the ComboBox.
            cbFeatureAction.Bounds = ClickedItem;

            //Set default text for ComboBox to match the item that is clicked.
            cbFeatureAction.Text = _lvItem.Text;

            //Display the ComboBox, and make sure that it is on top with focus.
            cbFeatureAction.Visible = true;
            cbFeatureAction.BringToFront();
            cbFeatureAction.Focus();
        }
    }
}
