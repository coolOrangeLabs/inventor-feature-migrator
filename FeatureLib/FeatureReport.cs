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
    public enum FinalizeActionEnum
    { 
        kNone,
        kSuppress,
        kDeleteAll,
        kDeleteRetainConsumedSketches,
        kDeleteRetainSketchesFeatAndWorkFeat
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // The ReportData holds report information about a specific migrated feature in a part
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ReportData
    { 
        private PartDocument _Document;
        private PartFeature _PartFeature;

        private string _FullFileName;
        private string _FeatureName;

        private FinalizeActionEnum _FinalizeAction;
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ReportData(PartDocument document, PartFeature newFeature)
        {
            _Document = document;

            _PartFeature = newFeature;

            _FullFileName = _Document.FullFileName;

            _FeatureName = (_PartFeature != null ? _PartFeature.Name : "");

            _FinalizeAction = FinalizeActionEnum.kNone;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Load()
        {
            _Document = FeatureUtilities.Application.Documents.Open(_FullFileName, false) 
                as PartDocument;

            if (_FeatureName != "")
            {
                _PartFeature = _Document.ComponentDefinition.Features[_FeatureName];
            }
            else
            {
                _PartFeature = null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PartDocument Document
        {
            get
            {
                return _Document;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PartFeature PartFeature
        {
            get
            {
                return _PartFeature;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string FeatureName
        {
            get
            {
                return _FeatureName;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public FinalizeActionEnum FinalizeAction
        {
            set
            {
                _FinalizeAction = value;
            }
            get
            {
                return _FinalizeAction;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetRenderStyle(AssemblyDocument asmDoc, 
            PartFeature AssemblyFeature, 
            string styleName)
        {
            string stylename = ""; 

            try
            {
                if (styleName == "As Assembly Feature")
                {
                    StyleSourceTypeEnum srcType;
                    RenderStyle asmStyle = AssemblyFeature.GetRenderStyle(out srcType);

                    RenderStyle style = null;

                    if (asmStyle != null)
                    {
                        stylename = asmStyle.Name;
                        style = _Document.RenderStyles[stylename];
                    }

                    _PartFeature.SetRenderStyle(srcType, style);
                }
                else if (styleName == "As Part")
                {
                    _PartFeature.SetRenderStyle(StyleSourceTypeEnum.kPartRenderStyle, null);
                }
                else
                {
                    stylename = styleName;

                    RenderStyle style = _Document.RenderStyles[stylename];

                    _PartFeature.SetRenderStyle(StyleSourceTypeEnum.kOverrideRenderStyle, style); 
                }
            }
            catch
            {
                try
                {
                    //copy render style from assembly
                    RenderStyle asmStyle = asmDoc.RenderStyles[stylename];

                    RenderStyle newStyle = _Document.RenderStyles.Add(stylename);

                    byte red, green, blue;

                    asmStyle.GetAmbientColor(out red, out green, out blue);
                    newStyle.SetAmbientColor(red, green, blue);

                    asmStyle.GetDiffuseColor(out red, out green, out blue);
                    newStyle.SetDiffuseColor(red, green, blue);
                    
                    asmStyle.GetEmissiveColor(out red, out green, out blue);
                    newStyle.SetEmissiveColor(red, green, blue);

                    asmStyle.GetSpecularColor(out red, out green, out blue);
                    newStyle.SetSpecularColor(red, green, blue);

                    newStyle.DisplayInteriorFaces = asmStyle.DisplayInteriorFaces;
                    newStyle.Opacity = asmStyle.Opacity;

                    newStyle.TextureFilename = asmStyle.TextureFilename;
                    newStyle.TextureRotation = asmStyle.TextureRotation;
                    newStyle.TextureScale = asmStyle.TextureScale;

                    newStyle.Reflectivity = asmStyle.Reflectivity;
                    newStyle.ReflectionImageFilename = asmStyle.ReflectionImageFilename;
                    newStyle.Refraction = asmStyle.Refraction;

                    if (styleName == "As Assembly Feature")
                    {
                        StyleSourceTypeEnum srcType;
                        AssemblyFeature.GetRenderStyle(out srcType);

                        _PartFeature.SetRenderStyle(srcType, newStyle);
                    }
                    else
                    {
                        _PartFeature.SetRenderStyle(StyleSourceTypeEnum.kOverrideRenderStyle, newStyle);
                    }
                }
                catch 
                { 
                    //giving up
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public enum ReportStatusEnum
    { 
        kSuccess,
        kPartial,
        kFailed
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // The FeatureReport holds report information about a specific migrated feature in an assembly.
    // because a migrated asm feature generates potentially several part features,  
    // FeatureReport hold a list of ReportData, one for each generated part feature due to migration
    // of this specific asm feature.
    //
    // The FeatureReport are displayed to the user through ReportControl/DetailReportControl dialogs
    //
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class FeatureReport
    {
        private int _nbUpToDateHealth;

        private string _Style;

        private PartFeature _AssemblyFeature;

        private Dictionary<string, bool> _UnrefDocuments;

        private List<ReportData> _ReportDataList;

        private FinalizeActionEnum _FinalizeAction;
        private readonly string _assemblyFeatureName;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public FeatureReport(PartFeature assemblyFeature)
        {
            _nbUpToDateHealth = 0;

            _AssemblyFeature = assemblyFeature;

            _assemblyFeatureName = _AssemblyFeature.Name;

            _ReportDataList = new List<ReportData>();

            _UnrefDocuments = new Dictionary<string, bool>();

            _FinalizeAction = FinalizeActionEnum.kNone;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public PartFeature AssemblyFeature
        {
            get
            {
                return _AssemblyFeature;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public FinalizeActionEnum FinalizeAction
        {
            set
            {
                _FinalizeAction = value;
            }
            get
            {
                return _FinalizeAction;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Style
        {
            set
            {
                _Style = value;
            }
            get
            {
                return _Style;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ReportStatusEnum ReportStatus
        {
            get
            {
                if(_nbUpToDateHealth == 0) 
                    return ReportStatusEnum.kFailed;

                if (_nbUpToDateHealth < _ReportDataList.Count) 
                    return ReportStatusEnum.kPartial;

                return ReportStatusEnum.kSuccess;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string[] UnreferencedDocuments
        {
            set
            {
                foreach (string unrefDoc in value)
                {
                    _UnrefDocuments.Add(unrefDoc, false);
                }
            }
            get
            {
                string [] unrefDocsArray = new string[_UnrefDocuments.Count];

                _UnrefDocuments.Keys.CopyTo(unrefDocsArray, 0);

                return unrefDocsArray;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetUnrefDocAction(string doc)
        { 
            return _UnrefDocuments[doc];
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetUnrefDocAction(string doc, bool delete)
        {
            _UnrefDocuments[doc] = delete;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetAllUnrefDocsAction(bool delete)
        {
            List<string> Keys = new List<string>();

            foreach (string doc in _UnrefDocuments.Keys)
            {
                Keys.Add(doc);
            }

            foreach (string doc in Keys)
            {
                _UnrefDocuments[doc] = delete;
            }

            Keys.Clear();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int UnrefDocsCount
        {
            get
            {
                return _UnrefDocuments.Count;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<ReportData> ReportDataList
        {
            get
            {
                return _ReportDataList;
            }
        }

        public string AssemblyFeatureName
        {
            get { return _assemblyFeatureName; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void addReportData(ReportData reportData)
        {
            if ((reportData.PartFeature != null) && (reportData.PartFeature.HealthStatus == HealthStatusEnum.kUpToDateHealth))
            {
                ++_nbUpToDateHealth;
            }

            _ReportDataList.Add(reportData);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SaveDocuments()
        {
            foreach (ReportData reportData in _ReportDataList)
            {
                reportData.Document.Save();
                reportData.Document.Close(true);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UnloadReportData()
        {
            foreach (ReportData reportData in _ReportDataList)
            {
                reportData.Document.Close(true);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ActionFinalize()
        {
            AssemblyDocument asmDoc = _AssemblyFeature.Parent.Document as AssemblyDocument;

            foreach (ReportData reportData in _ReportDataList)
            {
                if (reportData.PartFeature != null)
                {
                    switch(reportData.FinalizeAction)
                    {
                        case FinalizeActionEnum.kNone:
                            break;

                        case FinalizeActionEnum.kSuppress:
                            reportData.PartFeature.Suppressed = true;
                            break;

                        case FinalizeActionEnum.kDeleteAll:
                            reportData.PartFeature.Delete(false, false, false);
                            break;

                        case FinalizeActionEnum.kDeleteRetainConsumedSketches:
                            reportData.PartFeature.Delete(true, false, false);
                            break;

                        case FinalizeActionEnum.kDeleteRetainSketchesFeatAndWorkFeat:
                            reportData.PartFeature.Delete(true, true, true);
                            break;

                        default:
                            break;
                    }

                    //Set RenderStyle
                    reportData.SetRenderStyle(asmDoc, _AssemblyFeature, _Style);
                }

                //reportData.Document.Save();
                //reportData.Document.Close(true);

                switch(_FinalizeAction)
                {
                    case FinalizeActionEnum.kNone:
                        break;

                    case FinalizeActionEnum.kSuppress:
                        _AssemblyFeature.Suppressed = true;
                        break;

                    case FinalizeActionEnum.kDeleteAll:
                        _AssemblyFeature.Delete(false, false, false);
                        break;

                    case FinalizeActionEnum.kDeleteRetainConsumedSketches:
                        _AssemblyFeature.Delete(true, false, false);
                        break;

                    case FinalizeActionEnum.kDeleteRetainSketchesFeatAndWorkFeat:
                        _AssemblyFeature.Delete(true, true, true);
                        break;

                    default:
                        break;
                }
            }

            foreach (KeyValuePair<string, bool> entry in _UnrefDocuments)
            {
                if (entry.Value)
                {
                    FeatureUtilities.Application.FileManager.DeleteFile(entry.Key, FileManagementEnum.kDeleteFileMask);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Creates a log file of the feature migration
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void LogReport(List<FeatureReport> results, string logfile)
        { 
            System.IO.StreamWriter logger;

            if (!System.IO.File.Exists(logfile))
            {
              logger = new System.IO.StreamWriter(logfile);
            }
            else
            {
              logger = System.IO.File.AppendText(logfile);
            }
  
            logger.WriteLine("_______________________________________________________________________________________");
            logger.WriteLine("");
            logger.WriteLine("Migration Log  " + DateTime.Now.ToLongDateString() + " " +  DateTime.Now.ToLongTimeString());
            logger.WriteLine("_______________________________________________________________________________________");

            foreach(FeatureReport report in results)
            {
                logger.WriteLine("");
                logger.WriteLine("");
                logger.WriteLine("Assembly Feature: " + report.AssemblyFeatureName);
                logger.WriteLine("Status: " + report.ReportStatus.ToString());
                logger.WriteLine("Action: " + report.FinalizeAction.ToString());

                foreach (ReportData data in report.ReportDataList)
                {
                    logger.WriteLine("");
                    logger.WriteLine("\t-----");
                    logger.WriteLine("");
                    logger.WriteLine("\tPart Feature: " + data.FeatureName);
                    logger.WriteLine("");
                    logger.WriteLine("\tDocument: " + data.Document.FullFileName);
                    logger.Flush();
                }

                logger.WriteLine("");
                logger.WriteLine("\t-----");
                logger.WriteLine("");
                logger.WriteLine("\tUnreferenced Documents: " + report.UnrefDocsCount.ToString());

                foreach (string doc in report.UnreferencedDocuments)
                {
                    logger.WriteLine("");
                    logger.WriteLine("\t" + doc);
                    logger.Flush();
                }
            }

            logger.Close();
        }        
    }
}
