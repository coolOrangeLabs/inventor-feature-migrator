using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FeatureMigratorAddin.Addin;
using FeatureMigratorAddin.Utilities;
using Inventor;

namespace FeatureMigratorAddin.Commands
{
    class FeatureMigratorHelpCtrlCmd : AdnButtonCommandBase
    {
        public FeatureMigratorHelpCtrlCmd(Application Application) : base(Application)
        {
        }

        public override string DisplayName
        {
            get { return "Help"; }
        }

        public override string InternalName
        {
            get { return "coolOrange.FeatureMigratorAddin.FeatureMigratorHelpCtrlCmd"; }
        }

        public override CommandTypesEnum Classification
        {
            get { return CommandTypesEnum.kQueryOnlyCmdType;}
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
            get { return "Displays featureMigrator help"; }
        }

        public override string ToolTipText
        {
            get { return "Displays featureMigrator help"; }
        }

        public override ButtonDisplayEnum ButtonDisplay
        {
            get { return ButtonDisplayEnum.kDisplayTextInLearningMode; }
        }

        public override string IconName
        {
            get
            {
                return "FeatureMigratorAddin.resources.Help.ico";
            }
        }

        protected override void OnExecute(NameValueMap context)
        {
            System.Diagnostics.Process.Start("http://wiki.coolorange.com/display/featureMigrator/featureMigrator+Documentation+Home");
            Terminate();
        }

        protected override void OnHelp(NameValueMap context)
        {
            throw new NotImplementedException();
        }
    }
}
