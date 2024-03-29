Shader "Custom/Shutter" {

Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
Tags {"Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent"}

Blend SrcAlpha OneMinusSrcAlpha 
ZTest Always
ZWrite Off

Lighting Off
Fog { Mode Off }

Pass 
{
    CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag

    #include "UnityCG.cginc"
    #include "Extensions.cginc"

    struct appdata_t 
    {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
    };

    struct v2f 
    {
        float4 vertex : SV_POSITION;
        half2 texcoord : TEXCOORD0;
    };

    sampler2D _MainTex;
    float4 _MainTex_ST;
    fixed4 _Color;
     
    v2f vert (appdata_t v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
        return o;
    }

    fixed4 frag (v2f i) : SV_Target
    {
        fixed4 col = tex2D(_MainTex, i.texcoord) * _Color;
        return col;
    }
    ENDCG
}
}
}
