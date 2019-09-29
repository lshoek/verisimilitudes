Shader "Custom/Shutter" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }

    SubShader {
        Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent"}
        
        ZTest Always
        ZWrite Off
        Lighting Off
        Fog { Mode Off }

        Blend SrcAlpha OneMinusSrcAlpha 

        Pass {
            Color [_Color]
            SetTexture [_MainTex] { combine texture * primary } 
        }
    }
}