using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Paroxe.PdfRenderer.Internal.Viewer
{
    [CustomEditor(typeof(PDFViewer), true)]
    public class PDFViewerEditor : Editor
    {
        GUIStyle m_Background1;
        GUIStyle m_Background2;
        GUIStyle m_Background3;
        Texture2D m_Logo;
        PDFViewer pdfViewer = null;
        PRParoxeBanner m_ParoxeBanner;

        public override void OnInspectorGUI()
        {
            if (pdfViewer != null)
            {
                Undo.RecordObject(pdfViewer, "PDFViewer");

                if (pdfViewer.m_Internal != null)
                    Undo.RecordObject(pdfViewer.m_Internal, "PDFViewer_Internal");
            }

            if (m_Logo != null)
            {
                Rect rect = GUILayoutUtility.GetRect(m_Logo.width, m_Logo.height);
                GUI.DrawTexture(rect, m_Logo, ScaleMode.ScaleToFit);
            }

            if (pdfViewer.m_Internal == null && IsPrefabGhost(pdfViewer.transform))
            {
                return;
            }

            pdfViewer.m_Internal.m_BannerIsOpened = m_ParoxeBanner.DoOnGUI(pdfViewer.m_Internal.m_BannerIsOpened);

            GUILayout.BeginVertical("Box");
            pdfViewer.m_Internal.m_UiShowLoadOptions = PRHelper.GroupHeader("Load Options", pdfViewer.m_Internal.m_UiShowLoadOptions);
            if (pdfViewer.m_Internal.m_UiShowLoadOptions)
            {
                pdfViewer.FileSource = (PDFViewer.FileSourceType)EditorGUILayout.EnumPopup("Source", pdfViewer.FileSource);

                if (pdfViewer.FileSource == PDFViewer.FileSourceType.Resources
                    || pdfViewer.FileSource == PDFViewer.FileSourceType.StreamingAssets
                    || pdfViewer.FileSource == PDFViewer.FileSourceType.PersistentData)
                {
                    EditorGUI.indentLevel++;

                    pdfViewer.Folder = EditorGUILayout.TextField("Folder", pdfViewer.Folder);
                    pdfViewer.FileName = EditorGUILayout.TextField("File Name", pdfViewer.FileName);

                    if (pdfViewer.FileSource == PDFViewer.FileSourceType.Resources)
                    {

                        if (File.Exists(Application.dataPath + "/Resources/" + pdfViewer.GetFileLocation())
                            &&
                            !File.Exists(Application.dataPath + "/Resources/" +
                                         pdfViewer.GetFileLocation().Replace(".bytes", "") + ".bytes"))
                        {
                            EditorGUILayout.HelpBox(
                                "PDF file in resources folder need to have .bytes extension to allow PDFViewer to access it correctly. \n\r    For example => pdf_sample.pdf.bytes",
                                MessageType.Warning);
                            GUILayout.Space(10.0f);
                        }
                    }

                    EditorGUI.indentLevel--;
                }
                else if (pdfViewer.FileSource == PDFViewer.FileSourceType.Web)
                {
                    EditorGUI.indentLevel++;
                    pdfViewer.FileURL = EditorGUILayout.TextField("Url", pdfViewer.FileURL);
                    EditorGUI.indentLevel--;
                }
                else if (pdfViewer.FileSource == PDFViewer.FileSourceType.FilePath)
                {
                    EditorGUI.indentLevel++;
                    pdfViewer.FilePath = EditorGUILayout.TextField("File Path", pdfViewer.FilePath);

                    EditorGUI.indentLevel--;
                }

                if (pdfViewer.FileSource != PDFViewer.FileSourceType.Bytes
                    && pdfViewer.FileSource != PDFViewer.FileSourceType.None)
                {

#if UNITY_IOS

            if (pdfViewer.FileSource == PDFViewer.FileSourceType.Web)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(" ");
                EditorGUILayout.HelpBox(
                    "You may need to add NSAppTransportSecurity entry in info.plist of the XCode project to allow PDFViewer to download pdf from web:\n\r\n\r" +
                    "<key>NSAppTransportSecurity</key>\n\r" +
                    "    <dict>\n\r" +
                    "    <key>NSAllowsArbitraryLoads</key>\n\r" +
                    "        <true/>\n\r" +
                    "</dict>", MessageType.Info);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10.0f);

            }
#endif
                    if (pdfViewer.FileSource == PDFViewer.FileSourceType.StreamingAssets
                        || pdfViewer.FileSource == PDFViewer.FileSourceType.PersistentData
                        || pdfViewer.FileSource == PDFViewer.FileSourceType.Resources
                        || pdfViewer.FileSource == PDFViewer.FileSourceType.FilePath)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(" ");
                        GUI.color = Color.green;

                        if (GUILayout.Button("Browse"))
                        {
                            string baseDirectory = "";

                            if (pdfViewer.FileSource == PDFViewer.FileSourceType.PersistentData)
                                baseDirectory = Application.persistentDataPath;
                            else if (pdfViewer.FileSource == PDFViewer.FileSourceType.StreamingAssets)
                                baseDirectory = Application.streamingAssetsPath;
                            else if(pdfViewer.FileSource == PDFViewer.FileSourceType.Resources)
                                baseDirectory = "Assets/Resources";
                            else if (pdfViewer.FileSource == PDFViewer.FileSourceType.FilePath)
                            {
                                string projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, ".."));
                                projectRoot = projectRoot.Replace('\\', '/');

                                baseDirectory = projectRoot;
                            }

                            if (!Directory.Exists(baseDirectory))
                            {
                                Directory.CreateDirectory(baseDirectory);
                                AssetDatabase.Refresh();
                            }

                            string folder = "";
                            string fileName = "";
                            string filePath = "";
                            bool usePersistentData = false;
                            bool useStreamingAssets = false;
                            bool useResources = false;
                            bool useFilePath = false;

                            if (Browse(baseDirectory, ref fileName, ref folder, ref filePath, ref useStreamingAssets, ref usePersistentData, ref useResources, ref useFilePath))
                            {
                                if (useStreamingAssets)
                                    pdfViewer.FileSource = PDFViewer.FileSourceType.StreamingAssets;
                                else if (usePersistentData)
                                    pdfViewer.FileSource = PDFViewer.FileSourceType.PersistentData;
                                else if (useResources)
                                    pdfViewer.FileSource = PDFViewer.FileSourceType.Resources;
                                else if (useFilePath)
                                    pdfViewer.FileSource = PDFViewer.FileSourceType.FilePath;

                                if (pdfViewer.FileSource != PDFViewer.FileSourceType.FilePath)
                                {
                                    pdfViewer.Folder = folder;
                                    pdfViewer.FileName = fileName;
                                }
                                else
                                {
                                    pdfViewer.FilePath = filePath;
                                }
                            }
                        }
                        GUI.color = Color.white;
                        EditorGUILayout.EndHorizontal();
                    }

