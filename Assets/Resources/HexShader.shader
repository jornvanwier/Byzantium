Shader "Custom/HexShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGBA)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MidpointDetectionLimit ("MidpointDetectionLimit", Range(0,1)) = 0.05
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

        float remap (float value, float from1, float to1, float from2, float to2) {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

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
		float _MidpointDetectionLimit;

		#ifdef SHADER_API_D3D11
				StructuredBuffer<int> hexProps;
		#endif
		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			#ifdef SHADER_API_D3D11
			float3 Grass = float3(0,1,0);
			float3 Water = float3(0,0,1);
			float3 Desert = float3(1,0,0);

			float posX = IN.uv_MainTex.x * (_ArraySize) - 0.075 * _ArraySize;
			float posY = IN.uv_MainTex.y * (_ArraySize);

            float hexSize = sqrt(3)/3;

			float cubeX = posX * 2/3 / hexSize;
			float cubeZ = (-posX / 3 + sqrt(3)/3 * posY) / hexSize;
			float cubeY = -cubeX-cubeZ;

            int rX = round(cubeX);
            int rZ = round(cubeZ);
            int rY = round(cubeY);

            float xDiff = abs(cubeX - rX);
            float zDiff = abs(cubeZ - rZ);
            float yDiff = abs(cubeY - rY);

            if (xDiff > yDiff && xDiff > zDiff)
            {
                rX = -rY-rZ;
            }
            else if (yDiff > zDiff)
            {
                rY = -rX-rZ;
            }
            else
            {
                rZ = -rX-rY;
            }

            int x = rZ + (rX - (rX & 1)) / 2.0f,
                z = rX;

            int pixelVal = hexProps[ z * _ArraySize + x ];
			if (pixelVal == 0)
			{
				c.rgb = Grass;
			}
			else if (pixelVal == 1)
				c.rgb = Water;
			else
			{
			    c.rgb = Desert;
			}

			if(x < 0 || x >= _ArraySize || z < 0 || z >= _ArraySize)
			    c.rgb = Water;


            float midpointx = hexSize * 3 / 2 * z;
            float midpointy = hexSize * sqrt(3) * (x + 0.5 * (z&1));


            float4 val = tex2D (_MainTex, float2(remap(posX - midpointx, -sqrt(3)/3, sqrt(3)/3,0,1), remap(posY - midpointy, -sqrt(3)/3, sqrt(3)/3,0,1)));
            if(val.a >= 0.1)
            {
             c.r += val.r;
             c.g += val.g;
             c.b += val.b;
             c.rgb = float3(0,0,0);
            }

            //c.a = clamp(c.a,0,1);

            /*
            if(abs(posX - midpointx) < _MidpointDetectionLimit && abs(posY - midpointy) < _MidpointDetectionLimit)
                c.rgba = float4(1,0,0,1);
            */

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
