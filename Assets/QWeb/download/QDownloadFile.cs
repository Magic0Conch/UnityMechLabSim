using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
namespace QWeb
{
    public class QDownloadFile : QAbstractDownload
    {
        private const string tempSuffix = "temp";
        private int readWriteTimeOut = 4 * 1000;
        private int timeOutWait = 20 * 1000;
        private int oneReadLen = 16384;

        private NameValueCollection postData = new NameValueCollection();
        private bool isAlreadyDownloaded = false;
        private string savePath;
        private string tempPath;
        private bool isRedownload = false;
        private long bytesReceived;
        private long totalBytesReceived;
        private bool isStart;

        public bool isStop { get; private set; }
        public bool update { get; private set; }
        public string path { get; private set; }
        public string fileName { get; private set; }
        public string version { get; private set; }
        public string suffix { get; private set; }

        private Action<long, long> onStart;
        private Action onCompleted;
        private new Action<float> onProgress;

        public QDownloadFile(string url) : base(url) { }
        public void SetUrl(string url,NameValueCollection nv)
        {
            url += "?";
            foreach(string key in nv)
            {
                url += key + "=" + nv.Get(key);
            }
            base.SetUrl(url);
        }
        public QDownloadFile SetReadWriteTimeOut(int value) { readWriteTimeOut = value; return this; }
        public QDownloadFile SetTimeOutWait(int value) { timeOutWait = value; return this; }
        public QDownloadFile OneReadLen(int value) { oneReadLen = value; return this; }
        public QDownloadFile SetPath(string value) { path = value; return this; }
        public QDownloadFile SetFileName(string value) { fileName = value; return this; }
        public QDownloadFile SetVersion(int value) { version = value.ToString(); return this; }
        public QDownloadFile SetVersion(string value) { version = value; return this; }
        public QDownloadFile SetSuffix(string value) { suffix = value; return this; }
        public QDownloadFile Redownload(bool value) { isRedownload = value; return this; }
        public QDownloadFile OnCompleted(Action callback) { onCompleted = callback; return this; }
        public QDownloadFile OnStart(Action<long, long> callback) { onStart = callback; return this; }
        public QDownloadFile OnProgress(Action<float> callback) { onProgress = callback; return this; }
        public QDownloadFile SetPostData(NameValueCollection formData){postData = formData; return this;}
        public override void Cancel(){isStop = true;}

        public static void SetMin(int workerThreads, int completionPortThreads)
        {
            ThreadPool.SetMinThreads(workerThreads,completionPortThreads);
        }

        public static void SetMax(int workerThreads, int completionPortThreads)
        {
            ThreadPool.SetMaxThreads(workerThreads, completionPortThreads);
        }
        public void SetUpdate()
        {
            if (!isStart)
            {
                if (onProgress != null) onProgress((float)bytesReceived / totalBytesReceived);
                if (onReceivedAmount != null) onReceivedAmount(bytesReceived, totalBytesReceived);
                if (onSecondSize != null) onSecondSize.Invoke(string.Format("{0} kb/s", (bytesReceived / 1024d / mStopwatch.Elapsed.TotalSeconds).ToString("0.00")));
            }
            else
            {
                if (onStart != null) onStart(bytesReceived, totalBytesReceived);
                isStart = false;
            }
            update = false;
            if (bytesReceived >= totalBytesReceived) IsOk = true;
        }

        public override void Start()
        {
            
            isAlreadyDownloaded = false;
            IsOk = false;
            savePath = LinkPath(path, fileName, version, suffix);
            if (CheckFileExists(path)) return;
            tempPath = LinkPath(path, fileName, version, tempSuffix);
            CheckOldVersion(path,fileName,version,tempSuffix);
            QDownloadState.Add(this);
            mStopwatch.Start();
            ThreadPool.QueueUserWorkItem(v => { OnStart(); });
        }

        /// <summary>
        /// 获取计算网络文件的大小
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static long GetWebFileSize(string url, int timeOutWait = 2 * 1000, int readWriteTimeOut = 2 * 1000)
        {
            HttpWebRequest request = null;
            WebResponse respone = null;
            long length = 0;
            try
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                request.Timeout = timeOutWait;
                request.ReadWriteTimeout = readWriteTimeOut;
                respone = request.GetResponse();
                length = respone.ContentLength;
            }
            catch (WebException e)
            {
                throw e;
            }
            finally
            {
                if (respone != null) respone.Close();
                if (request != null) request.Abort();
            }
            return length;
        }

        /// <summary>
        /// 获取当前文件大小
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static long GetFileSize(string path)
        {
            return new FileInfo(path).Length;
        }

        public static float GetDownloadedProgress(string url, string path, string fileName, string version, string suffix)
        {
            if (File.Exists(LinkPath(path, fileName, version, suffix))) return 1;
            var tmp = LinkPath(path, fileName, version, tempSuffix);
            if (!File.Exists(tmp)) return 0;
            return (float)GetFileSize(tmp) / GetWebFileSize(url);
        }

