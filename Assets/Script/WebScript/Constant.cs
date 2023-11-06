using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;


public static class Constant
{
    public enum loginState
    {
        TEACHERSUCCESS, STUDENTSUCCESS, FAILED
    }
    public enum WebCommand
    {
        DOWNLOAD, UPLOAD, LOGIN, ADDUSERS, DELETEUSERS, GETALLUSERINFO, GETFILE, GETSINGLEINFO, INSERTSINGLEUSER, UPDATE,UPLOADCOMMONFILE, DOWNCOMMON, SUBMITSCORE,GEIINFO, SUBMITTIME, UPCOMPELETETIME, DOWNALL
    }
    public static class propname
    {
        public static string username = "username";
        public static string password = "password";
        public static string filename = "filename";
        public static string filetype = "filetype";
        public static string name = "name";
        public static string major = "major";
        public static string theClass = "class";
        public static string score1 = "score1";
        public static string score2 = "score2";
        public static string file1 = "file1";
        public static string file1date = "file1date";
        public static string file2 = "file2";
        public static string file2date = "file2date";
        public static string file3 = "file3";
        public static string file3date = "file3date";
        public static string file4 = "file4";
        public static string file4date = "file4date";
        public static string file5 = "file5";
        public static string file5date = "file5date";
        public static string file6 = "file6";
        public static string file6date = "file6date";
        public static string requesttype = "requesttype";
        public static string folderName = "folderName";
        public static string fileInDb = "fileInDb";
        public static string propName = "propName";
        public static string oriName = "oriName";
    }

}