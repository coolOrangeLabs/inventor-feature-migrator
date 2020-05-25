using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using FeatureMigratorAddin.Commands;
using FeatureMigratorAddin.UI;
using FeatureMigratorAddin.Utilities;
using FeatureMigratorLib;
using Inventor;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// FeatureMigratorAddin Inventor Add-in
//  
// Author: Felipe
// Creation date: 11/10/2011 12:04:31 PM
// 
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace FeatureMigratorAddin.Addin
{
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [Guid("4418349c-99b0-4e77-8769-3ffd7322c9f3"), ComVisible(true)]
    public class StandardAddInServer : Inventor.ApplicationAddInServer
    {
        // Create a logger for use in this class
        private static log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        // Inventor application object.
        private Inventor.Application m_inventorApplication;

        private FeatureMigratorDockWnd _dockableWindow;

        public StandardAddInServer()
        {
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            FileInfo fi = new FileInfo(thisAssembly.Location + ".log4net");
            log4net.GlobalContext.Properties["LogFileName"] = fi.DirectoryName + "\\Log\\featureMigrator";
            log4net.Config.XmlConfigurator.Configure(fi);
            log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        #region ApplicationAddInServer Members

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            // This method is called by Inventor when it loads the addin.
            // The AddInSiteObject provides access to the Inventor Application object.
            // The FirstTime flag indicates if the addin is loaded for the first time.

            // Initialize AddIn members.
            m_inventorApplication = addInSiteObject.Application;

            //Prevent user from unloading the add-in (except at Inventor shutdown).
            //This workarounds an issue concerning unloading/reloading an addin that
            //creates controls in a native Inventor Panel.
            //addInSiteObject.Parent.UserUnloadable = false;


            Type addinType = this.GetType();

            AdnInventorUtilities.Initialize(m_inventorApplication, addinType);

            //Initialize the FeatureUtilities library
            FeatureUtilities.Initialize(m_inventorApplication);

            //Initialize FeatureMigratorss for each type of feature we support
            FeatureUtilities.SetFeatureMigrator(ObjectTypeEnum.kExtrudeFeatureObject,
                new FeatureMigratorLib.ExtrudeFeatureMigrator());

            FeatureUtilities.SetFeatureMigrator(ObjectTypeEnum.kHoleFeatureObject,
                new FeatureMigratorLib.HoleFeatureMigrator());

            FeatureUtilities.SetFeatureMigrator(ObjectTypeEnum.kCircularPatternFeatureObject,
                new FeatureMigratorLib.CircularPatternFeatureMigrator());

            FeatureUtilities.SetFeatureMigrator(ObjectTypeEnum.kRectangularPatternFeatureObject,
                new FeatureMigratorLib.RectangularPatternFeatureMigrator());

            AdnCommand.AddCommand(
                new FeatureMigratorAsmCtrlCmd(
                    m_inventorApplication, 
                    addInSiteObject, 
                    _dockableWindow));

            AdnCommand.AddCommand(
                new FeatureMigratorPartCtrlCmd(
                    m_inventorApplication, 
                    addInSiteObject, 
                    _dockableWindow));

            AdnCommand.AddCommand(
                new FeatureMigratorSettingsCtrlCmd(
                    m_inventorApplication));

            AdnCommand.AddCommand(
                new FeatureMigratorHelpCtrlCmd(
                    m_inventorApplication));

            AdnCommand.AddCommand(
                new FeatureMigratorAboutCtrlCmd(
                    m_inventorApplication));
            
            // Only after all commands have been added,
            // load Ribbon UI from customized xml file.
            // Make sure "InternalName" of above commands is matching 
            // "internalName" tag described in xml of corresponding command.
            AdnRibbonBuilder.CreateRibbon(
                m_inventorApplication,
               addinType,
               "FeatureMigratorAddin.resources.ribbons.xml");
        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation

            // Release objects.
            Marshal.ReleaseComObject(m_inventorApplication);
            m_inventorApplication = null;

            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation
        {
            // This property is provided to allow the AddIn to expose an API 
            // of its own to other programs. Typically, this  would be done by
            // implementing the AddIn's API interface in a class and returning 
            // that class object through this property.

            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }

        #region COM Registration

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [ComRegisterFunction]
        private static void Register(Type t)
        {
            try
            {
                AdnAddInRegistration.RegisterAddIn(t,
                    SupportedSoftwareVersionEnum.kSupportedSoftwareVersionGreaterThan,
                    "15..",
                    true,
                    "1");
            }
            catch
            {
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [ComUnregisterFunction]
        private static void Unregister(Type t)
        {
            try
            {
                AdnAddInRegistration.UnregisterAddIn(t);
            }
            catch
            {
            }
        }

        #endregion

        #endregion
    }
}
