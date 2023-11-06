using System;
using System.Diagnostics;
using System.Net;
namespace QWeb
{
    public abstract class QAbstractWeb
    {
        public string url;

        protected Stopwatch mStopwatch = new Stopwatch();
        protected WebClient web;
        protected Action<string> onErrorMsg;
        protected Action<int> onProgress;
        protected Action<long, long> onReceivedAmount;
        protected Action<string> onSecondSize;

        public bool IsOk { get; protected set; }

        protected abstract void _OnCompleted();
        public abstract void Start();

        public QAbstractWeb(string url) { this.url = url; }
        public QAbstractWeb SetUrl(string url) { this.url = url;return this; }
        /// <summary>
        /// 该函数是库调用的
        /// </summary>
        public void SetCompleted(){_OnCompleted();IsOk = false; mStopwatch.Reset(); }
        public virtual QAbstractWeb OnProgress(Action<int> callback) { onProgress = callback; return this; }
        public QAbstractWeb OnReceivedAmount(Action<long, long> callback) { onReceivedAmount = callback; return this; }
        public QAbstractWeb OnErrorMsg(Action<string> callback) { onErrorMsg = callback; return this; }
        public QAbstractWeb OnSecondSize(Action<string> callback) { onSecondSize = callback;return this; }

        public virtual void Cancel()
        {
            if (web != null && web.IsBusy)
            {
                web.CancelAsync();
            }
            mStopwatch.Reset();
        }
    }

}
