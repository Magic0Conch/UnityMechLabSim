using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Common;
using UnityEngine.UI;
using System;

public class PhotonEngine : MonoBehaviour, IPhotonPeerListener
{
    GameObject connectedStatus;
    public static string connectString = "127.0.0.1:4530";
    public static PhotonEngine Instance;
    public static PhotonPeer Peer
    {
        get
        {
            return peer;
        }
    }
    private Dictionary<OperationCode, Request> RequestDict = new Dictionary<OperationCode, Request>();

    private static PhotonPeer peer;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject); return;
        }
    }

    // Use this for initialization
    void Start()
    {

        peer = new PhotonPeer(this, ConnectionProtocol.Tcp);
        peer.Connect(connectString, "MyGame1");

    }

    // Update is called once per frame
    void Update()
    {

        peer.Service();//需要一直调用Service方法,时时处理跟服务器端的连接
    }
    //当游戏关闭的时候（停止运行）调用OnDestroy
    private void OnDestroy()
    {
        //如果peer不等于空并且状态为正在连接
        if (peer != null && peer.PeerState == PeerStateValue.Connected)
        {
            peer.Disconnect();//断开连接
        }
    }

    //
    public void DebugReturn(DebugLevel level, string message)
    {

    }
    //如果客户端没有发起请求，但是服务器端向客户端通知一些事情的时候就会通过OnEvent来进行响应 
    public void OnEvent(EventData eventData)
    {


    }
    //当我们在客户端向服务器端发起请求后，服务器端接受处理这个请求给客户端一个响应就会在这个方法里进行处理
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        OperationCode opCode = (OperationCode)operationResponse.OperationCode;//得到响应的OperationCode
        Request request = null;
        
        bool temp = RequestDict.TryGetValue(opCode, out request);//是否得到这个响应
                                                                 // 如果得到这个响应
        print("的大:"+ opCode);
        if (temp)
        {
            request.OnOperationResponse(operationResponse);//处理Request里面的响应
            Debug.Log("找到对应的响应处理对象。");
        }
        else
        {
            Debug.Log("没有找到对应的响应处理对象");
        }
    }
    //public void OnStatusChanged(StatusCode statusCode)
    //{
    //    connectedStatus = GameObject.Find("ConnectedStatus");
    //    if (connectedStatus == null) return;
    //    if (statusCode == StatusCode.Disconnect)
    //    {
    //        Assets.Message.MessageBox(IntPtr.Zero, "您已断线，无法进行与网络有关的操作", "注意", 0);
    //        connectedStatus.GetComponent<Text>().text = "离线";
    //        BasicData.isConnected = false;
    //    }
    //    if(statusCode == StatusCode.Connect)
    //    {
    //        Assets.Message.MessageBox(IntPtr.Zero, "已经成功连接服务器", "注意", 0);
    //        connectedStatus.GetComponent<Text>().text = "在线";
    //        BasicData.isConnected = true;
    //    }
    //}

    //添加Requst
    public void AddRequst(Request requst)
    {
        RequestDict.Add(requst.OpCode, requst);

    }
    //移除Requst
    public void RemoveRequst(Request request)
    {
        RequestDict.Remove(request.OpCode);
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        throw new NotImplementedException();
    }
}