using System;
using System.ComponentModel;
using System.Windows.Forms;
using FeatureMigratorAddin.Utilities;
using Inventor;

namespace FeatureMigratorAddin.UI
{
    public partial class FeatureMigratorDockWnd : Form
    {
        ApplicationAddInSite _addInSite;

        ApplicationEvents _applicationEvents;

        public FeatureMigratorDockWnd(
           Inventor.ApplicationAddInSite addInSite,
           Inventor.DockingStateEnum initialDockingState)
        {
            if (addInSite == null) // We can't build the dockable window without the add-in site object.
                throw new ArgumentNullException("addInSite");

            _addInSite = addInSite;

            _applicationEvents = addInSite.Application.ApplicationEvents;

            _applicationEvents.OnActivateDocument += ApplicationEvents_OnActivateDocument;

            _applicationEvents.OnCloseDocument += _applicationEvents_OnCloseDocument;

            InitializeComponent();

            _browserControl.Initialize();

            _browserControl.RefreshControl(addInSite.Application.ActiveDocument);

            // Make sure the components object is created. (The designer doesn't always create it.)
            if (components == null)
                components = new Container();

            // Create the DockableWindow using a managed wrapper and add it to the components collection.
            components.Add(
                new DockableWindowWrapper(addInSite, this, initialDockingState),
                typeof(DockableWindowWrapper).Name);
        }

        void _applicationEvents_OnCloseDocument(_Document DocumentObject, string FullDocumentName, EventTimingEnum BeforeOrAfter, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            if (AdnInventorUtilities.InvApplication.Documents.Count == 0)
            {
                RefreshControl();
            }
            HandlingCode = HandlingCodeEnum.kEventHandled;
        }
        
        void ApplicationEvents_OnActivateDocument(_Document DocumentObject, EventTimingEnum BeforeOrAfter, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            HandlingCode = HandlingCodeEnum.kEventNotHandled;

            if (BeforeOrAfter == EventTimingEnum.kAfter)
                RefreshControl();
        }

        public void RefreshControl()
        {
            _browserControl.RefreshControl(_addInSite.Application.ActiveDocument);
        }
    }
}
