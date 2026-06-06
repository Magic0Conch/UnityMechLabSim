using System;
using System.Net;
using System.Text;

namespace QWeb
{
    public class QDownloadString : QAbstractDownload
    {
        private Encoding encoding;
        private string result;
        private Action<string> onCompleted;

        public QDownloadString(string url) : base(url) { }
        public QDownloadString(string url, QStringCode code) : base(url){SetCode(code);}

        public QDownloadString SetCode(QStringCode code)
        {
            switch (code)
            {
                case QStringCode.Default: encoding = Encoding.Default; break;
                case QStringCode.UTF8: encoding = Encoding.UTF8; break;
                case QStringCode.ASCII: encoding = Encoding.ASCII; break;
                case QStringCode.UTF7: encoding = Encoding.UTF7; break;
                case QStringCode.UTF32: encoding = Encoding.UTF32; break;
                case QStringCode.Unicode: encoding = Encoding.Unicode; break;
                case QStringCode.BigEndianUnicode: encoding = Encoding.BigEndianUnicode; break;
            }
            return this;
        }

        public QDownloadString OnCompleted(Action<string> callback)
        {
            onCompleted = callback;
            return this;
        }


        protected override void OnStart()
        {
            web.Encoding = encoding;
            web.DownloadStringCompleted += OnDownloadStringCompleted;
            web.DownloadStringAsync(new Uri(url));
        }

        private void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            web.DownloadStringCompleted -= OnDownloadStringCompleted;
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
            onCompleted(result);
        }
    }
}