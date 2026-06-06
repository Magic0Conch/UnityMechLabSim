using System.IO;
using System.Collections;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using System.Threading;
using Aliyun.OSS.Util;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Text;
public class ZipUtility : MonoBehaviour
{
    public Text text;
    public Slider slider;
    int totalLength;
    UrlDataSource[] datas;
    public void GetFilesByNames(string[] names, string anspath)
    {
        totalLength = names.Length;
        MemoryStream ms = new MemoryStream();
        byte[] buffer = null;
        datas = new UrlDataSource[totalLength];
        using (ZipFile file = ZipFile.Create(anspath))
        {
            file.BeginUpdate();
            for(int i = 0; i < totalLength; i++)
            {
                if (names[i] == null || names[i] == "")
                {
                    continue;
                }
                datas[i] = new UrlDataSource(names[i].Trim());
                file.Add(datas[i], Path.GetFileName(names[i].Trim()));
            }        
            file.CommitUpdate();
            buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
        }
    }

    private Action<float> GetProcessCallback;

    private void Update()
    {
        if (datas == null)
            return;
        long nowBytes = 0;
        long totalBytes = 0;
        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i] == null)
                continue;
            nowBytes += datas[i].downBytes;
            totalBytes += datas[i].totalBytes;
        }
        if(slider.value==0&&datas!=null)
            text.text = "压缩包传输完成：传输总字节量" + nowBytes;
        else
            text.text = "";

    }


    public class Config
    {
        public const string AccessKeyId = "LTAI4G9Nr7nHNQCjiDLwMabj";
        public const string AccessKeySecret = "qpFeC8MEE1MduUB4bt302Pkj4cWWzu";
        public const string EndPoint = "oss-cn-qingdao.aliyuncs.com";
        public const string Bucket = "cemm20201009";
        public static double compete = 0;
    }

    public class UrlDataSource : IStaticDataSource
    {
        public string Url { get; set; }
        public long downBytes { get; set; }
        public long totalBytes { get; set; }
        OssClient ossClient;
        public UrlDataSource(string url)
        {
            ossClient = new OssClient(Config.EndPoint, Config.AccessKeyId, Config.AccessKeySecret);
            this.Url = url;
        }
        Stream IStaticDataSource.GetSource()
        {
            return GetStreamUrl();
        }

        public Stream GetStreamUrl()
        {
            GetObjectRequest getObjectRequest = new GetObjectRequest(Config.Bucket,Url);
            getObjectRequest.StreamTransferProgress += GetStreamProcess;
            var obj = ossClient.GetObject(getObjectRequest);
            return obj.Content;
        }
        public void GetStreamProcess(object sender, StreamTransferProgressArgs args)
        {
            Config.compete += ((args.TransferredBytes - downBytes) * 100 / args.TotalBytes) / 100.0f;
            downBytes = args.TransferredBytes;
            totalBytes = args.TotalBytes;
            //print("now:" + (args.TransferredBytes * 100 / args.TotalBytes) / 100.0f);
        }
    }
}

