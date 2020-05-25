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
    public class HoleFeatureMigrator: IFeatureMigrator
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsMigrationSupported(PartFeature AsmFeature)
        {
            return (AsmFeature is HoleFeature);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsAssociativitySupported(PartFeature PartFeature)
        {
            return (PartFeature is HoleFeature);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public FeatureReport SendToParts(AssemblyDocument ParentDocument,
            List<ComponentOccurrence> Participants,
            PartFeature AsmFeature)
        {
            FeatureReport result = new FeatureReport(AsmFeature);

            //result.UnreferencedDocuments = FeatureUtilities.ReplaceOccurrences(ParentDocument, Participants);
            if (FeatureUtilities.CreateBackupFile)
                FeatureUtilities.BackupFile(Participants);

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
                HoleFeature holeFeature = AsmFeature as HoleFeature;

                HoleFeature newFeature = null;

                HolePlacementDefinition PlacementDefinition = CopyHolePlacementDefinition(partCompDef,
                    holeFeature.PlacementDefinition,
                    invTransfo);

                object DiameterOrTapInfo = GetDiameterOrTapInfo(holeFeature);

                switch (holeFeature.ExtentType)
                {
                    case PartFeatureExtentEnum.kThroughAllExtent:
                    {
                        newFeature = AddHoleFeatureByThroughAllExtent(partCompDef, 
                            holeFeature, 
                            PlacementDefinition, 
                            DiameterOrTapInfo);
                        break;
                    }
                    case PartFeatureExtentEnum.kDistanceExtent:
                    {
                        newFeature = AddHoleFeatureByDistanceExtent(partCompDef, 
                            holeFeature, 
                            PlacementDefinition, 
                            DiameterOrTapInfo);
                        break;
                    }
                    case PartFeatureExtentEnum.kToExtent:
                    {
                        newFeature = AddHoleFeatureByToFaceExtent(partCompDef, 
                            holeFeature, 
                            PlacementDefinition, 
                            DiameterOrTapInfo, 
                            invTransfo);
                        break;
                    }
                    default:

                        return null;
                }

                if (holeFeature.Tapped)
                {
                    CopyTapInfo(holeFeature, newFeature);
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
            HoleFeature asmFeature = AsmFeature as HoleFeature;
            HoleFeature partFeature = PartFeature as HoleFeature;

            try
            {
                if (!CompareHolePlacementDefinition(asmFeature.PlacementDefinition, 
                    partFeature.PlacementDefinition, 
                    invTransfo))
                    return false;

                if (!FeatureUtilities.CompareFeatureExtents(asmFeature.Extent, 
                    partFeature.Extent, 
                    invTransfo))
                    return false;

                if (!CompareHoles(asmFeature, partFeature, invTransfo))
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
            HoleFeature asmFeature = AsmFeature as HoleFeature;
            HoleFeature partFeature = PartFeature as HoleFeature;

            try
            {
                PartComponentDefinition partCompDef = partFeature.Parent as PartComponentDefinition;

                if (!UpdateHolePlacementDefinition(asmFeature.PlacementDefinition,
                    partFeature,
                    invTransfo))
                    return false;

                if (!UpdateHole(asmFeature, partFeature))
                    return false;

                bool needExtentTypeChange = !FeatureUtilities.UpdateFeatureExtent(asmFeature.Extent,
                    partFeature.Extent,
                    invTransfo);

                if (!needExtentTypeChange)
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

                        partFeature.SetDistanceExtent(asmDistanceExtent.Distance.Value,
                            asmDistanceExtent.Direction,
                            asmFeature.FlatBottom,
                            GetBottomTipAngle(asmFeature));
                        
                        break;
                    }
                    case PartFeatureExtentEnum.kToExtent:
                    {
                        ToExtent asmToExtent = asmFeature.Extent as ToExtent;

                        object ToEntity = FeatureUtilities.CopyFromToEntity(asmToExtent.ToEntity,
                           partCompDef,
                           invTransfo);

                        partFeature.SetToFaceExtent(ToEntity, false);

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
        // Returns text string depending on sketch placement type. 
        // Used for display in browser
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetPlacementType(HoleFeature Feature)
        {
            switch (Feature.PlacementType)
            { 
                case HolePlacementTypeEnum.kSketchPlacementType:

                    SketchHolePlacementDefinition shpDefOrig = Feature.PlacementDefinition as SketchHolePlacementDefinition;

                    SketchPoint sketchPoint = shpDefOrig.HoleCenterPoints[1] as SketchPoint;

                    PlanarSketch sketch = sketchPoint.Parent as PlanarSketch;

                    string strBased = " - Based: " + (sketch.PlanarEntity is WorkPlane ? "WorkPlane" : "Face");

                    return "Sketch Placement" + strBased;

                case HolePlacementTypeEnum.kLinearPlacementType:
                    return "Linear Placement";

                case HolePlacementTypeEnum.kConcentricPlacementType:
                    return "Concentric Placement";

                case HolePlacementTypeEnum.kPointPlacementType:
                    return "Point Placement";

                default:
                    return "Unknown Placement";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static HolePlacementDefinition CopyHolePlacementDefinition(PartComponentDefinition partCompDef,
            HolePlacementDefinition PlacementDefinition,
            Matrix invTransfo)
        {
            switch (PlacementDefinition.Type)
            {
                case ObjectTypeEnum.kSketchHolePlacementDefinitionObject:
                {
                    SketchHolePlacementDefinition shpDefOrig = PlacementDefinition as SketchHolePlacementDefinition;

                    PlanarSketch sketch = (shpDefOrig.HoleCenterPoints[1] as SketchPoint).Parent as PlanarSketch;

                    PlanarSketch newSketch = FeatureUtilities.CopySketchEmpty(partCompDef, sketch, invTransfo);

                    ObjectCollection holeCenterPoints = FeatureUtilities.Application.TransientObjects.CreateObjectCollection(null);

                    foreach (SketchPoint point in shpDefOrig.HoleCenterPoints)
                    {
                        Point modelPoint = sketch.SketchToModelSpace(point.Geometry);

                        modelPoint.TransformBy(invTransfo);

                        Point2d sketchPointDest = newSketch.ModelToSketchSpace(modelPoint);

                        holeCenterPoints.Add(newSketch.SketchPoints.Add(sketchPointDest, point.HoleCenter));
                    }

                    SketchHolePlacementDefinition shpDef = partCompDef.Features.HoleFeatures.CreateSketchPlacementDefinition(holeCenterPoints);

                    return shpDef as HolePlacementDefinition;
                }
                case ObjectTypeEnum.kLinearHolePlacementDefinitionObject:
                {
                    LinearHolePlacementDefinition lhpDefOrig = PlacementDefinition as LinearHolePlacementDefinition;

                    Point point = lhpDefOrig.Parent.HoleCenterPoints[1] as Point;
                    point.TransformBy(invTransfo);

                    UnitVector axis = FeatureUtilities.GetDirection(lhpDefOrig.Plane);
                    axis.TransformBy(invTransfo);

                    WorkPoint workpoint = partCompDef.WorkPoints.AddFixed(point, FeatureUtilities.ConstructionWorkPoint);
                    WorkAxis workaxis = partCompDef.WorkAxes.AddFixed(point, axis, FeatureUtilities.ConstructionWorkAxis);

                    //CreateLinearPlacementDefinition
                    //Cannot be used here because "ReferenceEntityOne" & "ReferenceEntityTwo" need to be an edge

                    PointHolePlacementDefinition phpDef = partCompDef.Features.HoleFeatures.CreatePointPlacementDefinition(workpoint, workaxis);

                    return phpDef as HolePlacementDefinition;
                }
                case ObjectTypeEnum.kConcentricHolePlacementDefinitionObject:
                {
                    ConcentricHolePlacementDefinition chpDefOrig = PlacementDefinition as ConcentricHolePlacementDefinition;

                    Point point = chpDefOrig.Parent.HoleCenterPoints[1] as Point;
                    point.TransformBy(invTransfo);

                    UnitVector axis = FeatureUtilities.GetDirection(chpDefOrig.Plane);
                    axis.TransformBy(invTransfo);

                    WorkPoint workpoint = partCompDef.WorkPoints.AddFixed(point, FeatureUtilities.ConstructionWorkPoint);
                    WorkAxis workaxis = partCompDef.WorkAxes.AddFixed(point, axis, FeatureUtilities.ConstructionWorkAxis);

                    //CreateConcentricPlacementDefinition
                    //Cannot be used here because "ConcentricReference" need to be a circular Edge or a cylindrical Face

                    PointHolePlacementDefinition phpDef = partCompDef.Features.HoleFeatures.CreatePointPlacementDefinition(workpoint, workaxis);

                    return phpDef as HolePlacementDefinition;
                }
                case ObjectTypeEnum.kPointHolePlacementDefinitionObject:
                {
                    PointHolePlacementDefinition phpDefOrig = PlacementDefinition as PointHolePlacementDefinition;

                    WorkPoint workpointOrig = phpDefOrig.Point as WorkPoint;

                    Point point = workpointOrig.Point;
                    point.TransformBy(invTransfo);

                    UnitVector axis = FeatureUtilities.GetDirection(phpDefOrig.Direction);
                    axis.TransformBy(invTransfo);

                    WorkPoint workpoint = partCompDef.WorkPoints.AddFixed(point, FeatureUtilities.ConstructionWorkPoint);
                    WorkAxis workaxis = partCompDef.WorkAxes.AddFixed(point, axis, FeatureUtilities.ConstructionWorkAxis);

                    PointHolePlacementDefinition phpDef = partCompDef.Features.HoleFeatures.CreatePointPlacementDefinition(workpoint, workaxis);

                    return phpDef as HolePlacementDefinition;
                }

                default:
                    return null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static bool CompareHolePlacementDefinition(HolePlacementDefinition asmPlacementDefinition,
            HolePlacementDefinition partPlacementDefinition,
            Matrix invTransfo)
        {
            switch (asmPlacementDefinition.Type)
            {
                case ObjectTypeEnum.kSketchHolePlacementDefinitionObject:
                {
                    if (!(partPlacementDefinition is SketchHolePlacementDefinition))
                        return false;

                    SketchHolePlacementDefinition asmPlacementDef = asmPlacementDefinition as SketchHolePlacementDefinition;
                    SketchHolePlacementDefinition partPlacementDef = partPlacementDefinition as SketchHolePlacementDefinition;

                    if (asmPlacementDef.HoleCenterPoints.Count != partPlacementDef.HoleCenterPoints.Count)
                        return false;

                    PlanarSketch asmSketch = (asmPlacementDef.HoleCenterPoints[1] as SketchPoint).Parent as PlanarSketch;

                    PlanarSketch partSketch = (partPlacementDef.HoleCenterPoints[1] as SketchPoint).Parent as PlanarSketch;

                    for (int i = 1; i <= asmPlacementDef.HoleCenterPoints.Count; ++i)
                    {
                        SketchPoint asmPoint = asmPlacementDef.HoleCenterPoints[i] as SketchPoint;

                        Point asmModelPoint = asmSketch.SketchToModelSpace(asmPoint.Geometry);

                        asmModelPoint.TransformBy(invTransfo);


                        SketchPoint partPoint = partPlacementDef.HoleCenterPoints[i] as SketchPoint;

                        Point partModelPoint = partSketch.SketchToModelSpace(partPoint.Geometry);

                        if (!asmModelPoint.IsEqualTo(partModelPoint, FeatureUtilities.Tolerance))
                            return false;
                    }

                    break;
                }
                case ObjectTypeEnum.kLinearHolePlacementDefinitionObject:
                {
                    if (!(partPlacementDefinition is PointHolePlacementDefinition))
                        return false;

                    LinearHolePlacementDefinition asmPlacementDef = asmPlacementDefinition as LinearHolePlacementDefinition;
                    PointHolePlacementDefinition partPlacementDef = partPlacementDefinition as PointHolePlacementDefinition;

                    Point asmPoint = asmPlacementDef.Parent.HoleCenterPoints[1] as Point;
                    asmPoint.TransformBy(invTransfo);

                    WorkPoint partPoint = partPlacementDef.Point as WorkPoint;

                    if (!asmPoint.IsEqualTo(partPoint.Point, FeatureUtilities.Tolerance))
                        return false;

                    UnitVector asmAxis = FeatureUtilities.GetDirection(asmPlacementDef.Plane);
                    asmAxis.TransformBy(invTransfo);

                    UnitVector partAxis = FeatureUtilities.GetDirection(partPlacementDef.Direction);

                    if (!asmAxis.IsEqualTo(partAxis, FeatureUtilities.Tolerance))
                        return false;

                    break;
                }
                case ObjectTypeEnum.kConcentricHolePlacementDefinitionObject:
                {
                    if (!(partPlacementDefinition is PointHolePlacementDefinition))
                        return false;

                    ConcentricHolePlacementDefinition asmPlacementDef = asmPlacementDefinition as ConcentricHolePlacementDefinition;
                    PointHolePlacementDefinition partPlacementDef = partPlacementDefinition as PointHolePlacementDefinition;

                    Point asmPoint = asmPlacementDef.Parent.HoleCenterPoints[1] as Point;
                    asmPoint.TransformBy(invTransfo);

                    WorkPoint partPoint = partPlacementDef.Point as WorkPoint;

                    if (!asmPoint.IsEqualTo(partPoint.Point, FeatureUtilities.Tolerance))
                        return false;

                    UnitVector asmAxis = FeatureUtilities.GetDirection(asmPlacementDef.Plane);
                    asmAxis.TransformBy(invTransfo);

                    UnitVector partAxis = FeatureUtilities.GetDirection(partPlacementDef.Direction);

                    if (!asmAxis.IsEqualTo(partAxis, FeatureUtilities.Tolerance))
                        return false;

                    break;
                }
                case ObjectTypeEnum.kPointHolePlacementDefinitionObject:
                {
                    if (!(partPlacementDefinition is PointHolePlacementDefinition))
                        return false;

                    PointHolePlacementDefinition asmPlacementDef = asmPlacementDefinition as PointHolePlacementDefinition;
                    PointHolePlacementDefinition partPlacementDef = partPlacementDefinition as PointHolePlacementDefinition;

                    Point asmPoint = (asmPlacementDef.Point as WorkPoint).Point;
                    asmPoint.TransformBy(invTransfo);

                    WorkPoint partPoint = partPlacementDef.Point as WorkPoint;

                    if (!asmPoint.IsEqualTo(partPoint.Point, FeatureUtilities.Tolerance))
                        return false;

                    UnitVector asmAxis = FeatureUtilities.GetDirection(asmPlacementDef.Direction);
                    asmAxis.TransformBy(invTransfo);

                    UnitVector partAxis = FeatureUtilities.GetDirection(partPlacementDef.Direction);

                    if (!asmAxis.IsEqualTo(partAxis, FeatureUtilities.Tolerance))
                        return false;

                    break;
                }

                default:
                    return false;
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static bool UpdateHolePlacementDefinition(HolePlacementDefinition asmPlacementDefinition,
            HoleFeature PartFeature,
            Matrix invTransfo)
        {
            HolePlacementDefinition partPlacementDef = CopyHolePlacementDefinition(PartFeature.Parent as PartComponentDefinition,
                asmPlacementDefinition,
                invTransfo);

            if (partPlacementDef == null)
            {
                return false;
            }

            PartFeature.PlacementDefinition = partPlacementDef;
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static object GetDiameterOrTapInfo(HoleFeature Feature)
        {
            try
            {
                if (!Feature.Tapped)
                {
                    return Feature.HoleDiameter.Value;
                }
                else
                {
                    return Feature.TapInfo;
                }
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
        static bool CopyTapInfo(HoleFeature AsmFeature, HoleFeature PartFeature)
        {
            try
            {   
                if (AsmFeature.TapInfo is HoleTapInfo)
                {
                    HoleTapInfo asmHoleTapInfo = AsmFeature.TapInfo as HoleTapInfo;

                    PartFeature.TapInfo = asmHoleTapInfo; 

                    HoleTapInfo partHoleTapInfo = PartFeature.TapInfo as HoleTapInfo;

                    partHoleTapInfo.RightHanded = asmHoleTapInfo.RightHanded;
                    partHoleTapInfo.ThreadType = asmHoleTapInfo.ThreadType;
                    partHoleTapInfo.Class = asmHoleTapInfo.Class;
                    partHoleTapInfo.FullTapDepth = asmHoleTapInfo.FullTapDepth;
                    partHoleTapInfo.ThreadDepth.Value = asmHoleTapInfo.ThreadDepth.Value;
                    partHoleTapInfo.FullThreadDepth = asmHoleTapInfo.FullThreadDepth;
                    partHoleTapInfo.Internal = asmHoleTapInfo.Internal;
                    partHoleTapInfo.MajorDiameterMax = asmHoleTapInfo.MajorDiameterMax;
                    partHoleTapInfo.MajorDiameterMin = asmHoleTapInfo.MajorDiameterMin;
                    partHoleTapInfo.Metric = asmHoleTapInfo.Metric;
                    partHoleTapInfo.MinorDiameterMax = asmHoleTapInfo.MinorDiameterMax;
                    partHoleTapInfo.MinorDiameterMin = asmHoleTapInfo.MinorDiameterMin;
                    partHoleTapInfo.PitchDiameterMax = asmHoleTapInfo.PitchDiameterMax;
                    partHoleTapInfo.PitchDiameterMin = asmHoleTapInfo.PitchDiameterMin;
                    partHoleTapInfo.TapDrillDiameter = asmHoleTapInfo.TapDrillDiameter;
                    //partHoleTapInfo.ThreadBasePoints
                    partHoleTapInfo.ThreadDepth.Value = asmHoleTapInfo.ThreadDepth.Value;
                    //partHoleTapInfo.ThreadDirection

                    return true;
                }

                if (AsmFeature.TapInfo is TaperedThreadInfo)
                {
                    TaperedThreadInfo asmTaperedThreadInfo = AsmFeature.TapInfo as TaperedThreadInfo;

                    PartFeature.TapInfo = asmTaperedThreadInfo;

                    TaperedThreadInfo partTaperedThreadInfo = PartFeature.TapInfo as TaperedThreadInfo;

                    partTaperedThreadInfo.RightHanded = asmTaperedThreadInfo.RightHanded;
                    partTaperedThreadInfo.ThreadType = asmTaperedThreadInfo.ThreadType;
                    partTaperedThreadInfo.BasicMinorDiameter = asmTaperedThreadInfo.BasicMinorDiameter;
                    partTaperedThreadInfo.EffectiveDiameter = asmTaperedThreadInfo.EffectiveDiameter;
                    partTaperedThreadInfo.EffectiveLength = asmTaperedThreadInfo.EffectiveLength;
                    partTaperedThreadInfo.EngagementDiameter = asmTaperedThreadInfo.EngagementDiameter;
                    partTaperedThreadInfo.EngagementLength = asmTaperedThreadInfo.EngagementLength;
                    partTaperedThreadInfo.ExternalPitchDiameter = asmTaperedThreadInfo.ExternalPitchDiameter;
                    partTaperedThreadInfo.FullThreadDepth = asmTaperedThreadInfo.FullThreadDepth;
                    partTaperedThreadInfo.Internal = asmTaperedThreadInfo.Internal;
                    partTaperedThreadInfo.Metric = asmTaperedThreadInfo.Metric;
                    partTaperedThreadInfo.NominalExternalDiameter = asmTaperedThreadInfo.NominalExternalDiameter;
                    partTaperedThreadInfo.NominalExternalLength = asmTaperedThreadInfo.NominalExternalLength;
                    partTaperedThreadInfo.OutsidePipeDiameter = asmTaperedThreadInfo.OutsidePipeDiameter;
                    partTaperedThreadInfo.OverallExternalLength = asmTaperedThreadInfo.OverallExternalLength;
                    partTaperedThreadInfo.Pitch = asmTaperedThreadInfo.Pitch;
                    //partTaperedThreadInfo.ThreadBasePoints
                    //partTaperedThreadInfo.ThreadDirection
                    partTaperedThreadInfo.ThreadHeight = asmTaperedThreadInfo.ThreadHeight;
                    partTaperedThreadInfo.ThreadsPerInch = asmTaperedThreadInfo.ThreadsPerInch;
                    partTaperedThreadInfo.VanishThread = asmTaperedThreadInfo.VanishThread;
                    partTaperedThreadInfo.WrenchMakeupDiameter = asmTaperedThreadInfo.WrenchMakeupDiameter;
                    partTaperedThreadInfo.WrenchMakeupLength = asmTaperedThreadInfo.WrenchMakeupLength;

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // This argument is only used when the FlatBottom argument is False. 
        // If FlatBottom is false and this argument is not supplied a default value of 118 degrees is assigned. 
        // This can be either a numeric value or a string. 
        // A parameter for this value will be created and the supplied string or value is assigned to the parameter. 
        // If a value is input, the units are radians. 
        // If a string is input, the units can be specified as part of the string or it will default to the current angle units 
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static object GetBottomTipAngle(HoleFeature Feature)
        {
            if(Feature.FlatBottom) return null;

            try
            {
                double BottomTipAngle = (double)(Feature.BottomTipAngle.Value);
                return BottomTipAngle;
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
        static HoleFeature AddHoleFeatureByDistanceExtent(PartComponentDefinition partCompDef, 
            HoleFeature Feature, 
            HolePlacementDefinition PlacementDefinition, 
            object DiameterOrTapInfo)
        {
            HoleFeature newFeature = null;

            DistanceExtent DistanceExtent = Feature.Extent as DistanceExtent;

            switch (Feature.HoleType)
            {
                case HoleTypeEnum.kDrilledHole:
                {
                    newFeature = partCompDef.Features.HoleFeatures.AddDrilledByDistanceExtent(PlacementDefinition, 
                        DiameterOrTapInfo, 
                        DistanceExtent.Distance.Value, 
                        DistanceExtent.Direction, 
                        Feature.FlatBottom,
                        GetBottomTipAngle(Feature));

                    break;
                }
                case HoleTypeEnum.kCounterSinkHole:
                {
                    newFeature = partCompDef.Features.HoleFeatures.AddCSinkByDistanceExtent(PlacementDefinition, 
                        DiameterOrTapInfo,
                        DistanceExtent.Distance.Value,
                        DistanceExtent.Direction, 
                        Feature.CSinkDiameter.Value, Feature.CSinkAngle.Value,
                        Feature.FlatBottom,
                        GetBottomTipAngle(Feature));

                    break;
                }
                case HoleTypeEnum.kCounterBoreHole:
                {
                    newFeature = partCompDef.Features.HoleFeatures.AddCBoreByDistanceExtent(PlacementDefinition, 
                        DiameterOrTapInfo,
                        DistanceExtent.Distance.Value,
                        DistanceExtent.Direction, 
                        Feature.CBoreDiameter.Value,
                        Feature.CBoreDepth.Value,
                        Feature.FlatBottom,
                        GetBottomTipAngle(Feature));

                    break;
                }
                case HoleTypeEnum.kSpotFaceHole:
                {
                    newFeature = partCompDef.Features.HoleFeatures.AddSpotFaceByDistanceExtent(PlacementDefinition, 
                        DiameterOrTapInfo,
                        DistanceExtent.Distance.Value,
                        DistanceExtent.Direction, 
                        Feature.SpotFaceDiameter.Value, 
                        Feature.SpotFaceDepth.Value,
                        Feature.FlatBottom,
                        GetBottomTipAngle(Feature));

                    break;
                }
                default:

                    return null;
            }

            return newFeature;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static HoleFeature AddHoleFeatureByThroughAllExtent(PartComponentDefinition partCompDef, 
            HoleFeature Feature, 
            HolePlacementDefinition PlacementDefinition, 
            object DiameterOrTapInfo)
        {
            HoleFeature newFeature = null;

            ThroughAllExtent ThroughAllExtent = Feature.Extent as ThroughAllExtent;

            switch (Feature.HoleType)
            {
                case HoleTypeEnum.kDrilledHole:
                {
                    newFeature = partCompDef.Features.HoleFeatures.AddDrilledByThroughAllExtent(PlacementDefinition, 
                        DiameterOrTapInfo, 
                        ThroughAllExtent.Direction);

                    break;
                }
                case HoleTypeEnum.kCounterSinkHole:
                {
                    newFeature = partCompDef.Features.HoleFeatures.AddCSinkByThroughAllExtent(PlacementDefinition, 
                        DiameterOrTapInfo, 
                        ThroughAllExtent.Direction, 
                        Feature.CSinkDiameter.Value, 
                        Feature.CSinkAngle.Value);

                    break;
                }
                case HoleTypeEnum.kCounterBoreHole:
                {
                    newFeature = partCompDef.Features.HoleFeatures.AddCBoreByThroughAllExtent(PlacementDefinition, 
                        DiameterOrTapInfo, 
                        ThroughAllExtent.Direction, 
                        Feature.CBoreDiameter.Value, 
                        Feature.CBoreDepth.Value);

                    break;
                }
                case HoleTypeEnum.kSpotFaceHole:
                {
                    newFeature = partCompDef.Features.HoleFeatures.AddSpotFaceByThroughAllExtent(PlacementDefinition, 
                        DiameterOrTapInfo, 
                        ThroughAllExtent.Direction, 
                        Feature.SpotFaceDiameter.Value, 
                        Feature.SpotFaceDepth.Value);

                    break;
                }
                default:

                    return null;
            }

            return newFeature;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static HoleFeature AddHoleFeatureByToFaceExtent(PartComponentDefinition partCompDef, 
            HoleFeature Feature, 
            HolePlacementDefinition 
            PlacementDefinition, 
            object DiameterOrTapInfo,
            Matrix invTransfo)
        {
            HoleFeature newFeature = null;

            ToExtent ToExtent = Feature.Extent as ToExtent;

            object ToEntity = FeatureUtilities.CopyFromToEntity(ToExtent.ToEntity,
                               partCompDef,
                               invTransfo);

            switch (Feature.HoleType)
            {
                case HoleTypeEnum.kDrilledHole:
                {
                    newFeature = partCompDef.Features.HoleFeatures.AddDrilledByToFaceExtent(PlacementDefinition,
                        DiameterOrTapInfo,
                        ToEntity,
                        ToExtent.ExtendToFace,
                        Feature.FlatBottom,
                        GetBottomTipAngle(Feature));

                    break;
                }
                case HoleTypeEnum.kCounterSinkHole:
                {
                    newFeature = partCompDef.Features.HoleFeatures.AddCSinkByToFaceExtent(PlacementDefinition, 
                        DiameterOrTapInfo,
                        ToEntity,
                        ToExtent.ExtendToFace, 
                        Feature.CSinkDiameter.Value, 
                        Feature.CSinkAngle.Value,
                        Feature.FlatBottom,
                        GetBottomTipAngle(Feature));

                    break;
                }
                case HoleTypeEnum.kCounterBoreHole:
                {
                    newFeature = partCompDef.Features.HoleFeatures.AddCBoreByToFaceExtent(PlacementDefinition, 
                        DiameterOrTapInfo,
                        ToEntity,
                        ToExtent.ExtendToFace,
                        Feature.CBoreDiameter.Value, 
                        Feature.CBoreDepth.Value,
                        Feature.FlatBottom,
                        GetBottomTipAngle(Feature));

                    break;
                }
                case HoleTypeEnum.kSpotFaceHole:
                {
                    newFeature = partCompDef.Features.HoleFeatures.AddSpotFaceByToFaceExtent(PlacementDefinition, 
                        DiameterOrTapInfo,
                        ToEntity,
                        ToExtent.ExtendToFace, 
                        Feature.SpotFaceDiameter.Value, 
                        Feature.SpotFaceDepth.Value,
                        Feature.FlatBottom,
                        GetBottomTipAngle(Feature));

                    break;
                }
                default:

                    return null;
            }

            return newFeature;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static bool CompareDoubleVariant(object value1, object value2)
        {
            if (value1 == null && value2 == null)
                return true;

            if (value1 is double && value2 is double)
            {
                double d1 = (double)value1;
                double d2 = (double)value2;

                return FeatureUtilities.IsEqual(d1, d2);
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static bool CompareTapInfo(object asmTapInfo, object partTapInfo, Matrix invTransfo)
        {
            if (asmTapInfo == null && partTapInfo == null)
                return true;

            if(asmTapInfo is HoleTapInfo && partTapInfo is HoleTapInfo)
            {
                HoleTapInfo asmHoleTapInfo = asmTapInfo as HoleTapInfo;
                HoleTapInfo partHoleTapInfo = partTapInfo as HoleTapInfo;

                if (asmHoleTapInfo.Class != partHoleTapInfo.Class)
                    return false;

                if (asmHoleTapInfo.CustomThreadDesignation != partHoleTapInfo.CustomThreadDesignation)
                    return false;

                if (asmHoleTapInfo.FullTapDepth != partHoleTapInfo.FullTapDepth)
                    return false;

                if (asmHoleTapInfo.FullThreadDepth != partHoleTapInfo.FullThreadDepth)
                    return false;

                if (asmHoleTapInfo.Internal != partHoleTapInfo.Internal)
                    return false;

                if (!CompareDoubleVariant(asmHoleTapInfo.MajorDiameterMax, partHoleTapInfo.MajorDiameterMax))
                    return false;

                if (!CompareDoubleVariant(asmHoleTapInfo.MajorDiameterMin, partHoleTapInfo.MajorDiameterMin))
                    return false;

                if (asmHoleTapInfo.Metric != partHoleTapInfo.Metric)
                    return false;

                if (!CompareDoubleVariant(asmHoleTapInfo.MinorDiameterMax, partHoleTapInfo.MinorDiameterMax))
                    return false;

                if (!CompareDoubleVariant(asmHoleTapInfo.MinorDiameterMin, partHoleTapInfo.MinorDiameterMin))
                    return false;

                if (asmHoleTapInfo.NominalSize != partHoleTapInfo.NominalSize)
                    return false;

                if (asmHoleTapInfo.Pitch != partHoleTapInfo.Pitch)
                    return false;

                if (!CompareDoubleVariant(asmHoleTapInfo.PitchDiameterMax, partHoleTapInfo.PitchDiameterMax))
                    return false;

                if (!CompareDoubleVariant(asmHoleTapInfo.PitchDiameterMin, partHoleTapInfo.PitchDiameterMin))
                    return false;

                if (asmHoleTapInfo.RightHanded != partHoleTapInfo.RightHanded)
                    return false;

                if (!CompareDoubleVariant(asmHoleTapInfo.TapDrillDiameter, partHoleTapInfo.TapDrillDiameter))
                    return false;

                //asmHoleTapInfo.ThreadBasePoints != partHoleTapInfo.ThreadBasePoints))

                if (!FeatureUtilities.IsEqual(asmHoleTapInfo.ThreadDepth.Value, partHoleTapInfo.ThreadDepth.Value))
                    return false;

                if (asmHoleTapInfo.ThreadDesignation != partHoleTapInfo.ThreadDesignation)
                    return false;

                //asmHoleTapInfo.ThreadDirection != partHoleTapInfo.ThreadDirection)
                    
                if (asmHoleTapInfo. ThreadType != partHoleTapInfo.ThreadType)
                    return false;

                if (asmHoleTapInfo.ThreadTypeIdentifier != partHoleTapInfo.ThreadTypeIdentifier)
                    return false;

                return true;
            }

            if (asmTapInfo is TaperedThreadInfo && partTapInfo is TaperedThreadInfo)
            {
                TaperedThreadInfo asmThreadInfo = asmTapInfo as TaperedThreadInfo;
                TaperedThreadInfo partThreadpInfo = partTapInfo as TaperedThreadInfo;

                if (!CompareDoubleVariant(asmThreadInfo.BasicMinorDiameter, partThreadpInfo.BasicMinorDiameter))
                    return false;

                if (asmThreadInfo.CustomThreadDesignation != partThreadpInfo.CustomThreadDesignation)
                    return false;

                if (!CompareDoubleVariant(asmThreadInfo.EffectiveDiameter, partThreadpInfo.EffectiveDiameter))
                    return false;

                if (!CompareDoubleVariant(asmThreadInfo.EffectiveLength, partThreadpInfo.EffectiveLength))
                    return false;

                if (!CompareDoubleVariant(asmThreadInfo.EngagementDiameter, partThreadpInfo.EngagementDiameter))
                    return false;

                if (!CompareDoubleVariant(asmThreadInfo.EngagementLength, partThreadpInfo.EngagementLength))
                    return false;

                if (!CompareDoubleVariant(asmThreadInfo.ExternalPitchDiameter, partThreadpInfo.ExternalPitchDiameter))
                    return false;

                if (asmThreadInfo.FullThreadDepth != partThreadpInfo.FullThreadDepth)
                    return false;

                if (asmThreadInfo.Internal != partThreadpInfo.Internal)
                    return false;

                if (asmThreadInfo.Metric != partThreadpInfo.Metric)
                    return false;

                if (!CompareDoubleVariant(asmThreadInfo.NominalExternalDiameter, partThreadpInfo.NominalExternalDiameter))
                    return false;

                if (!CompareDoubleVariant(asmThreadInfo.NominalExternalLength, partThreadpInfo.NominalExternalLength))
                    return false;

                if (!CompareDoubleVariant(asmThreadInfo.OutsidePipeDiameter, partThreadpInfo.OutsidePipeDiameter))
                    return false;

                if (!CompareDoubleVariant(asmThreadInfo.OverallExternalLength, partThreadpInfo.OverallExternalLength))
                    return false;

                if(asmThreadInfo.Pitch != partThreadpInfo.Pitch)
                    return false;

                if (asmThreadInfo.RightHanded != partThreadpInfo.RightHanded)
                    return false;

                //asmThreadInfo.ThreadBasePoints != partThreadpInfo.ThreadBasePoints

                if (asmThreadInfo.ThreadDesignation != partThreadpInfo.ThreadDesignation)
                    return false;

                //asmThreadInfo.ThreadDirection != partThreadpInfo.ThreadDirection

                if (!CompareDoubleVariant(asmThreadInfo.ThreadHeight, partThreadpInfo.ThreadHeight))
                    return false;

                if (asmThreadInfo.ThreadsPerInch != partThreadpInfo.ThreadsPerInch)
                    return false;

                 if (asmThreadInfo.ThreadType != partThreadpInfo.ThreadType)
                    return false;

                if (asmThreadInfo.ThreadTypeIdentifier != partThreadpInfo.ThreadTypeIdentifier)
                    return false;

                if (!CompareDoubleVariant(asmThreadInfo.VanishThread, partThreadpInfo.VanishThread))
                    return false;

                if (!CompareDoubleVariant(asmThreadInfo.WrenchMakeupDiameter, partThreadpInfo.WrenchMakeupDiameter))
                    return false;

                if (!CompareDoubleVariant(asmThreadInfo.WrenchMakeupLength, partThreadpInfo.WrenchMakeupLength))
                    return false;

                return true;
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static bool CompareHoles(HoleFeature AsmFeature, HoleFeature PartFeature, Matrix invTransfo)
        {
            if (AsmFeature.HoleType != PartFeature.HoleType)
                return false;

            switch (AsmFeature.HoleType)
            {
                case HoleTypeEnum.kDrilledHole:
                {
                    break;
                }
                case HoleTypeEnum.kCounterSinkHole:
                {
                    if(!FeatureUtilities.IsEqual(AsmFeature.CSinkDiameter.Value, PartFeature.CSinkDiameter.Value))  
                        return false;

                    if(!FeatureUtilities.IsEqual(AsmFeature.CSinkAngle.Value, PartFeature.CSinkAngle.Value))  
                        return false;

                    break;
                }
                case HoleTypeEnum.kCounterBoreHole:
                {
                    if(!FeatureUtilities.IsEqual(AsmFeature.CBoreDiameter.Value, PartFeature.CBoreDiameter.Value))  
                        return false;

                    if(!FeatureUtilities.IsEqual(AsmFeature.CBoreDepth.Value, PartFeature.CBoreDepth.Value))  
                        return false;

                    break;
                }
                case HoleTypeEnum.kSpotFaceHole:
                {
                    if(!FeatureUtilities.IsEqual(AsmFeature.SpotFaceDiameter.Value, PartFeature.SpotFaceDiameter.Value))  
                        return false;

                    if(!FeatureUtilities.IsEqual(AsmFeature.SpotFaceDepth.Value, PartFeature.SpotFaceDepth.Value))  
                        return false;

                    break;
                }
                default:
                    break;
            }

            //FlatBottom:
            //Property that is meaningful only if the hole’s extent has been defined by distance (blind depth)
            //In that case this property returns a True if a non-zero bottom tip angle has been specified
            if(AsmFeature.ExtentType == PartFeatureExtentEnum.kDistanceExtent)
            {
                if(AsmFeature.FlatBottom != PartFeature.FlatBottom)
                    return false;
            }

            //DiameterOrTapInfo
            try
            {
                if (!FeatureUtilities.IsEqual(AsmFeature.HoleDiameter.Value, PartFeature.HoleDiameter.Value))
                    return false;
            }
            catch
            {

            }

            if (!CompareTapInfo(AsmFeature.TapInfo, PartFeature.TapInfo, invTransfo))
                return false;

            //BottomTipAngle
            try
            {
                if(!FeatureUtilities.IsEqual(AsmFeature.BottomTipAngle.Value, PartFeature.BottomTipAngle.Value))
                    return false;
            }
            catch
            {
                
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static bool UpdateHole(HoleFeature AsmFeature, HoleFeature PartFeature)
        {
            try
            {
                switch (AsmFeature.HoleType)
                {
                    case HoleTypeEnum.kCounterBoreHole:
                        PartFeature.SetCBore(AsmFeature.CBoreDiameter.Value, AsmFeature.CBoreDepth.Value);
                        break;

                    case HoleTypeEnum.kCounterSinkHole:
                        PartFeature.SetCSink(AsmFeature.CSinkDiameter.Value, AsmFeature.CSinkAngle.Value);
                        break;

                    case HoleTypeEnum.kDrilledHole:
                        PartFeature.SetDrilled();
                        break;

                    case HoleTypeEnum.kSpotFaceHole:
                        PartFeature.SetSpotFace(AsmFeature.SpotFaceDiameter.Value, AsmFeature.SpotFaceDepth.Value);
                        break;

                    default:
                        return false;
                }

                if (AsmFeature.Tapped)
                {
                    CopyTapInfo(AsmFeature, PartFeature);
                }
                else
                {
                    PartFeature.HoleDiameter.Value = AsmFeature.HoleDiameter.Value;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
