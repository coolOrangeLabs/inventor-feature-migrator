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
    public class FeatureAttributeManager
    {
        private static string _AttributeSet = "Autodesk.FeatureMigratorLib.FeatureInfo";

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool HasDocTrackingTag(PartDocument document)
        {
            AttributeSet set = null;

            try
            {
                set = document.ComponentDefinition.AttributeSets[_AttributeSet];
                return set.NameIsUsed["SourcePartDoc"];
            }
            catch
            {
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void CreateDocTrackingTag(string srcFullFileName, PartDocument destDoc)
        {
            AttributeSet set = null;

            try
            {
                set = destDoc.ComponentDefinition.AttributeSets[_AttributeSet];
                set["SourcePartDoc"].Value = srcFullFileName;
            }
            catch
            {
                set = destDoc.ComponentDefinition.AttributeSets.Add(_AttributeSet, true);
                set.Add("SourcePartDoc", ValueTypeEnum.kStringType, srcFullFileName);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void CreatePartFeatureTag(AssemblyDocument AsmDocument, 
            PartFeature AsmFeature, 
            PartFeature PartFeature,
            ComponentOccurrence Occurrence)
        {
            if (PartFeature == null)
                return;

            AttributeSet set = PartFeature.AttributeSets.Add(_AttributeSet, true);

            set.Add("AsmFile", ValueTypeEnum.kStringType, AsmDocument.FullFileName);
            set.Add("AsmDoc", ValueTypeEnum.kStringType, AsmDocument.FullDocumentName);
            set.Add("AsmIntName", ValueTypeEnum.kStringType, AsmDocument.InternalName);
            set.Add("AsmFeature", ValueTypeEnum.kStringType, AsmFeature.Name);


            Byte[] contextData = new byte[] { };

            int keyContext = AsmDocument.ReferenceKeyManager.CreateKeyContext();

            AsmDocument.ReferenceKeyManager.SaveContextToArray(keyContext, ref contextData);

            set.Add("ContextData", ValueTypeEnum.kByteArrayType, contextData);


            Byte[] featureRefKey = new byte[] { };

            AsmFeature.GetReferenceKey(ref featureRefKey, keyContext);

            set.Add("AsmFeatureRefKey", ValueTypeEnum.kByteArrayType, featureRefKey);


            Byte[] occRefKey = new byte[] { };

            Occurrence.GetReferenceKey(ref occRefKey, keyContext);

            set.Add("OccurrenceRefKey", ValueTypeEnum.kByteArrayType, occRefKey);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void CreateOccurrenceTag(AssemblyDocument AsmDocument, PartFeature PartFeature, ComponentOccurrence occurrence)
        {
            AttributeSet set = PartFeature.AttributeSets[_AttributeSet];

            Byte[] contextData = set["ContextData"].Value as Byte[];

            int keyContext = AsmDocument.ReferenceKeyManager.LoadContextFromArray(ref contextData);

            Byte[] refKey = new byte[] { };

            occurrence.GetReferenceKey(ref refKey, keyContext);

            set.Add("OccurrenceRefKey", ValueTypeEnum.kByteArrayType, refKey);


            PartDocument doc = PartFeature.Parent.Document as PartDocument;

            set = doc.ComponentDefinition.AttributeSets[_AttributeSet];

            string srcDocPath = set["SourcePartDoc"].Value as string;

            PartDocument srcDoc = FeatureUtilities.Application.Documents.Open(srcDocPath, false) as PartDocument;


            foreach (PartFeature feature in doc.ComponentDefinition.Features)
            {
                if (feature.AttributeSets.get_NameIsUsed(_AttributeSet))
                {
                    AttributeSet setFeature = feature.AttributeSets[_AttributeSet];

                    string AsmFeature = setFeature["AsmFeature"].Value as string;

                    if (!setFeature.get_NameIsUsed("OccurrenceRefKey")) 
                    {
                        ObjectCollection collec = srcDoc.AttributeManager.FindObjects(_AttributeSet, "AsmFeature", AsmFeature);

                        if (collec.Count > 0)
                        {
                            PartFeature srcFeature = collec[1] as PartFeature;

                            object val = srcFeature.AttributeSets[_AttributeSet]["OccurrenceRefKey"].Value;

                            setFeature.Add("OccurrenceRefKey", ValueTypeEnum.kByteArrayType, val);
                        }
                    }
                }
            }

            srcDoc.Close(true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsMigratedFromAssemblyFeature(PartFeature PartFeature)
        {
            //Simple check. Do not check for attributes
            return PartFeature.AttributeSets.get_NameIsUsed(_AttributeSet);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsParentAssemblyResolved(PartFeature PartFeature)
        {
            try
            {
                AttributeSet set = PartFeature.AttributeSets[_AttributeSet];

                string FullFileName = set["AsmFile"].Value as string;

                return System.IO.File.Exists(FullFileName);
            }
            catch
            {
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static AssemblyDocument FindParentAssembly(PartFeature PartFeature, out bool closeAsm)
        {
            closeAsm = false;

            try
            {
                AttributeSet set = PartFeature.AttributeSets[_AttributeSet];

                string FullFileName = set["AsmFile"].Value as string;

                string FullDocumentName = set["AsmDoc"].Value as string;

                string InternalName = set["AsmIntName"].Value as string;


                if(!System.IO.File.Exists(FullFileName))
                {
                    //Fire up FileResolution Dialog
                    FileResolutionDlg dlg = new FileResolutionDlg(FullFileName, InternalName);
                    dlg.ShowDialog(new WindowWrapper((IntPtr)FeatureUtilities.Application.MainFrameHWND));

                    if(dlg.Filename == "")
                        return null;

                    FullFileName = dlg.Filename;
                }

                Document AsmDoc;

                try
                {
                    //if doc was already open, let closeAsm = false (we dont want to close it then)
                    AsmDoc = FeatureUtilities.Application.Documents.get_ItemByName(FullFileName);   
                }
                catch
                {
                    AsmDoc = FeatureUtilities.Application.Documents.Open(FullFileName, false);
                    closeAsm = true;
                }

                //Perform internal name check
                if (AsmDoc.InternalName != set["AsmIntName"].Value as string)
                {
                    if (closeAsm)
                        AsmDoc.Close(true);
                    
                    //Not the original doc
                    System.Windows.Forms.MessageBox.Show("The assembly file you selected is invalid: Internal name doesn't match original document",
                        "Invalid File",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Error);

                    return null;
                }

                //Update AsmFile attribute with new filename 
                //for all existing features migrated from this assembly
                if (FullFileName != (string)set["AsmFile"].Value)
                {
                    Document document = PartFeature.Parent.Document as Document;
                    UpdateForResolvedAssembly(document, (string)set["AsmFile"].Value, FullFileName);
                }

                return AsmDoc as AssemblyDocument;
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
        public static void UpdateForResolvedAssembly(Document document,
            string oldFullFileName, 
            string newFullFileName)
        {
            AttributeManager AttributeManager = document.AttributeManager;

            AttributesEnumerator AttributesEnum = AttributeManager.FindAttributes(_AttributeSet, 
                "AsmFile", 
                oldFullFileName);

            foreach (Inventor.Attribute attribute in AttributesEnum)
            {
                attribute.Value = newFullFileName;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool FindParentFeature(AssemblyDocument ParentDocument, 
            PartFeature partFeature, 
            out PartFeature asmFeature,
            out ComponentOccurrence occurrence)
        {
            asmFeature = null;
            occurrence = null;

            try
            {
                AttributeSet set = partFeature.AttributeSets[_AttributeSet];

                Byte[] contextData = set["ContextData"].Value as Byte[];

                int keyContext = ParentDocument.ReferenceKeyManager.LoadContextFromArray(ref contextData);


                Byte[] refKey1 = set["AsmFeatureRefKey"].Value as Byte[];

                object MatchType1;

                asmFeature = ParentDocument.ReferenceKeyManager.BindKeyToObject(ref refKey1,
                    keyContext, 
                    out MatchType1) as PartFeature;


                Byte[] refKey2 = set["OccurrenceRefKey"].Value as Byte[];

                object MatchType2;

                occurrence = ParentDocument.ReferenceKeyManager.BindKeyToObject(ref refKey2,
                    keyContext,
                    out MatchType2) as ComponentOccurrence;

                if (asmFeature != null && occurrence != null)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

    }
}
