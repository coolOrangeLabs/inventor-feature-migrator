////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved 
// Written by Jan Liska & Philippe Leefsma 2011 - ADN/Developer Technical Services
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

using System;
using FeatureMigratorAddin.Addin;
using FeatureMigratorAddin.UI;
using FeatureMigratorAddin.Utilities;
using Inventor;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// FeatureMigratorPartCtrlCmd Inventor Add-in Command
//  
// Author: Felipe
// Creation date: 11/10/2011 4:15:05 PM
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace FeatureMigratorAddin.Commands
{
    class FeatureMigratorPartCtrlCmd : AdnButtonCommandBase
    {
        ApplicationAddInSite _addInSiteObject;

        FeatureMigratorDockWnd _dockableWindow;

        public FeatureMigratorPartCtrlCmd(
            Inventor.Application Application,
            ApplicationAddInSite addInSiteObject,
            FeatureMigratorDockWnd dockableWindow)
            : base(Application)
        {
            _addInSiteObject = addInSiteObject;

            _dockableWindow = dockableWindow;
        }

        public override string DisplayName
        {
            get
            {
                return "featureMigrator";
            }
        }

        public override string InternalName
        {
            get
            {
                return "coolOrange.FeatureMigratorAddin.FeatureMigratorPartCtrlCmd";
            }
        }

        public override CommandTypesEnum Classification
        {
            get
            {
                return CommandTypesEnum.kEditMaskCmdType;
            }
        }

        public override string ClientId
        {
            get
            {
                Type t = typeof(StandardAddInServer);
                return t.GUID.ToString("B");
            }
        }

        public override string Description
        {
            get
            {
                return "Displays featureMigrator Control";
            }
        }

        public override string ToolTipText
        {
            get
            {
                return "Displays featureMigrator Control";
            }
        }

        public override ButtonDisplayEnum ButtonDisplay
        {
            get
            {
                return ButtonDisplayEnum.kDisplayTextInLearningMode;
            }
        }

        public override string IconName
        {
            get
            {
                return "FeatureMigratorAddin.resources.featureMigrator-AllSize.ico";
            }
        }

        protected override void OnExecute(NameValueMap context)
        {
            if (_dockableWindow == null)
            {
                _dockableWindow = new FeatureMigratorDockWnd(
                    _addInSiteObject,
                    DockingStateEnum.kDockLeft);

                _dockableWindow.Show();
            }
            else
            {
                _dockableWindow.RefreshControl();
                _dockableWindow.Visible = true;
            }

            Terminate();
        }

        protected override void OnHelp(NameValueMap context)
        {

        }

        protected override void OnLinearMarkingMenu(
           ObjectsEnumerator SelectedEntities,
           SelectionDeviceEnum SelectionDevice,
           CommandControls LinearMenu,
           NameValueMap AdditionalInfo)
        {
            // Add this button to linear context menu
            //LinearMenu.AddButton(_controlDef, true, true, "", false);
        }

        protected override void OnRadialMarkingMenu(
            ObjectsEnumerator SelectedEntities,
            SelectionDeviceEnum SelectionDevice,
            RadialMarkingMenu RadialMenu,
            NameValueMap AdditionalInfo)
        {
            // Add this button to radial context menu
            //RadialMenu.NorthControl = _controlDef;
        }
    }
}
