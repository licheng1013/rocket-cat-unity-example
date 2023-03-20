using System;
using System.Collections;
using Google.Protobuf;
using UnityEngine;
using UnityWebSocket;

/// <summary>
/// socket可以和 ExternalMessage 类进行捆绑使用
/// </summary>
public class SocketClient : MonoBehaviour{
    private static SocketClient v;
    private static readonly string address = "ws://localhost:10100/ws";
    public static WebSocket socket;
    private void Awake(){
        Connect();//链接
        StartCoroutine(TryConnect()); //检查链接
    }
    private static IEnumerator TryConnect(){ //尝试重新链接
        while (true){
            yield return new WaitForSeconds (5);
            if (socket.ReadyState != WebSocketState.Open){
                Connect();
            }
        }
    }
    private static void OnError(object sender, ErrorEventArgs e){//异常回调
        
    }
    private static void OnMessage(object sender, MessageEventArgs e){ //消息回调
        if (e.IsBinary){
            HandleManager.ParseMessage(e.RawData);
        }
    }
    private static void OnClose(object sender, CloseEventArgs e){//关闭回调
        
    }
    private static void OnOpen(object sender, OpenEventArgs e){//打开回调
        Debug.Log("链接服务器成功！");
    }
    public static void Send(int mergeCmd,IMessage m = null){
        Send(mergeCmd, (_) => { }, m);
    }

    /** m = 实际发送数据 */
    public static void Send(int mergeCmd,Action<Message> action, IMessage m = null){
        HandleManager.AddHandler(mergeCmd,action);
        socket.SendAsync(HandleManager.BuildMessage(mergeCmd, m));
    }
    public static void Connect(){
        socket = new WebSocket(address);
        // 注册回调
        socket.OnOpen += OnOpen;
        socket.OnClose += OnClose;
        socket.OnMessage += OnMessage;
        socket.OnError += OnError;
        socket.ConnectAsync();
    }
    /** 关闭链接 */
    public static void CloseAsync(){
        socket?.CloseAsync();
    }
}
