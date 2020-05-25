using System;
using FeatureMigratorAddin.Addin;
using FeatureMigratorAddin.UI;
using FeatureMigratorAddin.Utilities;
using Inventor;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// FeatureMigratorCtrlCmd Inventor Add-in Command
//  
// Author: Felipe
// Creation date: 11/10/2011 2:52:29 PM
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace FeatureMigratorAddin.Commands
{
    class FeatureMigratorAsmCtrlCmd : AdnButtonCommandBase
    {
        ApplicationAddInSite _addInSiteObject;

        FeatureMigratorDockWnd _dockableWindow;

        public FeatureMigratorAsmCtrlCmd(
            Inventor.Application Application,
            ApplicationAddInSite addInSiteObject,
            FeatureMigratorDockWnd dockableWindow) : base(Application)
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
                return "coolOrange.FeatureMigratorAddin.FeatureMigratorAsmCtrlCmd";
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
