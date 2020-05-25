////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Philippe Leefsma 2011 - ADN/Developer Technical Services
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using FeatureMigratorAddin.Utilities;

namespace FeatureMigratorAddin.Addin
{
    [RunInstaller(true)]
    public class AddinInstaller : Installer
    {
        #region Base Implementation

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion

        public AddinInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            try
            {
                base.Install(stateSaver);

                InstallUtils.InstallModeEnum installMode = InstallUtils.InstallModeEnum.kRegistryFree;

                Assembly Asm = Assembly.GetExecutingAssembly();
                FileInfo asmFile = new FileInfo(Asm.Location);

                stateSaver.Add("InstallMode", (int)installMode);

                switch (installMode)
                {
                    case InstallUtils.InstallModeEnum.kRegistryFree:

                        stateSaver.Add("PathToAddinFile",
                            InstallUtils.InstallRegistryFree(
                                stateSaver,
                                Asm,
                                InstallUtils.RegFreeModeEnum.kVersionIndep,
                                string.Empty));

                        // Example for version dependent

                        //stateSaver.Add("PathToAddinFile",
                        //    InstallUtils.InstallRegistryFree(
                        //        stateSaver,
                        //        Asm,
                        //        InstallUtils.RegFreeMode.kVersionDep,
                        //        "Inventor 2012"));

                        break;

                    case InstallUtils.InstallModeEnum.kRegistry:

                        InstallUtils.InstallRegistry(stateSaver, GetType().Assembly);
                        break;

                    case InstallUtils.InstallModeEnum.kBoth:

                        InstallUtils.InstallRegistry(stateSaver, GetType().Assembly);

                        //Only value that works for kBoth install mode is kUserOverride
                        stateSaver.Add("PathToAddinFile",
                           InstallUtils.InstallRegistryFree(
                               stateSaver,
                               Asm,
                               InstallUtils.RegFreeModeEnum.kUserOverride,
                               "Inventor 2012"));
                        break;

                    default:
                        break;
                }
            }
            catch (InstallException ex)
            {
                throw new InstallException(ex.Message);
            }
            catch
            {
                throw new InstallException("Error installing addin!");
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                InstallUtils.InstallModeEnum installMode =
                    (InstallUtils.InstallModeEnum)savedState["InstallMode"];

                string pathToAddinFile;

                switch (installMode)
                {
                    case InstallUtils.InstallModeEnum.kRegistryFree:
                        pathToAddinFile = (string)savedState["PathToAddinFile"];
                        InstallUtils.UninstallRegistryFree(savedState, pathToAddinFile);
                        break;

                    case InstallUtils.InstallModeEnum.kRegistry:
                        InstallUtils.UninstallRegistry(savedState, GetType().Assembly);
                        break;

                    case InstallUtils.InstallModeEnum.kBoth:
                        InstallUtils.UninstallRegistry(savedState, GetType().Assembly);
                        pathToAddinFile = (string)savedState["PathToAddinFile"];
                        InstallUtils.UninstallRegistryFree(savedState, pathToAddinFile);
                        break;

                    default:
                        break;
                }
            }
            catch
            {

            }

            base.Uninstall(savedState);
        }
    }
}

