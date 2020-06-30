using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmGroundSensor : MonoBehaviour
{
    public CapsuleCollider capcol;

    private Vector3 point1;
    private Vector3 point2;
    private float offset = 0.4f;
    private float radius;
    // Start is called before the first frame update
    void Awake()
    {
        radius = capcol.radius-0.05f;
        // print(radius);
        // print(transform.position);
        //print(point1);
        
        //print(point2);
        //point1 = capcol.p
        
    }

    void FixedUpdate()
    {
        point1 = transform.position - transform.up * (capcol.height / 2 - capcol.radius+offset);
        point2 = transform.position + transform.up * (capcol.height / 2 - capcol.radius-offset);
        //绘制一份专用的判断地面的胶囊
        Collider[] outputCol = Physics.OverlapCapsule(point1, point2, radius, LayerMask.GetMask("Ground"));
       //判断是否有重合
        if (outputCol.Length != 0)
        {
            //print("collision!");
            SendMessageUpwards("IsGround");

        }
        else
        {
            SendMessageUpwards("IsNotGround");
        }
    }
    private void OnDrawGizmos()
    {
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(point1, radius);
        Gizmos.DrawWireSphere(point2, radius);
    }
}
