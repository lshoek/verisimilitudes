Shader "Custom/StencilRead" {
	Properties {
		_Color ("Tint", Color) = (0, 0, 0, 1)
		_MainTex ("Texture", 2D) = "white" {}

		_Smoothness ("Smoothness", Range(0, 1)) = 0
		_Metallic ("Metalness", Range(0, 1)) = 0
		[HDR] _Emission ("Emission", color) = (0,0,0)
		[IntRange] _StencilRef ("Stencil Reference Value", Range(0,255)) = 0
		
		_Value ("Value", Float) = 1.0
		_Density ("Density", Range(2,100)) = 30
	}
	SubShader {
		Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

        //stencil operation
		Stencil{
			Ref [_StencilRef]
			Comp Equal
		}

		CGPROGRAM

		#pragma surface surf Standard
		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;

		half _Smoothness;
		half _Metallic;
		half3 _Emission;

		float _Value;
		float _Density;

		struct Input 
		{
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
		};

		void surf (Input i, inout SurfaceOutputStandard o) 
		{
			fixed3 col = tex2D(_MainTex, i.uv_MainTex).rgb;
			col *= _Color.rgb;

			fixed3 normCol = i.worldNormal*0.5+0.5;

			float2 c = i.uv_MainTex * _Density;
            c = floor(c) / 2;
            float checker = frac(c.x + c.y) * 2;
            
            // either combine or view exclusively
            col.rgb = lerp(col, normCol, checker).rgb;
			col.rgb = lerp(fixed3(1,1,1), fixed3(1,0,1), checker).rgb;

			o.Albedo = col.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Emission = _Emission;
		}
		ENDCG
	}
	FallBack "Standard"
}
