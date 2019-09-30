Shader "Custom/StencilRead" {

Properties {
	_Color ("Tint", Color) = (0, 0, 0, 1)
	_MainTex ("Texture", 2D) = "white" {}

	_Smoothness ("Smoothness", Range(0, 1)) = 0
	_Metallic ("Metalness", Range(0, 1)) = 0
	[HDR] _Emission ("Emission", color) = (0,0,0)
	[IntRange] _StencilRef ("Stencil Reference Value", Range(0,255)) = 0
	
	_Index ("Index", Int) = 0
	_Density ("Density", Range(2,100)) = 30
}

SubShader {

	Tags { "RenderType"="Transparent" "Queue"="Geometry"}

    //stencil operation
	Stencil {
		Ref [_StencilRef]
		Comp Equal
	}

	CGPROGRAM
    #pragma target 3.0 
	#pragma surface surf Standard alpha:blend
	
	#define inv(i) (1.0 - i)

	sampler2D _MainTex;
	fixed4 _Color;

	half _Smoothness;
	half _Metallic;
	half3 _Emission;

	float _Density;
	int _Index;

	struct Input 
	{
		float2 uv_MainTex;
		float3 worldPos;
		float3 worldNormal;
	};

	void surf (Input i, inout SurfaceOutputStandard o) 
	{
		fixed4 col = tex2D(_MainTex, i.uv_MainTex);
		col.rgb *= _Color.rgb;

		fixed3 normCol = i.worldNormal*0.5+0.5;

		float2 c = i.uv_MainTex * _Density;
        c = floor(c) / 2;
        float checker = frac(c.x + c.y) * 2;
        
        // either combine or view exclusively

        if (_Index == 1)
        {
        	col.rgb = lerp(col, normCol, checker).rgb;
        }
        else if (_Index == 2)
        {
			col.rgb = lerp(fixed3(1,1,1), fixed3(1,0,1), checker).rgb;
        }
    	else if (_Index == 3)
    	{
			col.rgb = lerp(fixed3(1,1,1), fixed3(0,0,0), checker).rgb;
    	}
		else if (_Index == 4)
		{
			col.rgb = fixed3(inv(col.r), inv(col.g), inv(col.b));
		}
		else if (_Index == 5)
		{
			col.rgb = normCol.rgb;
		}

		o.Albedo = col.rgb;
		o.Alpha = col.a;
		o.Metallic = _Metallic;
		o.Smoothness = _Smoothness;
		o.Emission = _Emission;
	}
	ENDCG
}

FallBack "Standard"
}
