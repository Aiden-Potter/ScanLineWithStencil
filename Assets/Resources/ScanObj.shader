﻿Shader "Unlit/ScanObj"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeColor("Color",Color)= (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGINCLUDE
            #include "UnityCG.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
			    float3 normal : NORMAL;
			    float3 viewDir : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _EdgeColor;
        ENDCG
     
      
        Pass
        {

            ZTest Less
          
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);    
                return col;
            }
            ENDCG
        }
        
         Pass{
          
            ZTest Greater
            Blend One One
            CGPROGRAM
            #pragma vertex vert_pre
            #pragma fragment frag_pre
            v2f vert_pre (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
                return o;
            }
            fixed4 frag_pre (v2f i) : SV_Target
            {
                float NdotV = 1 - dot(i.normal, i.viewDir) ;
                return _EdgeColor*NdotV;
            }
            ENDCG
        }
    }		
}
