using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandFixed : MonoBehaviour
{
    public Vector3 offsetV3;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void OnAnimatorIK(int layerIndex)
    {
       // Debug.Log("!!!!");
        if (anim.GetBool("defense")) return;
        Transform leftHand = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        //unity humannoid骨架， unity计算骨头运动量
        leftHand.localEulerAngles += offsetV3;
        anim.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(leftHand.localEulerAngles));
    }
    private void Update()
    {
       // Debug.Log("???");
    }
}
