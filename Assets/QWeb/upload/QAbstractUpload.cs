using System;
using System.Net;
namespace QWeb
{
    public abstract class QAbstractUpload : QAbstractWeb
    {

        protected byte[] result;
        protected Action<byte[]> onCompleted;

        protected abstract void OnStart();
        public QAbstractUpload(string url) : base(url){}

        public override void Start()
        {
            IsOk = false;
            using (web = new WebClient())
            {
                try
                {
                    QDownloadState.Add(this);
                    web.UploadProgressChanged += OnUploadProgressChanged;
                    mStopwatch.Start();
                    OnStart();
                }
                catch (Exception e)
                {
                    if (onErrorMsg != null) onErrorMsg(e.Message);
                    mStopwatch.Reset();
                }
            }
        }

        protected void OnUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            if (onProgress != null) onProgress.Invoke(e.ProgressPercentage);
            if (onReceivedAmount != null) onReceivedAmount.Invoke(e.BytesReceived, e.TotalBytesToReceive);
        }
    }
}