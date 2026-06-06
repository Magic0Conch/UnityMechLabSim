using UnityEngine;

namespace Paroxe.PdfRenderer
{
    /// <summary>
    /// Represent a colored rect within a page. This class is used mainly
    /// for search results highlightment. Will maybe used for text selection in
    /// the future.
    /// </summary>
    public struct PDFColoredRect
    {
        public Rect pageRect;
        public Color color;

        public PDFColoredRect(Rect pageRect, Color color)
        {
            this.pageRect = pageRect;
            this.color = color;
        }
    }
}