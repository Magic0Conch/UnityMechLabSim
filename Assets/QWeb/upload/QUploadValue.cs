using System;
using System.Collections.Specialized;
using System.Net;
namespace QWeb
{

    public class QUploadValue : QAbstractUpload
    {
        private NameValueCollection data = new NameValueCollection();

        public QUploadValue OnCompleted(Action<byte[]> callback) { onCompleted = callback; return this; }
        public QUploadValue AddKey(string name, string value) { data.Add(name, value); return this; }
        
        public QUploadValue Remove(string name) { data.Remove(name); return this; }
        public QUploadValue(string url) : base(url) { }

        protected override void OnStart()
        {
            web.UploadValuesCompleted += UploadValuesCompleted;
            web.UploadValuesAsync(new Uri(url), data);
        }

        private void UploadValuesCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
            web.UploadValuesCompleted -= UploadValuesCompleted;
            web.UploadProgressChanged -= OnUploadProgressChanged;

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
            if (onCompleted != null) onCompleted(result);
        }
    }
}