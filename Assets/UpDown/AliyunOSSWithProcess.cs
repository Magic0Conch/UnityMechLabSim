using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AliyunOSSWithProcess : MonoBehaviour
{
    public string callbackUrl;
    string foldername_;
    string callbackBody;
    public Web web;
    public string downpathInAli= "icudt44l.dat";
    public string downpath;
    //= "bucket=${bucket}&object=${object}&etag=${etag}&size=${size}&mimeType=${mimeType}";
                                        //"my_var1=${x:var1}&my_var2=${x:var2}";
     string callBody = "bucket=${bucket}&object=${object}&etag=${etag}&size=${size}&mimeType=${mimeType}";

    string getCallBody(Dictionary<string, string> InfoDic)
    {
       
        string callbackBody = "";
        foreach(string key in InfoDic.Keys)
        {
            callbackBody += key + "=" + InfoDic[key]+"&";
        }
        callbackBody.Remove(callbackBody.Length - 1);
        return callbackBody;
    }
    // UI 的相关组件变量
    //public InputField filePathInputField;
    //public Button putButton;
    //public Image processImage;
    public Slider slider;
    // Oss对象，文件路径，文件名变量
    private OssClient ossClient;
    string filePath;
    string fileName;
    UnityAction action;
    // 进度的回调函数，以及线程，进度变量
    Action<float> PutProcessCallback;
    Action<float> GetProcessCallback;
    Thread putLocalThread;
    Thread getLocalThread;
    float putProcess = 0;
    

    // Start is called before the first frame update

    public void AddListenter_(Button putButton,UnityAction act,string url, string foldername)
    {
        action = act;
        callbackUrl = url;
        // 绑定按钮事件
        putButton.onClick.AddListener(() => {
            // 多线程进度上传函数
            string path = WebData.OpenDialog();
            if (path == null)
            {
                Assets.Message.MessageBox("操作已取消");
                return;
            }
            if (WebData.username == "")
            {
                Assets.Message.MessageBox("请登录");
                return;
            }
            Dictionary<string, string> InfoDic = new Dictionary<string, string>();
            InfoDic[Constant.propname.requesttype] = Constant.WebCommand.UPLOAD.ToString();
            InfoDic[Constant.propname.folderName] = foldername;
            foldername_ = foldername;
            InfoDic[Constant.propname.filename] = WebData.username;
            InfoDic[Constant.propname.filetype] = Path.GetExtension(path);
            InfoDic[Constant.propname.username] = WebData.username;
            InfoDic[Constant.propname.fileInDb] = slider.name.ToString();
            InfoDic[Constant.propname.oriName] = Path.GetFileName(path);
            callbackBody = getCallBody(InfoDic);
            print(callbackBody);
            PutObjectWithProcessByThread((process) => {
                Debug.Log("上传进度为:" + process);
            }
            ,
            path,
            Path.GetFileName(path));
        });
    }
    
    public void upInterface(UnityAction act, string url, string foldername)
    {

            action = act;
            callbackUrl = url;
            // 绑定按钮事件
            // 多线程进度上传函数
            
            string path = WebData.OpenDialog();
            if (path == null)
            {
                Assets.Message.MessageBox("操作已取消");
                return;
            }

            Dictionary<string, string> InfoDic = new Dictionary<string, string>();
            InfoDic[Constant.propname.requesttype] = Constant.WebCommand.UPLOADCOMMONFILE.ToString();
            InfoDic[Constant.propname.folderName] = foldername;
            InfoDic[Constant.propname.username] = foldername;
            foldername_ = foldername;            
            InfoDic[Constant.propname.filetype] = Path.GetExtension(path); 
            callbackBody = getCallBody(InfoDic);
            print(callbackBody);
            WebData.username = "";
            PutObjectWithProcessByThread((process) => {
                Debug.Log("上传进度为:" + process);
            }
            ,
            path,
            Path.GetFileName(path));
            
    }
    public void upInterface(UnityAction act, string url, string foldername,string path)
    {
        action = act;
        callbackUrl = url;
        // 绑定按钮事件
        // 多线程进度上传函数
       

        Dictionary<string, string> InfoDic = new Dictionary<string, string>();
        InfoDic[Constant.propname.requesttype] = Constant.WebCommand.UPLOADCOMMONFILE.ToString();
        InfoDic[Constant.propname.folderName] = foldername;
        InfoDic[Constant.propname.username] = foldername;
        foldername_ = foldername;
        InfoDic[Constant.propname.filetype] = Path.GetExtension(path);
        callbackBody = getCallBody(InfoDic);
        print(callbackBody);
        WebData.username = "";
        PutObjectWithProcessByThread((process) => {
            Debug.Log("上传进度为:" + process);
        }
        ,
        path,
        Path.GetFileName(path));

    }
    public void AddListenter_d(string path=null,UnityAction callback = null)
    {

        if (path == null || path == "")
        {
            path = WebData.SaveProject();
            downpath =  path+"/"+Path.GetFileName(downpathInAli.Trim());
        }
        else
        {
            downpath =  path;

        }
        if (path == null)
        {
            Assets.Message.MessageBox("操作已取消");
            return;
        }
        GetObjectWithProcessByThread((process) => {
            Debug.Log("down进度为:" + process);
            if (process >= 1)
                action = callback;
        }
        );
    }
    void Start()
    {
        // new OssClient 对象
        ossClient = new OssClient(Config.EndPoint, Config.AccessKeyId, Config.AccessKeySecret);

        // 绑定按钮事件
        //putButton.onClick.AddListener(() => {

        //    if (filePathInputField == null && filePathInputField.text == "")
        //    {
        //        return;
        //    }

        //    string path = filePathInputField.text.Trim();

        //    // 多线程进度上传函数
        //    PutObjectWithProcessByThread((process) => {
        //        Debug.Log("上传进度为:" + process);
        //    },
        //    filePathInputField.text,
        //    Path.GetFileName(path));

        //});
    }

    // Update is called once per frame
    void Update()
    {
        // 因为 UI 只能在主线程中，所以在 Update 中监控进度给 UI
        if (PutProcessCallback != null||GetProcessCallback!= null)
        {
            slider.value = putProcess;
            //processImage.fillAmount = putProcess;
            if (putProcess >= 1)
            {
                //Assets.Message.MessageBox("传输完成，请确认文件是否成功上传（当前状态是否改变）");
                PutProcessCallback = null;
                putProcess = 0;
                slider.value = 0;
                if (action != null)
                {
                    action.Invoke();
                    action = null;
                }
            }
        }

    }


    /// <summary>
    /// 子线程上传文件，避免卡顿
    /// </summary>
    /// <param name="action"></param>
    /// <param name="filePath"></param>
    /// <param name="fileName"></param>
    public void PutObjectWithProcessByThread(Action<float> action, string filePath, string fileName)
    {
        PutProcessCallback = action;
        this.fileName = fileName;
        this.filePath = filePath;

        putLocalThread = new Thread(PutObjectWithProcess);
        StartCoroutine(web.Get(callbackUrl + "?" + callbackBody));
        putLocalThread.Start();
    }

    /// <summary>
    /// 子线程下载文件
    /// </summary>
    /// <param name="action"></param>
    /// <param name="filePath"></param>
    /// <param name="fileName"></param>
    public void GetObjectWithProcessByThread(Action<float> action)
    {
        GetProcessCallback = action;
        getLocalThread = new Thread(GetObjectWithProcess);
        getLocalThread.Start();
    }

    /// <summary>
    /// 获取下载进度
    /// </summary>
    void GetObjectWithProcess()
    {
        try
        {
            downpathInAli = downpathInAli.Trim();
            print(downpathInAli+downpathInAli.Length);
            GetObjectRequest getObjectRequest = new GetObjectRequest(Config.Bucket, downpathInAli);
            getObjectRequest.StreamTransferProgress += GetStreamProcess;
            var obj = ossClient.GetObject(getObjectRequest);
            print(obj);
            using (var requestStream = obj.Content)
            {
                byte[] bt = new byte[1024*1024];
                var path = File.Open(downpath, FileMode.OpenOrCreate);           
                var len = 0;
                while((len = requestStream.Read(bt, 0, 1024)) != 0)//读oss的文件
                {
                    path.Write(bt, 0, len);//写入文件
                }
                path.Close();
            }
            print("下载成功");
        }
        catch (OssException e)
        {
            //Assets.Message.MessageBox("上传错误"+e);
            Debug.LogError("带有进度本地文件数据下载错误：" + e);
        }
        catch (Exception e)
        {
            //Assets.Message.MessageBox("上传错误"+e);
            Debug.LogError("带有进度本地文件数据下载错误：" + e);
        }
        finally
        {
            // 终止进程
            getLocalThread.Abort();
        }

    }


    /// <summary>
    /// 获取上传进度
    /// </summary>
    void PutObjectWithProcess()
    {
        try
        {
            
            using (var fs = File.Open(filePath, FileMode.Open))
            {
                PutObjectRequest putObjectRequest = new PutObjectRequest(Config.Bucket,foldername_+"/"+ WebData.username+foldername_+Path.GetExtension(filePath), fs);
                putObjectRequest.StreamTransferProgress += PutStreamProcess;                
               // var result = client.PutObject(putObjectRequest);

                ossClient.PutObject(putObjectRequest);
                Debug.Log("带有进度本地文件上传成功");
                Loom.QueueOnMainThread((param) =>
                {
                    Assets.Message.MessageBox("传输完成，请确认文件是否成功上传（当前状态是否改变）");
                }, null);
                
            }
        }
        catch (OssException e)
        {
            Loom.QueueOnMainThread((param) =>
            {
                Assets.Message.MessageBox("上传错误，请重试" + e);
                //Assets.Message.MessageBox("传输完成，请确认文件是否成功上传（当前状态是否改变）");
            }, null);
            Debug.LogError("带有进度本地文件数据上传错误：" + e);
        }
        catch (Exception e)
        {
            Loom.QueueOnMainThread((param) =>
            {
                //Assets.Message.MessageBox("传输完成，请确认文件是否成功上传（当前状态是否改变）");
                Assets.Message.MessageBox("上传错误，请重试" + e);
            }, null);
            Debug.LogError("带有进度本地文件数据上传错误：" + e);
        }
        finally
        {
            // 终止进程
            putLocalThread.Abort();
        }

    }

    /// <summary>
    /// 文件上传流事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void PutStreamProcess(object sender, StreamTransferProgressArgs args)
    {
        putProcess = (args.TransferredBytes * 100 / args.TotalBytes) / 100.0f;
        PutProcessCallback.Invoke(putProcess);
    }


    /// <summary>
    /// 文件下载流事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// 暂时用putprogresss代替
    void GetStreamProcess(object sender, StreamTransferProgressArgs args)
    {
        putProcess = (args.TransferredBytes * 100 / args.TotalBytes) / 100.0f;
        GetProcessCallback.Invoke(putProcess);
        print(putProcess + "as");
    }

    private static ObjectMetadata BuildCallbackMetadata(string callbackUrl, string callbackBody)
    {
        string callbackHeaderBuilder = new CallbackHeaderBuilder(callbackUrl, callbackBody).Build();
        string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().
            AddCallbackVariable("x:var1", "x:value1").AddCallbackVariable("x:var2", "x:value2").Build();
        var metadata = new ObjectMetadata();
        metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
        metadata.AddHeader(HttpHeaders.CallbackVar, CallbackVariableHeaderBuilder);
        return metadata;
    }
}

public class Config
{
    public const string AccessKeyId = "LTAI4G9Nr7nHNQCjiDLwMabj";
    public const string AccessKeySecret = "qpFeC8MEE1MduUB4bt302Pkj4cWWzu";
    public const string EndPoint = "oss-cn-qingdao.aliyuncs.com";
    public const string Bucket = "cemm20201009";

}
