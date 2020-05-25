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
using System.Windows.Forms;

using Inventor;

namespace FeatureMigratorLib
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //use: PatternMigrationModeEnum 
    //     PatternFeatures can be migrated to the Parts using 2 modes: 
    //     As patterned single features (kSendToPartsAsCollectionMode) or as PatternFeature (kSendToPartsAsPatternMode)
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public enum PatternMigrationModeEnum
    {
        kSendToPartsAsCollectionMode,
        kSendToPartsAsPatternMode
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // use: a small utility class in order to create child dialogs
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class WindowWrapper : System.Windows.Forms.IWin32Window
    {
        private IntPtr mHwnd;

        public WindowWrapper(IntPtr handle)
        {
            mHwnd = handle;
        }

        public IntPtr Handle
        {
            get
            {
                return mHwnd;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //use: FeatureUtilities class 
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class FeatureUtilities
    {
        private static double _Tolerance = 0.0001;

        private static Inventor.Application _Application;
        private static TransientGeometry _Tg;

        private static bool _constructionWorkPoint = true;
        private static bool _constructionWorkAxis = true;
        private static bool _constructionWorkPlane = true;
        private static string _cbAction = "Suppress if Succeeded";
        private static string _cbActionPart = "Suppress";

        private static DefaultFeatureMigrator _DefautlFeatureMigrator = new DefaultFeatureMigrator();

        private static Dictionary<ObjectTypeEnum, IFeatureMigrator> _FeatureMigratorMap;
        private static string _renderStyle = "As Part";
        private static bool _singleFeatureOptions = false;
        private static bool _createBackupFile = true;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //use: Initialize the FeatureUtilities library
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Initialize(Inventor.Application Application)
        {
            _Application = Application;

            _Tg = _Application.TransientGeometry;

            _FeatureMigratorMap = new Dictionary<ObjectTypeEnum, IFeatureMigrator>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //use: Adds a FeatureMigrator at runtime to the FeatureUtilities library
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetFeatureMigrator(ObjectTypeEnum FeatureType, IFeatureMigrator FeatureMigrator)
        {
            //Allows Runtime switch between FeatureMigrators
            if (_FeatureMigratorMap.ContainsKey(FeatureType))
            {
                _FeatureMigratorMap[FeatureType] = FeatureMigrator;
            }

            _FeatureMigratorMap.Add(FeatureType, FeatureMigrator);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //use: Returns a FeatureMigrator for the specified type of feature
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static IFeatureMigrator GetFeatureMigrator(ObjectTypeEnum featureType)
        {
            if (_FeatureMigratorMap.ContainsKey(featureType))
            {
                return _FeatureMigratorMap[featureType];
            }

            return _DefautlFeatureMigrator;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Inventor.Application Application
        {
            set
            {
                _Application = value;

                _Tg = _Application.TransientGeometry;
            }
            get
            {
                return _Application;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Get/Set Construction WorkPoint property
        // Construction workfeatures are invisible to the user
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool ConstructionWorkPoint
        {
            set
            {
                _constructionWorkPoint = value;
            }
            get
            {
                return _constructionWorkPoint;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Get/Set Construction WorkAxis property
        //  Construction workfeatures are invisible to the user
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool ConstructionWorkAxis
        {
            set
            {
                _constructionWorkAxis = value;
            }
            get
            {
                return _constructionWorkAxis;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Get/Set Construction WorkPlane property
        // Construction workfeatures are invisible to the user
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool ConstructionWorkPlane
        {
            set
            {
                _constructionWorkPlane = value;
            }
            get
            {
                return _constructionWorkPlane;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Get/Set Combobox Action property
        // 
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string CbAction
        {
            set
            {
                _cbAction = value;
            }
            get
            {
                return _cbAction;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Get/Set Combobox Action Part property
        // 
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string CbActionPart
        {
            set
            {
                _cbActionPart = value;
            }
            get
            {
                return _cbActionPart;
            }
        }

        public static string RenderStyle
        {
            set { _renderStyle = value; }
            get { return _renderStyle; }
        }

        public static bool SingleFeatureOptions
        {
            set { _singleFeatureOptions = value; }
            get { return _singleFeatureOptions; }
        }

        public static bool CreateBackupFile
        {
            set { _createBackupFile = value; }
            get { return _createBackupFile; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Get/Set Internal Tolerance used by the utulity functions of the library
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static double Tolerance
        {
            set
            {
                _Tolerance = value;
            }
            get
            {
                return _Tolerance;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //use: Performs equal comparison between two doubles using internal tolerance 
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsEqual(object value1, object value2)
        {
            return (Math.Abs((double)value1 - (double)value2) < _Tolerance);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Returns True if the Feature can be migrated by the add-in, False otherwise.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsSupportedFeature(PartFeature Feature)
        {
            return GetFeatureMigrator(Feature.Type).IsMigrationSupported(Feature);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Returns Feature Image index, used for the display in the browser tree.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static int GetFeatureImageIndex(PartFeature Feature)
        {
            switch (Feature.Type)
            {
                case ObjectTypeEnum.kExtrudeFeatureObject:
                    return 4;

                case ObjectTypeEnum.kHoleFeatureObject:
                    return 6;

                case ObjectTypeEnum.kRevolveFeatureObject:
                    return 8;

                case ObjectTypeEnum.kFilletFeatureObject:
                    return 10;

                case ObjectTypeEnum.kChamferFeatureObject:
                    return 12;

                case ObjectTypeEnum.kSweepFeatureObject:
                    return 14;

                case ObjectTypeEnum.kMoveFaceFeatureObject:
                    return 16;

                case ObjectTypeEnum.kRectangularPatternFeatureObject:
                    return 18;

                case ObjectTypeEnum.kCircularPatternFeatureObject:
                    return 20;

                case ObjectTypeEnum.kMirrorFeatureObject:
                    return 22;

                default:

                    break;
            }

            return 2;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Returns Normal as UnitVetor for different type of input entities.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static UnitVector GetNormal(object planarEntity)
        {
            if (planarEntity is Face)
            {
                Face face = planarEntity as Face;

                return GetFaceNormal(face);
            }

            if (planarEntity is WorkPlane)
            {
                WorkPlane workplane = planarEntity as WorkPlane;

                return workplane.Plane.Normal;
            }

            if (planarEntity is Faces)
            {
                Face face = (planarEntity as Faces)[1];
                return GetFaceNormal(face);
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Returns Normal as UnitVector for input Face.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static UnitVector GetFaceNormal(Face face, Point point)
        {
            SurfaceEvaluator oEvaluator = face.Evaluator;

            Double[] Points = { point.X, point.Y, point.Z };

            Double[] GuessParams = new Double[2];
            Double[] MaxDev = new Double[2];
            Double[] Params = new Double[2];
            SolutionNatureEnum[] sol = new SolutionNatureEnum[2];

            oEvaluator.GetParamAtPoint(ref Points, ref GuessParams, ref MaxDev, ref Params, ref sol);

            Double[] normal = new Double[3];

            oEvaluator.GetNormal(ref Params, ref normal);

            return _Tg.CreateUnitVector(normal[0], normal[1], normal[2]);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Returns Normal as UnitVector for input Face.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static UnitVector GetFaceNormal(Face face)
        {
            SurfaceEvaluator oEvaluator = face.Evaluator;

            Double[] Points = { face.PointOnFace.X, face.PointOnFace.Y, face.PointOnFace.Z };

            Double[] GuessParams = new Double[2];
            Double[] MaxDev = new Double[2];
            Double[] Params = new Double[2];
            SolutionNatureEnum[] sol = new SolutionNatureEnum[2];

            oEvaluator.GetParamAtPoint(ref Points, ref GuessParams, ref MaxDev, ref Params, ref sol);

            Double[] normal = new Double[3];

            oEvaluator.GetNormal(ref Params, ref normal);

            return _Tg.CreateUnitVector(normal[0], normal[1], normal[2]);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Returns Direction for the input Entity. 
        //Input Entity can be an edge, a work axis, a work plane (normal is used), a planar face (normal is used) or a Path.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static UnitVector GetDirection(object entity)
        {
            if (entity is WorkAxis)
            {
                WorkAxis workaxis = entity as WorkAxis;
                return workaxis.Line.Direction;
            }

            if (entity is Edge)
            {
                Edge edge = entity as Edge;

                if (edge.Geometry is Line)
                {
                    Line line = edge.Geometry as Line;
                    return line.Direction;
                }

                if (edge.Geometry is LineSegment)
                {
                    LineSegment lineseg = edge.Geometry as LineSegment;
                    return lineseg.Direction;
                }
            }

            if (entity is WorkPlane)
            {
                WorkPlane workplane = entity as WorkPlane;

                return workplane.Plane.Normal;
            }

            if (entity is Face)
            {
                return FeatureUtilities.GetFaceNormal(entity as Face);
            }

            if (entity is Faces)
            {
                Face face = (entity as Faces)[1];
                return GetFaceNormal(face);
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Copies the full content of an existing sketch into a newly created sketch.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static PlanarSketch CopySketch(PartComponentDefinition compDef, 
            PlanarSketch originalSketch, 
            Matrix invTransfo, 
            out UnitVector xAxis, 
            out UnitVector yAxis)
        {
            Inventor.Point origin = originalSketch.OriginPointGeometry;
            origin.TransformBy(invTransfo);

            UnitVector normal = originalSketch.PlanarEntityGeometry.Normal;
            normal.TransformBy(invTransfo);

            xAxis = GetSketchXAxis(originalSketch);
            xAxis.TransformBy(invTransfo);

            yAxis = normal.CrossProduct(xAxis);

            WorkPlane workplane = compDef.WorkPlanes.AddFixed(origin, xAxis, yAxis, _constructionWorkPlane);
            WorkAxis workaxis = compDef.WorkAxes.AddFixed(origin, xAxis, _constructionWorkAxis);
            WorkPoint workpoint = compDef.WorkPoints.AddFixed(origin, _constructionWorkPoint);

            PlanarSketch newSketch = compDef.Sketches.AddWithOrientation(
                workplane, 
                workaxis, 
                true, 
                true, 
                workpoint, 
                false);

            originalSketch.CopyContentsTo(newSketch as Sketch);

            return newSketch;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Generates a new sketch with same orientation than input sketch, without copying the content.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static PlanarSketch CopySketchEmpty(PartComponentDefinition compDef, 
            PlanarSketch originalSketch, 
            Matrix invTransfo)
        {
            Inventor.Point origin = originalSketch.OriginPointGeometry;
            origin.TransformBy(invTransfo);

            UnitVector normal = originalSketch.PlanarEntityGeometry.Normal;
            normal.TransformBy(invTransfo);

            UnitVector xAxis = GetSketchXAxis(originalSketch);
            xAxis.TransformBy(invTransfo);

            UnitVector yAxis = normal.CrossProduct(xAxis);

            WorkPlane workplane = compDef.WorkPlanes.AddFixed(origin, xAxis, yAxis, _constructionWorkPlane);
            WorkAxis workaxis = compDef.WorkAxes.AddFixed(origin, xAxis, _constructionWorkAxis);
            WorkPoint workpoint = compDef.WorkPoints.AddFixed(origin, _constructionWorkPoint);

            PlanarSketch newSketch = compDef.Sketches.AddWithOrientation(workplane, workaxis, true, true, workpoint, false);

            return newSketch;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Returns the X Axis of input sketch.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static UnitVector GetSketchXAxis(PlanarSketch sketch)
        { 
            Point point = sketch.SketchToModelSpace(_Application.TransientGeometry.CreatePoint2d(1,0));

            UnitVector xAxis = sketch.OriginPointGeometry.VectorTo(point).AsUnitVector();

            return xAxis;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Returns an UnitVector orthogonal to input vector
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static UnitVector GetOrthoVector(UnitVector vector)
        {
            if (Math.Abs(vector.Z) < _Tolerance)
            {
                return _Application.TransientGeometry.CreateUnitVector(0, 0, 1);
            }
            else if (Math.Abs(vector.Y) < _Tolerance)
            {
                return _Application.TransientGeometry.CreateUnitVector(0, 1, 0);
            }
            else
            {
                //Expr: - xx'/y = y'
                return _Application.TransientGeometry.CreateUnitVector(1, -vector.X / vector.Y, 0);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Returns two orthogonal vectors depending on the input normal
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void GetOrthoBase(UnitVector normal, 
            out UnitVector xAxis, 
            out UnitVector yAxis)
        {
            xAxis = GetOrthoVector(normal);

            yAxis = normal.CrossProduct(xAxis);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Returns an orthonormal base depending on the input planar entity
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void GetEntityOrthoBase(object PlanarEntity, 
            out Point origin, 
            out UnitVector xAxis, 
            out UnitVector yAxis)
        {
            origin = GetPoint(PlanarEntity);

            UnitVector normal = GetNormal(PlanarEntity);

            GetOrthoBase(normal, out xAxis, out yAxis);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Get Point object out of input Entity.
        //
        //Planar entity: Face or WorkPlane object. 
        //Point entity: Vertex, SketchPoint, SketchPoint3D, WorkPoint or Edge object.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Inventor.Point GetPoint(object entity)
        {
            if (entity is Face)
            {
                return (entity as Face).PointOnFace;
            }
            if (entity is WorkPlane)
            {
                return (entity as WorkPlane).Plane.RootPoint;
            }
            if (entity is Vertex)
            {
                return (entity as Vertex).Point;
            }
            if (entity is SketchPoint)
            {
                SketchPoint sketchPoint = entity as SketchPoint;

                PlanarSketch sketch = sketchPoint.Parent as PlanarSketch;

                return sketch.SketchToModelSpace(sketchPoint.Geometry);
            }
            if (entity is SketchPoint3D)
            {
                return (entity as SketchPoint3D).Geometry;
            }
            if (entity is WorkPoint)
            {
                return (entity as WorkPoint).Point;
            }
            if (entity is Edge)
            {
                return GetEdgeMidPoint(entity as Edge);
            }
            if (entity is Faces)
            {
                Face face = (entity as Faces)[1];
                return face.PointOnFace;
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Get Axis object out of input Entity. 
        //Can be a linear edge, a work axis, or any face that can define an axis (i.e. every face type except planar and spherical).
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static WorkAxis GetAxis(PartComponentDefinition partCompDef,
            object axisEntity,
            Matrix invTransfo)
        {
            if (axisEntity is WorkAxis)
            {
                WorkAxis workAxis = axisEntity as WorkAxis;

                Point origin = workAxis.Line.RootPoint;
                origin.TransformBy(invTransfo);

                UnitVector direction = workAxis.Line.Direction;
                direction.TransformBy(invTransfo);

                return partCompDef.WorkAxes.AddFixed(origin, direction, _constructionWorkAxis);
            }

            if (axisEntity is Edge)
            {
                Edge edge = axisEntity as Edge;

                Point origin = null;
                UnitVector direction = null;

                if (edge.Geometry is Line)
                {
                    Line line = edge.Geometry as Line;

                    origin = line.RootPoint;
                    direction = line.Direction;
                }

                if (edge.Geometry is LineSegment)
                {
                    LineSegment lineseg = edge.Geometry as LineSegment;

                    origin = lineseg.StartPoint;
                    direction = lineseg.Direction;
                }

                origin.TransformBy(invTransfo);
                direction.TransformBy(invTransfo);

                return partCompDef.WorkAxes.AddFixed(origin, direction, _constructionWorkAxis);
            }

            if (axisEntity is Face)
            {
                Face face = axisEntity as Face;

                Point origin = null;
                UnitVector direction = null;

                switch (face.SurfaceType)
                { 
                    case SurfaceTypeEnum.kCylinderSurface:

                        Cylinder cylinder = face.Geometry as Cylinder;

                        origin = cylinder.BasePoint;
                        direction = cylinder.AxisVector;

                        origin.TransformBy(invTransfo);
                        direction.TransformBy(invTransfo);

                        return partCompDef.WorkAxes.AddFixed(origin, direction, _constructionWorkAxis);

                    case SurfaceTypeEnum.kPlaneSurface:
                    case SurfaceTypeEnum.kSphereSurface:
                    case SurfaceTypeEnum.kUnknownSurface:
                    default:
                        return null;
                }
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Get middle Point on an Edge.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Inventor.Point GetEdgeMidPoint(Edge edge)
        {
            double minParam, maxParam;
            edge.Evaluator.GetParamExtents(out minParam, out maxParam);

            double len;
            edge.Evaluator.GetLengthAtParam(minParam, maxParam, out len);

            double midParam;
            edge.Evaluator.GetParamAtLength(minParam, len * 0.5, out midParam);

            double[] Params = new double[] { midParam };
            double[] point = new double[3];

            edge.Evaluator.GetPointAtParam(ref Params, ref point);

            return _Application.TransientGeometry.CreatePoint(point[0], point[1], point[2]); ;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Use: Special Copy method to handle From/To Entity inside features
        //     
        //     
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static object CopyFromToEntity(object ToFromEntity,
            PartComponentDefinition partCompDef,
            Matrix invTransfo)
        {
            if (ToFromEntity is Vertex        ||
                ToFromEntity is Edge          ||
                ToFromEntity is SketchPoint   ||
                ToFromEntity is SketchPoint3D ||
                ToFromEntity is WorkPoint     ||
                ToFromEntity is Edge)
            {
                Inventor.Point point = FeatureUtilities.GetPoint(ToFromEntity);
                point.TransformBy(invTransfo);

                WorkPoint workPoint = partCompDef.WorkPoints.AddFixed(point, 
                    FeatureUtilities.ConstructionWorkPoint);

                return workPoint;
            }

            if (ToFromEntity is Face    ||
                ToFromEntity is Faces   ||
                ToFromEntity is WorkPlane)
            {
                Point origin = null;
                UnitVector xAxis = null;
                UnitVector yAxis = null;

                FeatureUtilities.GetEntityOrthoBase(ToFromEntity,
                    out origin,
                    out xAxis,
                    out yAxis);

                origin.TransformBy(invTransfo);
                xAxis.TransformBy(invTransfo);
                yAxis.TransformBy(invTransfo);

                WorkPlane workPlane = partCompDef.WorkPlanes.AddFixed(origin,
                    xAxis,
                    yAxis,
                    FeatureUtilities.ConstructionWorkPlane);

                return workPlane;
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Generate PathSegment in the part sketch using entities used by the original profile
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static ObjectCollection GetPathSegments(Profile originalProfile, PlanarSketch partSketch)
        {
            ObjectCollection pathSegments = _Application.TransientObjects.CreateObjectCollection(null);

            //Iterate through all curve segments in original profile
            //And find corresponding curve in part sketch

            foreach (ProfilePath path in originalProfile)
            {
                foreach (ProfileEntity profileEntity in path)
                { 
                    foreach (SketchEntity sketchEntity in partSketch.SketchEntities)
                    {
                        if(CompareSketchEntities(profileEntity.SketchEntity, sketchEntity, _Tolerance))
                        {
                            pathSegments.Add(sketchEntity);
                            break;
                        }
                    }
                }
            }

            return pathSegments;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Maches "copyProfile" ProfilePaths to "originalProfile" ProfilePaths.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void CopyProfile(Profile originalProfile, Profile copyProfile)
        {
            List <ProfilePath> pathsToInclude = new List <ProfilePath>();

            for(int i=1; i<=copyProfile.Count; ++i)
            {
                ProfilePath path = copyProfile[i];

                foreach (ProfilePath origPath in originalProfile)
                {
                    if (origPath.Count == path.Count)
                    {
                        int nbMatchingEntities = 0;

                        //We dont assume the path contains the entities in the same order
                        //so need the double loop :(
                        for(int j=1; j<=path.Count; ++j)
                        {
                            for(int k=1; k<=path.Count; ++k)
                            {
                                if (CompareSketchEntities(origPath[j].SketchEntity, path[k].SketchEntity, _Tolerance))
                                {
                                    ++nbMatchingEntities;
                                    break;
                                }
                            }
                        }

                        //All the entities are matching -> Include this path
                        if (nbMatchingEntities == path.Count)
                        {
                            path.AddsMaterial = origPath.AddsMaterial;
                            
                            pathsToInclude.Add(path);
                            break;
                        }
                    }
                }
            }

            foreach (ProfilePath path in copyProfile)
            {
                if (!pathsToInclude.Contains(path))
                {
                    path.Delete();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Returns True if the 2 sketches are identical, False otherwise.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool CompareSketches(PlanarSketch asmSketch,
            PlanarSketch partSketch,
            Matrix invTransfo)
        {
            Point origin = asmSketch.OriginPointGeometry;
            origin.TransformBy(invTransfo);

            if (!origin.IsEqualTo(partSketch.OriginPointGeometry, _Tolerance))
            {
                return false;
            }

            UnitVector asmAxis = GetSketchXAxis(asmSketch);
            asmAxis.TransformBy(invTransfo);

            UnitVector partAxis = GetSketchXAxis(partSketch);

            if (!asmAxis.IsEqualTo(partAxis, _Tolerance))
            {
                return false;
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Update part sketch from the assembly sketch
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void UpdateSketch(PlanarSketch asmSketch, 
            PlanarSketch partSketch, 
            Matrix invTransfo, 
            out UnitVector xAxis,
            out UnitVector yAxis)
        {
            Point origin = asmSketch.OriginPointGeometry;
            origin.TransformBy(invTransfo);

            WorkPoint workpoint = partSketch.OriginPoint as WorkPoint;
            workpoint.SetFixed(origin);
            
            UnitVector asmAxis = GetSketchXAxis(asmSketch);
            asmAxis.TransformBy(invTransfo);

            WorkAxis workaxis = partSketch.AxisEntity as WorkAxis;
            workaxis.SetFixed(origin, asmAxis);

            xAxis = asmAxis;
            yAxis = partSketch.PlanarEntityGeometry.Normal.CrossProduct(xAxis);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Returns True if the 2 profiles are identical, False otherwise
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool CompareProfiles(Profile originalProfile, Profile copyProfile)
        {
            if (originalProfile.Count != copyProfile.Count)
                return false;

            int nbOfMatchingPaths = 0;

            for (int i = 1; i <= copyProfile.Count; ++i)
            {
                ProfilePath path = copyProfile[i];

                foreach (ProfilePath origPath in originalProfile)
                {
                    if (origPath.Count == path.Count)
                    {
                        int nbMatchingEntities = 0;

                        //We dont assume the path contains the entities in the same order
                        //so need the double loop :(
                        for (int j = 1; j <= path.Count; ++j)
                        {
                            for (int k = 1; k <= path.Count; ++k)
                            {
                                if (CompareSketchEntities(origPath[j].SketchEntity, path[k].SketchEntity, _Tolerance))
                                {
                                    ++nbMatchingEntities;
                                    break;
                                }
                            }
                        }

                        //All the entities are matching -> candidate for matching
                        if (nbMatchingEntities == path.Count)
                        {
                            if (path.AddsMaterial == origPath.AddsMaterial)
                            {
                                ++nbOfMatchingPaths;
                                break;
                            }
                        }
                    }
                }
            }

            return (nbOfMatchingPaths == copyProfile.Count);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Returns null if part profile is up-to-date
        // Otherwise copy sketch content and recreate new Profile
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Profile UpdateProfile(Document asmDocument, 
            Document partDocument, 
            Profile asmProfile, 
            Profile partProfile,
            bool forceUpdate)
        {
            if (!forceUpdate && CompareProfiles(asmProfile, partProfile))
            {
                return null;
            }

            Inventor.View asmView = asmDocument.Views.Add();
            asmView.Activate();

            asmDocument.SelectSet.Clear();
            asmDocument.SelectSet.Select(asmProfile.Parent);

            FeatureUtilities.Application.CommandManager.ControlDefinitions["AppCopyCmd"].Execute();

            PlanarSketch partSketch = partProfile.Parent as PlanarSketch;

            Inventor.View partView = partDocument.Views.Add();
            partView.Activate();

            for (int i = 1; i <= partSketch.SketchEntities.Count; ++i )
            {
                try
                {
                    SketchEntity entity = partSketch.SketchEntities[i];
                    entity.Delete();
                }
                catch
                { }
            }

            partSketch.Edit();

            _Application.CommandManager.ControlDefinitions["AppPasteCmd"].Execute();

            partSketch.ExitEdit();

            Profile newPartProfile = partSketch.Profiles.AddForSolid(true, null, null);

            FeatureUtilities.CopyProfile(asmProfile, newPartProfile);

            partView.Close();

            if(asmDocument.Views.Count > 1)
                asmView.Close();

            return newPartProfile;
        }
              
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: returns true if the 2 sketch entities are identical (respectively in their own sketch),
        //      false otherwise.
        // 
        // Inventor::SketchEntity
        //    Inventor::SketchArc
        //    Inventor::SketchCircle
        //    Inventor::SketchEllipse
        //    Inventor::SketchEllipticalArc
        //    Inventor::SketchFixedSpline
        //    Inventor::SketchLine
        //    Inventor::SketchOffsetSpline
        //    Inventor::SketchPoint
        //    Inventor::SketchSpline
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static bool CompareSketchEntities(SketchEntity entity1, 
            SketchEntity entity2, 
            double tolerance)
        {
            if(entity1.Type != entity2.Type)
            {
                return false;
            }

            switch (entity1.Type)
            { 
                case ObjectTypeEnum.kSketchCircleObject:
                case ObjectTypeEnum.kSketchCircleProxyObject:

                    SketchCircle circle1 = entity1 as SketchCircle;
                    SketchCircle circle2 = entity2 as SketchCircle;

                    if(Math.Abs(circle1.Radius-circle2.Radius) > tolerance)
                    {
                        return false;
                    }

                    if (circle1.CenterSketchPoint.Geometry.DistanceTo(circle2.CenterSketchPoint.Geometry) > tolerance)
                    {
                        return false;
                    }

                    break;

                case ObjectTypeEnum.kSketchLineObject:
                case ObjectTypeEnum.kSketchLineProxyObject:

                    SketchLine line1 = entity1 as SketchLine;
                    SketchLine line2 = entity2 as SketchLine;

                    if (line1.StartSketchPoint.Geometry.DistanceTo(line2.StartSketchPoint.Geometry) > tolerance)
                    {
                        return false;
                    }

                    if (line1.EndSketchPoint.Geometry.DistanceTo(line2.EndSketchPoint.Geometry) > tolerance)
                    {
                        return false;
                    }

                    break;

                case ObjectTypeEnum.kSketchArcObject:
                case ObjectTypeEnum.kSketchArcProxyObject:

                    SketchArc arc1 = entity1 as SketchArc;
                    SketchArc arc2 = entity2 as SketchArc;

                    if (Math.Abs(arc1.Radius - arc2.Radius) > tolerance)
                    {
                        return false;
                    }
                    if (arc1.CenterSketchPoint.Geometry.DistanceTo(arc2.CenterSketchPoint.Geometry) > tolerance)
                    {
                        return false;
                    }
                    if (arc1.StartSketchPoint.Geometry.DistanceTo(arc2.StartSketchPoint.Geometry) > tolerance)
                    {
                        return false;
                    }
                    if (arc1.EndSketchPoint.Geometry.DistanceTo(arc2.EndSketchPoint.Geometry) > tolerance)
                    {
                        return false;
                    }
                   
                    break;

                case ObjectTypeEnum.kSketchEllipticalArcObject:
                case ObjectTypeEnum.kSketchEllipticalArcProxyObject:

                    SketchEllipticalArc eArc1 = entity1 as SketchEllipticalArc;
                    SketchEllipticalArc eArc2 = entity2 as SketchEllipticalArc;

                    if (Math.Abs(eArc1.MinorRadius - eArc2.MinorRadius) > tolerance)
                    {
                        return false;
                    }
                    if (Math.Abs(eArc1.MajorRadius - eArc2.MajorRadius) > tolerance)
                    {
                        return false;
                    }
                    if (eArc1.CenterSketchPoint.Geometry.DistanceTo(eArc2.CenterSketchPoint.Geometry) > tolerance)
                    {
                        return false;
                    }
                    if (eArc1.StartSketchPoint.Geometry.DistanceTo(eArc2.StartSketchPoint.Geometry) > tolerance)
                    {
                        return false;
                    }
                    if (eArc1.EndSketchPoint.Geometry.DistanceTo(eArc2.EndSketchPoint.Geometry) > tolerance)
                    {
                        return false;
                    }

                    break;

                case ObjectTypeEnum.kSketchEllipseObject:
                case ObjectTypeEnum.kSketchEllipseProxyObject:

                    SketchEllipse ell1 = entity1 as SketchEllipse;
                    SketchEllipse ell2 = entity2 as SketchEllipse;

                    if (Math.Abs(ell1.MinorRadius - ell2.MinorRadius) > tolerance)
                    {
                        return false;
                    }
                    if (Math.Abs(ell1.MajorRadius - ell2.MajorRadius) > tolerance)
                    {
                        return false;
                    }
                    if (ell1.CenterSketchPoint.Geometry.DistanceTo(ell2.CenterSketchPoint.Geometry) > tolerance)
                    {
                        return false;
                    }
                    if (!ell1.MajorAxisVector.IsEqualTo(ell2.MajorAxisVector, tolerance))
                    {
                        return false;
                    }
                    
                    break;

                case ObjectTypeEnum.kSketchFixedSplineObject:
                case ObjectTypeEnum.kSketchFixedSplineProxyObject:
                
                case ObjectTypeEnum.kSketchOffsetSplineObject:
                case ObjectTypeEnum.kSketchOffsetSplineProxyObject:

                case ObjectTypeEnum.kSketchSplineObject:
                case ObjectTypeEnum.kSketchSplineProxyObject:

                    BSplineCurve2d bsp1 = (BSplineCurve2d)entity1.GetType().InvokeMember("Geometry", System.Reflection.BindingFlags.GetProperty, null, entity1, null);
                    BSplineCurve2d bsp2 = (BSplineCurve2d)entity2.GetType().InvokeMember("Geometry", System.Reflection.BindingFlags.GetProperty, null, entity2, null);

                    double[] poles1 = new double[]{};
                    double[] knots1 = new double[]{};
                    double[] weights1 = new double[]{};

                    bsp1.GetBSplineData(ref poles1, ref knots1, ref weights1);

                    double[] poles2 = new double[] { };
                    double[] knots2 = new double[] { };
                    double[] weights2 = new double[] { };

                    bsp2.GetBSplineData(ref poles2, ref knots2, ref weights2);

                    if((poles1.Length != poles2.Length) || (knots1.Length != knots2.Length))
                    {
                        return false;
                    }

                    for (int i = 0; i < poles1.Length; ++i)
                    {
                        if (Math.Abs(poles1[i] - poles2[i]) > tolerance)
                        {
                            return false;
                        }
                    }

                    for (int i = 0; i < knots1.Length; ++i)
                    {
                        if (Math.Abs(knots1[i] - knots2[i]) > tolerance)
                        {
                            return false;
                        }
                    }

                    if (((weights1 == null) && (weights2 != null)) || ((weights1 != null) && (weights2 == null)))
                    {
                        return false;
                    }

                    if((weights1 != null) && (weights2 != null))
                    {
                        if(weights1.Length != weights2.Length)
                        {
                            return false;
                        }

                        for (int i = 0; i < weights1.Length; ++i)
                        {
                            if (Math.Abs(weights1[i] - weights2[i]) > tolerance)
                            {
                                return false;
                            }
                        }
                    }

                    break;

                default:
                    return false;
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // use: a small internal utility function
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsSameType(object asmEntity, object partEntity)
        {
            if ((asmEntity is Vertex       ||
                asmEntity is Edge          ||
                asmEntity is SketchPoint   ||
                asmEntity is SketchPoint3D ||
                asmEntity is WorkPoint     ||
                asmEntity is Edge)

                &&

                (partEntity is Vertex       ||
                partEntity is Edge          ||
                partEntity is SketchPoint   ||
                partEntity is SketchPoint3D ||
                partEntity is WorkPoint     ||
                partEntity is Edge))
            {
                return true;
            }

            if ((asmEntity is Face ||
                asmEntity is Faces ||
                asmEntity is WorkPlane)

                &&

                (partEntity is Face ||
                partEntity is Faces ||
                partEntity is WorkPlane))
            {
                return true;
            }

            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: returns true if "point" belongs to plane defined by "normal" & "pointOnPlane"
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsOnPlane(UnitVector normal, Point pointOnPlane, Point point)
        {
            //First compute equation of the plane Ax + By + Cz - D = 0
            double D = normal.X * pointOnPlane.X +
                       normal.Y * pointOnPlane.Y +
                       normal.Z * pointOnPlane.Z;

            double distFromPlane = Math.Abs(normal.X * point.X +
                                  normal.Y * point.Y +
                                  normal.Z * point.Z - D);

            return (distFromPlane < _Tolerance);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: a small internal utility function
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsEqual(object asmEntity,
            object partEntity,
            Matrix invTransfo)
        {
            if(!FeatureUtilities.IsSameType(asmEntity, partEntity))
                return false;

            if (asmEntity is Vertex        ||
                asmEntity is Edge          ||
                asmEntity is SketchPoint   ||
                asmEntity is SketchPoint3D ||
                asmEntity is WorkPoint     ||
                asmEntity is Edge)
            {
                Inventor.Point asmPoint = FeatureUtilities.GetPoint(asmEntity);
                asmPoint.TransformBy(invTransfo);

                Inventor.Point partPoint = FeatureUtilities.GetPoint(partEntity);
                
                return partPoint.IsEqualTo(asmPoint, _Tolerance);
            }

             if ((asmEntity is Face ||
                  asmEntity is Faces ||
                  asmEntity is WorkPlane))
            {

                UnitVector asmNormal = FeatureUtilities.GetNormal(asmEntity);
                asmNormal.TransformBy(invTransfo);

                UnitVector partNormal = FeatureUtilities.GetNormal(partEntity);

                if (!partNormal.IsParallelTo(asmNormal, _Tolerance))
                    return false;

                Point pointOnPlane = FeatureUtilities.GetPoint(asmEntity);
                pointOnPlane.TransformBy(invTransfo);

                Point partPoint = FeatureUtilities.GetPoint(partEntity);

                return IsOnPlane(asmNormal, pointOnPlane, partPoint);
            }
            
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Compare Extents of two features, returns true if extents are identical, false otherwise
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool CompareFeatureExtents(PartFeatureExtent asmExtent, 
            PartFeatureExtent partExtent, 
            Matrix invTransfo)
        {
            try
            {
                if (asmExtent.Type != partExtent.Type)
                    return false;

                switch (asmExtent.Type)
                {
                    case ObjectTypeEnum.kThroughAllExtentObject:
                    {
                        ThroughAllExtent asmThroughAllExtent = asmExtent as ThroughAllExtent;
                        ThroughAllExtent partThroughAllExtent = partExtent as ThroughAllExtent;

                        if (partThroughAllExtent.Direction != asmThroughAllExtent.Direction)
                            return false;

                        break;
                    }
                    case ObjectTypeEnum.kDistanceExtentObject:
                    {
                        DistanceExtent asmDistanceExtent = asmExtent as DistanceExtent;
                        DistanceExtent partDistanceExtent = partExtent as DistanceExtent;

                        if (!IsEqual(partDistanceExtent.Distance.Value, asmDistanceExtent.Distance.Value))
                            return false;

                        if (partDistanceExtent.Direction != asmDistanceExtent.Direction)
                            return false;

                        break;
                    }
                    case ObjectTypeEnum.kToExtentObject:
                    {
                        ToExtent asmToExtent = asmExtent as ToExtent;
                        ToExtent partToExtent = partExtent as ToExtent;

                        if (!FeatureUtilities.IsEqual(asmToExtent.ToEntity, 
                            partToExtent.ToEntity, 
                            invTransfo))
                            return false;

                        break;
                    }
                    case ObjectTypeEnum.kFromToExtentObject:
                    {
                        FromToExtent asmFromToExtent = asmExtent as FromToExtent;
                        FromToExtent partFromToExtent = partExtent as FromToExtent;

                        if (!FeatureUtilities.IsEqual(asmFromToExtent.FromFace, 
                            partFromToExtent.FromFace, 
                            invTransfo))
                            return false;

                        if (!FeatureUtilities.IsEqual(asmFromToExtent.ToFace,
                            partFromToExtent.ToFace,
                            invTransfo))
                            return false;

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
        // Forbbiden characters: \/:*?"<>|
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static string GetValidFileName(string occurrenceName)
        {
            string str1 = occurrenceName.Replace('\\', '_');
            string str2 = str1.Replace('/', '_');
            string str3 = str2.Replace(':', '_');
            string str4 = str3.Replace('*', '_');
            string str5 = str4.Replace('?', '_');
            string str6 = str5.Replace('"', '_');
            string str7 = str6.Replace('<', '_');
            string str8 = str7.Replace('>', '_');
            string str9 = str8.Replace('|', '_');

            return str9;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Create the definition document for a replaced occurrence.
        //      doesn't do much at the moment but better isolate this in its own method for further improvements
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void CreateDefinitionDocument(string srcFullFileName, string destFullFileName)
        {
            _Application.FileManager.CopyFile(srcFullFileName,
                   destFullFileName,
                   FileManagementEnum.kOverwriteExistingFile);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Replace definition documents for each affected occurrence by a unique copy of the original part.
        //      This is one of the key method in the migration process.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string[] ReplaceOccurrences(AssemblyDocument parentDocument, 
            List<ComponentOccurrence> participants)
        {
            List<string> unreferencedDocuments = new List<string>();

            foreach (ComponentOccurrence occurrence in participants)
            {
                if (!FeatureUtilities.IsValidOccurrence(occurrence))
                {
                    continue;
                }

                Document document = occurrence.Definition.Document as Document;

                //We don't want to replace document if it is already replaced by a unique copy.
                //If it is the case, then the document generated previously contains
                //a "DocTrackingTag" generated by the FeatureAttributeManager
                if (FeatureAttributeManager.HasDocTrackingTag(document as PartDocument))
                    continue;

                string srcFullFileName = document.FullFileName;

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(srcFullFileName);

                string destFullFileName = fileInfo.DirectoryName 
                    + "\\" 
                    + GetValidFileName(occurrence.Name) 
                    + fileInfo.Extension;

                if (System.IO.File.Exists(destFullFileName))
                {
                    FileOverwriteDlg dlg = new FileOverwriteDlg(destFullFileName);
                    dlg.ShowDialog(new WindowWrapper((IntPtr)_Application.MainFrameHWND));

                    destFullFileName = dlg.Filename;
                }

                CreateDefinitionDocument(srcFullFileName, destFullFileName);
       
                occurrence.Replace(destFullFileName, false);

                if (document.ReferencingDocuments.Count == 0)
                {
                    unreferencedDocuments.Add(srcFullFileName);
                }

                //Place doc tracking tag: prevent further replacements 
                //if same doc involved in a feature migration
                FeatureAttributeManager.CreateDocTrackingTag(srcFullFileName,
                    occurrence.Definition.Document as PartDocument);

                foreach (ComponentOccurrence parentOccurrence in occurrence.OccurrencePath)
                {
                    if (!FeatureUtilities.IsValidOccurrence(parentOccurrence))
                    {
                        continue;
                    }

                    if (parentOccurrence.DefinitionDocumentType == DocumentTypeEnum.kAssemblyDocumentObject)
                    {
                        AssemblyDocument parentAssembly = parentOccurrence.Definition.Document as AssemblyDocument;
                        parentAssembly.Update();
                    }
                }
            }

            //!!Assembly needs update after Occurrence.Replace!!
            parentDocument.Update();

            return unreferencedDocuments.ToArray();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: detects if an occurrence is a "False Participant" to a specific feature.
        //      A "False Participant" is an occurrence that is added to the feature's participants list
        //      but whose geometry isn't affected by the feature.
        //      obviously the migration will fail for such a participant, so we want to filter those out
        //      when migrating a feature. Also those false participants will appear grayed in the browser.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsFalseParticipant(PartFeature Feature, ComponentOccurrence Occurrence)
        {
            foreach (FaceProxy face in Feature.Faces)
            {
                if (face.ContainingOccurrence == Occurrence)
                {
                    return false;
                }
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Add an assembly Feature node to the browser control
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool AddAsmFeatureNode(TreeViewMultiSelect TreeView, 
            TreeNode root, 
            PartFeature Feature)
        {
            try
            {
                //Feature needs to be healthy in order to be added in the browser
                //Unexpected errors may occur otherwise
                if (!((Feature.HealthStatus == HealthStatusEnum.kUpToDateHealth) ||
                     Feature.HealthStatus == HealthStatusEnum.kSuppressedHealth))
                    return false;

                //Discard unsupported features
                if (!FeatureUtilities.IsSupportedFeature(Feature))
                    return false;

                int ImageIndex = FeatureUtilities.GetFeatureImageIndex(Feature);

                TreeNode FeatureNode = root.Nodes.Add(Feature.Name, Feature.Name);
                FeatureNode.Tag = Feature;
                FeatureNode.ImageIndex = ImageIndex;
                FeatureNode.SelectedImageIndex = FeatureNode.ImageIndex;

                TreeView.SetSuppressedState(FeatureNode, Feature.Suppressed);

                switch (Feature.Type)
                {
                    case ObjectTypeEnum.kExtrudeFeatureObject:
                    {
                        ExtrudeFeature ExtrudeFeature = Feature as ExtrudeFeature;
                        PlanarSketch sketch = ExtrudeFeature.Profile.Parent as PlanarSketch;

                        string strBased = " - Based: " + ExtrudeFeatureMigrator.GetSketchBase(ExtrudeFeature);

                        TreeNode sketchNode = FeatureNode.Nodes.Add(sketch.Name, sketch.Name + strBased, 26);
                        sketchNode.Tag = sketch;
                        break;
                    }
                    case ObjectTypeEnum.kHoleFeatureObject:
                    {
                        HoleFeature HoleFeature = Feature as HoleFeature;

                        string strPlacement = HoleFeatureMigrator.GetPlacementType(HoleFeature);

                        TreeNode plcNode = FeatureNode.Nodes.Add(strPlacement, strPlacement, 26);
                        plcNode.Tag = HoleFeature.PlacementDefinition;
                        break;
                    }

                    case ObjectTypeEnum.kRevolveFeatureObject:
                    case ObjectTypeEnum.kFilletFeatureObject:
                    case ObjectTypeEnum.kChamferFeatureObject:
                    case ObjectTypeEnum.kSweepFeatureObject:
                    case ObjectTypeEnum.kMoveFaceFeatureObject:
                    case ObjectTypeEnum.kRectangularPatternFeatureObject:
                    case ObjectTypeEnum.kCircularPatternFeatureObject:
                    case ObjectTypeEnum.kMirrorFeatureObject:
                    default:
                        break;
                }

                foreach (ComponentOccurrence occurrence in Feature.Participants)
                {
                    int imgIdx = (occurrence.DefinitionDocumentType == DocumentTypeEnum.kPartDocumentObject ? 25 : 24);

                    TreeNode occNode = FeatureNode.Nodes.Add(occurrence.Name, occurrence.Name, imgIdx);
                    occNode.Tag = occurrence;

                    if (IsFalseParticipant(Feature, occurrence))
                    {
                        occNode.ForeColor = System.Drawing.SystemColors.GrayText;
                        occNode.ImageIndex = 27;
                        occNode.SelectedImageIndex = occNode.ImageIndex;

                        occNode.Tag = null;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Add a part Feature node to the browser control
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool AddPartFeatureNode(TreeViewMultiSelect TreeView, 
            TreeNode root, 
            PartFeature Feature)
        {
            try
            {
                //Feature needs to be healthy in order to be added in the browser
                //Unexpected errors may occur otherwise
                if (Feature.HealthStatus != HealthStatusEnum.kUpToDateHealth)
                    return false;

                //Discard non migrated features
                if (!FeatureAttributeManager.IsMigratedFromAssemblyFeature(Feature))
                    return false;

                //Discard non associative features
                if (!FeatureUtilities.IsAssociativitySupported(Feature))
                    return false;

                int ImageIndex = FeatureUtilities.GetFeatureImageIndex(Feature);

                TreeNode FeatureNode = root.Nodes.Add(Feature.Name, Feature.Name);
                FeatureNode.Tag = Feature;
                FeatureNode.ImageIndex = ImageIndex;
                FeatureNode.SelectedImageIndex = FeatureNode.ImageIndex;

                TreeView.SetSuppressedState(FeatureNode, Feature.Suppressed);

                string status = "";
                bool up2date = false;

                if (!FeatureAttributeManager.IsParentAssemblyResolved(Feature))
                {
                    up2date = false;
                    status = "Status: Assembly Unresolved";   
                }
                else
                {
                    up2date = IsPartFeatureUpToDate(Feature);
                    status = "Status: " + (up2date ? " Up-to-date" : "Out-of-date");
                }

                TreeNode statusNode = FeatureNode.Nodes.Add("Status:" + Feature.Name, status, 2);

                statusNode.Tag = up2date;

                return true;
            }
            catch
            {
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // The collection could contain the various part features, sheet metal features, 
        // work planes, work axes, work points, or a SurfaceBody. 
        // Only the primary (result) surface body, obtained from ComponentDefinition.SurfaceBodies.Item(1) is a valid input. 
        // If a SurfaceBody is supplied, the only other objects that can be in the collection are 
        // work planes, work axes, work points, and surface part features. 
        // Finish features such as fillets and chamfers may be included only if their parent feature is also included. 
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static ObjectCollection CopyParentFeatures(PartComponentDefinition partCompDef,
            Document ParentDocument,
            ObjectCollection AsmParentFeatures,
            Matrix invTransfo,
            ComponentOccurrence occurrence,
            out ReportData[] reports)
        {
            ObjectCollection ParentFeatures = FeatureUtilities.Application.TransientObjects.CreateObjectCollection(null);

            List<ReportData> reportList = new List<ReportData>();

            foreach (object obj in AsmParentFeatures)
            {
                if (obj is PartFeature)
                {
                    PartFeature parentFeature = obj as PartFeature;

                    if (parentFeature.Suppressed) continue;

                    PartFeature newFeature = null;

                    switch (parentFeature.Type)
                    {
                        case ObjectTypeEnum.kExtrudeFeatureObject:
                        {
                            ExtrudeFeature parentExtrude = parentFeature as ExtrudeFeature;

                            ParentDocument = FeatureUtilities.Application.Documents.Open(ParentDocument.FullFileName, true);

                            PlanarSketch asmSketch = parentExtrude.Profile.Parent as PlanarSketch;

                            ParentDocument.SelectSet.Clear();
                            ParentDocument.SelectSet.Select(asmSketch);

                            FeatureUtilities.Application.CommandManager.ControlDefinitions["AppCopyCmd"].Execute();

                            Document occurenceDocument = FeatureUtilities.Application.Documents.Open((partCompDef.Document as Document).FullFileName, true);

                            break;
                        }

                        default: 
                            break;
                    }

                    newFeature = GetFeatureMigrator(parentFeature.Type).CopyFeature(partCompDef, parentFeature, invTransfo);

                    if (newFeature != null)
                    {
                        reportList.Add(new ReportData(partCompDef.Document as PartDocument, 
                            newFeature));
                        ParentFeatures.Add(newFeature);
                    }
                }
            }

            reports = reportList.ToArray();

            return ParentFeatures;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: a small utility method: converts HealthStatusEnum to a string
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetHealthStatusString(HealthStatusEnum status)
        {
            switch (status)
            {
                case HealthStatusEnum.kBeyondStopNodeHealth:
                    return "Beyond Stop Node";
                case HealthStatusEnum.kCannotComputeHealth:
                    return "Cannot Compute";
                case HealthStatusEnum.kDeletedHealth:
                    return "Deleted";
                case HealthStatusEnum.kDriverLostHealth:
                    return "Driver Lost";
                case HealthStatusEnum.kInconsistentHealth:
                    return "Inconsistent";
                case HealthStatusEnum.kInErrorHealth:
                    return "In Error";
                case HealthStatusEnum.kOutOfDateHealth:
                    return "Out Of Date";
                case HealthStatusEnum.kRedundantHealth:
                    return "Redundant";
                case HealthStatusEnum.kSuppressedHealth:
                    return "Suppressed";
                case HealthStatusEnum.kUnknownHealth:
                    return "Unknown";
                case HealthStatusEnum.kUpToDateHealth:
                    return "Up To Date";
                default:
                    return "HealthStatus Unknown";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Performs migration for an assembly feature
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static FeatureReport SendToParts(AssemblyDocument parentDocument, 
            PartFeature Feature, 
            List<ComponentOccurrence> participants)
        {
            FeatureReport result = null;

            result = GetFeatureMigrator(Feature.Type).SendToParts(parentDocument, participants, Feature);

            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Performs update for a list of part features migrated from assembly
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void UpdateFeaturesFromAsm(PartDocument partDocument, List <PartFeature> Features)
        {
            foreach (PartFeature partFeature in Features)
            {
                bool closeAsm;

                AssemblyDocument asm = FeatureAttributeManager.FindParentAssembly(partFeature, out closeAsm);

                if (asm != null)
                {
                    PartFeature asmFeature;
                    ComponentOccurrence occurrence;

                    if (FeatureAttributeManager.FindParentFeature(asm, partFeature, 
                        out asmFeature, 
                        out occurrence))
                    {
                        FeatureUtilities.UpdateFeatureFromAsm(asmFeature, partFeature, occurrence);
                    }

                    if (closeAsm)
                    {
                        asm.Close(true);
                    }
                }
            }

            partDocument.Update();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: returns true if PartFeature is of supported type for migration.
        //      The return value depends of the individual implementation of each FeatureMigrator
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsAssociativitySupported(PartFeature Feature)
        {
            return GetFeatureMigrator(Feature.Type).IsAssociativitySupported(Feature);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Performs update of "partExtent" from "asmExtent"
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool UpdateFeatureExtent(PartFeatureExtent asmExtent,
            PartFeatureExtent partExtent,
            Matrix invTransfo)
        {
            if(asmExtent.Type != partExtent.Type)
                return false;

            PartFeature partFeature = partExtent.Parent;

            PartComponentDefinition partCompDef = partFeature.Parent as PartComponentDefinition;

            switch (asmExtent.Type)
            {
                case ObjectTypeEnum.kThroughAllExtentObject:
                {
                    ThroughAllExtent asmThroughAllExtent = asmExtent as ThroughAllExtent;

                    ThroughAllExtent partThroughAllExtent = partExtent as ThroughAllExtent;

                    if(partThroughAllExtent.Direction != asmThroughAllExtent.Direction)
                        partThroughAllExtent.Direction = asmThroughAllExtent.Direction;

                    break;
                }
                case ObjectTypeEnum.kDistanceExtentObject:
                {
                    DistanceExtent asmDistanceExtent = asmExtent as DistanceExtent;

                    DistanceExtent partDistanceExtent = partExtent as DistanceExtent;

                    partDistanceExtent.Distance.Value = asmDistanceExtent.Distance.Value;

                    if(partDistanceExtent.Direction != asmDistanceExtent.Direction)
                        partDistanceExtent.Direction = asmDistanceExtent.Direction;

                    break;
                }
                case ObjectTypeEnum.kToExtentObject:
                {
                    ToExtent asmToExtent = asmExtent as ToExtent;

                    object ToEntity = FeatureUtilities.CopyFromToEntity(asmToExtent.ToEntity,
                            partCompDef,
                            invTransfo);

                    ToExtent partToExtent = partExtent as ToExtent;

                    partToExtent.ToEntity = ToEntity;

                    partToExtent.ExtendToFace = false;

                    break;
                }
                case ObjectTypeEnum.kFromToExtentObject:
                {
                    FromToExtent asmFromToExtent = asmExtent as FromToExtent;

                    object FromEntity = FeatureUtilities.CopyFromToEntity(asmFromToExtent.FromFace,
                           partCompDef,
                           invTransfo);

                    object ToEntity = FeatureUtilities.CopyFromToEntity(asmFromToExtent.ToFace,
                       partCompDef,
                       invTransfo);

                    FromToExtent partFromToExtent = partExtent as FromToExtent;

                    partFromToExtent.FromFace = FromEntity;
                    partFromToExtent.ExtendFromFace = false;

                    partFromToExtent.ToFace = ToEntity;
                    partFromToExtent.ExtendToFace = false;

                    break;
                }
                default:
                    return false;
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Performs update of "partFeature" from "asmFeature"
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool UpdateFeatureFromAsm(PartFeature asmFeature, 
            PartFeature partFeature, 
            ComponentOccurrence occurrence)
        {
            Matrix invTransfo = occurrence.Transformation;
            invTransfo.Invert();

            return GetFeatureMigrator(asmFeature.Type).UpdateFeatureFromAsm(asmFeature, 
                partFeature, 
                invTransfo);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Returns if "partFeature" is up-to-date regarding of the properties of the corresponding 
        //      assembly feature it was migrated from.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsPartFeatureUpToDate(PartFeature partFeature)
        {
            bool status = false;

            bool closeAsm;

            AssemblyDocument asm = FeatureAttributeManager.FindParentAssembly(partFeature, out closeAsm);

            if (asm != null)
            {
                PartFeature asmFeature;
                ComponentOccurrence occurrence;

                if (FeatureAttributeManager.FindParentFeature(asm, partFeature, out asmFeature, out occurrence))
                {
                    Matrix invTransfo = occurrence.Transformation;
                    invTransfo.Invert();

                    status = GetFeatureMigrator(asmFeature.Type).IsPartFeatureUpToDate(asmFeature,
                            partFeature,
                            invTransfo);
                }

                if (closeAsm)
                {
                    asm.Close(true);
                }
            }

            return status;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: 
        //      
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsValidOccurrence(ComponentOccurrence occurrence)
        {
            if (occurrence.Suppressed)
            {
                return false;
            }

            if (occurrence.ReferencedFileDescriptor != null)
            {
                ReferenceStatusEnum status = occurrence.ReferencedFileDescriptor.ReferenceStatus;

                if (status == ReferenceStatusEnum.kMissingReference ||
                    status == ReferenceStatusEnum.kOutOfDateReference ||
                    status == ReferenceStatusEnum.kUnknownReference)
                {
                    return false;
                }
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Use: Returns true if assembly document and all its sub-components are saved on the disk.
        //      This is needed in order for the migration to succeed.
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public enum AssemblyStateEnum
        {
            kAssemblyNotSaved,
            kNotMasterLODActive,
            kAssemblyOk,
            kException
        }

        public static AssemblyStateEnum CheckAssemblyState(AssemblyDocument asm)
        {
            try
            {
                if (asm.FullFileName == "")
                    return AssemblyStateEnum.kAssemblyNotSaved;

                return CheckAssemblyStateRec(asm.ComponentDefinition.Occurrences);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace,
                   "Exception occurred!",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);

                return AssemblyStateEnum.kException;
            }
        }

        private static AssemblyStateEnum CheckAssemblyStateRec(ComponentOccurrences occurrences)
        {
            //Iterate through the components in the current collection
            foreach (ComponentOccurrence occurrence in occurrences)
            {
                if (!FeatureUtilities.IsValidOccurrence(occurrence))
                {
                    continue;
                }

                Document doc = occurrence.Definition.Document as Document;

                if (doc.FullFileName == "")
                    return AssemblyStateEnum.kAssemblyNotSaved;

                //Recursively call this function for the sub-occurrences of the current component
                AssemblyStateEnum res = CheckAssemblyStateRec(occurrence.SubOccurrences);

                if (res != AssemblyStateEnum.kAssemblyOk)
                    return res;
            }

            return AssemblyStateEnum.kAssemblyOk;
        }

        private static AssemblyStateEnum CheckAssemblyStateRec(ComponentOccurrencesEnumerator occurrences)
        {
            //Iterate through the components in the current collection
            foreach (ComponentOccurrence occurrence in occurrences)
            {
                if (!FeatureUtilities.IsValidOccurrence(occurrence))
                {
                    continue;
                }

                Document doc = occurrence.Definition.Document as Document;

                if (doc.FullFileName == "")
                    return AssemblyStateEnum.kAssemblyNotSaved;

                //Recursively call this function for the sub-occurrences of the current component
                AssemblyStateEnum res = CheckAssemblyStateRec(occurrence.SubOccurrences);

                if (res != AssemblyStateEnum.kAssemblyOk)
                    return res;
            }

            return AssemblyStateEnum.kAssemblyOk;
        }

        internal static void BackupFile(List<ComponentOccurrence> participants)
        {

            foreach (ComponentOccurrence occurrence in participants)
            {
                if (!FeatureUtilities.IsValidOccurrence(occurrence))
                {
                    continue;
                }

                var document = occurrence.Definition.Document as Document;

                string srcFullFileName = document.FullFileName;

                var fileInfo = new System.IO.FileInfo(srcFullFileName);

                string destFullFileName = fileInfo.DirectoryName
                                          + "\\"
                                          + GetValidFileName(occurrence.Name) + "backup"
                                          + fileInfo.Extension;

                if (System.IO.File.Exists(destFullFileName))
                {
                    var dlg = new FileOverwriteDlg(destFullFileName);
                    dlg.ShowDialog(new WindowWrapper((IntPtr) _Application.MainFrameHWND));

                    destFullFileName = dlg.Filename;
                }

                CreateDefinitionDocument(srcFullFileName, destFullFileName);
            }
        }
    }
}
