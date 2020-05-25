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
    public class RectangularPatternFeatureMigrator: IFeatureMigrator
    {
        private static PatternMigrationModeEnum _PatternMigrationMode = PatternMigrationModeEnum.kSendToPartsAsCollectionMode;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
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
            return (AsmFeature is RectangularPatternFeature);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsAssociativitySupported(PartFeature partFeature)
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
            RectangularPatternFeature patternFeature = AsmFeature as RectangularPatternFeature;

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
            RectangularPatternFeature AsmFeature)
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
            RectangularPatternFeature AsmFeature)
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

                RectangularPatternFeature[] newFeatures = CopyRectangularPatternFeature(partCompDef,
                    AsmFeature,
                    invTransfo,
                    elementIdx,
                    occurrence,
                    out reports);

                foreach (ReportData reportData in reports)
                {
                    result.addReportData(reportData);
                }

                foreach (RectangularPatternFeature newFeature in newFeatures)
                {
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
        public struct RectangularPatternFeatureData
        {
            public WorkAxis xDirAxis;
            public WorkAxis yDirAxis;

            public bool naturalXDirection;
            public bool naturalYDirection;

            public object xCount;
            public object yCount;

            public object xSpacing;
            public object ySpacing;

            public PatternSpacingTypeEnum xDirectionSpacingType;
            public PatternSpacingTypeEnum yDirectionSpacingType;

            public SketchPoint3D xDirStartPoint3D;
            public SketchPoint3D yDirStartPoint3D;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static RectangularPatternFeatureData newRectangularPatternFeatureData()
        {
            RectangularPatternFeatureData featureData = new RectangularPatternFeatureData();

            featureData.xDirAxis = null;
            featureData.yDirAxis = null;

            featureData.naturalXDirection = true;
            featureData.naturalYDirection = true;

            featureData.xCount = null;
            featureData.yCount = null;

            featureData.xSpacing = null;
            featureData.ySpacing = null;

            featureData.xDirectionSpacingType = PatternSpacingTypeEnum.kDefault;
            featureData.yDirectionSpacingType = PatternSpacingTypeEnum.kDefault;

            featureData.xDirStartPoint3D = null;
            featureData.yDirStartPoint3D = null;

            return featureData;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static RectangularPatternFeature[] CopyRectangularPatternFeature(PartComponentDefinition partCompDef, 
            RectangularPatternFeature AsmFeature, 
            Matrix invTransfo,
            int elementIdx,
            ComponentOccurrence occurrence,
            out ReportData[] reports)
        {
            List<RectangularPatternFeature> newFeatures = new List<RectangularPatternFeature>();

            ObjectCollection ParentFeatures = FeatureUtilities.CopyParentFeatures(partCompDef,
                AsmFeature.Parent.Document as Document,
                AsmFeature.ParentFeatures,
                invTransfo,
                occurrence,
                out reports);

            if (ParentFeatures.Count == 0) return null;

            Sketch3D sketch3D = partCompDef.Sketches3D.Add();

            List<RectangularPatternFeatureData> FeaturesDataList = new List<RectangularPatternFeatureData>();

            //Only along X Axis
            if (AsmFeature.XDirectionEntity != null && AsmFeature.YDirectionEntity == null)
            {
                UnitVector xDirection = FeatureUtilities.GetDirection(AsmFeature.XDirectionEntity);
                xDirection.TransformBy(invTransfo);

                Point xDirStartPoint = null;
                WorkAxis xDirAxis = null;
                SketchPoint3D xDirStartPoint3D = null;

                try
                {
                    xDirStartPoint = FeatureUtilities.GetPoint(AsmFeature.XDirectionStartPoint);
                    xDirStartPoint.TransformBy(invTransfo);

                    xDirAxis = partCompDef.WorkAxes.AddFixed(xDirStartPoint, xDirection, FeatureUtilities.ConstructionWorkAxis);

                    xDirStartPoint3D = sketch3D.SketchPoints3D.Add(xDirStartPoint, false);
                }
                catch
                {
                    xDirAxis = partCompDef.WorkAxes.AddFixed(partCompDef.WorkPoints[1].Point, xDirection, FeatureUtilities.ConstructionWorkAxis);
                }

                double count1 = (double)(AsmFeature.XCount.Value) - elementIdx + 1;

                //Check it's not the last pattern element
                if (count1 != 0 && elementIdx != (double)(AsmFeature.XCount.Value))
                {
                    RectangularPatternFeatureData featureData = newRectangularPatternFeatureData();

                    featureData.xCount = count1;

                    featureData.xSpacing = AsmFeature.XSpacing.Value;

                    featureData.naturalXDirection = AsmFeature.NaturalXDirection;

                    featureData.xDirectionSpacingType = AsmFeature.XDirectionSpacingType;

                    featureData.xDirAxis = xDirAxis;

                    featureData.xDirStartPoint3D = xDirStartPoint3D;

                    FeaturesDataList.Add(featureData);
                }

                double count2 = elementIdx;

                //Check it's not the first pattern element
                if (count2 != 0 && elementIdx != 1)
                {
                    RectangularPatternFeatureData featureData = newRectangularPatternFeatureData();

                    featureData.xCount = count2;

                    featureData.xSpacing = AsmFeature.XSpacing.Value;

                    featureData.naturalXDirection = !AsmFeature.NaturalXDirection;

                    featureData.xDirectionSpacingType = AsmFeature.XDirectionSpacingType;

                    featureData.xDirAxis = xDirAxis;

                    featureData.xDirStartPoint3D = xDirStartPoint3D;

                    FeaturesDataList.Add(featureData);
                }
            }

            //Only along Y Axis
            if (AsmFeature.YDirectionEntity != null && AsmFeature.XDirectionEntity == null)
            {
               
            }

            //Only along both Axes
            if (AsmFeature.XDirectionEntity != null && AsmFeature.YDirectionEntity != null)
            {
               
            }

            foreach (RectangularPatternFeatureData featureData in FeaturesDataList)
            {
                RectangularPatternFeature newFeature = partCompDef.Features.RectangularPatternFeatures.Add(ParentFeatures,

                    featureData.xDirAxis,
                    featureData.naturalXDirection,
                    featureData.xCount,
                    featureData.xSpacing,
                    featureData.xDirectionSpacingType,
                    featureData.xDirStartPoint3D,

                    featureData.yDirAxis,
                    featureData.naturalYDirection,
                    featureData.yCount,
                    featureData.ySpacing,
                    featureData.yDirectionSpacingType,
                    featureData.yDirStartPoint3D,

                    AsmFeature.ComputeType,
                    AsmFeature.OrientationMethod);

                foreach (FeaturePatternElement element in newFeature.PatternElements)
                {
                    if (newFeature.HealthStatus == HealthStatusEnum.kUpToDateHealth)
                    {
                        break;
                    }

                    if (element.Faces.Count == 0 && element.Index != 1)
                    {
                        element.Suppressed = true;
                    }
                }

                if (newFeature != null) newFeatures.Add(newFeature);
            }

            return newFeatures.ToArray();
        }      
    }
}
