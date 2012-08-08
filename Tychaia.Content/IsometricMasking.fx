//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that we're drawing onto the screen.
texture ScreenTexture;
texture DepthMapTexture;
float2 DepthMapSize;
float EntityDepth;
bool IgnoreDepth;
int RotationMode;
float2 Offset;
float2 Size;
 
// Our sampler for the texture, which is just going to be pretty simple
sampler ScreenSampler = sampler_state
{
    Texture = <ScreenTexture>;
    AddressU = Wrap;
    AddressV = Wrap;
    AddressW = Wrap;
};
sampler DepthMapSampler = sampler_state
{
    Texture = <DepthMapTexture>;
    AddressU = Wrap;
    AddressV = Wrap;
    AddressW = Wrap;
};
 
//------------------------ PIXEL SHADER ----------------------------------------
// This pixel shader looks up the value in the depth map and compares it to the
// current entity depth, determining whether or not each pixel should be obscured.
float4 PixelShaderFunction(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
    // The screen texture coordinate is between 0 and 1 on each dimension, representing
    // each "end" of the axis for the screen texture.
    float4 screenColor = tex2D(ScreenSampler, TextureCoordinate);

    // We need to convert the texture coordinate of the screen texture to the appropriate
    // values for the depth map using the Size and Offset parameters.
    float2 depthMapCoordinate;
    depthMapCoordinate.x = (Offset.x + (TextureCoordinate.x * Size.x)) / DepthMapSize.x;
    depthMapCoordinate.y = (Offset.y + (TextureCoordinate.y * Size.y)) / DepthMapSize.y;
    float4 depthMapColor = tex2D(DepthMapSampler, depthMapCoordinate);

    // Retrieve the depth map value from the appropriate channel.
    float depthMapResult = 0;
    if (RotationMode == 0)
        depthMapResult = depthMapColor.r;
    else if (RotationMode == 1)
        depthMapResult = depthMapColor.g;
    else if (RotationMode == 2)
        depthMapResult = depthMapColor.b;
    else if (RotationMode == 3)
        depthMapResult = depthMapColor.a;

    // If the current entity depth is greater than the depth map value,
    // return the entity pixel, otherwise transparent.
    if (EntityDepth >= depthMapResult || IgnoreDepth)
        return screenColor;
    else
    {
        screenColor.r = 0;
        screenColor.g = 0;
        screenColor.b = 0;
        screenColor.a = 0;
        return screenColor;
    }
}
 
//-------------------------- TECHNIQUES ----------------------------------------
// This technique is pretty simple - only one pass, and only a pixel shader
technique IsometricMasking
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}