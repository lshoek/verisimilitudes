Shader "Custom/Mix" 
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Layer0 ("Layer0", 2D) = "white" {}
        _Layer1 ("Layer1", 2D) = "white" {}
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
			uniform sampler2D _Layer0;
			uniform sampler2D _Layer1;

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
				fixed4 l0col = tex2D(_Layer0, i.uv);
				fixed4 l1col = tex2D(_Layer1, i.uv);
				fixed4 l2col = tex2D(_MainTex, i.uv);

				fixed4 col = clamp(l0col + l1col + l2col, fxd4(0), fxd4(1.0));
				return col;
			}
			ENDCG
        }
    } 
}
