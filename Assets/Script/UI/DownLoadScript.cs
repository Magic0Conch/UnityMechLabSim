using Common;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]

public class OpenDialogFile
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenDialogDir
{
    public IntPtr hwndOwner = IntPtr.Zero;
    public IntPtr pidlRoot = IntPtr.Zero;
    public String pszDisplayName = null;
    public String lpszTitle = null;
    public UInt32 ulFlags = 0;
    public IntPtr lpfn = IntPtr.Zero;
    public IntPtr lParam = IntPtr.Zero;
    public int iImage = 0;
}

public class DllOpenFileDialog
{
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenDialogFile ofn);

    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenDialogFile ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

    [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);

}

public class DownLoadScript : MonoBehaviour
{
    static Assets.Script.GetfileRequest getfileRequest;
    public static string SaveProject()
    {
        OpenDialogDir ofn2 = new OpenDialogDir();
        ofn2.pszDisplayName = new string(new char[2000]); ;     // 存放目录路径缓冲区  
        ofn2.lpszTitle = "Open Project";// 标题  
        //ofn2.ulFlags = BIF_NEWDIALOGSTYLE | BIF_EDITBOX; // 新的样式,带编辑框  
        IntPtr pidlPtr = DllOpenFileDialog.SHBrowseForFolder(ofn2);

        char[] charArray = new char[2000];
        for (int i = 0; i < 2000; i++)
            charArray[i] = '\0';

        DllOpenFileDialog.SHGetPathFromIDList(pidlPtr, charArray);
        string fullDirPath = new String(charArray);
        fullDirPath = fullDirPath.Substring(0, fullDirPath.IndexOf('\0'));

        Debug.Log(fullDirPath);//这个就是选择的目录路径。
        return fullDirPath;
    }


    public static void DownLoadFile(string downLoadFileName)
    {
        BasicData.folderPath = SaveProject();
        BasicData.theUsername = downLoadFileName;
        getfileRequest.DefaultRequse();
    }
    public static void DownLoadFileM(string downLoadFileName)
    {
        
        BasicData.theUsername = downLoadFileName;
        getfileRequest.DefaultRequse();
    }

    public static void getFileScript()
    {
        getfileRequest = GameObject.Find("PhotonEngine").GetComponent<Assets.Script.GetfileRequest>();
    }

    public void ExperimentIntroduction()
    {
        if(!BasicData.isConnected)
        {
            Assets.Message.MessageBox("您已断线，无法进行次操作。");
            return;
        }
        getFileScript();
        DownLoadFile("guidance");
    }
    public void ExperimentReport()
    {
        if (!BasicData.isConnected)
        {
            Assets.Message.MessageBox("您已断线，无法进行次操作。");
            return;
        }
        getFileScript();
        DownLoadFile("template");
    }

    public void Upload()
    {
        if (!BasicData.isConnected)
        {
            Assets.Message.MessageBox("您已断线，无法进行次操作。");
            return;
        }
        FileOpenDialog dialog = new FileOpenDialog();
        dialog.structSize = Marshal.SizeOf(dialog);
        dialog.filter = "Docx files\0*.docx\0Doc Files\0*.doc\0\0";
        dialog.file = new string(new char[256]);
        dialog.maxFile = dialog.file.Length;
        dialog.fileTitle = new string(new char[64]);
        dialog.maxFileTitle = dialog.fileTitle.Length;
        dialog.initialDir = Application.dataPath;  //默认路径
        dialog.title = "Open File Dialog";
        dialog.defExt = "doc";
        dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

        if (DialogShow.GetOpenFileName(dialog))
        {
            Debug.Log(dialog.file);
            byte[] byteArray = FileBinaryConvertHelper.File2Bytes(dialog.file);//文件转成byte二进制数组
            Dictionary<byte, object> data = new Dictionary<byte, object>();
            data.Add((byte)ParameterCode.Username, BasicData.username);
            data.Add((byte)ParameterCode.Report, byteArray);
            PhotonEngine.Peer.OpCustom((byte)OperationCode.Wordfile, data, true);

        }
    }
}
