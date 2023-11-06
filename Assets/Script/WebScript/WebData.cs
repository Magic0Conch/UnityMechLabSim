using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class WebData
{
    public static string username = "";
    public static string connectUri = "";
    public static string baseHttp = "http://localhost:8888";

    public static int score1 = 0;
    public static int score2 = 0;

    public static string OpenDialog()
    {
        FileOpenDialog dialog = new FileOpenDialog();
        dialog.structSize = Marshal.SizeOf(dialog);
        //dialog.filter = "Doc Files\0*.doc\0Docx files\0*.docx\0zip files\0*.zip\0zip files\0*.zip\0pdf files\0*.pdf\07z files\0*.7z\0rar files\0*.rar\0xls files\0*.xls\0xlsx files\0*.xlsx\0\0";
        dialog.filter = "all files\0*.*\0\0";
        dialog.file = new string(new char[256]);
        dialog.maxFile = dialog.file.Length;
        dialog.fileTitle = new string(new char[64]);
        dialog.maxFileTitle = dialog.fileTitle.Length;
        dialog.title = "Open File Dialog";
        dialog.defExt = "doc";
        dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

        if (DialogShow.GetOpenFileName(dialog))
        {
            return dialog.file;
        }

        return null;
    }



    public static string SaveProject()
    {
        OpenDialogDir ofn2 = new OpenDialogDir();
        ofn2.pszDisplayName = new string(new char[2000]); 
        ofn2.lpszTitle = "Open Project";// 标题  
        ofn2.ulFlags = 0x00000040; 

        IntPtr pidlPtr = DllOpenFileDialog.SHBrowseForFolder(ofn2);

        char[] charArray = new char[2000];
        for (int i = 0; i < 2000; i++)
            charArray[i] = '\0';

        DllOpenFileDialog.SHGetPathFromIDList(pidlPtr, charArray);
        string fullDirPath = new String(charArray);
        fullDirPath = fullDirPath.Substring(0, fullDirPath.IndexOf('\0'));

        Debug.Log(fullDirPath);
        return fullDirPath;
    }
}

