
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Google.Protobuf;
using UnityEngine;

/// <summary>
/// 一个通用项目对象，
/// </summary>
public static class HandleManager{ //回调处理
    /** 消息保存 */
    private static readonly Dictionary<int,Action<Message>> handlers = new();
    public static void AddHandler(int cmd,int subCmd,  Action<Message> IHandler){  // 注册回调
        Debug.Log("注册路由:"+cmd+"-"+subCmd);
        AddHandler(GetMergeCmd(cmd,subCmd),IHandler);
    }
    /** 添加回调 */
    public static void AddHandler(int mergeCmd,  Action<Message> IHandler){
        handlers.TryAdd(mergeCmd, IHandler);
    }

    /** 分发消息 */
    private static void PackageHandler( int mergeCmd, Message data ) {
        try{
            var handler = handlers[mergeCmd]; //对于路由不存在的错误处理
            handler?.Invoke(data);
        } catch (KeyNotFoundException e){
            Debug.Log(GetCmd(mergeCmd)+"-"+GetSubCmd(mergeCmd)+"路由不存在");
        } catch (Exception e){
            Console.WriteLine(e);
            throw;
        }
    }
    public static void ParseMessage(byte[] bytes){ //解析消息
        var message = new Message();
        message.MergeFrom(bytes);
        //Debug.Log(message);
        PackageHandler((int)message.Merge,message);
    }

    //---------------------------路由命令处理
    private static int GetCmd(int merge) {//获取cmd
        return merge >> 16;
    }

    private static int GetSubCmd(int merge) {//获取subCmd
        return merge & 0xFFFF;
    }
    public static int GetMergeCmd(int cmd, int subCmd) {  //获取mergeCmd
        return (cmd << 16) + subCmd;
    }
    
    //---------------------------封装发送结果处理
    private static byte[] BuildMessage(int cmd,int subCmd,[AllowNull] IMessage v){ // 封装消息发送
        var data = ByteString.Empty;
        if (v != null){
            data = v.ToByteString();
        }
        var message = new Message(){
            Merge = GetMergeCmd(cmd, subCmd),
            Body = data,
        };
        return message.ToByteArray();
    }
    
    //---------------------------封装发送结果处理
    public static byte[] BuildMessage(int merge,IMessage v){ // 封装消息发送
        return BuildMessage(GetCmd(merge), GetSubCmd(merge), v);
    }
  
}
