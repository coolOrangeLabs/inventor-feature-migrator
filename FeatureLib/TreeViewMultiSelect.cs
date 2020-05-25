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
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Inventor;


namespace FeatureMigratorLib
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // TreeViewMultiSelect implements a multi-selectable TreeView control.
    // the Treeview node can be sorted as well, though this functionality is not used here
    // because the FeatureMigrator displays the features as they appear in Inventor native browser.
    //  
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class TreeViewMultiSelect : System.Windows.Forms.TreeView
    {
        protected List<TreeNode> _SelectedNodes;

        protected TreeNode _lastSelected;

        private NodeSorter _NodeSorter;

        public event SelectNodeHandler SelectNodeEvent;

        public TreeViewMultiSelect()
        {
            _lastSelected = null;
            _SelectedNodes = new List<TreeNode>();

            _NodeSorter = new NodeSorter();

            TreeViewNodeSorter = _NodeSorter;

            Sort();
        }

        public void SetSortType(SortType sortType)
        {
            _NodeSorter.SetSortType(sortType);
        }

        public void ClearSelection()
        {
            _lastSelected = null;

            foreach (TreeNode node in _SelectedNodes)
            {
                SetSelected(node, false, false);
            }

            _SelectedNodes.Clear();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public List<TreeNode> SelectedNodes
        {
            get
            {
                return _SelectedNodes;
            }
        }

        public List<TreeNode> SelectedNodesBottomUp
        {
            get
            {
                List<TreeNode> NodesBottomUp = new List<TreeNode>();

                while (_SelectedNodes.Count != 0)
                {
                    TreeNode selectedNode = _SelectedNodes[0];

                    for (int i = 1; i < _SelectedNodes.Count; ++i)
                    {
                        TreeNode node = _SelectedNodes[i];

                        if (node.Index > selectedNode.Index)
                        {
                            selectedNode = node;
                        }
                    }

                    NodesBottomUp.Add(selectedNode);

                    _SelectedNodes.Remove(selectedNode);
                }

                return NodesBottomUp;
            }
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                base.OnNodeMouseClick(e);
                return;
            }

            bool bControl = (ModifierKeys == Keys.Control);
            bool bShift = (ModifierKeys == Keys.Shift);

            if (bControl)
            {
                if (!_SelectedNodes.Contains(e.Node))
                {
                    SetSelected(e.Node, true, true);
                }
                else
                {
                    SetSelected(e.Node, false, true);

                    if (e.Node == _lastSelected) _lastSelected = null;
                }

            }
            else if (bShift && (_lastSelected != null))
            {
                if (_lastSelected.Parent == e.Node.Parent)
                {
                    TreeNode ParentNode = _lastSelected.Parent;

                    int minIdx = (_lastSelected.Index < e.Node.Index ? _lastSelected.Index : e.Node.Index);
                    int maxIdx = (_lastSelected.Index > e.Node.Index ? _lastSelected.Index : e.Node.Index);

                    for (int i = minIdx; i < maxIdx + 1; ++i)
                    {
                        TreeNode node = ParentNode.Nodes[i];
                        SetSelected(node, true, true);
                    }
                }
            }
            else
            {
                ClearSelection();

                _lastSelected = e.Node;

                SetSelected(e.Node, true, true);
            }

            base.OnNodeMouseClick(e);
        }

        public void OnSelect(SelectNodeEventArgs e)
        {
            if (SelectNodeEvent != null) 
                SelectNodeEvent(this, e);
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        //protected override void OnAfterSelect(TreeViewEventArgs e)
        //{ 
        //}

        public bool IsNodeSelected(TreeNode node)
        {
            return _SelectedNodes.Contains(node);
        }

        public void SetSelected(TreeNode node, bool selected, bool remove)
        {
            if (selected)
            {
                node.BackColor = SystemColors.Highlight;

                if (node.ForeColor != SystemColors.GrayText)
                {
                    node.ForeColor = SystemColors.HighlightText;
                }

                if (!_SelectedNodes.Contains(node)) 
                    _SelectedNodes.Add(node);
                
                return;
            }

            node.BackColor = this.BackColor;

            if (node.ForeColor != SystemColors.GrayText)
            {
                node.ForeColor = this.ForeColor;
            }

            if (_SelectedNodes.Contains(node) && remove) 
                _SelectedNodes.Remove(node);
        }

        public void SetSuppressedState(TreeNode node, bool Suppressed)
        {
            PartFeature Feature = node.Tag as PartFeature;

            if (Suppressed)
            {
                node.ForeColor = SystemColors.GrayText;
                node.ImageIndex = FeatureUtilities.GetFeatureImageIndex(Feature) + 1;
                node.SelectedImageIndex = node.ImageIndex;
                return;
            }

            node.ForeColor = this.ForeColor;
            node.ImageIndex = FeatureUtilities.GetFeatureImageIndex(Feature);
            node.SelectedImageIndex = node.ImageIndex;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public delegate void SelectNodeHandler(object o, SelectNodeEventArgs e);

    public class SelectNodeEventArgs : EventArgs
    {
        public readonly TreeNode Node;

        public SelectNodeEventArgs(TreeNode node)
        {
            Node = node;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public enum SortType
    {
        kSortTypeAscending,
        kSortTypeDescending,
        kSortTypeNone
    }

    class NodeSorter : System.Collections.IComparer
    {
        SortType mSortType;

        public NodeSorter()
        {
            mSortType = SortType.kSortTypeNone;
        }

        public void SetSortType(SortType sortType)
        {
            mSortType = sortType;
        }

        public int Compare(object x, object y)
        {
            TreeNode xNode = x as TreeNode;
            TreeNode yNode = y as TreeNode;

            switch (mSortType)
            {
                case SortType.kSortTypeAscending:

                    return string.Compare(xNode.Text, yNode.Text);

                case SortType.kSortTypeDescending:

                    return string.Compare(yNode.Text, xNode.Text);

                case SortType.kSortTypeNone:
                default:

                    return 0;
            }
        }
    }
}
