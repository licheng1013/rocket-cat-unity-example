using System;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 此脚本只用于测试，可随时删除
/// </summary>
public class TestUi : MonoBehaviour{
    public InputField textField;
    public long userId;
    public GameObject editMenu;
    public GameObject player;
    public Transform playerPoint;
    public bool isStartGame;
    public static TestUi getTestUi(){
        return FindObjectOfType<TestUi>();
    }

    private void Start(){
        // 注册路由
        HandleManager.AddHandler(TestPlayer.mergeCmd, (e) => {
            var listDto = new ListTestDto();
            listDto.MergeFrom(e.Body);
           // Debug.Log(listDto.List);
           var players = FindObjectsOfType<TestPlayer>();
           foreach (var v in players){
                foreach (var dto in listDto.List){
                    if (v.userId == dto.UserId){
                        Debug.Log("玩家:"+v.userId+"移动:"+dto);
                        var vector3 = v.transform.position;
                        v.transform.position = new Vector3(vector3.x + dto.X, vector3.y + dto.Y);
                        break;
                    }
                }
            }
        });
        
        // 注册监听回调
        HandleManager.AddHandler(7, 2, (e) => {
            GetTestUi().editMenu.SetActive(false);
            var listTestDto = new ListTestDto();
            listTestDto.MergeFrom(e.Body);
            Debug.Log(listTestDto);
            foreach (var dto in listTestDto.List){
                var p = Instantiate(player, playerPoint);
                p.GetComponent<TestPlayer>().SetInfo(dto.UserId);
            }
            GetTestUi().isStartGame = true;
        });
    }

    private static TestUi GetTestUi(){
        return FindObjectOfType<TestUi>();
    }



    public void AddMatch(){
        var mergeCmd = HandleManager.GetMergeCmd(7, 0);
        var messageDto = new MessageDto(){
            LongData = Convert.ToInt32(textField.text.Trim())
        };
        userId = messageDto.LongData;
        SocketClient.Send(mergeCmd, (e) => {
            
            Debug.Log("请求成功!");
        }, messageDto);
    }

    public void Restart(){
        var mergeCmd = HandleManager.GetMergeCmd(7, 3); 
        var messageDto = new MessageDto(){ //退出登入
            LongData = userId
        };
        SocketClient.Send(mergeCmd, (e) => {
            Debug.Log("请求成功!");
        }, messageDto);
        
        SceneManager.LoadScene(0);
    }


}