Shader "Unlit/ScanLine"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _StencilTex("StencilTex",2D) = "white"{}
        _ScanLineWidth("Width",float) = 10
        _ScanLineColor("Color",Color) = (1,1,1,0)
        _Distance("Distance",float) = 0
        _Nonlinear("Nonlinear",float) = 2
        _Ref("Ref",Int) = 1
    }
    SubShader
    {
        //Cull Off
        ZWrite Off
        ZTest Always

        CGINCLUDE
            #include "UnityCG.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 interpolatedRay:TEXCOORD1;
                float4 vertex : SV_POSITION;

            };

            sampler2D _MainTex;
            sampler2D _StencilTex;
			half4 _StencilTex_TexelSize;
            sampler2D _CameraDepthTexture;
            float _Distance;
            float _ScanLineWidth;
            float4 _ScanLineColor;
            float _Nonlinear;
            float4x4 _FrustumCorner;
            float3 _scanCenter;
            float _maxDis;
        ENDCG
        
        Pass
        {          
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                int rayIndex;
				if(v.uv.x<0.5&&v.uv.y<0.5){
					rayIndex = 0;
				}else if(v.uv.x>0.5&&v.uv.y<0.5){
					rayIndex = 1;
				}else if(v.uv.x>0.5&&v.uv.y>0.5){
					rayIndex = 2;
				}else{
					rayIndex = 3;
				}
				o.interpolatedRay = _FrustumCorner[rayIndex];
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                half4 col = tex2D(_MainTex, i.uv);
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv);
                float4 stencilVal = tex2D(_StencilTex, i.uv);
                float linear01Depth = Linear01Depth(depth);
                float3 pixelWorldPos =_WorldSpaceCameraPos+linear01Depth*i.interpolatedRay;//计算像素对应的世界坐标

                float pixelDis = distance(pixelWorldPos,_scanCenter);

                //模板局部后处理，脚本里限制了maxDistance
                if(stencilVal.a==0&& pixelDis<_Distance  &&  pixelDis>_Distance-_ScanLineWidth&&linear01Depth<1)
                {
                    //非线性+远距离消散
                    //float diff = pow(1 - (_Distance - pixelDis) / (_ScanLineWidth),_Nonlinear)*pow((1-_Distance),20) ;
                    float diff =pow(1-(_Distance-pixelDis)/_ScanLineWidth,_Nonlinear)*pow(1-(_Distance/_maxDis),2);
                    _ScanLineColor *= diff;
                    return col + _ScanLineColor;
                }
                return col;
            }
            ENDCG
        }
    }
}
