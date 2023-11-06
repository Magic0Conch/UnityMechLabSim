using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Web;
using System.Text;
using NPOI.SS.UserModel;
using System.Reflection;
using System.Xml;
using QWeb;
using static Constant;
using System.Collections.Specialized;

public class Web : MonoBehaviour
{
    public class User
    {
        public string username
        { get; set; }
        public string password
        { get; set; }
        public string name
        { get; set; }
        public string major
        { get; set; }
        public string theClass
        { get; set; }
        public string score1
        { get; set; }
        public string score2
        { get; set; }
        public string attemptTimes
        { get; set; }
        public string file1
        { get; set; }
        public string file1date
        { get; set; }
        public string file2
        { get; set; }
        public string file2date
        { get; set; }
        public string file3
        { get; set; }
        public string file3date
        { get; set; }
        public string file4
        { get; set; }
        public string file4date
        { get; set; }
        public string file5
        { get; set; }
        public string file5date
        { get; set; }
        public string file6
        { get; set; }
        public string file6date
        { get; set; }
        public string file7
        { get; set; }
        public string file7date
        { get; set; }
        public string file8
        { get; set; }
        public string file8date
        { get; set; }
        public string file9
        { get; set; }
        public string file9date
        { get; set; }
        public string file10
        { get; set; }
        public string file10date
        { get; set; }
        public string completeTime
        { get; set; }
        public string readTime1
        { get; set; }
        public string readTime2
        { get; set; }
        public string readTime3
        { get; set; }
        public User()
        {
            username = password = name = major = theClass = score1 = score2 = attemptTimes = file1 = file1date = file2 = file2date = file3 = file3date = file4 = file4date = file5 = file5date = file6 = file6date = file7 = file7date = file8 = file8date = file9 = file9date = file10 = file10date = completeTime = readTime1 = readTime2 = readTime3 = null;
        }
    }
    public IEnumerator DownloadFile(Uri uri, string downloadPath, Slider sliderProgress)
    {
        using (UnityWebRequest downloader = UnityWebRequest.Get(uri))
        {
            downloader.downloadHandler = new DownloadHandlerFile(downloadPath);
            print("开始下载");
            downloader.SendWebRequest();
            print("同步进度条");
            while (!downloader.isDone)
            {
                //print(downloader.downloadProgress);
                sliderProgress.value = downloader.downloadProgress;
                sliderProgress.GetComponentInChildren<Text>().text = (downloader.downloadProgress * 100).ToString("F2") + "%";
                yield return null;
            }
            if (downloader.error != null)
            {
                Debug.LogError(downloader.error);
            }
            else
            {
                print("下载结束");
                sliderProgress.value = 1f;
                sliderProgress.GetComponentInChildren<Text>().text = 100.ToString("F2") + "%";
            }
        }
    }  

    public IEnumerator UpLoadFile(string url, string path, string folderName, Slider sliderProgress,Action action = null)
    {
        Dictionary<string, string> InfoDic = new Dictionary<string, string>();
        InfoDic[Constant.propname.requesttype] = Constant.WebCommand.UPLOAD.ToString();
        InfoDic[Constant.propname.folderName] = folderName;
        InfoDic[Constant.propname.filename] = WebData.username;
        InfoDic[Constant.propname.filetype] = Path.GetExtension(path);
        InfoDic[Constant.propname.username] = WebData.username;
        InfoDic[Constant.propname.fileInDb] = sliderProgress.name.ToString();
        InfoDic[Constant.propname.oriName] = Path.GetFileName(path);
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        foreach (var item in InfoDic)
            formData.Add(new MultipartFormDataSection(item.Key, item.Value));
        formData.Add(new MultipartFormFileSection("file", File.ReadAllBytes(path)));
        UnityWebRequest request = UnityWebRequest.Post(url, formData);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SendWebRequest();
        while (request.uploadHandler.progress < 1)
        {
            sliderProgress.value = request.uploadHandler.progress;
            sliderProgress.GetComponentInChildren<Text>().text = (request.uploadHandler.progress * 100).ToString("F2") + "%";
            yield return null;
        }
        sliderProgress.value = request.uploadHandler.progress;
        sliderProgress.GetComponentInChildren<Text>().text = (request.uploadHandler.progress * 100).ToString("F2") + "%";
        while (!request.isDone)
            yield return null;
        sliderProgress.value = 0;
        sliderProgress.GetComponentInChildren<Text>().text = "0.00%";
        if (request.isHttpError || request.isNetworkError)
        {
            sliderProgress.value = 0;
            sliderProgress.GetComponentInChildren<Text>().text = "0.00%";
            Assets.Message.MessageBox("上传失败，请检查网络\n" + request.error);
        }
        else
        {
            action?.Invoke();
            Assets.Message.MessageBox("上传成功");
            //Debug.Log("UploadState：" + request.downloadHandler.text);
        }



    }

