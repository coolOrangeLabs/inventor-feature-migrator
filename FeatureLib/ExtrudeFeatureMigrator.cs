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
using Inventor;

namespace FeatureMigratorLib
{

    public class ExtrudeFeatureMigrator : IFeatureMigrator
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsMigrationSupported(PartFeature AsmFeature)
        {
            return (AsmFeature is ExtrudeFeature);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsAssociativitySupported(PartFeature PartFeature)
        {
            return (PartFeature is ExtrudeFeature);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public FeatureReport SendToParts(AssemblyDocument ParentDocument, 
            List<ComponentOccurrence> Participants, 
            PartFeature AsmFeature)
        {
            ExtrudeFeature extrudeFeature = AsmFeature as ExtrudeFeature;

            FeatureReport result = new FeatureReport(AsmFeature);

            if (FeatureUtilities.CreateBackupFile)
                FeatureUtilities.BackupFile(Participants);

            PlanarSketch asmSketch = extrudeFeature.Profile.Parent as PlanarSketch;
            
            foreach (ComponentOccurrence occurrence in Participants)
            {
                if (!FeatureUtilities.IsValidOccurrence(occurrence))
                {
                    continue;
                }

                Matrix invTransfo = occurrence.Transformation;
                invTransfo.Invert();

                PartComponentDefinition partCompDef = occurrence.Definition as PartComponentDefinition;

                PartFeature newFeature = CopyFeature(partCompDef, AsmFeature, invTransfo);
                  
                //Place Feature Tag: associativity handling
                FeatureAttributeManager.CreatePartFeatureTag(ParentDocument, 
                    AsmFeature, 
                    newFeature,
                    occurrence);

                ReportData reportData = new ReportData(partCompDef.Document as PartDocument, 
                    newFeature);

                result.addReportData(reportData);
            }
    
            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PartFeature CopyFeature(PartComponentDefinition partCompDef,
            PartFeature AsmFeature,
            Matrix invTransfo)
        {
            try
            {
                ExtrudeFeature extrudeFeature = AsmFeature as ExtrudeFeature;

                ExtrudeFeature newFeature = null;

                UnitVector xAxis = null;
                UnitVector yAxis = null;

                PlanarSketch asmSketch = extrudeFeature.Profile.Parent as PlanarSketch;

                PlanarSketch partSketch = FeatureUtilities.CopySketch(partCompDef, 
                    asmSketch, 
                    invTransfo, 
                    out xAxis, 
                    out yAxis);

                ObjectCollection pathSegments = FeatureUtilities.GetPathSegments(extrudeFeature.Profile,
                    partSketch);

                Profile partProfile = partSketch.Profiles.AddForSolid(true, pathSegments, null);

                FeatureUtilities.CopyProfile(extrudeFeature.Profile, partProfile);

                switch (extrudeFeature.ExtentType)
                {
                    case PartFeatureExtentEnum.kThroughAllExtent:
                    {
                        ThroughAllExtent ThroughAllExtent = extrudeFeature.Extent as ThroughAllExtent;

                        newFeature = partCompDef.Features.ExtrudeFeatures.AddByThroughAllExtent(partProfile,
                            ThroughAllExtent.Direction,
                            extrudeFeature.Operation,
                            extrudeFeature.TaperAngle.Value);

                        break;
                    }
                    case PartFeatureExtentEnum.kDistanceExtent:
                    {
                        DistanceExtent DistanceExtent = extrudeFeature.Extent as DistanceExtent;

                        newFeature = partCompDef.Features.ExtrudeFeatures.AddByDistanceExtent(partProfile,
                            DistanceExtent.Distance.Value,
                            DistanceExtent.Direction,
                            extrudeFeature.Operation,
                            extrudeFeature.TaperAngle.Value);

                        break;
                    }
                    case PartFeatureExtentEnum.kToExtent:
                    {
                        ToExtent ToExtent = extrudeFeature.Extent as ToExtent;

                        object ToEntity = FeatureUtilities.CopyFromToEntity(ToExtent.ToEntity,
                            partCompDef,
                            invTransfo);
                       
                        newFeature = partCompDef.Features.ExtrudeFeatures.AddByToExtent(partProfile,
                            ToEntity,
                            extrudeFeature.Operation,
                            false,
                            extrudeFeature.TaperAngle.Value);

                        break;
                    }
                    case PartFeatureExtentEnum.kFromToExtent:
                    {
                        FromToExtent FromToExtent = extrudeFeature.Extent as FromToExtent;

                        object FromEntity = FeatureUtilities.CopyFromToEntity(FromToExtent.FromFace,
                            partCompDef,
                            invTransfo);

                        object ToEntity = FeatureUtilities.CopyFromToEntity(FromToExtent.ToFace,
                            partCompDef,
                            invTransfo);

                        newFeature = partCompDef.Features.ExtrudeFeatures.AddByFromToExtent(partProfile,
                            FromEntity,
                            false,
                            ToEntity,
                            false,
                            extrudeFeature.Operation,
                            extrudeFeature.TaperAngle.Value);

                        break;
                    }
                    default:
                        break;
                }

                return newFeature as PartFeature;
            }
            catch
            {
                return null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsPartFeatureUpToDate(PartFeature AsmFeature, 
            PartFeature PartFeature, 
            Matrix invTransfo)
        {
            ExtrudeFeature asmFeature = AsmFeature as ExtrudeFeature;
            ExtrudeFeature partFeature = PartFeature as ExtrudeFeature;

            try
            {
                PlanarSketch asmSketch = asmFeature.Profile.Parent as PlanarSketch;
                PlanarSketch partSketch = partFeature.Profile.Parent as PlanarSketch;

                if (!FeatureUtilities.CompareSketches(asmSketch, partSketch, invTransfo))
                    return false;

                if (!FeatureUtilities.CompareProfiles(asmFeature.Profile, partFeature.Profile))
                    return false;

                if (!FeatureUtilities.IsEqual(asmFeature.TaperAngle.Value, partFeature.TaperAngle.Value))
                    return false;

                if (!FeatureUtilities.CompareFeatureExtents(asmFeature.Extent, partFeature.Extent, invTransfo))
                    return false;

                return true;
            }
            catch
            {
                //Something went wrong
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool UpdateFeatureFromAsm(PartFeature AsmFeature,
            PartFeature PartFeature,
            Matrix invTransfo)
        {
            ExtrudeFeature asmFeature = AsmFeature as ExtrudeFeature;
            ExtrudeFeature partFeature = PartFeature as ExtrudeFeature;

            try
            {
                PartComponentDefinition partCompDef = partFeature.Parent as PartComponentDefinition;

                PlanarSketch partSketch = partFeature.Profile.Parent as PlanarSketch;

                UnitVector xAxis = null;
                UnitVector yAxis = null;

                FeatureUtilities.UpdateSketch(asmFeature.Profile.Parent as PlanarSketch,
                        partSketch,
                        invTransfo,
                        out xAxis,
                        out yAxis);

                Document asmDocument = asmFeature.Parent.Document as Document;

                bool suppressed = partFeature.Suppressed;

                //Feature needs to be suppressed if we change the Profile
                partFeature.Suppressed = true;

                Profile newPartProfile = FeatureUtilities.UpdateProfile(asmDocument,
                    partCompDef.Document as Document,
                    asmFeature.Profile,
                    partFeature.Profile,
                    true);

                if (newPartProfile != null)
                {
                    partFeature.Profile = newPartProfile;
                }

                partFeature.Suppressed = suppressed;

                partFeature.TaperAngle.Value = asmFeature.TaperAngle.Value;

                if (FeatureUtilities.UpdateFeatureExtent(asmFeature.Extent, partFeature.Extent, invTransfo))
                    return true;

                switch (asmFeature.ExtentType)
                {
                    case PartFeatureExtentEnum.kThroughAllExtent:
                    {
                        ThroughAllExtent asmThroughAllExtent = asmFeature.Extent as ThroughAllExtent;

                        partFeature.SetThroughAllExtent(asmThroughAllExtent.Direction);

                        break;
                    }
                    case PartFeatureExtentEnum.kDistanceExtent:
                    {
                        DistanceExtent asmDistanceExtent = asmFeature.Extent as DistanceExtent;

                        partFeature.SetDistanceExtent(asmDistanceExtent.Distance.Value, asmDistanceExtent.Direction);

                        break;
                    }
                    case PartFeatureExtentEnum.kToExtent:
                    {
                        ToExtent asmToExtent = asmFeature.Extent as ToExtent;

                        object ToEntity = FeatureUtilities.CopyFromToEntity(asmToExtent.ToEntity,
                           partCompDef,
                           invTransfo);

                        partFeature.SetToExtent(ToEntity, false);

                        break;
                    }
                    case PartFeatureExtentEnum.kFromToExtent:
                    {
                        FromToExtent asmFromToExtent = asmFeature.Extent as FromToExtent;

                        object FromEntity = FeatureUtilities.CopyFromToEntity(asmFromToExtent.FromFace,
                           partCompDef,
                           invTransfo);

                        object ToEntity = FeatureUtilities.CopyFromToEntity(asmFromToExtent.ToFace,
                           partCompDef,
                           invTransfo);

                        partFeature.SetFromToExtent(FromEntity, false, ToEntity, false);

                        break;
                    }
                    default:
                        return false;
                }

                return true;
            }
            catch
            {
                //Something went wrong
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetSketchBase(ExtrudeFeature ExtrudeFeature)
        {
            PlanarSketch sketch = ExtrudeFeature.Profile.Parent as PlanarSketch;

            return (sketch.PlanarEntity is WorkPlane ? "WorkPlane" : "Face");
        }

    }
}
