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

namespace FeatureMigratorLib
{
    public partial class FileResolutionDlg : Form
    {
        private string _Filename;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: The FileResolutionDlg is displayed when the user attemps to update a part feature 
        //      that was migrated and the parent assembly is not found either because the location has changed
        //      or the filename was modified.
        //      The user is then prompted to manully locate the original assembly file.
        //      Obviously if this original file is not found, the tool won't be able to perform 
        //      any update of the part feature.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public FileResolutionDlg(string FullFileName, string InternalName)
        {
            InitializeComponent();

            _Filename = "";
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public String Filename
        {
            get
            {
                return _Filename;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void bOk_Click(object sender, EventArgs e)
        {
            Inventor.FileDialog fileDlg = null;
            FeatureUtilities.Application.CreateFileDialog(out fileDlg);

            fileDlg.Filter = "Inventor Assemblies (*.iam)|*.iam";
            fileDlg.FilterIndex = 1;
            fileDlg.DialogTitle = "File resolution";
            fileDlg.InitialDirectory = "C:\\Documents and Settings\\" + System.Windows.Forms.SystemInformation.UserName + "\\Desktop\\";
            //fileDlg.FileName = "C:\Temp\" & strActiveFileName
            fileDlg.MultiSelectEnabled = false;
            fileDlg.OptionsEnabled = true;
            fileDlg.CancelError = true;

            try
            {
                fileDlg.ShowOpen();
                _Filename = fileDlg.FileName;
            }
            catch
            {
                _Filename = "";
            }

            Close();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void bAbort_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
