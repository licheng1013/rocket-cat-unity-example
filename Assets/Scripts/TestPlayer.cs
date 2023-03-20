using UnityEngine;
using UnityEngine.UI;

public class TestPlayer : MonoBehaviour{
    public long userId;
  
    public Text text;

    public Canvas canvas;

    public void SetInfo(long uid){
        userId = uid;
        text.text = uid.ToString();
    }
    public static readonly int mergeCmd = HandleManager.GetMergeCmd(7, 1);
    private float moveX;
    private float moveY;

    // Update is called once per frame
    private void Update(){
        if (userId != TestUi.getTestUi().userId){ // Is not current user, We need skip and return
            return;
        }
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        if (moveX != 0 || moveY != 0){
            //transform.Translate(new Vector2(moveX, moveY) * (Time.deltaTime * 5));
            SocketClient.Send(mergeCmd, new TestDto{
                X = (int)moveX,
                Y = (int)moveY,
                UserId = userId
            });
        }
    }


}