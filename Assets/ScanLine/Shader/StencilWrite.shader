Shader "Unlit/StencilWrite"
{
    Properties
    {
        _Ref("Ref",Int) = 1
    }
    SubShader
    {
       ColorMask 0
       ZTest Always
       ZWrite Off
       Stencil{
            Ref [_Ref]
            Comp Always
            Pass Replace            
       }
        Pass
        {
            
        }
    }
}
