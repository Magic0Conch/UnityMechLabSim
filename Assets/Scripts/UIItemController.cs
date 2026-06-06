using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using Common;

public class FileBinaryConvertHelper
{
    /// <summary>
    /// 将文件转换为byte数组
    /// </summary>
    /// <param name="path">文件地址</param>
    /// <returns>转换后的byte数组</returns>
    public static byte[] File2Bytes(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            return new byte[0];
        }

        FileInfo fi = new FileInfo(path);
        byte[] buff = new byte[fi.Length];

        FileStream fs = fi.OpenRead();
        fs.Read(buff, 0, Convert.ToInt32(fs.Length));
        fs.Close();

        return buff;
    }

    /// <summary>
    /// 将byte数组转换为文件并保存到指定地址
    /// </summary>
    /// <param name="buff">byte数组</param>
    /// <param name="savepath">保存地址</param>
    public static void Bytes2File(byte[] buff, string savepath)
    {
        if (System.IO.File.Exists(savepath))
        {
            System.IO.File.Delete(savepath);
        }

        FileStream fs = new FileStream(savepath, FileMode.CreateNew);
        BinaryWriter bw = new BinaryWriter(fs);
        if (buff!=null)
        bw.Write(buff, 0, buff.Length);
        bw.Close();
        fs.Close();
    }



}

public class UIItemController : MonoBehaviour
{
    public void onSumbitReport()
    {
        string thePath = Application.streamingAssetsPath + "/template.doc";
        byte[] byteArray = FileBinaryConvertHelper.File2Bytes(thePath);//文件转成byte二进制数组
        string JarContent = Convert.ToBase64String(byteArray);//将二进制转成string类型，可以存到数据库里面了
        Dictionary<byte, object> data = new Dictionary<byte, object>();
        print(JarContent.Substring(0, 20));
        data.Add((byte)ParameterCode.Username, BasicData.username);
        data.Add((byte)ParameterCode.Report, byteArray);
        PhotonEngine.Peer.OpCustom((byte)OperationCode.Wordfile, data, true);
    }

    public void CommonButtonClick()
    {
        Image image = transform.GetChild(0).GetComponent<Image>();
        image.enabled = !(image.enabled);
    }

    public void OpenFile()
    {
        Assets.Script.GetfileRequest getfileRequest = GetComponent<Assets.Script.GetfileRequest>();
        BasicData.theUsername = "template";
        getfileRequest.DefaultRequse();
        string path = Application.dataPath;
        Application.OpenURL("file:///" + path + "/StreamingAssets/" + "template.doc");


    }
    public GameObject thePannel;
    public void CheckNotice()
    {
        thePannel.SetActive(!thePannel.activeInHierarchy);
    }

    public void OnPreview()
    {
        BasicData.theUsername = "guidance";
        Assets.Script.GetfileRequest getfileRequest = GetComponent<Assets.Script.GetfileRequest>();
        getfileRequest.DefaultRequse();
        string path = UnityEngine.Application.dataPath;
        UnityEngine.Application.OpenURL("file:///" + path + "/StreamingAssets/" + "guidance.doc");
    }

    public void EndAdjust()
    {
        BasicData.ExistClock = false;
        GameObject.Find("校准Btn").SetActive(false);
        if(BasicData.levelSwitch[4])
        {
            GameObject.Find("百分表带支架").SetActive(false);
            BasicData.levelSwitch[4] = false;
            BasicData.levelSwitch[6] = true;
            if (BasicData.levelSwitch[7])
                BasicData.levelSwitch[8] = true;
        }
        if(BasicData.levelSwitch[10])
        {
            GameObject.Find("clock2").SetActive(false);
            BasicData.levelSwitch[15] = true;
            if (BasicData.levelSwitch[11])
            {
                BasicData.levelSwitch[16] = true;
                BasicData.levelSwitch[10] = false;
            }
        }
        else if(BasicData.levelSwitch[14])
        {
            GameObject.Find("clock3").SetActive(false);
            BasicData.levelSwitch[14] = false;
            BasicData.levelSwitch[17] = true;
        }
    }

    public void OnCommonMenuButtonClick()
    {
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeInHierarchy);
    }
   
    bool toolBarStable = true;
    IEnumerator ChangeToolBar()
    {
        toolBarStable = false;
        RectTransform rect = transform.GetComponent<RectTransform>();
        const float moveSpeed = 400f;
        const float leftLimite = -393.8001f;
        const float rightLimite = -331f;
        float moveDist = 70f;
        while (true)
        {
            float frameDist = Time.deltaTime * moveSpeed;
            rect.Translate(Vector3.left * frameDist*(BasicData.toolBarStatu?1:-1));
            moveDist -= frameDist;
            if(rect.anchoredPosition.x<leftLimite||rect.anchoredPosition.x > rightLimite)
            {
                toolBarStable = true;
                rect.anchoredPosition = new Vector2(BasicData.toolBarStatu ?leftLimite: rightLimite, rect.anchoredPosition.y);
                gameObject.transform.GetChild(0).GetComponent<Image>().enabled = !gameObject.transform.GetChild(0).GetComponent<Image>().enabled;
                gameObject.transform.GetChild(1).GetComponent<Image>().enabled = !gameObject.transform.GetChild(1).GetComponent<Image>().enabled;
                BasicData.toolBarStatu = !BasicData.toolBarStatu;
                yield break;
            }
            yield return null;
        }
    }
    public void EnableToolBar()
    {
        if(toolBarStable)
            StartCoroutine(ChangeToolBar());
    }

    public void SpeedButtonClick()
    {
        Image[] images = new Image[3];
        int lightIndex = 0;
        images[0] = transform.GetChild(0).GetComponent<Image>();
        images[1] = transform.GetChild(1).GetComponent<Image>();
        images[2] = transform.GetChild(2).GetComponent<Image>();
        for(int i = 0;i<3;i++)
        {
            if (images[i].enabled)
            {
                images[i].enabled = false;
                lightIndex = i;
            }
        }
        lightIndex = (lightIndex + 1) % 3;
        images[lightIndex].enabled = true;
        BasicData.speedMutiple = (lightIndex + 1)* (lightIndex + 1);
    }
}
