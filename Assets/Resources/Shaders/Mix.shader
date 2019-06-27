// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Mix" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Layer1 ("Layer1", 2D) = "white" {}
        _Layer2 ("Layer2", 2D) = "white" {}
    }
    
    SubShader 
    {
        Pass 
        {  
        	ZTest Always
   			ZWrite Off
   			Cull Off
			Lighting Off

            CGPROGRAM
			#pragma vertex VERT
			#pragma fragment FRAG
			#include "UnityCG.cginc"
			#include "Extensions.cginc"

			uniform sampler2D _MainTex;	
			uniform sampler2D _Layer1;
			uniform sampler2D _Layer2;

				struct v2f
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			v2f VERT(appdata_img v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 FRAG (v2f i) : COLOR
			{
				fixed4 l1col = tex2D(_Layer1, i.uv);
				fixed4 l2col = tex2D(_Layer2, i.uv);
				fixed4 l3col = tex2D(_MainTex, i.uv);

				fixed4 col = lerp(l3col, l2col, l2col.a);
				col = lerp(col, l1col, l1col.a);

				return col;
			}
			ENDCG
        }
    } 
}
