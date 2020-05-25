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
using System.Windows.Forms;
using FeatureMigratorAddin.Utilities;
using FeatureMigratorLib;
using Inventor;

namespace FeatureMigratorAddin.UI
{
    public partial class BrowserControl : UserControl
    {
        System.Windows.Forms.ContextMenu _ctxMenuTree;
        System.Windows.Forms.ContextMenu _ctxMenuSettings;

        //Assembly context
        AssemblyDocument _AssemblyDocument;

        //Part context
        PartDocument _PartDocument;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public BrowserControl()
        {
            InitializeComponent();
        }

        public virtual void Initialize()
        {
            TreeView.ImageList = imageFeatures;
            
            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 2500;
            toolTip1.InitialDelay = 1500;
            toolTip1.ReshowDelay = 500;

            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = false;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(bRefresh, "Refresh Features View");

            TreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(TreeView_NodeMouseClick);
        }

        public void InitializeAsm()
        {
            panelTop.MouseClick += new MouseEventHandler(panelTop_MouseClick);

            _ctxMenuTree = new ContextMenu();
            _ctxMenuTree.MenuItems.Add("Send to Parts").Click += new EventHandler(SendToParts);

            _ctxMenuSettings = new ContextMenu();
            _ctxMenuSettings.MenuItems.Add("Settings...").Click += new EventHandler(DisplaySettings);
        }

        public void InitializePart()
        {
            _ctxMenuTree = new ContextMenu();
            _ctxMenuTree.MenuItems.Add("Update from Assembly").Click += new EventHandler(UpdateFeaturesFromAsm);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void panelTop_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _ctxMenuSettings.Show(panelTop, this.PointToClient(Cursor.Position));
            }
        }

