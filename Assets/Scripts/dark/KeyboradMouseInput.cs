using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 还没有做手柄的playerInput
/// </summary>
public class KeyboradMouseInput : IUserInput
{
    // Start is called before the first frame update
    [Header("===== Key Setting =====")]
    public string keyUp;
    public string keyDown;
    public string keyLeft;
    public string keyRight;//alt快捷键

    public string keyA;
    public string keyB;
    public string keyC;
    public string keyD;
    public string keyLock;

    public string keyJRight;
    public string keyJUp;
    public string keyJDown;
    public string keyJLeft;
    

     //public MyButton RB = new MyButton();
     //public MyButton LB = new MyButton();
     public MyButton btnA = new MyButton();
     public MyButton btnB = new MyButton();
     public MyButton btnC = new MyButton();
     public MyButton btnD = new MyButton();
     public MyButton btnLock = new MyButton();


    [Header("===== Mouse Setting =====")]
    public bool mouseEnabled = false;
    public float xSensitivity;
    public float ySensitivity;

    //键盘鼠标的input
    void Update()
    {
        btnA.Tick(Input.GetKey(keyA));
        btnB.Tick(Input.GetKey(keyB));
        btnC.Tick(Input.GetKey(keyC));
        btnD.Tick(Input.GetKey(keyD));//对手柄上的按键用getbutton
        btnLock.Tick(Input.GetKey(keyLock));

        
        if (mouseEnabled)
        {
            Jup = Input.GetAxis("Mouse Y")*3*ySensitivity;
            Jright = Input.GetAxis("Mouse X")*2.5f*xSensitivity;
        }else
        {
            Jup = (Input.GetKey(keyJUp) ? 1.0f : 0) - (Input.GetKey(keyJDown) ? 1.0f : 0);
            Jright = (Input.GetKey(keyJRight) ? 1.0f : 0) - (Input.GetKey(keyJLeft) ? 1.0f : 0);
        }
        

        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0) - (Input.GetKey(keyDown) ? 1.0f : 0);
        targetDright = (Input.GetKey(keyRight) ? 1.0f : 0) - (Input.GetKey(keyLeft) ? 1.0f : 0);
        if (!inputEnabled)
        {
            targetDup = 0;
            targetDright = 0;
        }
        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, .1f);//0.1s变化时间
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, .1f);


        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;//中转变量

        Dmag = Mathf.Sqrt(Dup2 * Dup2 + Dright2 * Dright2);
        Dvec = transform.right * Dright2 + transform.forward * Dup2;


        run = (btnA.IsPressing&&(!btnA.IsDelaying))||btnA.IsExtending;//人物跑的时候有一开始启动延时和退出延时
        jump = btnA.OnPressed&&btnA.IsExtending;//double trigger
        roll = btnA.OnReleased && btnA.IsDelaying;       
        attack = btnC.OnPressed;
        defense = btnD.IsPressing;
        lockon = btnLock.OnPressed;

    }
 
}
