using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerControl : MonoBehaviour
{
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// 给模型的动画事件调用的方法
    /// </summary>
    /// <param name="triggerName"></param>
    void ResetTrigger(string triggerName)
    {
        //print(signal);
        anim.ResetTrigger(triggerName);
    }
}
