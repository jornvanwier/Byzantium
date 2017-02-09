﻿Shader "HexagonmapShader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _POMHeightScale ("POM hieght scale", Range(-0.2,0)) = -0.05

        _DefaultGlossiness ("Smoothness", Range(0,1)) = 0.5
        _DefaultMetallic ("Metallic", Range(0,1)) = 0.0
        
        _MainTex          ("Default Albedo Map (RGB)", 2D) = "white" {}
        _DefaultHeightMap ("Default Height map (A)",2D) = "black" {}
        _DefaultNormalMap ("Default Normal map (RGB)", 2D) = "white" {}
        _DefaultAmbOccMap ("Default Ambient Occlusion map", 2D) = "white" {}

    }
    SubShader {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM

        #include "ShaderTypes.cginc"
        #include "ShaderFunctions.cginc"

        #pragma surface surf Standard fullforwardshadows

        #pragma target 5.0

        sampler2D _MainTex;
        sampler2D _DefaultNormalMap;
        sampler2D _DefaultHeightMap;
        sampler2D _DefaultAmbOccMap;

        UNITY_DECLARE_TEX2DARRAY(_AlbedoMaps);
        UNITY_DECLARE_TEX2DARRAY(_HeightMaps);
        UNITY_DECLARE_TEX2DARRAY(_NormalMaps);
        UNITY_DECLARE_TEX2DARRAY(_AmbOccMaps);
        UNITY_DECLARE_TEX2DARRAY(_GlossyMaps);
        UNITY_DECLARE_TEX2DARRAY(_MetallMaps);

        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
        };

        float   _ArraySize;
        half    _DefaultGlossiness;
        half    _DefaultMetallic;
        fixed4  _Color;
        float   _BorderSize;
        float4  _BorderColor;
        float   _Softening;
        float   _POMHeightScale;

        #ifdef SHADER_API_D3D11
            SamplerState LinearRepeatSampler;

            StructuredBuffer<int> _HexagonBuffer;
            Texture2DArray _TileSets[32];
            
            Texture2DArray sampleTileSet(int x, int y)
            {
                int pixelVal = _HexagonBuffer[y * _ArraySize + x];
                return _TileSets[pixelVal];
            }

        #endif




        void surf (Input IN, inout SurfaceOutputStandard o) {

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            #ifdef SHADER_API_D3D11

            float4x4 TBN_matrix = UNITY_MATRIX_TEXTURE0;
            float3 viewDirNormalized = normalize(mul(IN.viewDir, TBN_matrix));


            float3 Grass = float3(0,1,0);
            float3 Water = float3(0,0,1);
            float3 Desert = float3(1,0,0);
            float3 Path = float3(1,1,1);

            float PI = 3.14159265f;
            float distanceToSide = 1.0/4.0 * sqrt(3.0);
            float hexSize = sqrt(3)/3;

            HexagonData data = CalculateMapCoords(IN.uv_MainTex, _ArraySize, hexSize);

            float midpointx = hexSize * 3 / 2 * data.hexagonPositionOffset.y;
            float midpointy = hexSize * sqrt(3) * (data.hexagonPositionOffset.x + 0.5 * (data.hexagonPositionOffset.y&1));

            float diffx = data.hexagonPositionFloat.x - midpointx;
            float diffy = data.hexagonPositionFloat.y - midpointy;

            float2 uvn = float2(remap(diffx, -sqrt(3)/3, sqrt(3)/3,0,1), remap(diffy, -sqrt(3)/3, sqrt(3)/3,0,1));



            float2 offset = ParallaxOcclusionMapping(viewDirNormalized, uvn, _POMHeightScale, _DefaultHeightMap);



            float offsetCorrectY = offset.y - 0.5f;
            float offsetCorrectX = offset.x - 0.5f;

            float rotateY60 = offsetCorrectY * sin(1.0 / 6.0 * PI) + offsetCorrectX * cos(1.0 / 6.0 * PI);
            float rotateY120 = offsetCorrectY * sin(5.0 / 6.0 * PI) + offsetCorrectX * cos(5.0 / 6.0 * PI);

            if(offset.y < 0.5 - distanceToSide && offsetCorrectX < xNegSide(offsetCorrectY) && offsetCorrectX > xPosSide(offsetCorrectY))
            {
                offset.y = 0.5 + distanceToSide - (0.5 - distanceToSide - offset.y);
            }      
            
            if(offset.y > 0.5 + distanceToSide && offsetCorrectX < xPosSide(offsetCorrectY) && offsetCorrectX > xNegSide(offsetCorrectY))
            {
                offset.y = 0.5 - distanceToSide + (offset.y - 0.5 - distanceToSide);
            }
            

            if(rotateY60 > distanceToSide && offsetCorrectY < yPosSide(offsetCorrectX) && offsetCorrectY > 0)
            {
                offset.x -= 0.75;
                offset.y -= distanceToSide;
            }

            if(rotateY60 < - distanceToSide && offsetCorrectY > yPosSide(offsetCorrectX) && offsetCorrectY < 0)
            {
                offset.x += 0.75;
                offset.y += distanceToSide;
            }
            
            if(rotateY120 > distanceToSide && offsetCorrectX < xNegSide(offsetCorrectY) && offsetCorrectY > 0)
            {
                offset.x += 0.75;
                offset.y -= distanceToSide;
            }
            
            if(rotateY120 < - distanceToSide && offsetCorrectX > xNegSide(offsetCorrectY) && offsetCorrectY < 0)
            {
                offset.x -= 0.75;
                offset.y += distanceToSide;
            }





            float4 grass = tex2D (_MainTex, offset );

             o.Normal = UnpackNormal(tex2D(_DefaultNormalMap, offset));
             o.Normal = 1.0 - o.Normal;

            

            if(data.hexagonPositionOffset.x < 0 || data.hexagonPositionOffset.x >= _ArraySize || data.hexagonPositionOffset.y < 0 || data.hexagonPositionOffset.y >= _ArraySize)
            {
                c.rgb = Water;
            }
            else
            {
                int pixelVal = _HexagonBuffer[ data.hexagonPositionOffset.y * _ArraySize + data.hexagonPositionOffset.x ];
                if (pixelVal == 0)
                {
                    c.rgb = grass;
                }
                else if (pixelVal == 1)
                {
                    c.rgb = Water;
                }
                else if (pixelVal == 2)
                {
                    c.rgb = Desert;
                }
                else if (pixelVal == 3)
                {
                    c.rgb = Path;
                }
            }


            c.rgb *= tex2D(_DefaultAmbOccMap, offset);

            #endif

            o.Albedo = c.rgb + float3(1,1,1) * (normalize(IN.viewDir.x) * 0.0001);

            o.Metallic = _DefaultMetallic;
            o.Smoothness = _DefaultGlossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}