    public void QUpFile(string url, string path, string folderName, Slider sliderProgress, Action action = null)
    {
        QWebManager.Add(new QUploadValue(url)
           .AddKey(propname.requesttype, WebCommand.UPLOAD.ToString())
           .AddKey(propname.folderName, folderName)
           .AddKey(propname.filename, WebData.username)
           .AddKey(propname.filetype, Path.GetExtension(path))
           .AddKey(propname.username, WebData.username)
           .AddKey(propname.fileInDb, sliderProgress.name.ToString())
           .AddKey(propname.oriName, Path.GetFileName(path))
           .AddKey("file",Convert.ToBase64String(File.ReadAllBytes(path)))
           .OnCompleted(b => {
               sliderProgress.value = 0;
               sliderProgress.GetComponentInChildren<Text>().text = "0%";
               action?.Invoke();
               Assets.Message.MessageBox("上传成功");
               Debug.Log(b.ToUTF8());
           })
           .OnProgress(i => {
               sliderProgress.value = i*1.0f/100;
               sliderProgress.GetComponentInChildren<Text>().text = i + "%";
           })
           .OnErrorMsg(msg=> {
               sliderProgress.value = 0;
               sliderProgress.GetComponentInChildren<Text>().text = "0%";
               Assets.Message.MessageBox("上传失败\n" + msg);
           })
           );
        //formData.Add(new MultipartFormFileSection("file", File.ReadAllBytes(path)));

    }

