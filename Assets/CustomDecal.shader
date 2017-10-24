Shader "CustomDecal" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	_DecalTex("Decal (RGBA)", 2D) = "black" {}
	_FrameTex("Frame (RGBA)", 2D) = "black" {}
	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 250

		CGPROGRAM
		\#pragma surface surf Lambert

		sampler2D _MainTex;
	sampler2D _DecalTex;
	sampler2D _FrameTex;
	fixed4 _Color;

	struct Input {
		float2 uv_MainTex;
		float2 uv_DecalTex;
		float2 uv_FrameTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		half4 decal = tex2D(_DecalTex, IN.uv_DecalTex);
		half4 frame = tex2D(_FrameTex, IN.uv_FrameTex);
		c.rgb = lerp(c.rgb, decal.rgb, decal.a);
		c.rgb = lerp(c.rgb, frame.rgb, frame.a);
		c *= _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}

		Fallback "Diffuse"
}