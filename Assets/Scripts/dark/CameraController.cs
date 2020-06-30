using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraController : MonoBehaviour
{
   private class LockTarget
    {
        public GameObject obj;
        public float halfHeight;
        public LockTarget(GameObject _obj,float _halfHeight)
        {
            obj = _obj;
            halfHeight = _halfHeight;
        }
    }
    public IUserInput pi;
    private GameObject playerHandle;
    private GameObject cameraHandle;
    public Image lockDot;
    public float horizontalSpeed = 20.0f;
    public float verticalSpeed = 80.0f;
    private Vector3 cameraDampVelocity;

    private float tempEulerx;
    private GameObject model;
    private Camera m_camera;
    
    private LockTarget lockTarget;
    public bool lockState = false;

    void Awake()
    {
        lockDot.enabled = false;
        lockState = false;
        cameraHandle = transform.parent.gameObject;
        playerHandle = cameraHandle.transform.parent.gameObject;
        tempEulerx = 20;
        model = playerHandle.GetComponent<ActorController>().model;
        m_camera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;//隐藏鼠标指针并将其锁定在屏幕中间
        //Cursor.visible = false;
    }

    void FixedUpdate()
    {
        if(lockTarget == null)
        {
            Vector3 tempModelEuler = model.transform.eulerAngles;
            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);
            // ！会有死锁的做法----------------------------------------
            //cameraHandle.transform.Rotate(Vector3.right, pi.Jup * -80.0f * Time.deltaTime);
            //cameraHandle.transform.localEulerAngles = new Vector3(
            //   Mathf.Clamp(cameraHandle.transform.localEulerAngles.x, -40, 30), 0, 0);
            // ！会有死锁的做法----------------------------------------
            tempEulerx -= pi.Jup * verticalSpeed * Time.fixedDeltaTime;
            tempEulerx = Mathf.Clamp(tempEulerx, -40, 30);
            cameraHandle.transform.localEulerAngles = new Vector3(tempEulerx, 0, 0);
            //localEuler只能一次赋值vec3
            //保证旋转摄像机时候人物不动
            model.transform.eulerAngles = tempModelEuler;
        }
        else
        {
            Vector3 tempForward = lockTarget.obj.transform.position - model.transform.position;
            tempForward.y = 0;
            playerHandle.transform.forward = tempForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform.position);
        }


        //camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position, 0.2f);
        m_camera.transform.position = Vector3.SmoothDamp(m_camera.transform.position,
            transform.position, ref cameraDampVelocity, 0.1f);
        //camera.transform.eulerAngles = transform.eulerAngles;
        m_camera.transform.LookAt(cameraHandle.transform);
        //摄像机跟随脚本放在物理部分FixedUpdate，第三段攻击的动画位置更新应该和摄像机更新的位置一致
    }

    private void Update()
    {
        if (lockTarget!=null)
        {
            lockDot.rectTransform.position =  Camera.main.WorldToScreenPoint
                (lockTarget.obj.transform.position  + new Vector3(0, lockTarget.halfHeight, 0));

            if (Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 10.0f)
            {
                lockTarget = null;
                lockState = false;
                lockDot.enabled = false;
            }
        }
       
    }
    /// <summary>
    /// 切换lock状态
    /// </summary>
    public void LockUnLock()
    {

     
        Vector3 modelOrgin1 = model.transform.position;
        Vector3 modelOrgin2 = modelOrgin1 + new Vector3();
        Vector3 boxCenter = modelOrgin2 + model.transform.forward * 5f;
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5f),
                model.transform.rotation,LayerMask.GetMask("Enemy"));
        if(cols.Length ==0)
        {
            lockTarget = null;//重复出现了多次的三行代码
            lockDot.enabled = false;
            lockState = false;
        }
        else
        {
            foreach(var col in cols)
            {
                if (lockTarget!=null && lockTarget.obj == col.gameObject)//如果两次锁定同一个目标则取消锁定
                {
                    lockTarget = null;
                    lockDot.enabled = false;
                    lockState = false;
                    break;
                }
                lockTarget = new LockTarget(col.gameObject,col.bounds.extents.y);//传入半高
                lockDot.enabled = true;
                lockState = true;
                break;
            }
        }
     
    }
    
}