    public IEnumerator UpLoadFile(string url, string path, Slider sliderProgress,string username, Action action = null,bool vanish = false)
    {
        Dictionary<string, string> InfoDic = new Dictionary<string, string>();
        InfoDic[Constant.propname.requesttype] = Constant.WebCommand.UPLOADCOMMONFILE.ToString();
        InfoDic[Constant.propname.filetype] = Path.GetExtension(path);
        InfoDic[Constant.propname.username] = username;
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        foreach (var item in InfoDic)
            formData.Add(new MultipartFormDataSection(item.Key, item.Value));
        formData.Add(new MultipartFormFileSection("file", File.ReadAllBytes(path)));
        UnityWebRequest request = UnityWebRequest.Post(url, formData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SendWebRequest();
        while (request.uploadHandler.progress < 1)
        {
            sliderProgress.value = request.uploadHandler.progress;
            sliderProgress.GetComponentInChildren<Text>().text = (request.uploadHandler.progress * 100).ToString("F2") + "%";
            yield return null;
        }
        sliderProgress.value = request.uploadHandler.progress;
        sliderProgress.GetComponentInChildren<Text>().text = (request.uploadHandler.progress * 100).ToString("F2") + "%";
        while (!request.isDone)
            yield return null;
        sliderProgress.value = 0;
        sliderProgress.GetComponentInChildren<Text>().text = "0.00%";
        if(vanish)
            sliderProgress.transform.parent.gameObject.SetActive(false);
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log(request.error);
            sliderProgress.value = 0;
            sliderProgress.GetComponentInChildren<Text>().text = "0.00%";
            Assets.Message.MessageBox("上传失败，请检查网络");
        }
        else
        {
            action?.Invoke();
            Assets.Message.MessageBox("上传成功");
        }
    }
    public IEnumerator UpLoadText(string url, string text, Slider sliderProgress, string username, Action action = null, bool vanish = false)
    {
        Dictionary<string, string> InfoDic = new Dictionary<string, string>();
        InfoDic[Constant.propname.requesttype] = Constant.WebCommand.UPLOADCOMMONFILE.ToString();
        InfoDic[Constant.propname.filetype] = "txt";
        InfoDic[Constant.propname.username] = username;
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        foreach (var item in InfoDic)
            formData.Add(new MultipartFormDataSection(item.Key, item.Value));
//        formData.Add(new MultipartFormFileSection("file", bts));
        UnityWebRequest request = UnityWebRequest.Post(url, formData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SendWebRequest();
        while (request.uploadHandler.progress < 1)
        {
            sliderProgress.value = request.uploadHandler.progress;
            sliderProgress.GetComponentInChildren<Text>().text = (request.uploadHandler.progress * 100).ToString("F2") + "%";
            yield return null;
        }
        sliderProgress.value = request.uploadHandler.progress;
        sliderProgress.GetComponentInChildren<Text>().text = (request.uploadHandler.progress * 100).ToString("F2") + "%";
        while (!request.isDone)
            yield return null;
        sliderProgress.value = 0;
        sliderProgress.GetComponentInChildren<Text>().text = "0.00%";
        if (vanish)
            sliderProgress.transform.parent.gameObject.SetActive(false);
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log(request.error);
            sliderProgress.value = 0;
            sliderProgress.GetComponentInChildren<Text>().text = "0.00%";
            Assets.Message.MessageBox("上传失败，请检查网络");
        }
        else
        {
            action?.Invoke();
            Assets.Message.MessageBox("上传成功");
        }
    }
    public IEnumerator AddUsersByExcel(string url,string excelFilePath,Action<string> action = null)
    {
        Dictionary<string, string> InfoDic = new Dictionary<string, string>();
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        print(excelFilePath);
        XmlDocument xml = ExcelToXml(excelFilePath);
        print(xml);
        formData.Add(new MultipartFormDataSection("file",xml.InnerXml));
        formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.ADDUSERS.ToString()));
        UnityWebRequest request = UnityWebRequest.Post(url, formData);
        yield return request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
            Assets.Message.MessageBox(request.error);
        }
        else
        {
            action?.Invoke(request.downloadHandler.text);
        }

    }
    public IEnumerator GetInfo(string url,string username,string propName,Action<string> callback=null)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.GETSINGLEINFO.ToString()));
        formData.Add(new MultipartFormDataSection(Constant.propname.propName, propName));
        formData.Add(new MultipartFormDataSection(Constant.propname.username, username));
        UnityWebRequest request = UnityWebRequest.Post(url, formData);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        callback?.Invoke(request.downloadHandler.text);
    }
    public XmlDocument ExcelToXml(string excelFilePath)
    {
        print("xml" + excelFilePath);
        IWorkbook workbook = WorkbookFactory.Create(excelFilePath);
        ISheet sheet = workbook.GetSheetAt(0);//获取第一个工作薄
        IRow firstRow = sheet.GetRow(0);
        List<string> propName = new List<string>();
        int len = 0;
        for (int i = 0; ; i++, len++)
        {
            if (firstRow.GetCell(i) == null)
                break;
            propName.Add(firstRow.GetCell(i).StringCellValue);
        }
        List<string>[] userInfo = new List<string>[len];
        for (int i = 0; i < len; i++)
            userInfo[i] = new List<string>();
        int lineLen = sheet.PhysicalNumberOfRows;
        for (int i = 1; i < lineLen; i++)
        {
            IRow row = sheet.GetRow(i);
            if (row == null)
                break;
            for (int j = 0; j < len; j++)
            {
                if (row.GetCell(j) == null)
                {
                    userInfo[j].Add("0");
                    continue;
                }
                CellType cellType = row.GetCell(j).CellType;
                switch (cellType)
                {
                    case CellType.NUMERIC:
                        userInfo[j].Add(row.GetCell(j).NumericCellValue.ToString());
                        break;
                    case CellType.STRING:
                        userInfo[j].Add(row.GetCell(j).StringCellValue.Trim());
                        break;
                    
                }
            }
        }
        List<User> users = new List<User>();
        for (int i = 0; i < lineLen-1; i++)
        {
            User user = new User();
            try
            {
                if (len >= 5)
                {
                    user.username = userInfo[0][i];
                    user.password = userInfo[1][i];
                    user.name = userInfo[2][i];
                    user.major = userInfo[3][i];
                    user.theClass = userInfo[4][i];
                }
                else
                {
                    user.username = userInfo[0][i];
                    user.password = userInfo[0][i];
                    user.name = userInfo[1][i];
                    user.major = userInfo[2][i];
                    user.theClass = userInfo[3][i];
                }
                users.Add(user);       
            }
            catch(Exception e) {
                Debug.LogError(e.Message);
            }
        }
        return CreateXml(users);
    }
    public XmlDocument CreateXml(List<User> users)
    {
        StringBuilder sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.Append("<root>");
        foreach (User user in users)
        {
            sb.AppendFormat("<user>");
            foreach (PropertyInfo info in user.GetType().GetProperties())
            {
                string s = null;
                try
                {
                    s = info.GetValue(user).ToString();
                }
                catch (Exception) { }
                if (s == null) continue;
                sb.AppendFormat("<{0}>{1}</{2}>", info.Name, s, info.Name);
            }
            sb.AppendFormat("</user>");
        }
        sb.Append("</root>");
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(sb.ToString());
        return xmlDocument;
    }
    public IEnumerator GetAllUserInfo(string url, Action<string> action = null)
    {
        List<IMultipartFormSection> dataform = new List<IMultipartFormSection>();
        dataform.Add(new MultipartFormDataSection(Constant.propname.requesttype, Constant.WebCommand.GETALLUSERINFO.ToString()));
        UnityWebRequest request = UnityWebRequest.Post(url, dataform);
        yield return request.SendWebRequest();
        action?.Invoke(request.downloadHandler.text);
    }
    public IEnumerator Get(string url, Action<string> action = null)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        print(url+"请求响应中");
        yield return request.SendWebRequest();
        print("获取结果"+ request.downloadHandler.text);
        action?.Invoke(request.downloadHandler.text);
    }
    public IEnumerator ExcuteSend(string url,List<IMultipartFormSection> formData,Action<string> action = null)
    {
        UnityWebRequest request = UnityWebRequest.Post(url, formData);
        yield return request.SendWebRequest();
        action?.Invoke(request.downloadHandler.text);
    }
    public IEnumerator ExcuteSend(string url,List<IMultipartFormSection> formData,Action action)
    {
        UnityWebRequest request = UnityWebRequest.Post(url, formData);
        yield return request.SendWebRequest();
        action?.Invoke();
    }
    public void getFileDown(string url, NameValueCollection formData,Slider slider,string path,Action action = null)
    {
        url += "?";
        foreach (string key in formData)
        {
            url +=key + "=" + formData.Get(key)+"&";
        }
        url = url.Substring(0, url.Length - 1);
        print(url);
        QWebManager.Add(new QDownloadFile(url)
            .SetPostData(formData)
            .SetPath(Path.GetFullPath(path))
            .SetFileName(Path.GetFileName(path))
            .OnStart((a,b)=> {
                print(Path.GetFullPath(path) + Path.GetFileName(path));
            })
            .OnCompleted(()=>{
                if (slider != null)
                    Assets.Message.MessageBox("成功");
                action?.Invoke();
                slider.value = 0;
                try
                {
                    slider.GetComponentInChildren<Text>().text = "0%";
                }
                catch { }
            })            
            .OnErrorMsg((msg)=> {
                Assets.Message.MessageBox("失败："+msg);

            })
            .OnProgress(i => {
                slider.value = i*1.0f/100;
                if( slider.GetComponentInChildren<Text>().text!=null )
                    slider.GetComponentInChildren<Text>().text = i + "%";
            })
                
            );
    }
    public IEnumerator getFileDown(string url, List<IMultipartFormSection> formData,Slider slider,string path,Action action = null)
    {
        
        UnityWebRequest request = UnityWebRequest.Post(url, formData);  
        
        
        request.downloadHandler = new DownloadHandlerBuffer();
        UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = request.SendWebRequest();
        //request.downloadHandler = new DownloadHandlerFile(@"C:\Users\dell\Desktop\asdsadh.zip");
        while (!request.downloadHandler.isDone)
        {
            if(slider!=null)
            {
                print(unityWebRequestAsyncOperation.webRequest.downloadedBytes);
                slider.value = unityWebRequestAsyncOperation.webRequest.downloadProgress;
                //print(unityWebRequestAsyncOperation.progress + ":" + unityWebRequestAsyncOperation.webRequest.downloadProgress+  ":" + request.uploadProgress + ":" + request.uploadHandler.progress+":"+ request.downloadHandler.text);
                try
                {
                    
                    slider.GetComponentInChildren<Text>().text = (unityWebRequestAsyncOperation.webRequest.downloadProgress * 100).ToString("F2") + "%";
                }
                catch { }
            }
            yield return null;
        }
        if (slider != null)
        {
            slider.value = 0;
            try
            {
                slider.GetComponentInChildren<Text>().text = "0.00%";

            }
            catch
            {

            }
        }
        try
        {
            slider.transform.parent.gameObject.SetActive(false);
        }
        catch
        {

        }
        
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log(request.error);
            if (slider != null)
            {
                slider.value = 0;
                try
                {
                slider.GetComponentInChildren<Text>().text = "0.00%";

                }
                catch { }
            }
            Assets.Message.MessageBox("失败，请检查网络");
        }
        else
        {
            //action?.Invoke();
            if(slider!=null)
                Assets.Message.MessageBox("成功");
            //Debug.Log("UploadState：" + request.downloadHandler.text);
            byte[] bytes = request.downloadHandler.data;
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            action?.Invoke();
        }

    }
    public IEnumerator getFileDownMain(string url, List<IMultipartFormSection> formData,Slider slider,string path,Action action = null)
    {
        
        UnityWebRequest request = UnityWebRequest.Post(url, formData);
        request.SendWebRequest();
        while (!request.downloadHandler.isDone)
        {
            if(slider!=null)
            {
                slider.value = request.downloadProgress;
                try
                {

                slider.GetComponentInChildren<Text>().text = (request.downloadProgress* 100).ToString("F2") + "%";
                }
                catch { }
            }
            yield return null;
        }
        if (slider != null)
        {
            slider.value = 0;
            try
            {
                slider.GetComponentInChildren<Text>().text = "0.00%";
            }
            catch
            {
            }
        }
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log(request.error);
            if (slider != null)
            {
                slider.value = 0;
                try
                {
                    slider.GetComponentInChildren<Text>().text = "0.00%";
                }
                catch { }
            }
            Assets.Message.MessageBox("失败，请检查网络");
            action?.Invoke();
        }
        else
        {
            byte[] bytes = request.downloadHandler.data;
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            action?.Invoke();
        }

    }

    public IEnumerator getFileDownAdType(string url, List<IMultipartFormSection> formData, Slider slider, string path, Action action = null)
    {

        UnityWebRequest request = UnityWebRequest.Post(url, formData);
        request.SendWebRequest();
        //request.downloadHandler = new DownloadHandlerFile(@"C:\Users\dell\Desktop\asdsadh.zip");
        while (!request.downloadHandler.isDone)
        {
            if (slider != null)
            {
                slider.value = request.downloadProgress;
                slider.GetComponentInChildren<Text>().text = (request.downloadProgress * 100).ToString("F2") + "%";
            }
            yield return null;
        }
        if (slider != null)
        {
            slider.value = 0;
            slider.GetComponentInChildren<Text>().text = "0.00%";
        }
        //slider.transform.parent.gameObject.SetActive(false);

        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log(request.error);
            if (slider != null)
            {
                slider.value = 0;
                slider.GetComponentInChildren<Text>().text = "0.00%";
            }
            Assets.Message.MessageBox("失败，请检查网络");
        }
        else
        {
            //action?.Invoke();
            if (slider != null)
                Assets.Message.MessageBox("成功");
            //Debug.Log("UploadState：" + request.downloadHandler.text);
            byte[] bytes = request.downloadHandler.data;
            int n = bytes[0] * 256 + bytes[1];

            switch (n)
            {
                case 255 * 256 + 216:
                    path += ".jpg";
                    break;
                case 208 * 256 + 207:
                    path += ".doc";
                    break;
                case 80 * 256 + 75:
                    path += ".zip";
                    break;
                case 82 * 256 + 97:
                    path += ".rar";
                    break;
            }
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            action?.Invoke();
        }

    }

}