        /// <summary>
        /// 检查路径和文件名
        /// </summary>
        private static void CheckPath(string path, string fileName)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("The path cannot be empty！");
            else if (string.IsNullOrEmpty(fileName))
                throw new Exception("File name must not be empty！");
        }

        /// <summary>
        /// 链接文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="version"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        private static string LinkPath(string path, string fileName, string version, string suffix)
        {
            CheckPath(path, fileName);
            if (string.IsNullOrEmpty(version))
            {
                if (string.IsNullOrEmpty(suffix)) return string.Format("{0}/{1}", path, fileName);
                else return string.Format("{0}/{1}.{2}", path, fileName, suffix);
            }
            else
            {
                if (string.IsNullOrEmpty(suffix)) return string.Format("{0}/{1}_{2}", path, fileName, version);
                else return string.Format("{0}/{1}_{2}.{3}", path, fileName, version, suffix);
            }
        }
        
        /// <summary>
        /// 移除旧版本文件
        /// </summary>
        /// <param name="temp"></param>
        private void CheckOldVersion(string path, string fileName, string version, string suffix)
        {
            if (!Directory.Exists(path)) return;
            var dir = new DirectoryInfo(path);
            var files = dir.GetFiles(string.Format("{0}*", fileName));
            var savefile = string.Format("{0}_{1}",fileName,version);

            foreach (var file in files)
            {
                if (savefile != Path.GetFileNameWithoutExtension(file.FullName))
                {
                    try
                    {
                        File.Delete(file.FullName);
                    }
                    catch (Exception e)
                    {
                        if (onErrorMsg != null) onErrorMsg(e.Message);
                        mStopwatch.Reset();
                    }
                }
            }
        }

        /// <summary>
        /// 检查文件是否已下载
        /// </summary>
        /// <returns></returns>
        private bool CheckFileExists(string savePath)
        {
            if (File.Exists(savePath))
            {
                if (isRedownload)
                {
                    File.Delete(savePath);
                    return false;
                }
                isAlreadyDownloaded = true;
                IsOk = true;
                _OnCompleted();
                return true;
            }
            return false;
        }

        protected override void OnStart()
        {
            FileStream fs = null;
            long startPos = 0;
            if (File.Exists(tempPath))
            {
                fs = File.OpenWrite(tempPath);
                startPos = fs.Length;
                fs.Seek(startPos, SeekOrigin.Current); //移动文件流中的当前指针
            }
            else
            {
                string direName = Path.GetDirectoryName(tempPath);
                if (!Directory.Exists(direName)) Directory.CreateDirectory(direName);
                fs = new FileStream(tempPath, FileMode.Create);
            }

            HttpWebRequest request = null;
            WebResponse respone = null;
            Stream ns = null;
            try
            {
                //WebRequest request0 = WebRequest.Create();
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(url + GetTimeStamp());
                foreach (string key in postData.Keys)
                {
                    stringBuilder.AppendFormat("&{0}={1}", key, postData.Get(key));
                }
                request = WebRequest.Create(stringBuilder.ToString()) as HttpWebRequest;
                request.ReadWriteTimeout = readWriteTimeOut;
                request.Timeout = timeOutWait;
                if (startPos > 0) request.AddRange((int)startPos);
                respone = request.GetResponse();
                ns = respone.GetResponseStream();
                totalBytesReceived = respone.ContentLength + startPos;
                bytesReceived = startPos;

                if (totalBytesReceived != 0 && bytesReceived >= totalBytesReceived)
                {
                    fs.Flush();
                    fs.Close();
                    fs = null;
                    IsOk = true;
                }
                else
                {
                    byte[] bytes = new byte[oneReadLen];
                    int readSize = ns.Read(bytes, 0, oneReadLen); // 读取第一份数据
                    isStart = true;
                    update = true;
                    while (readSize > 0 && !isStop)
                    {
                        fs.Write(bytes, 0, readSize);       // 将下载到的数据写入临时文件
                        bytesReceived += readSize;
                        update = true;
                        // 判断是否下载完成
                        // 下载完成将temp文件，改成正式文件
                        if (totalBytesReceived != 0 && bytesReceived >= totalBytesReceived)
                        {
                            fs.Flush();
                            fs.Close();
                            fs = null;
                            update = true;
                            break;
                        }

                        readSize = ns.Read(bytes, 0, oneReadLen);
                    }
                }
            }
            catch (Exception e)
            {
                if (onErrorMsg != null) onErrorMsg(e.Message);
                mStopwatch.Reset();
            }
            finally
            {
                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                    fs = null;
                }
                if (ns != null) ns.Close();
                if (respone != null) respone.Close();
                if (request != null) request.Abort();
            }
        }

        private string GetTimeStamp()
        {
            return "?" + (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).ToString();
        }

        protected override void _OnCompleted()
        {
            if (!isAlreadyDownloaded)
            {
                if (File.Exists(savePath)) File.Delete(savePath);
                File.Move(tempPath, savePath);
            }
            onCompleted();
        }
    }

}