        void DisplaySettings(object sender, EventArgs e)
        {
            var document = AdnInventorUtilities.InvApplication.ActiveDocument;
            if (document == null)
                return;
            ReportControl dlg = new ReportControl(document);
            dlg.Location = Cursor.Position;
            dlg.ShowDialog();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void bRefresh_Click(object sender, EventArgs e)
        {
            RefreshControl(AdnInventorUtilities.InvApplication.ActiveDocument);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void RefreshControl(Document document)
        {
            TreeView.Nodes.Clear();

            if (document == null)
                return;

            switch (document.DocumentType)
            { 
                case DocumentTypeEnum.kAssemblyDocumentObject:
                    RefreshTreeViewAsm(document as AssemblyDocument);
                    break;
                case DocumentTypeEnum.kPartDocumentObject:
                    RefreshTreeViewPart(document as PartDocument);
                    break;
                default:
                    return;
            }
        }

        protected void RefreshTreeViewAsm(AssemblyDocument document)
        {
            InitializeAsm();

            _AssemblyDocument = document;

            TreeNode root = TreeView.Nodes.Add("AsmRoot", _AssemblyDocument.DisplayName, 24);

            foreach (PartFeature Feature in _AssemblyDocument.ComponentDefinition.Features)
            {
                FeatureUtilities.AddAsmFeatureNode(TreeView, root, Feature);
            }

            root.Expand();
        }

        protected void RefreshTreeViewPart(PartDocument document)
        {
            InitializePart();

             _PartDocument = document;

             TreeNode root = TreeView.Nodes.Add("PartRoot", _PartDocument.DisplayName, 25);

             foreach (PartFeature Feature in _PartDocument.ComponentDefinition.Features)
             {
                 FeatureUtilities.AddPartFeatureNode(TreeView, root, Feature);
             }

             root.Expand();
         }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Document document = AdnInventorUtilities.InvApplication.ActiveDocument;

            if (document == null)
                return;

            switch (document.DocumentType)
            {
                case DocumentTypeEnum.kAssemblyDocumentObject:
                    TreeView_NodeMouseClickAsm(sender, e);
                    break;
                case DocumentTypeEnum.kPartDocumentObject:
                    TreeView_NodeMouseClickPart(sender, e);
                    break;
                default:
                    return;
            }
        }
        
        protected void TreeView_NodeMouseClickAsm(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (!TreeView.IsNodeSelected(e.Node))
                    {
                        TreeView.ClearSelection();
                        TreeView.SetSelected(e.Node, true, false);
                    }

                    SelectInventorFeatures();

                    if (e.Node.Tag is PartFeature)
                    {
                        _ctxMenuTree.MenuItems[0].Enabled = FeatureUtilities.IsSupportedFeature(e.Node.Tag as PartFeature);
                        _ctxMenuTree.Show(TreeView, e.Location);
                    }
                }

                else
                {
                    SelectInventorFeatures();
                }
            }
            catch
            {
                //Exception here means that the browser 
                //is out-ot-sync with the features state.
                RefreshControl(AdnInventorUtilities.InvApplication.ActiveDocument);
            }
        }

        protected void TreeView_NodeMouseClickPart(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (!TreeView.IsNodeSelected(e.Node))
                    {
                        TreeView.ClearSelection();
                        TreeView.SetSelected(e.Node, true, false);
                    }

                    SelectInventorFeatures();

                    if (e.Node.Tag is PartFeature)
                    {
                        if (FeatureUtilities.IsAssociativitySupported(e.Node.Tag as PartFeature))
                            _ctxMenuTree.Show(TreeView, e.Location);
                    }
                }
                else
                {
                    SelectInventorFeatures();
                }
            }
            catch
            {
                //Exception here means that the browser 
                //is out-ot-sync with the features state.
                RefreshControl(AdnInventorUtilities.InvApplication.ActiveDocument);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SendToParts(object sender, EventArgs e)
        {
            //Check assembly state before the features migration 
            switch (FeatureUtilities.CheckAssemblyState(_AssemblyDocument))
            {
                case FeatureUtilities.AssemblyStateEnum.kAssemblyNotSaved:
                    {
                        string msg = "Assembly and its sub-components need to be saved on the disk" + System.Environment.NewLine +
                            "for the features migration."
                            + System.Environment.NewLine +
                            "Please save each file first and run the command again...";

                        System.Windows.Forms.MessageBox.Show(msg,
                            "Feature Migrator",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return;
                    }

                case FeatureUtilities.AssemblyStateEnum.kException:
                    return;

                default:
                    break;
            }

            Transaction Tx = null;

            try
            {
                Tx = FeatureUtilities.Application.TransactionManager.StartTransaction(
                    _AssemblyDocument as _Document,
                    "Send Features");

                List<FeatureReport> results = new List<FeatureReport>();

                foreach (TreeNode node in TreeView.SelectedNodesBottomUp)
                {
                    if (!(node.Tag is PartFeature))
                        continue;

                    PartFeature Feature = node.Tag as PartFeature;

                    if (Feature.Suppressed)
                    {
                        //Do not migrate a suppressed feature
                        continue;
                    }

                    List<ComponentOccurrence> Participants = new List<ComponentOccurrence>();

                    foreach (TreeNode occNode in node.Nodes)
                    {
                        //Check if node is enabled
                        if (occNode.Tag != null && occNode.Tag is ComponentOccurrence)
                        {
                            Participants.Add(occNode.Tag as ComponentOccurrence);
                        }
                    }

                    if (Participants.Count != 0)
                    {
                        FeatureReport result = FeatureUtilities.SendToParts(_AssemblyDocument, Feature, Participants);

                        if (result != null)
                        {
                            results.Add(result);
                        }
                    }
                }

                if (results.Count != 0)
                {
                    if (FeatureUtilities.SingleFeatureOptions)
                    {
                        ActionPreFinalize(results);

                        DetailReportControl detailReportControl = new DetailReportControl(_AssemblyDocument, results);

                        detailReportControl.ShowAsChildModal();
                    }
                    else
                    {
                        ActionPreFinalize(results);
                        ActionFinalize(results);
                    }
                    System.IO.FileInfo fi = new System.IO.FileInfo(_AssemblyDocument.FullFileName);

                    string logfile = fi.DirectoryName + "\\" + fi.Name.Substring(0, fi.Name.Length - 4) + ".log";
                    FeatureReport.LogReport(results, logfile);
                }

                Tx.End();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace,
                    "Exception occurred!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);

                Tx.Abort();
            }
            finally
            {
                RefreshControl(_AssemblyDocument as Document);
                _AssemblyDocument.Update2(true);
                this.Parent.Refresh();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ActionPreFinalize(List<FeatureReport> reports)
        {
            foreach (FeatureReport report in reports)
            {
                switch (FeatureUtilities.CbAction)
                {
                    case "Suppress if Succeeded": //Suppress if succeed

                        if (report.ReportStatus == ReportStatusEnum.kSuccess)
                        {
                            report.FinalizeAction = FinalizeActionEnum.kSuppress;
                        }
                        break;

                    case "Suppress always": //Suppress always

                        report.FinalizeAction = FinalizeActionEnum.kSuppress;
                        break;

                    case "Delete if Succeeded": //Delete if succeed

                        if (report.ReportStatus == ReportStatusEnum.kSuccess)
                        {
                            report.FinalizeAction = FinalizeActionEnum.kDeleteAll;
                        }
                        break;

                    case "Delete always": //Delete always

                        report.FinalizeAction = FinalizeActionEnum.kDeleteAll;
                        break;

                    case "None": //None
                    default:
                        break;
                }

                report.Style = FeatureUtilities.RenderStyle;

                foreach (ReportData data in report.ReportDataList)
                {
                    if (data.PartFeature == null)
                        continue;

                    if (data.PartFeature.HealthStatus != HealthStatusEnum.kUpToDateHealth)
                    {
                        switch (FeatureUtilities.CbActionPart)
                        {
                            case "Suppress": //Suppress
                                data.FinalizeAction = FinalizeActionEnum.kSuppress;
                                break;

                            case "Delete": //delete
                                data.FinalizeAction = FinalizeActionEnum.kDeleteAll;
                                break;

                            case "None": //None
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
        private void ActionFinalize(IEnumerable<FeatureReport> reports)
        {
            foreach (FeatureReport report in reports)
            {
                report.ActionFinalize();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void UpdateFeaturesFromAsm(object sender, EventArgs e)
        {
            List<PartFeature> Features = new List<PartFeature>();

            foreach (TreeNode node in TreeView.SelectedNodes)
            {
                if (node.Tag is PartFeature)
                {
                    Features.Add(node.Tag as PartFeature);
                }
            }

            FeatureUtilities.UpdateFeaturesFromAsm(_PartDocument, Features);

            RefreshControl(_PartDocument as Document);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void SelectInventorFeatures()
        {
            SelectSet ss = AdnInventorUtilities.InvApplication.ActiveDocument.SelectSet;

            ss.Clear();

            foreach (TreeNode node in TreeView.SelectedNodes)
            {
                if (node.Tag is PartFeature)
                {
                    ss.Select(node.Tag);
                }
            }
        }
    }
}
