using UnityEngine;
using UnityEngine.UI;
using QWeb;
using static Constant;

public class Demo1 : MonoBehaviour
{

    public Text startcu;
    public Text startto;
    public Text t;
    public Text BytesReceived;
    public Text TotalBytesReceived;
    public Text res;
    public RawImage img;
    //http://47.104.207.149/commonFile/m1.pdf
    private void Awake()
    {
        //QWebManager.Add(new QDownloadString("http://47.104.207.149/commonFile/m1.pdf")
        //    .SetCode(QStringCode.UTF8)
        //    .OnCompleted(str=> {
        //        res.text = str;
        //    })
        //    .OnReceivedAmount((a, b) => {
        //        BytesReceived.text = a.ToString();
        //        TotalBytesReceived.text = b.ToString();
        //    })
        //    .OnProgress(i=> { t.text = i.ToString(); })

        //    );

        //QWebManager.Add(new QDownloadData("https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1532929258346&di=6a26ac825c1661b5b0e4cf3a0923609a&imgtype=0&src=http%3A%2F%2Fattach.bbs.miui.com%2Falbum%2F201605%2F09%2F155052ru0wm064s8jh0mju.jpg")
        //    .OnCompletedToTexture2D(t => {
        //        img.texture = t;
        //    })
        //    .OnProgress(i => { t.text = i.ToString(); })
        //    );



        //QWebManager.Add(new QUploadString("http://localhost:33401/api/values")
        //    .SetData("cusPhone=15011111111&cusPassword=123456&customerDeviceCode=1111111111111111111111111111111111111111")
            
        //    .OnCompleted(str =>
        //    {
        //        res.text = str;
        //    })

        //    );

        //Dictionary<string, string> InfoDic = new Dictionary<string, string>();
        //InfoDic[Constant.propname.requesttype] = Constant.WebCommand.UPLOAD.ToString();
        //InfoDic[Constant.propname.folderName] = folderName;
        //InfoDic[Constant.propname.filename] = WebData.username;
        //InfoDic[Constant.propname.filetype] = Path.GetExtension(path);
        //InfoDic[Constant.propname.username] = WebData.username;
        //InfoDic[Constant.propname.fileInDb] = sliderProgress.name.ToString();
        //InfoDic[Constant.propname.oriName] = Path.GetFileName(path);
        //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();


        QWebManager.Add(new QUploadValue("http://localhost:33401/api/values")
            .AddKey(propname.requesttype,WebCommand.UPLOAD.ToString())
            .AddKey(propname.folderName, "123456")
            .AddKey(propname.filename, "1111111111111111111111111111111111111111")
            .AddKey(propname.filetype, "1111111111111111111111111111111111111111")
            .AddKey(propname.username, "1111111111111111111111111111111111111111")
            .AddKey(propname.fileInDb, "1111111111111111111111111111111111111111")
            .AddKey(propname.oriName, "1111111111111111111111111111111111111111")
            .OnCompleted(b=> {
                Debug.Log(b.ToUTF8());
            }));

        //DownloadFile("http://xx.xx.xx.xx:8080", 0);
       // DownloadFile("http://xx.xx.xx.xx:8080", 1);
    }

    void DownloadFile(string host,int version)
    {
        QWebManager.Add(new QDownloadFile(host+"/uploadImg/815319d1658148229311b055024d42df.scene")
            .SetPath(Application.dataPath.Replace("/Assets", "") + "/Data/")
            .SetFileName("815319d1658148229311b055024d42df")
            .SetVersion(version)
            .SetSuffix("scene")
            .OnCompleted(() =>
            {
                res.text = "完成";
            })
            .OnStart((a, t) =>
            {
                startcu.text = a.ToString();
                startto.text = t.ToString();
            })
            .OnProgress(i => {
                t.text = i.ToString();
            })
            .OnReceivedAmount((a, b) =>
            {
                BytesReceived.text = a.ToString();
                TotalBytesReceived.text = b.ToString();
            })
            .OnErrorMsg(msg =>
            {
                Debug.Log(msg);
            })
            .OnSecondSize(time=> {
                Debug.Log(time);
            }));
    }

}
