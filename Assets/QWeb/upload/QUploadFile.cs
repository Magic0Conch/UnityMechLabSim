using System;
using System.Net;
namespace QWeb
{
    public class QUploadFile : QAbstractUpload
    {
        private string data;

        public QUploadFile OnCompleted(Action<byte[]> callback) { onCompleted = callback; return this; }
        public QUploadFile SetData(string value) { data = value; return this; }

        public QUploadFile(string url) : base(url){}


        protected override void OnStart()
        {
            web.UploadFileCompleted += UploadFileCompleted;
            web.UploadFileAsync(new Uri(url), data);
        }

        private void UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            web.UploadFileCompleted -= UploadFileCompleted;
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