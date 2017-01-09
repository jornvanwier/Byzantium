Shader "Custom/HexShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Grass (RGBA)", 2D) = "white" {}
		_MainTex2 ("Water (RGBA)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MidpointDetectionLimit ("MidpointDetectionLimit", Range(0,1)) = 0.05
		_Degrees ("Degress", Range(0,360)) = 0
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
		sampler2D _MainTex2;

		struct Input {
			float2 uv_MainTex;
		};

		float _ArraySize;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _MidpointDetectionLimit;
		float _Degrees;

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

            float midpointx = hexSize * 3 / 2 * z;
            float midpointy = hexSize * sqrt(3) * (x + 0.5 * (z&1));

            float diffx = posX - midpointx;
            float diffy = posY - midpointy;

            float2 uvn = float2(remap(diffx, -sqrt(3)/3, sqrt(3)/3,0,1), remap(diffy, -sqrt(3)/3, sqrt(3)/3,0,1));
            float4 grass = tex2D (_MainTex, uvn );
            float4 water = tex2D (_MainTex2, uvn );

			if (pixelVal == 0)
			{
				c.rgb = grass.rgb;
			}
			else if (pixelVal == 1)
				c.rgb = water.rgb;
			else
			{
			    c.rgb = Desert;
			}

			if(x < 0 || x >= _ArraySize || z < 0 || z >= _ArraySize)
			    c.rgb = water.rgb;



            /*
            //if(val.a >= 0.1)
            {
             c.r += val.r;
             c.g += val.g;
             c.b += val.b;
             c.rgb = float3(0,0,0);
             IN.uv_MainTex = uvn;
            }
            c.rgba = val;
            */

            //c.a = clamp(c.a,0,1);

            float PI = 3.14159265f;


            //if(abs(diffy) > _MidpointDetectionLimit || abs(rotate60) > _MidpointDetectionLimit || abs(rotate120) > _MidpointDetectionLimit)
            float rotate60 = diffy * sin(1.0 / 6.0 * PI) + diffx * cos(1.0 / 6.0 * PI);
            float rotate120 = diffy * sin(-1.0 / 6.0 * PI) + diffx * cos(-1.0 / 6.0 * PI);

            if(abs(rotate60) > _MidpointDetectionLimit || abs(rotate120) > _MidpointDetectionLimit || abs(diffy) > _MidpointDetectionLimit)
                c.rgba = float4(0,1,0,1);



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
