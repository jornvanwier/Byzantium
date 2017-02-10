float remap (float value, float from1, float to1, float from2, float to2) {
    return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
}

float2 ParallaxMapping(float3 viewDir, float sampledHeight, float heightScale)
{
    return viewDir.xz / viewDir.y * (sampledHeight * heightScale * heightScale);
}



float yPosSide(float x)
{
    return sqrt(3) * x;
}

float yNegSide(float x)
{
    return yPosSide(x) * -1;
}

float xPosSide(float y)
{
    return y / sqrt(3);
}

float xNegSide(float y)
{
    return xPosSide(y) * -1;
}

HexagonData CalculateMapCoords(float2 uv, int array_size, float hexSize)
{
    float posX = uv.x * (array_size) - 0.075 * array_size;
    float posY = uv.y * (array_size) - 0.005 * array_size;

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

    HexagonData data;

    data.hexagonPositionOffset = int2(x,z);
    data.hexagonPositionCubical = int3(rX,rZ,rY);
    data.hexagonPositionFloat = float2(posX, posY);

    return data;
}

float2 ParallaxOcclusionMapping(float3 viewDir, float2 texCoords, float heightScale, sampler2D parallaxMap)
{
    static const float minLayers = 10;
    static const float maxLayers = 20;
    float numLayers = lerp(maxLayers, minLayers, abs(dot(float3(0.0, 0.0, 1.0), viewDir)));

    float layerDepth = 1.0 / numLayers;
    static float currentLayerDepth = 0.0;

    float2 P = viewDir.xz / viewDir.y * heightScale;
    float2 deltaTexCoords = P / numLayers;

    float2  currentTexCoords     = texCoords;
    float currentDepthMapValue   = tex2D(parallaxMap, currentTexCoords).r;

     while(currentLayerDepth < currentDepthMapValue)
     {
         currentTexCoords -= deltaTexCoords;

         currentDepthMapValue = tex2Dlod(parallaxMap, float4(currentTexCoords,0,0)).r;

         currentLayerDepth += layerDepth;
     }

    float2 prevTexCoords = currentTexCoords + deltaTexCoords;

    float afterDepth  = currentDepthMapValue - currentLayerDepth;
    float beforeDepth = tex2D(parallaxMap, prevTexCoords).r - currentLayerDepth + layerDepth;

    float weight = afterDepth / (afterDepth - beforeDepth);
    float2 finalTexCoords = prevTexCoords * weight + currentTexCoords * (1.0 - weight);

    return finalTexCoords;
}