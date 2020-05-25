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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace FeatureMigratorLib
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // The ComboListView allows a ComboBox to be displayed in one of its ListView cell.
    // This feature is used by the detailReportControl listview.
    //
    // Adapted from Microsoft KB Article: http://support.microsoft.com/kb/320344
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ComboListView : System.Windows.Forms.ListView
    {
        //Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        public ComboListView()
        {
           
        }

        //UserControl1 overrides dispose to clean up the component list.
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.
        [System.Diagnostics.DebuggerStepThrough()] 
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;

        protected override void WndProc(ref Message msg)
        {
            //Look for the WM_VSCROLL or the WM_HSCROLL messages.
            if((msg.Msg == WM_VSCROLL) || (msg.Msg == WM_HSCROLL))
            {
                //Move focus to the ListView to cause ComboBox to lose focus.
                Focus();
            }

            //Pass message to default handler.
            base.WndProc(ref msg);
        }
    }
}
