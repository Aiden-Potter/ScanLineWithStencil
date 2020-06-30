using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ScanLineCtrl : MonoBehaviour
{
    public Material scanLineMat;
    public Material stencilProcessMat;
    public float range;//0-1
    public float maxRange = 20f;
    public bool isScanning = false;
    public float scanSpeed = 5f;

    RenderTexture camearRenderTex;
    RenderTexture buffer;
    public GameObject player;
    private Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;
        range = 0f;
        camearRenderTex = new RenderTexture(Screen.width,Screen.height,24);
        buffer = new RenderTexture(Screen.width, Screen.height, 24);
        scanLineMat.SetTexture("_StencilTex", buffer);
        scanLineMat.SetFloat("_maxDis", maxRange);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            //scanLine 启动
            //检测敌人
            isScanning = true;
            range = 0f;            
            //替换材质
            Messenger<Vector3, float>.Broadcast(Messages.ScanBegin, transform.position, maxRange);
        }
        //如果在摄像机视野中则不替换

        if (isScanning&& range < maxRange)
        {
            range += scanSpeed * Time.deltaTime;
            scanLineMat.SetVector("_scanCenter", player.transform.position);
            //计算视锥体
            float aspect = camera.aspect;
            float farPlaneDistance = camera.farClipPlane;
            Vector3 midup = Mathf.Tan(camera.fieldOfView / 2 * Mathf.Deg2Rad) * farPlaneDistance * camera.transform.up;
            Vector3 midright = Mathf.Tan(camera.fieldOfView / 2 * Mathf.Deg2Rad) * farPlaneDistance * camera.transform.right * aspect;
            Vector3 farPlaneMid = camera.transform.forward * farPlaneDistance;

            Vector3 bottomLeft = farPlaneMid - midup - midright;
            Vector3 bottomRight = farPlaneMid - midup + midright;
            Vector3 upLeft = farPlaneMid + midup - midright;
            Vector3 upRight = farPlaneMid + midup + midright;

            Matrix4x4 frustumCorner = new Matrix4x4();
            frustumCorner.SetRow(0, bottomLeft);
            frustumCorner.SetRow(1, bottomRight);
            frustumCorner.SetRow(2, upRight);
            frustumCorner.SetRow(3, upLeft);//只需要计算四个角的向量在vert里进行赋值,frag里自动按照像素插值得到方向向量

            scanLineMat.SetMatrix("_FrustumCorner", frustumCorner);
            //float depth = UnityDepthChangeTo01Depth(ViewDepthChangeToUnityDepth(range));//更换了尺度，原先是按0-1传递，现在按世界坐标值传递
            scanLineMat.SetFloat("_Distance", range);
        }
        else
        {
            isScanning = false;

        }

    }

    private void OnPreRender()
    {
        Camera.main.targetTexture = camearRenderTex;
        Graphics.SetRenderTarget(camearRenderTex);
    }
    private void OnPostRender()
    {
        Camera.main.targetTexture = null;//你把我模板缓冲区给我吐出来！
        Graphics.SetRenderTarget(buffer);
        GL.Clear(true, true, new Color(0, 0, 0, 0));

        //将渲染目标设置为Buffer的颜色缓冲区和CameraRenderTexture的深度缓冲区   
        Graphics.SetRenderTarget(buffer.colorBuffer, camearRenderTex.depthBuffer);
        //根据 Stencil Buffer的值选择性渲染      

        if (scanLineMat != null && isScanning == true)
        {

            Graphics.Blit(camearRenderTex, stencilProcessMat, 0);
            Graphics.Blit(camearRenderTex, null as RenderTexture,scanLineMat);
            
        }
        else
        {
            Graphics.Blit(camearRenderTex, null as RenderTexture);
        }
    }
    
    float Depth01ChangeToUnityDepth(float depth01)
    {
        float res = (Camera.main.nearClipPlane - Camera.main.farClipPlane * depth01)
                    / (depth01 * (Camera.main.nearClipPlane - Camera.main.farClipPlane));
        return res;
    }
    float UnityDepthChangeTo01Depth(float unityDepth)
    {
        float res = Camera.main.nearClipPlane /
            ((Camera.main.nearClipPlane - Camera.main.farClipPlane) * unityDepth + Camera.main.farClipPlane);
        return res;
    }
    float ViewDepthChangeToUnityDepth(float viewDepth)
    {
        float res = (1 / viewDepth - 1 / Camera.main.nearClipPlane) /
            (Camera.main.nearClipPlane * Camera.main.farClipPlane / (Camera.main.nearClipPlane - Camera.main.farClipPlane));
        return res;
    }
}
