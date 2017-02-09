﻿Shader "HexagonmapShader" {
    Properties {
        _POMHeightScale ("POM hieght scale", Range(-0.2,0)) = -0.05
        
        _MainTex          ("1024x1024 Texture for UV's", 2D) = "white" {}
        _DefaultHeightMap ("Default Height map (A)",2D) = "black" {}

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
        sampler2D _DefaultHeightMap;

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
        fixed4  _Color;
        float   _POMHeightScale;

        #ifdef SHADER_API_D3D11
            StructuredBuffer<int> _HexagonBuffer;    
        #endif




        void surf (Input IN, inout SurfaceOutputStandard o) {

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

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



            int pixelVal = _HexagonBuffer[ data.hexagonPositionOffset.y * _ArraySize + data.hexagonPositionOffset.x ];



             o.Normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_NormalMaps, float3(offset, pixelVal)));
             o.Normal = 1.0 - o.Normal;

            
            if(data.hexagonPositionOffset.x < 0 || data.hexagonPositionOffset.x >= _ArraySize || data.hexagonPositionOffset.y < 0 || data.hexagonPositionOffset.y >= _ArraySize)
            {
                c.rgb = Water;
            }
            else
            {
                c.rgb = UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(offset, pixelVal));                
            }


            c.rgb *= UNITY_SAMPLE_TEX2DARRAY(_AmbOccMaps, float3(offset, pixelVal));

            o.Metallic = UNITY_SAMPLE_TEX2DARRAY(_MetallMaps, float3(offset, pixelVal));
            o.Smoothness = UNITY_SAMPLE_TEX2DARRAY(_GlossyMaps, float3(offset, pixelVal));

            #endif

            o.Albedo = c.rgb + float3(1,1,1) * (normalize(IN.viewDir.x) * 0.0001);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}