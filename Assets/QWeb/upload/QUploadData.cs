using System;
using System.Net;
namespace QWeb
{
    public class QUploadData : QAbstractUpload
    {
        private byte[] data;

        public QUploadData OnCompleted(Action<byte[]> callback) { onCompleted = callback; return this; }
        public QUploadData SetData(byte[] value) { data = value; return this; }
        public QUploadData(string url) : base(url){}
        
        protected override void OnStart()
        {
            web.UploadDataCompleted += OnUploadDataCompleted;
            web.UploadDataAsync(new Uri(url), data);
        }

        private void OnUploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
        {
            web.UploadDataCompleted -= OnUploadDataCompleted;
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