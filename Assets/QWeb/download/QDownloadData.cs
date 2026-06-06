

using System;
using System.Net;
using UnityEngine;

namespace QWeb
{
    public class QDownloadData : QAbstractDownload
    {
        private byte[] result;
        private Action<byte[]> onCompleted;
        private Action<Texture2D> onTexture;

        public QDownloadData(string url) : base(url) { }
        public QDownloadData OnCompleted(Action<byte[]> callback) { onCompleted = callback; return this; }
        public QDownloadData OnCompletedToTexture2D(Action<Texture2D> callback) { onTexture = callback; return this; }

        protected override void OnStart()
        {
            web.DownloadDataAsync(new Uri(url));
            web.DownloadDataCompleted += OnDownloadDataCompleted;
        }

        private void OnDownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            web.DownloadDataCompleted -= OnDownloadDataCompleted;
            web.DownloadProgressChanged -= OnDownloadProgressChanged;

            if (e.Cancelled) return;
            if (e.Error == null)
            {
                result = e.Result;
                IsOk = true;
            }
            else
            {
                if (onErrorMsg != null) onErrorMsg(e.Error.Message);
            }
        }

        protected override void _OnCompleted()
        {
            if (onCompleted != null)
            {
                onCompleted(result);
            }
            else if (onTexture != null)
            {
                Texture2D t = new Texture2D(0, 0);
                t.LoadImage(result);
                onTexture(t);
            }
        }
    }
}
