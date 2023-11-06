using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BasicData 
{
    public static bool isConnected = false;



    public static string username;
    public static bool[] levelSwitch = new bool[100];
    public static int speedMutiple = 2;
    public static bool toolBarStatu = false;
    public static bool banYMove = false;
    public static bool isfixed = false;
    public static bool iscol = false;
    public static bool colZ = false;
    public static bool ExistClock = false;
    public static float X = 0;
    public static float Y = 0;
    public static float Z = 0;
    public static Vector3 offset;
    public static int CompeleteTime = 0;

    public static string thePersonalInfo = "";
    public static int Scores = 0;
    public static int costTime = 0;
    #region t
    public static string allUsers = "";
    public static string theUsername;
    public static string thePassword;
    public static string theProblemRecord;
    public static string downLoadFilename = "idle";
    public static string folderPath = "";
    public static bool isChosen = false;
    #endregion
}
