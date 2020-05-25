using System;
using FeatureMigratorAddin.Addin;
using FeatureMigratorAddin.Utilities;
using FeatureMigratorLib;
using Inventor;

namespace FeatureMigratorAddin.Commands
{
    class FeatureMigratorSettingsCtrlCmd : AdnButtonCommandBase
    {
        public FeatureMigratorSettingsCtrlCmd(Application application) : base(application)
        {
        }

        public override string DisplayName
        {
            get { return "Settings"; }
        }

        public override string InternalName
        {
            get { return "coolOrange.FeatureMigratorAddin.FeatureMigratorSettingsCtrlCmd"; }
        }

        public override CommandTypesEnum Classification
        {
            get { return CommandTypesEnum.kQueryOnlyCmdType; }
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
            get { return "Displays featureMigrator Settings"; }
        }

        public override string ToolTipText
        {
            get { return "Displays featureMigrator Settings"; }
        }

        public override ButtonDisplayEnum ButtonDisplay
        {
            get { return ButtonDisplayEnum.kDisplayTextInLearningMode; }
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
            var document = Application.ActiveDocument;
            if (document == null)
                return;
            ReportControl reportControl = new ReportControl(document);
            reportControl.ShowAsChildModal();
            Terminate();
        }

        protected override void OnHelp(NameValueMap context)
        {
            throw new NotImplementedException();
        }
    }
}
