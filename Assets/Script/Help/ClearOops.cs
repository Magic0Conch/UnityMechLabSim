using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;

public class ClearOops : MonoBehaviour
{
    private Thread CloseOops;         
    private bool ThreadRunning = true;

    
    void Start()
    {
        
        CloseOops = new Thread(ClearOopsWindows)
        {
            IsBackground = true//设为当前进程的后台进程，使得在退出程序时该子线程一并结束
        };

        CloseOops.Start();
    }

    
    
    void ClearOopsWindows()
    {
        
        while (ThreadRunning)
        {
            Clear.FindAndCloseWindow();
        }
    }

    
    void OnApplicationQuit()
    {
        ThreadRunning = false;
    }


    //调用win32API中的方法关闭Oops弹出框
    class Clear
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        public static void FindAndCloseWindow()
        {
            IntPtr lHwnd = FindWindow(null, "Oops");
            if (lHwnd != IntPtr.Zero)
            {
                SendMessage(lHwnd, WM_SYSCOMMAND, SC_CLOSE, 0);
            }
        }
    }
}
