using System;
using System.Collections.Generic;
using Inventor;

namespace FeatureMigratorLib
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // The meaning of the IFeatureMigrator interface methods is pretty straightforward
    //   
    //  IsMigrationSupported
    //  This method determines if an Assembly Feature is going to appear in the FeatureMigrator browser. Obviously the specific Feature Migrator should return « true » only if it is able to migrate this feature type.
    //   
    //  IsAssociativitySupported   
    //  This method determines if a Part Feature is going to appear in the FeatureMigrator browser. Obviously the specific Feature Migrator should return « true » only if it is able to update this feature type.
    //
    //  SendToParts
    //  This method performs the actual migration of an Assembly Feature into each (copied) Part associated with the Participants list.
    //     
    //  CopyFeature
    //  This method is copying an existing assembly feature in the Part associated with the  PartComponentDefinition argument. The newly created PartFeature is returned.
    //   
    //  IsPartFeatureUpToDate
    //  Determines if the Part Feature is « up-to-date » regarding to the Assembly Feature. The result is used to create the TreeNode Status in the Part Browser.
    //
    //  UpdateFeatureFromAsm
    //  This methods performs the update of the Part Feature regarding the current properties of the Assembly Feature.
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public interface IFeatureMigrator
    {
        bool IsMigrationSupported(PartFeature AsmFeature);

        bool IsAssociativitySupported(PartFeature PartFeature);

        FeatureReport SendToParts(AssemblyDocument ParentDocument,
            List<ComponentOccurrence> Participants,
            PartFeature AsmFeature);

        PartFeature CopyFeature(PartComponentDefinition partCompDef,
           PartFeature AsmFeature,
           Matrix invTransfo);

        bool IsPartFeatureUpToDate(PartFeature AsmFeature, 
            PartFeature PartFeature, 
            Matrix invTransfo);

        bool UpdateFeatureFromAsm(PartFeature AsmFeature,
            PartFeature PartFeature,
            Matrix invTransfo);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Implements a default FeatureMigrator used for unsupported features
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    class DefaultFeatureMigrator: IFeatureMigrator
    {
        public bool IsMigrationSupported(PartFeature AsmFeature)
        {
            return false;
        }

        public bool IsAssociativitySupported(PartFeature PartFeature)
        {
            return false;
        }

        public FeatureReport SendToParts(AssemblyDocument ParentDocument,
            List<ComponentOccurrence> Participants,
            PartFeature AsmFeature)
        {
            return null;
        }

        public PartFeature CopyFeature(PartComponentDefinition partCompDef,
           PartFeature AsmFeature,
           Matrix invTransfo)
        {
            return null;
        }

        public bool IsPartFeatureUpToDate(PartFeature AsmFeature,
            PartFeature PartFeature,
            Matrix invTransfo)
        {
            return false;
        }

        public bool UpdateFeatureFromAsm(PartFeature AsmFeature,
            PartFeature PartFeature,
            Matrix invTransfo)
        {
            return false;
        }
    }

}
