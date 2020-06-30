using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanObj : MonoBehaviour
{
    public bool beDetected = false;
    public bool isVisible = false;//没找到特别好的可见性判断方法，估计是用射线
    public Material defaultMat;
    public Material beDetectedMat;
    private Renderer render;
    
    private void OnEnable()
    {
        Messenger<Vector3,float>.AddListener(Messages.ScanBegin, StartDetect);
    }
    private void Start()
    {
        render = gameObject.GetComponent<Renderer>();
        defaultMat = render.material;
        beDetectedMat = Resources.Load<Material>("beDetectedMat");
    }   

    private void OnDisable()
    {
        Messenger<Vector3, float>.RemoveListener(Messages.ScanBegin, StartDetect);
    }
    void StartDetect(Vector3 _pos, float _maxDis)
    {
        StartCoroutine(CheckDistance(_pos, _maxDis));
    }
    IEnumerator CheckDistance(Vector3 _pos,float _maxDis)
    {
        yield return new WaitForSeconds(1f);
        float dis = (transform.position - _pos).magnitude;
        if (_maxDis>dis)
        {
            beDetected = true;
        }
        else
        {
            beDetected = false;
        }
    }
    private void Update()
    { 
        if(beDetected)
        {
            render.material = beDetectedMat;
        }
        else
        {
            render.material = defaultMat;
        }
    }
}
