using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{

    public GameObject model;
    public IUserInput pi;
    public CameraController camcon;
    public float movingspeed = 1.4f;
    public float runMultiplier = 2.5f;
    public float jumpVelocity = 3.0f;
    public float rollVelocity = 1.0f;

    [Header("===== Fricion Setting =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;
     
    //[SerializeField]
    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec;
    private Vector3 thrustVec;
    private bool lockPlanar = false;//平面锁死
    private bool trackDirection = false;
    private CapsuleCollider col;
    //private float lerpWeight = 0;
    private Vector3 deltaPos;
    public bool canAttack;
    
    void Awake()
    {
        
        IUserInput[] inputs  = GetComponents<IUserInput>();
        //不要写死（getcomponent反射返回的是最上面的那个IuserInput）
        foreach (var input in inputs)
        {
            if (input.enabled)
            {
                pi = input;
                break;
            }
        }
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        if(pi.lockon)
        {
            camcon.LockUnLock();
        }

        if (!camcon.lockState)
        {
            anim.SetFloat("forward", Mathf.Lerp(anim.GetFloat("forward"), pi.Dmag * (pi.run ? 2.0f : 1.0f), 0.1f));
            anim.SetFloat("right", 0f);
        }
        else
        {//锁定时候角色动作不同
            Vector3 localDVec = transform.InverseTransformVector(pi.Dvec);//把世界坐标转化成局部坐标
            anim.SetFloat("forward", Mathf.Lerp(anim.GetFloat("forward"), localDVec.z * (pi.run ? 2.0f : 1.0f), 0.1f));
            anim.SetFloat("right", Mathf.Lerp(anim.GetFloat("right"), localDVec.x * (pi.run ? 2.0f : 1.0f), 0.1f));
        }


        //---------
        anim.SetBool("defense", pi.defense);
        if (pi.defense)
        {
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1.0f, 0.1f));
        }
        else
        {
            anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0.0f, 0.1f));
        }
        //---------
        if (rigid.velocity.magnitude > 8.0f||pi.roll)
        {
            anim.SetTrigger("roll");
            canAttack = false;
        }
        if (pi.jump)
        {
            anim.SetTrigger("jump");
            canAttack = false;
        }
        if (pi.attack&&(CheckState("ground")||CheckTag("attack"))&&canAttack)
        {
            anim.SetTrigger("attack");
        }

        if (!camcon.lockState)
        {
            if (pi.Dmag > 0.1f)
            {
                //改变模型的foward就可以旋转了
                model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.2f);
                //球形插值，缓动旋转
            }

            if (!lockPlanar)//用于表示跳跃时锁死之前的速度
            {
                planarVec = pi.Dmag * model.transform.forward * movingspeed * (pi.run ? runMultiplier : 1.0f);
                //斜向走会比较快//已修正
            }
        }
        else
        {
            if(!trackDirection)
             model.transform.forward = transform.forward;//playerInput物体的前方
            else
            {
                model.transform.forward = planarVec.normalized;
            }


            if(!lockPlanar)
            planarVec = pi.Dvec * movingspeed * (pi.run ? runMultiplier : 1.0f);
        }
        
    }


    void FixedUpdate()
    {
        //rigid.position += planarVec * Time.fixedDeltaTime;
        //rigid.velocity = planarVec;//直接这样是没有重力分量的
        rigid.position += deltaPos;
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z)+thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
        
    }

    private bool CheckState(string stateName,string LayerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(LayerName)).IsName(stateName);
    }

    private bool CheckTag(string tagName, string LayerName = "Base Layer")
    {
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(LayerName)).IsTag(tagName);
    }
    void OnJumpEnter()
    {
        
        pi.inputEnabled = false;
        lockPlanar = true;
        thrustVec = new Vector3(0, jumpVelocity, 0);
        trackDirection = true;
    }
    //void OnJumpExit()
    //{
    //    //print("On Jump Exit!!");
    //    pi.inputEnabled = true;
    //    lockPlanar = false;
    //}

    public void IsGround()
    {
        //print("Is Ground!!");
        anim.SetBool("isGround", true);
    }
    public void IsNotGround()
    {
        //print("Is Not Ground!!");
        anim.SetBool("isGround", false);
    }
    public void OnGroundEnter()
    {
        pi.inputEnabled = true;
        lockPlanar = false;
        canAttack = true;
        col.material = frictionOne;
        trackDirection = false;
    }

    public void OnGroundExit()
    {
        col.material = frictionZero;
    }
    public void OnFallEnter()
    {
        pi.inputEnabled = false;
        lockPlanar = true;
    }
    public void OnRollEnter()
    {
        trackDirection = true;
        thrustVec = new Vector3(0, rollVelocity, 0);
        pi.inputEnabled = false;
        lockPlanar = true;
        
    }
    public void OnJabEnter()
    {
        //thrustVec = model.transform.forward*(-20);
        pi.inputEnabled = false;
        lockPlanar = true;
    }
    public void OnJabUpdate()
    {
        thrustVec = model.transform.forward * (anim.GetFloat("jabVelocity"));
    }
    public void OnRollUpdate()
    {
        thrustVec = model.transform.forward * rollVelocity;
        //print(1);
    }


    public void OnAttack1hEnter()
    {
        pi.inputEnabled = false;
        //lockPlanar = true;
        //anim.SetLayerWeight(anim.GetLayerIndex("attack"), 1.0f);
      

    }
    public void OnAttackUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        //Debug.Log(thrustVec);

    }
    public void OnUpdateRM(object _deltaPos)
    {
        Vector3 v = (Vector3)_deltaPos;
        //Debug.Log("x:" + v.x + " ,z:" + v.z);
        if (CheckState("attack1hC"))
        {
            
            deltaPos += (0.1f * deltaPos + 0.9f * (Vector3)_deltaPos) / 2.0f;
        }
       
    }  
}

