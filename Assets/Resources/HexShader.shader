Shader "Custom/HexShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		float _ArraySize;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		#ifdef SHADER_API_D3D11
				StructuredBuffer<int> hexProps;
		#endif
		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			
			#ifdef SHADER_API_D3D11
			float3 Grass = float3(0,1,0);
			float3 Water = float3(0,0,1);

			float step = 1.0f / _ArraySize;

			float posX = IN.uv_MainTex.x * _ArraySize; //Position as float in 0 - size
			float posY = IN.uv_MainTex.y * _ArraySize; //Position as float in 0 - size

			int HexX = (int)posX; //Absolute hexcoords
			int HexY = (int)posY; //Absolute hexcoords
			
			


			if (hexProps[ HexX * _ArraySize + HexY ] == 0)
			{
				c.rgb = Water;
			}
			else
				c.rgb = Grass;

			#endif
			o.Albedo = c.rgb;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