#if UNITY_EDITOR_WIN
                    if (pdfViewer.FileSource != PDFViewer.FileSourceType.Asset && pdfViewer.FileSource != PDFViewer.FileSourceType.DocumentObject)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(" ");

                        GUI.color = Color.cyan;

                        using (new EditorGUI.DisabledGroupScope(string.IsNullOrEmpty(pdfViewer.FileName.Trim())))
                        {
                            if (GUILayout.Button("Reveal File"))
                            {
                                string filePath = "";

                                if (pdfViewer.FileSource == PDFViewer.FileSourceType.Resources)
                                {
                                    filePath = Application.dataPath + "/Resources/" + pdfViewer.GetFileLocation();
                                }
                                else
                                {
                                    filePath = pdfViewer.GetFileLocation();
                                }

                                if (pdfViewer.FileSource != PDFViewer.FileSourceType.Web)
                                {
                                    if (!File.Exists(filePath))
                                    {
                                        if (pdfViewer.FileSource == PDFViewer.FileSourceType.Resources &&
                                            File.Exists(filePath + ".bytes"))
                                        {
                                            ShowInExplorer(filePath + ".bytes");
                                        }
                                        else
                                        {
                                            EditorUtility.DisplayDialog("Error",
                                                "The file path is badly formed, contains invalid characters or doesn't exists:\r\n\r\n" +
                                                filePath, "Ok");
                                        }
                                    }
                                    else
                                    {
                                        ShowInExplorer(filePath);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        System.Diagnostics.Process.Start(filePath);
                                    }
                                    catch
                                    {
                                        EditorUtility.DisplayDialog("Error",
                                            "The URL is badly formed or contains invalid characters:\r\n\r\n" + filePath, "Ok");
                                    }
                                }
                            }
                        }

                        GUI.color = Color.white;
                        EditorGUILayout.EndHorizontal();
                    }
