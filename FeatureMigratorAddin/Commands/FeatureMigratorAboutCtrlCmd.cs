using System;
using System.Reflection;
using FeatureMigratorAddin.Addin;
using FeatureMigratorAddin.Utilities;
using Inventor;

namespace FeatureMigratorAddin.Commands
{
    class FeatureMigratorAboutCtrlCmd : AdnButtonCommandBase
    {

        public FeatureMigratorAboutCtrlCmd(Application Application) : base(Application)
        {
        }

        public override string DisplayName
        {
            get { return "About"; }
        }

        public override string InternalName
        {
            get { return "coolOrange.FeatureMigratorAddin.FeatureMigratorAboutCtrlCmd"; }
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
            get { return "Displays featureMigrator Info"; }
        }

        public override string ToolTipText
        {
            get { return "Displays featureMigrator Info"; }
        }

        public override ButtonDisplayEnum ButtonDisplay
        {
            get { return ButtonDisplayEnum.kDisplayTextInLearningMode; }
        }

        public override string IconName
        {
            get
            {
                return "FeatureMigratorAddin.resources.about.ico";
            }
        }

        protected override void OnExecute(NameValueMap context)
        {
            var frmSplashAbout = new FrmSplash()
            {
                lblInfo = {Text = "Free License"},
                versionlbl = {Text = "2019"},
                buildlbl = { Text = Assembly.GetExecutingAssembly().GetName().Version.ToString() },
                BackgroundImage = resources.featureMigrator1
            };
            RegisterCommandForm(frmSplashAbout, true);
        }

        protected override void OnHelp(NameValueMap context)
        {
            throw new NotImplementedException();
        }
    }
}
