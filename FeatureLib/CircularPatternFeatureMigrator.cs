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
    public class CircularPatternFeatureMigrator: IFeatureMigrator
    {
        private static PatternMigrationModeEnum _PatternMigrationMode = PatternMigrationModeEnum.kSendToPartsAsCollectionMode;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // The PatternMigrationMode for this Manager
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static PatternMigrationModeEnum SendToPartsMode
        {
            set
            {
                _PatternMigrationMode = value;
            }
            get
            {
                return _PatternMigrationMode;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsMigrationSupported(PartFeature AsmFeature)
        {
            return (AsmFeature is CircularPatternFeature);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsAssociativitySupported(PartFeature PartFeature)
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public FeatureReport SendToParts(AssemblyDocument ParentDocument,
            List<ComponentOccurrence> Participants,
            PartFeature AsmFeature)
        {
            CircularPatternFeature patternFeature = AsmFeature as CircularPatternFeature;

            switch (_PatternMigrationMode)
            { 
                case PatternMigrationModeEnum.kSendToPartsAsCollectionMode:
                    return SendToPartsAsCollection(ParentDocument, Participants, patternFeature);

                case PatternMigrationModeEnum.kSendToPartsAsPatternMode:
                    return SendToPartsAsPattern(ParentDocument, Participants, patternFeature);

                default:
                    return null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PartFeature CopyFeature(PartComponentDefinition partCompDef,
           PartFeature AsmFeature,
           Matrix invTransfo)
        {
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsPartFeatureUpToDate(PartFeature AsmFeature,
           PartFeature PartFeature,
           Matrix invTransfo)
        {
           return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool UpdateFeatureFromAsm(PartFeature AsmFeature,
             PartFeature PartFeature,
             Matrix invTransfo)
        {
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static FeatureReport SendToPartsAsCollection(AssemblyDocument ParentDocument,
            List<ComponentOccurrence> Participants,
            CircularPatternFeature AsmFeature)
        {
            FeatureReport result = new FeatureReport(AsmFeature as PartFeature);

            if (FeatureUtilities.CreateBackupFile)
                FeatureUtilities.BackupFile(Participants);

            foreach (ComponentOccurrence occurrence in Participants)
            {
                if (!FeatureUtilities.IsValidOccurrence(occurrence))
                {
                    continue;
                }

                //Find if a pattern element affects this occurrence
                foreach (FeaturePatternElement element in AsmFeature.PatternElements)
                {
                    bool affected = false;

                    foreach (FaceProxy faceProxy in element.Faces)
                    {
                        if (faceProxy.ContainingOccurrence == occurrence)
                        {
                            affected = true;
                            break;
                        }
                    }

                    //first occurrence won't be migrated to parts. 
                    //Need to migrate parent feature for that
                    if (!affected)
                        continue;

                    Matrix invTransfo = occurrence.Transformation;

                    Matrix patternElemTransfo = element.Transform;
                    patternElemTransfo.Invert();

                    invTransfo.TransformBy(patternElemTransfo);

                    invTransfo.Invert();

                    PartComponentDefinition partCompDef = occurrence.Definition as PartComponentDefinition;

                    ReportData[] reports = null;

                    ObjectCollection ParentFeatures = FeatureUtilities.CopyParentFeatures(partCompDef,
                        ParentDocument as Document,
                        AsmFeature.ParentFeatures,
                        invTransfo,
                        occurrence,
                        out reports);

                    foreach (ReportData reportData in reports)
                    {
                        //Place Feature Tag: associativity handling
                        //Not supported yet
                        /*FeatureAttributeManager.CreatePartFeatureTag(ParentDocument, 
                            AsmFeature as PartFeature, 
                            reportData.PartFeature,
                            occurrence);*/

                        result.addReportData(reportData);
                    }
                }
            }

            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static FeatureReport SendToPartsAsPattern(AssemblyDocument ParentDocument,
            List<ComponentOccurrence> Participants,
            CircularPatternFeature AsmFeature)
        {
            FeatureReport result = new FeatureReport(AsmFeature as PartFeature);
            
            if (FeatureUtilities.CreateBackupFile)
                FeatureUtilities.BackupFile(Participants);

            foreach (ComponentOccurrence occurrence in Participants)
            {
                if (!FeatureUtilities.IsValidOccurrence(occurrence))
                {
                    continue;
                }

                bool loop = true;

                int elementIdx = 1;

                Matrix patternElemTransfo = FeatureUtilities.Application.TransientGeometry.CreateMatrix();

                //Find first pattern element that affect this occurrence
                foreach (FeaturePatternElement element in AsmFeature.PatternElements)
                {
                    foreach (FaceProxy faceProxy in element.Faces)
                    {
                        if (faceProxy.ContainingOccurrence == occurrence)
                        {
                            patternElemTransfo = element.Transform;
                            patternElemTransfo.Invert();

                            elementIdx = element.Index;

                            loop = false;
                            break;
                        }
                    }

                    if (!loop) break;
                }

                Matrix invTransfo = occurrence.Transformation;

                invTransfo.TransformBy(patternElemTransfo);

                invTransfo.Invert();


                PartComponentDefinition partCompDef = occurrence.Definition as PartComponentDefinition;

                ReportData[] reports = null;

                CircularPatternFeature[] newFeatures = CopyCircularPatternFeature(partCompDef, 
                    AsmFeature, 
                    invTransfo, 
                    elementIdx,
                    occurrence,
                    out reports);

                foreach (ReportData reportData in reports)
                {
                    result.addReportData(reportData);
                }

                foreach (CircularPatternFeature newFeature in newFeatures)
                {
                    double count = (double)(newFeature.Count.Value);

                    if (newFeature.HealthStatus != HealthStatusEnum.kUpToDateHealth)
                    {
                        foreach (FeaturePatternElement element in newFeature.PatternElements)
                        {
                            if (element.Faces.Count == 0 && element.Index != 1)
                            {
                                element.Suppressed = true;
                                --count;
                            }
                        }
                    }

                    //Single pattern occurrence -> delete the pattern feature
                    if (count == 1)
                    {
                        newFeature.Delete(false, false, false);
                        continue;
                    }

                    result.addReportData(new ReportData(partCompDef.Document as PartDocument, 
                        newFeature as PartFeature));
                }
            }

            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private struct CircularPatternFeatureData
        { 
            public WorkAxis axis;
            public bool naturalAxisDirection;
            public double count;
            public double angle;
            public bool fitWithinAngle;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static CircularPatternFeature[] CopyCircularPatternFeature(PartComponentDefinition partCompDef,
            CircularPatternFeature Feature,
            Matrix invTransfo,
            int elementIdx,
            ComponentOccurrence occurrence,
            out ReportData[] reports)
        {
            List<CircularPatternFeature> newFeatures = new List<CircularPatternFeature>();

            ObjectCollection ParentFeatures = FeatureUtilities.CopyParentFeatures(partCompDef, 
                Feature.Parent.Document as Document, 
                Feature.ParentFeatures, 
                invTransfo,
                occurrence,
                out reports);

            if (ParentFeatures.Count == 0) 
                return null;
           
            List<CircularPatternFeatureData> FeaturesDataList = new List<CircularPatternFeatureData>();

            WorkAxis axis = FeatureUtilities.GetAxis(partCompDef, Feature.AxisEntity, invTransfo);

            double step = (double)Feature.Angle.Value / (double)Feature.Count.Value;

            double count1 = (double)Feature.Count.Value - elementIdx + 1;

            //Check it's not the last pattern element
            if (elementIdx != (double)(Feature.Count.Value))
            {
                CircularPatternFeatureData featureData = new CircularPatternFeatureData();

                featureData.axis = axis;
                featureData.naturalAxisDirection = Feature.NaturalAxisDirection;
                featureData.count = count1;
                featureData.angle = step * (count1 - 1);
                featureData.fitWithinAngle = Feature.FitWithinAngle;

                FeaturesDataList.Add(featureData);
            }

            double count2 = elementIdx;

            //Check it's not the first pattern element
            if (elementIdx != 1)
            {
                CircularPatternFeatureData featureData = new CircularPatternFeatureData();

                featureData.axis = axis;
                featureData.naturalAxisDirection = !Feature.NaturalAxisDirection;
                featureData.count = count2;
                featureData.angle = step * (count2 - (elementIdx == (double)(Feature.Count.Value) ? 0 : 1));
                featureData.fitWithinAngle = Feature.FitWithinAngle;

                FeaturesDataList.Add(featureData);
            }

            foreach (CircularPatternFeatureData featureData in FeaturesDataList)
            {
                CircularPatternFeature newFeature = partCompDef.Features.CircularPatternFeatures.Add(ParentFeatures,
                        featureData.axis,
                        featureData.naturalAxisDirection,
                        featureData.count,
                        featureData.angle,
                        featureData.fitWithinAngle,
                        Feature.ComputeType);

                if (newFeature != null)
                {
                    newFeatures.Add(newFeature);
                }
            }

            return newFeatures.ToArray();
        }
    }
}
