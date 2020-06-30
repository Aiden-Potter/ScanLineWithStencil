using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTimer 
{
   public enum STATE
    {
        IDLE,RUN,FINISHED
    }

    public STATE state = STATE.IDLE;

    public float duration;
    private float currntTime =0f;

    public void Tick()
    {
        if(state == STATE.IDLE)
        {
        }else if(state == STATE.RUN)
        {
            currntTime += Time.deltaTime;       
            if (currntTime >= duration)
            {
                state = STATE.FINISHED;
            }        

        }else if(state == STATE.FINISHED)
        {
           
        }else
        {
            Debug.Log("Timer ERROR");
        }
    }
    /// <summary>
    /// 靠着Button的StartTimer调用
    /// </summary>
    public void Go()
    {
        currntTime = 0f;
        state = MyTimer.STATE.RUN;
    }
}