#endif
                }

                if (pdfViewer.FileSource == PDFViewer.FileSourceType.Bytes)
                {
                    EditorGUI.indentLevel++;
                    serializedObject.Update();

                    GameObject oldObject = pdfViewer.BytesSupplierObject;

                    var bytesSupplierObject = serializedObject.FindProperty("m_BytesSupplierObject");
                    EditorGUILayout.PropertyField(bytesSupplierObject, new GUIContent("Supplier Object"));
                    serializedObject.ApplyModifiedProperties();

                    int selectedIndex = 0;

                    if (pdfViewer.BytesSupplierObject != null)
                    {
                        try
                        {
                            List<BytesSupplierInfo> possibleSuppliers = new List<BytesSupplierInfo>();
                            possibleSuppliers.Add(new BytesSupplierInfo(null, null, ""));

                            Component[] components = pdfViewer.BytesSupplierObject.GetComponents(typeof(Component));

                            foreach (Component component in components)
                            {
                                Type type = component.GetType();
                                MethodInfo[] methods = type.GetMethods();

                                if (methods.Length == 0)
                                    continue;

                                foreach (MethodInfo method in methods)
                                {
                                    if ((method.GetParameters() == null || method.GetParameters().Length == 0)
                                        && method.ReturnType == typeof(byte[]))
                                    {
                                        possibleSuppliers.Add(new BytesSupplierInfo(pdfViewer.BytesSupplierObject, component,
                                            method.Name));

                                        if (oldObject == pdfViewer.BytesSupplierObject
                                            && method.Name == pdfViewer.BytesSupplierFunctionName
                                            && component == pdfViewer.BytesSupplierComponent)
                                        {
                                            selectedIndex = possibleSuppliers.Count - 1;
                                        }
                                    }
                                }
                            }

                            string[] supplierTitles = new string[possibleSuppliers.Count];

                            for (int i = 0; i < supplierTitles.Length; ++i)
                            {
                                if (i == 0)
                                {
                                    supplierTitles[0] = "No Function";
                                    continue;
                                }
                                supplierTitles[i] = possibleSuppliers[i].m_Behaviour.name + "." +
                                                    possibleSuppliers[i].m_MethodName;
                            }

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PrefixLabel("Supplier Function");

                            var choiceIndex = EditorGUILayout.Popup(selectedIndex, supplierTitles);

                            if (choiceIndex == 0)
                            {
                                pdfViewer.BytesSupplierComponent = null;
                                pdfViewer.BytesSupplierFunctionName = "";
                            }
                            else
                            {
                                pdfViewer.BytesSupplierComponent = possibleSuppliers[choiceIndex].m_Behaviour;
                                pdfViewer.BytesSupplierFunctionName = possibleSuppliers[choiceIndex].m_MethodName;
                            }

                            EditorGUILayout.EndHorizontal();
                        }
                        catch (Exception)
                        {
                            pdfViewer.BytesSupplierComponent = null;
                            pdfViewer.BytesSupplierFunctionName = "";
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                if (pdfViewer.FileSource == PDFViewer.FileSourceType.Asset)
                {
                    pdfViewer.PDFAsset =
                        (PDFAsset)EditorGUILayout.ObjectField("PDF Asset", pdfViewer.PDFAsset, typeof(PDFAsset), true);
                }

                pdfViewer.LoadOnEnable = EditorGUILayout.Toggle("Load On Enable", pdfViewer.LoadOnEnable);
                pdfViewer.UnloadOnDisable = EditorGUILayout.Toggle("Unload On Disable", pdfViewer.UnloadOnDisable);

                if (pdfViewer.FileSource == PDFViewer.FileSourceType.Asset)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("");
                    EditorGUILayout.HelpBox(
                        "To convert pdf file to .asset right click on pdf and select \"PDF Renderer\\Convert to .asset\"",
                        MessageType.Info);
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(10.0f);
                }

                GUILayout.Space(10.0f);
            }
            GUILayout.EndVertical();


            GUILayout.BeginVertical("Box");
            pdfViewer.m_Internal.m_UiShowPasswordOptions = PRHelper.GroupHeader("Password Options", pdfViewer.m_Internal.m_UiShowPasswordOptions);
            if (pdfViewer.m_Internal.m_UiShowPasswordOptions)
            {
                EditorGUI.indentLevel++;
                pdfViewer.Password = EditorGUILayout.PasswordField("Password", pdfViewer.Password);
                EditorGUI.indentLevel--;

                GUILayout.Space(10.0f);
            }
            GUILayout.EndVertical();


            GUILayout.BeginVertical("Box");
            pdfViewer.m_Internal.m_UiShowViewerSettings = PRHelper.GroupHeader("Viewer Settings", pdfViewer.m_Internal.m_UiShowViewerSettings);
            if (pdfViewer.m_Internal.m_UiShowViewerSettings)
            {
                pdfViewer.PageFitting = (PDFViewer.PageFittingType)EditorGUILayout.EnumPopup("Page Fitting", pdfViewer.PageFitting);
                if (pdfViewer.PageFitting == PDFViewer.PageFittingType.Zoom || Application.isPlaying)
                {
                    EditorGUI.indentLevel++;
                    pdfViewer.ZoomFactor = EditorGUILayout.FloatField("Zoom", pdfViewer.ZoomFactor,
                        GUILayout.ExpandWidth(false));
                    EditorGUI.indentLevel--;
                }

                pdfViewer.VerticalMarginBetweenPages = EditorGUILayout.FloatField("Page Margins (px)",
                    pdfViewer.VerticalMarginBetweenPages, GUILayout.ExpandWidth(false));
                pdfViewer.MinZoomFactor = EditorGUILayout.FloatField("Minimum Zoom", pdfViewer.MinZoomFactor,
                    GUILayout.ExpandWidth(false));
                pdfViewer.MaxZoomFactor = EditorGUILayout.FloatField("Maximum Zoom", pdfViewer.MaxZoomFactor,
                    GUILayout.ExpandWidth(false));
                pdfViewer.ZoomStep = EditorGUILayout.FloatField("Zoom Step", pdfViewer.ZoomStep,
                    GUILayout.ExpandWidth(false));
                pdfViewer.ScrollSensitivity = EditorGUILayout.FloatField("Scroll Sensitivity (px)",
                    pdfViewer.ScrollSensitivity, GUILayout.ExpandWidth(false));
                pdfViewer.ShowTopBar = EditorGUILayout.Toggle("Show Top Bar", pdfViewer.ShowTopBar);
                pdfViewer.ShowVerticalScrollBar = EditorGUILayout.Toggle("Show VScrollBar", pdfViewer.ShowVerticalScrollBar);
                pdfViewer.ShowHorizontalScrollBar = EditorGUILayout.Toggle("Show HScrollBar",
                    pdfViewer.ShowHorizontalScrollBar);

                if (pdfViewer.m_Internal.m_LeftPanel != null)
                {
                    pdfViewer.ShowBookmarksViewer = EditorGUILayout.Toggle("Show Bookmarks Viewer", pdfViewer.ShowBookmarksViewer);
                    pdfViewer.ShowThumbnailsViewer = EditorGUILayout.Toggle("Show Thumbnails Viewer", pdfViewer.ShowThumbnailsViewer);
                }

                pdfViewer.AllowOpenURL = EditorGUILayout.Toggle("Allow Open URL", pdfViewer.AllowOpenURL);

                Color oldColor = pdfViewer.BackgroundColor;
                pdfViewer.BackgroundColor = EditorGUILayout.ColorField("Viewer BG Color", pdfViewer.BackgroundColor);

                if (oldColor != pdfViewer.BackgroundColor)
                {
                    EditorUtility.SetDirty(pdfViewer.m_Internal.m_Viewport.GetComponent<Image>());
                }

                GUILayout.Space(10.0f);
            }
            GUILayout.EndVertical();


            if (pdfViewer.m_Internal.m_SearchPanel != null)
            {
                GUILayout.BeginVertical("Box");
                pdfViewer.m_Internal.m_UiShowSearchSettings = PRHelper.GroupHeader("Search Settings", pdfViewer.m_Internal.m_UiShowSearchSettings);
                if (pdfViewer.m_Internal.m_UiShowSearchSettings)
                {
                    pdfViewer.SearchResultColor = EditorGUILayout.ColorField("Result Color", pdfViewer.SearchResultColor);
                    pdfViewer.SearchResultPadding = EditorGUILayout.Vector2Field("Result Padding",
                        pdfViewer.SearchResultPadding);
                    pdfViewer.SearchTimeBudgetPerFrame = EditorGUILayout.Slider("Time (% per frame)",
                        pdfViewer.SearchTimeBudgetPerFrame, 0.0f, 1.0f);

                    GUILayout.Space(10.0f);
                }
                GUILayout.EndVertical();
            }


            GUILayout.BeginVertical("Box");
            pdfViewer.m_Internal.m_UiShowOtherSettings = PRHelper.GroupHeader("Other Settings", pdfViewer.m_Internal.m_UiShowOtherSettings);
            if (pdfViewer.m_Internal.m_UiShowOtherSettings)
            {
                pdfViewer.ParagraphZoomingEnable = EditorGUILayout.Toggle("Paragraph Zooming", pdfViewer.ParagraphZoomingEnable, GUILayout.ExpandWidth(false));
                if (pdfViewer.ParagraphZoomingEnable)
                {
                    pdfViewer.ParagraphZoomFactor = EditorGUILayout.FloatField("    Zoom Factor", pdfViewer.ParagraphZoomFactor, GUILayout.ExpandWidth(false));
                    pdfViewer.ParagraphDetectionThreshold = EditorGUILayout.FloatField("    Detection Threshold (px)", pdfViewer.ParagraphDetectionThreshold, GUILayout.ExpandWidth(false));
                }
                pdfViewer.PageTileTexture = (Texture2D)EditorGUILayout.ObjectField("Page Tile Texture", pdfViewer.PageTileTexture, typeof(Texture2D), true);
                pdfViewer.PageColor = EditorGUILayout.ColorField("Page Color", pdfViewer.PageColor);


                GUILayout.Space(10.0f);
            }
            GUILayout.EndVertical();


            GUILayout.BeginVertical("Box");
            pdfViewer.m_Internal.m_UiShowRenderSettings = PRHelper.GroupHeader("Render Settings", pdfViewer.m_Internal.m_UiShowRenderSettings);
            if (pdfViewer.m_Internal.m_UiShowRenderSettings)
            {
                pdfViewer.MaxZoomFactorTextureQuality = EditorGUILayout.FloatField("Maximum Quality",
                    pdfViewer.MaxZoomFactorTextureQuality, GUILayout.ExpandWidth(false));
                pdfViewer.RenderAnnotations = EditorGUILayout.Toggle("Render Annotations", pdfViewer.RenderAnnotations,
                    GUILayout.ExpandWidth(false));
                pdfViewer.RenderGrayscale = EditorGUILayout.Toggle("Render Grayscale", pdfViewer.RenderGrayscale,
                    GUILayout.ExpandWidth(false));

                GUILayout.Space(10.0f);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Box");
            pdfViewer.m_Internal.m_UiShowDebugSettings = PRHelper.GroupHeader("Debug Settings", pdfViewer.m_Internal.m_UiShowDebugSettings);
            if (pdfViewer.m_Internal.m_UiShowDebugSettings)
            {
                pdfViewer.m_Internal.m_DrawDefaultInspector = EditorGUILayout.Toggle("Default Inspector", pdfViewer.m_Internal.m_DrawDefaultInspector);

                if (pdfViewer.m_Internal.m_DrawDefaultInspector)
                {
                    GUILayout.Space(10.0f);
                    DrawDefaultInspector();
                }

                GUILayout.Space(10.0f);
            }

            GUILayout.EndVertical();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(pdfViewer);
                EditorUtility.SetDirty(pdfViewer.m_Internal);
            }
        }

        protected virtual void OnDisable()
        {
            DestroyImmediate(m_Background1.normal.background);
            DestroyImmediate(m_Background2.normal.background);
            DestroyImmediate(m_Background3.normal.background);
        }

        protected virtual void OnEnable()
        {
            pdfViewer = (PDFViewer)target;

            m_Background1 = new GUIStyle();
            m_Background1.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.1f));
            m_Background2 = new GUIStyle();
            m_Background2.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.0f));
            m_Background3 = new GUIStyle();
            m_Background3.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.05f));

            MonoScript script = MonoScript.FromScriptableObject(this);
            string scriptPath = AssetDatabase.GetAssetPath(script);
            m_Logo =
                (Texture2D)
                    AssetDatabase.LoadAssetAtPath(Path.GetDirectoryName(scriptPath) + "/logo_pv.png", typeof(Texture2D));

            if (m_ParoxeBanner == null)
                m_ParoxeBanner = new PRParoxeBanner(Path.GetDirectoryName(scriptPath));
        }

        private static bool IsPrefabGhost(Transform This)
        {
            var TempObject = new GameObject();
            try
            {
                TempObject.transform.parent = This.parent;

                var OriginalIndex = This.GetSiblingIndex();

                This.SetSiblingIndex(int.MaxValue);
                if (This.GetSiblingIndex() == 0)
                {
                    return true;
                }

                This.SetSiblingIndex(OriginalIndex);
                return false;
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(TempObject);
            }
        }

        private static void ShowInExplorer(string filePath)
        {
            filePath = Path.GetFullPath(filePath.Replace(@"/", @"\"));
            if (File.Exists(filePath))
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select," + filePath);
            }
        }

        private static bool Browse(string startPath, ref string filename, ref string folder, ref string filePath, ref bool isStreamingAsset, ref bool isPersistentData, ref bool isResourcesAsset, ref bool isFilePath)
        {
            bool result = false;
            string path = UnityEditor.EditorUtility.OpenFilePanel("Browse video file", startPath, "*");

            if (!string.IsNullOrEmpty(path) && !path.EndsWith(".meta"))
            {
                string projectRoot = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, ".."));
                projectRoot = projectRoot.Replace('\\', '/');

                if (path.StartsWith(projectRoot))
                {
                    if (path.StartsWith(Application.streamingAssetsPath))
                    {
                        path = path.Remove(0, Application.streamingAssetsPath.Length);
                        filename = System.IO.Path.GetFileName(path);
                        path = System.IO.Path.GetDirectoryName(path);
                        if (path.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString()) || path.StartsWith(System.IO.Path.AltDirectorySeparatorChar.ToString()))
                        {
                            path = path.Remove(0, 1);
                        }
                        folder = path;

                        isPersistentData = false;
                        isStreamingAsset = true;
                        isResourcesAsset = false;
                        isFilePath = false;
                    }
                    else if (path.StartsWith(Application.persistentDataPath))
                    {
                        path = path.Remove(0, Application.persistentDataPath.Length);
                        filename = System.IO.Path.GetFileName(path);
                        path = System.IO.Path.GetDirectoryName(path);
                        if (path.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString()) || path.StartsWith(System.IO.Path.AltDirectorySeparatorChar.ToString()))
                        {
                            path = path.Remove(0, 1);
                        }
                        folder = path;

                        isPersistentData = true;
                        isStreamingAsset = false;
                        isResourcesAsset = false;
                        isFilePath = false;
                    }
                    else if (path.StartsWith(Application.dataPath + "/Resources"))
                    {
                        path = path.Remove(0, (Application.dataPath + "/Resources").Length);
                        filename = System.IO.Path.GetFileName(path);
                        path = System.IO.Path.GetDirectoryName(path);
                        if (path.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString()) || path.StartsWith(System.IO.Path.AltDirectorySeparatorChar.ToString()))
                        {
                            path = path.Remove(0, 1);
                        }
                        folder = path;

                        isPersistentData = false;
                        isStreamingAsset = false;
                        isResourcesAsset = true;
                        isFilePath = false;
                    }
                    else
                    {
                        path = path.Remove(0, projectRoot.Length + 1);
                        filePath = path;

                        isPersistentData = false;
                        isStreamingAsset = false;
                        isResourcesAsset = false;
                        isFilePath = true;
                    }
                }
                else
                {
                    filePath = path;

                    isPersistentData = false;
                    isStreamingAsset = false;
                    isResourcesAsset = false;
                    isFilePath = true;
                }

                result = true;
            }
            return result;
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.hideFlags = HideFlags.HideAndDontSave;
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        private class BytesSupplierInfo
        {
            public Component m_Behaviour;
            public GameObject m_GameObject;
            public string m_MethodName;

            public BytesSupplierInfo(GameObject gameObject, Component component, string methodName)
            {
                m_GameObject = gameObject;
                m_Behaviour = component;
                m_MethodName = methodName;
            }
        }
    }
}