//------------------------------ TEXTURE PROPERTIES ----------------------------
// This is the texture that we're drawing onto the screen.
texture ScreenTexture;
float CellDepth;
int RotationMode;
 
// Our sampler for the incoming texture.
sampler ScreenSampler = sampler_state
{
    Texture = <ScreenTexture>;
};

//------------------------ PIXEL SHADER ----------------------------------------
// Reads the incoming bitmap to draw and encodes depth information in the
// appropriate channel.
float4 PixelShaderFunction(float2 TextureCoordinate : TEXCOORD0) : COLOR0
{
    float4 screenColor = tex2D(ScreenSampler, TextureCoordinate);
    float alpha = screenColor.a;

    // Reset values.
    screenColor.r = 0;
    screenColor.g = 0;
    screenColor.b = 0;
    screenColor.a = 0;

    // Set the value in the channel depending on the rotation.
    if (alpha > 0)
    {
        if (RotationMode == 0)
            screenColor.r = CellDepth;
        else if (RotationMode == 1)
            screenColor.g = CellDepth;
        else if (RotationMode == 2)
            screenColor.b = CellDepth;
        else if (RotationMode == 3)
            screenColor.a = CellDepth;
    }
    
    return screenColor;
}
 
//-------------------------- TECHNIQUES ----------------------------------------
// This technique is pretty simple - only one pass, and only a pixel shader
technique IsometricDepthMap
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}