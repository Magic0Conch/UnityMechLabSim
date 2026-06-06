using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paroxe.PdfRenderer.Internal.Viewer
{
    public class PDFBookmarksViewer : UIBehaviour
    {
        public RectTransform m_BooksmarksContainer;
        public PDFBookmarkListItem m_ItemPrefab;
        public Image m_LastHighlightedImage;
#if !UNITY_WEBGL
        private CanvasGroup m_CanvasGroup;
        private bool m_Initialized = false;
        private RectTransform m_LeftPanel;
        private bool m_Loaded = false;
        private PDFDocument m_Document;
        private PDFViewer m_PDFViewer;
        private RectTransform m_RectTransform;
        private List<RectTransform> m_TopLevelItems;
#endif

        public void DoUpdate()
        {
#if !UNITY_WEBGL
            if (m_Initialized)
            {
                float containerHeight = 0.0f;
                foreach (RectTransform child in m_TopLevelItems)
                {
                    containerHeight += child.sizeDelta.y;
                }
            }

            if (m_RectTransform != null && m_LeftPanel != null &&
                m_RectTransform.sizeDelta.x != m_LeftPanel.sizeDelta.x - 24.0f)
            {
                m_RectTransform.sizeDelta = new Vector2(m_LeftPanel.sizeDelta.x - 24.0f, m_RectTransform.sizeDelta.y);
            }
#endif
        }

        private void Cleanup()
        {
#if !UNITY_WEBGL
            if (m_Loaded)
            {
                m_Loaded = false;
                m_Initialized = false;
                m_TopLevelItems = null;
                m_Document = null;

                int childCount = m_BooksmarksContainer.transform.childCount;
                for (int i = 1; i < childCount; ++i)
                {
                    Destroy(m_BooksmarksContainer.transform.GetChild(i).gameObject);
                }

                m_ItemPrefab.gameObject.SetActive(false);
                m_CanvasGroup.alpha = 0.0f;
            }
#endif
        }

        public void OnDocumentLoaded(PDFDocument document)
        {
#if !UNITY_WEBGL
            if (!m_Loaded && gameObject.activeInHierarchy)
            {
                m_Loaded = true;
                m_Document = document;

                m_TopLevelItems = new List<RectTransform>();

                m_RectTransform = transform as RectTransform;
                m_LeftPanel = transform.parent as RectTransform;

                PDFViewer viewer = GetComponentInParent<PDFViewer>();
                PDFBookmark rootBookmark = m_Document.GetRootBookmark(viewer);

                if (rootBookmark != null)
                {
                    m_ItemPrefab.gameObject.SetActive(true);

                    foreach (PDFBookmark child in rootBookmark.EnumerateChildrenBookmarks())
                    {
                        PDFBookmarkListItem item = null;

                        item = Instantiate(m_ItemPrefab.gameObject).GetComponent<PDFBookmarkListItem>();
                        RectTransform itemTransform = item.transform as RectTransform;
                        itemTransform.SetParent(m_BooksmarksContainer, false);
                        itemTransform.localScale = Vector3.one;
                        itemTransform.anchorMin = new Vector2(0.0f, 1.0f);
                        itemTransform.anchorMax = new Vector2(0.0f, 1.0f);
                        itemTransform.offsetMin = Vector2.zero;
                        itemTransform.offsetMax = Vector2.zero;

                        m_TopLevelItems.Add(item.transform as RectTransform);

                        item.Initilize(child, 0, false);
                    }

                    m_ItemPrefab.gameObject.SetActive(false);

                    m_Initialized = true;

                    if (GetComponentInParent<PDFViewerLeftPanel>().m_Thumbnails.gameObject.GetComponent<CanvasGroup>().alpha == 0.0f)
                        StartCoroutine(DelayedShow());
                }
            }
#endif
        }

#if !UNITY_WEBGL
        IEnumerator DelayedShow()
        {
            yield return null;
            yield return null;
            yield return null;
            m_CanvasGroup.alpha = 1.0f;
        }
#endif

        public void OnDocumentUnloaded()
        {
#if !UNITY_WEBGL
            Cleanup();
#endif
        }

#if !UNITY_WEBGL
        protected override void OnDisable()
        {
            base.OnDisable();

            if (m_Loaded)
            {
                Cleanup();
            }
        }
#endif

#if !UNITY_WEBGL
        protected override void OnEnable()
        {
            base.OnEnable();

            DoOnEnable();
        }
#endif

        public void DoOnEnable()
        {
#if !UNITY_WEBGL
            if (m_PDFViewer == null)
                m_PDFViewer = GetComponentInParent<PDFViewer>();
            if (m_CanvasGroup == null)
                m_CanvasGroup = GetComponent<CanvasGroup>();
            if (m_RectTransform == null)
                m_RectTransform = transform as RectTransform;

            m_ItemPrefab.gameObject.SetActive(false);
            m_CanvasGroup.alpha = 0.0f;

            if (!m_Loaded && m_PDFViewer.Document != null && m_PDFViewer.Document.IsValid)
                OnDocumentLoaded(m_PDFViewer.Document);
#endif
        }
    }
}