﻿Shader "HexagonmapShader" {
    Properties {
        _POMHeightScale ("POM height scale", Range (0.0,1.0)) = 0.1
        _AODistance ("AO Falloff", Range (0.0,1.0)) = 0.0
        _AODistanceDelta ("AO Delta after falloff", Range(0.0, 1.0)) = 0.0
        _MainTex          ("Selected tile texture", 2D) = "white" {}
        _DefaultHeightMap ("Default Height map (A)",2D) = "black" {}
        _HighlightColor ("Select color ", Color) = (0.5, 0.5, 0.5, 0.5)
    }
    SubShader {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM

        //For using RenderDoc's shader debuging.
        #pragma enable_d3d11_debug_symbols

        #pragma surface surf Standard fullforwardshadows

        #pragma target 5.0

        sampler2D _MainTex;
        sampler2D _DefaultHeightMap;

        UNITY_DECLARE_TEX2DARRAY(_AlbedoMaps);
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
        float   _AODistance;
        float   _AODistanceDelta;
        float4  _HighlightColor;

        #ifdef SHADER_API_D3D11
            StructuredBuffer<uint> _HexagonBuffer;
        #endif


        #include "ShaderTypes.cginc"
        #include "ShaderFunctions.cginc"

        void surf (Input IN, inout SurfaceOutputStandard o) {

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

            #ifdef SHADER_API_D3D11

            float4x4 TBN_matrix = UNITY_MATRIX_TEXTURE0;
            float3 viewDirNormalized = normalize(mul(IN.viewDir, TBN_matrix));

            float PI = 3.14159265f;
            float distanceToSide = 1.0/4.0 * sqrt(3.0);
            float hexSize = sqrt(3)/3;

            HexagonData data = CalculateMapCoords(IN.uv_MainTex, _ArraySize, hexSize);

            //early elimination for tiles outside of the boundaries.
            if(data.hexagonPositionOffset.x < 0 || data.hexagonPositionOffset.x >= _ArraySize || data.hexagonPositionOffset.y < 0 || data.hexagonPositionOffset.y >= _ArraySize)
            {
                discard;
            }

            float midpointx = hexSize * 3 / 2 * data.hexagonPositionOffset.y;
            float midpointy = hexSize * sqrt(3) * (data.hexagonPositionOffset.x + 0.5 * (data.hexagonPositionOffset.y&1));

            float diffx = data.hexagonPositionFloat.x - midpointx;
            float diffy = data.hexagonPositionFloat.y - midpointy;

            float2 uvn = float2(remap(diffx, -sqrt(3)/3, sqrt(3)/3,0,1), remap(diffy, -sqrt(3)/3, sqrt(3)/3,0,1));

            //Parallax mapping
            float2 offset = ParallaxOcclusionMapping(viewDirNormalized, uvn, -_POMHeightScale, _DefaultHeightMap);

            //Offset correction
            float offsetCorrectY = offset.y - 0.5f;
            float offsetCorrectX = offset.x - 0.5f;

            //Borders left-top,left-bottom, right-top and right-bottom
            float rotateY60 = offsetCorrectY * sin(1.0 / 6.0 * PI) + offsetCorrectX * cos(1.0 / 6.0 * PI);
            float rotateY120 = offsetCorrectY * sin(5.0 / 6.0 * PI) + offsetCorrectX * cos(5.0 / 6.0 * PI);


            //Mapping the textures on the borders correctly
            if(offset.y < 0.5 - distanceToSide && offsetCorrectX < xNegSide(offsetCorrectY) && offsetCorrectX > xPosSide(offsetCorrectY))
            {
                offset.y = 0.5 + distanceToSide - (0.5 - distanceToSide - offset.y);
                
                data.hexagonPositionOffset.x -= 1;
            }      
            
            if(offset.y > 0.5 + distanceToSide && offsetCorrectX < xPosSide(offsetCorrectY) && offsetCorrectX > xNegSide(offsetCorrectY))
            {
                offset.y = 0.5 - distanceToSide + (offset.y - 0.5 - distanceToSide);
                
                data.hexagonPositionOffset.x += 1;
            }
            

            if(rotateY60 > distanceToSide && offsetCorrectY < yPosSide(offsetCorrectX) && offsetCorrectY > 0)
            {
                offset.x -= 0.75;
                offset.y -= distanceToSide;

                if((uint)data.hexagonPositionOffset.y % 2 == 1)
                {
                    data.hexagonPositionOffset.x += 1;
                    data.hexagonPositionOffset.y += 1;
                }
                else
                {
                    data.hexagonPositionOffset.y += 1;
                }

            }

            if(rotateY60 < - distanceToSide && offsetCorrectY > yPosSide(offsetCorrectX) && offsetCorrectY < 0)
            {
                offset.x += 0.75;
                offset.y += distanceToSide;

                if((uint)data.hexagonPositionOffset.y % 2 == 0)
                {
                    data.hexagonPositionOffset.x -= 1;
                    data.hexagonPositionOffset.y -= 1;
                }
                else
                {
                    data.hexagonPositionOffset.y -= 1;
                }

            }
            
            if(rotateY120 > distanceToSide && offsetCorrectX < xNegSide(offsetCorrectY) && offsetCorrectY > 0)
            {
                offset.x += 0.75;
                offset.y -= distanceToSide;

                if((uint)data.hexagonPositionOffset.y % 2 == 0)
                {
                    data.hexagonPositionOffset.y -= 1;
                }
                else
                {
                    data.hexagonPositionOffset.y -= 1;
                    data.hexagonPositionOffset.x += 1;
                }

            }
            
            if(rotateY120 < - distanceToSide && offsetCorrectX > xNegSide(offsetCorrectY) && offsetCorrectY < 0)
            {
                offset.x -= 0.75;
                offset.y += distanceToSide;

                if((uint)data.hexagonPositionOffset.y % 2 == 1)
                {
                    data.hexagonPositionOffset.y += 1;
                }
                else
                {
                    data.hexagonPositionOffset.y += 1;
                    data.hexagonPositionOffset.x -= 1;
                }

            }


            //Getting the right hexagon.
            
            uint originalPixelVal = _HexagonBuffer[ data.hexagonPositionOffset.y * _ArraySize + data.hexagonPositionOffset.x ];
            uint pixelVal = originalPixelVal << 24;
            pixelVal = pixelVal >> 24;

            uint selected = originalPixelVal << 23;
            selected = selected >> 31;
            

            //Normal
             o.Normal = UnpackNormal(UNITY_SAMPLE_TEX2DARRAY(_NormalMaps, float3(offset, pixelVal)));
             o.Normal = 1.0 - o.Normal;

            //Albedo
            c.rgb = UNITY_SAMPLE_TEX2DARRAY(_AlbedoMaps, float3(offset, pixelVal));                

            //Ambient Occlusion
            float AOAngle = 1.0 - abs(dot(float3(0.0, 1.0, 0.0), viewDirNormalized));
            float AOSample = (UNITY_SAMPLE_TEX2DARRAY(_AmbOccMaps, float3(offset, pixelVal)));

            float HighlightMP = 1.0f;


            if(AOAngle < _AODistance)
                c.rgb *= AOSample;
            else
            {
                if(AOAngle >= _AODistance && AOAngle <= _AODistance + _AODistanceDelta)
                {
                    float val = _AODistance + _AODistanceDelta - AOAngle;
                    c.rgb *= 1 -  (10 * val) * (1.0 - AOSample);
                }
            }

            //Metallic
            o.Metallic = UNITY_SAMPLE_TEX2DARRAY(_MetallMaps, float3(offset, pixelVal));
            
            //Smoothness (Inverted Roughness)
            o.Smoothness = UNITY_SAMPLE_TEX2DARRAY(_GlossyMaps, float3(offset, pixelVal));

            //Highlight
            if(selected == 1)
            {      
                int multiplied = round(tex2D (_MainTex, offset).a); 
                if(multiplied > 0)         
                    c.rgb = _HighlightColor;
            }

            if(data.hexagonPositionOffset.y == 0 && data.hexagonPositionOffset.x == 0) {
                c.rgb = float3(1,1,1);
            }

            
            #endif

            //Need to include the viewDir because of the stupid shader compilation system,
            //it's default path is d3d9, in which case it eliminates the viewDir,
            //So viewDir is required, and thus we do this.
            o.Albedo = c.rgb + float3(1,1,1) * (normalize(IN.viewDir.x) * 0.0001);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}