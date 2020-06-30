using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 工具类
/// </summary>
public class MyButton
{
    public bool IsPressing = false;
    public bool OnPressed = false;
    public bool OnReleased = false;
    public bool IsExtending = false;
    public bool IsDelaying = false;

    public MyTimer exitTimer = new MyTimer();
    public MyTimer startTimer = new MyTimer();
    public float extendingDuration = 0.15f;
    public float startDuration = 0.15f;
    private bool curState = false;
    private bool lastState = false;


    public void Tick(bool input)
    {
        
        exitTimer.Tick();
        startTimer.Tick();
        curState = input;
        IsPressing = curState;

        OnReleased = false;
        OnPressed = false;
        IsExtending = false;
        IsDelaying = false;
        if (lastState != curState)
        {
            if (curState)
            {
                OnPressed = true;
                StartTimer(startTimer, startDuration);
                //Timer在RUN时，extending信号才会存在
            }      
            else
            {
                OnReleased = true;
                StartTimer(exitTimer, extendingDuration);
            }               
        }
        lastState = curState;

        if (exitTimer.state == MyTimer.STATE.RUN)
            IsExtending = true;

        if (startTimer.state == MyTimer.STATE.RUN)
            IsDelaying = true;
       
    }
    void StartTimer(MyTimer myTimer,float duration)
    {
        myTimer.duration = duration;
        myTimer.Go();
    }
   
}
   


