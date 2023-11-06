using System;
using System.Net;
namespace QWeb
{
    public abstract class QAbstractDownload : QAbstractWeb
    {

        protected abstract void OnStart();
        public QAbstractDownload(string url) : base(url) { }
        public void SetUrl(string theUrl)
        {
            base.SetUrl(theUrl);
        }
        public override void Start()
        {
            IsOk = false;
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) =>
            {
                return true;
            };

            using (web = new WebClient())
            {
                try
                {
                    QDownloadState.Add(this);
                    web.DownloadProgressChanged += OnDownloadProgressChanged;
                    mStopwatch.Start();
                    OnStart();
                }
                catch (Exception e)
                {
                    if (onErrorMsg != null) onErrorMsg(e.Message);
                    Cancel();
                }
            }
        }

        protected void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (onProgress != null) onProgress.Invoke(e.ProgressPercentage);
            if (onReceivedAmount != null) onReceivedAmount.Invoke(e.BytesReceived, e.TotalBytesToReceive);
            if (onSecondSize != null) onSecondSize.Invoke(string.Format("{0} kb/s", (e.BytesReceived/1024d/ mStopwatch.Elapsed.TotalSeconds).ToString("0.00")));
        }
    }
}