using System;
using System.Runtime.InteropServices;

namespace Assets
{
    class Message
    {
        //[DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        //public static extern int MessageBox(IntPtr handle, String message, String title, int type);//具体方法
        
        public static void MessageBox(String message,int type = 0,OnSure sureAction = null, OnCancel cacelAction = null)
        {
            DialogInfo dialogInfo = new DialogInfo();
            if(type == 0)
            {
                dialogInfo.openType = OpenMessageType.SureandCancle;
                dialogInfo.warnInfo = message;
                dialogInfo.cancleBtnInfo = "关闭";
            }
            else
            {
                
                dialogInfo.openType = OpenMessageType.SureandCancle;
                dialogInfo.warnInfo = message;
                
            }
            dialogInfo.onSure = (OnSure)sureAction;
            dialogInfo.onCancel = (OnCancel)cacelAction;
            MessageBoxUI boxUI = new MessageBoxUI(dialogInfo);
        }

        //"123", "345", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification

        [DllImport("User32.dll ", EntryPoint = "SetParent")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int Width, int Height, int flags);
    }
}
