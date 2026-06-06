using System;
using System.Net;
using System.Text;

namespace QWeb
{

    public class QUploadString : QAbstractUpload
    {
        protected new string result;
        protected new Action<string> onCompleted;
        private Encoding encoding;
        private string data;

        public QUploadString OnCompleted(Action<string> callback) { onCompleted = callback; return this; }
        public QUploadString SetData(string value) { data = value; return this; }
        public QUploadString(string url) : base(url){}
        public QUploadString(string url,QStringCode code) : base(url) { SetCode(code); }

        public QUploadString SetCode(QStringCode code)
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

        protected override void OnStart()
        {
            web.Encoding = encoding;
            web.UploadStringCompleted += UploadStringCompleted;
            web.UploadStringAsync(new Uri(url), data);
        }

        private void UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            web.UploadStringCompleted -= UploadStringCompleted;
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