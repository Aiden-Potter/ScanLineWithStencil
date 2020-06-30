using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionControl : MonoBehaviour
{
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    //private void Update()
    //{
    //    Debug.Log(anim.deltaPosition.z);
    //}

     private void OnAnimatorMove()
     {
         //anim.ApplyBuiltinRootMotion();
         SendMessageUpwards("OnUpdateRM", anim.deltaPosition);
         //Debug.Log(anim.deltaPosition);
     }
}
