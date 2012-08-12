using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using System;
using Protogame;

namespace Tychaia.Content.Extensions
{
    /// <summary>
    /// This is an alternate texture importer that does not require DirectX, sourced from
    /// http://theinstructionlimit.com/a-fast-2d-texture-contentimporter-using-devilnet.  We
    /// use this to import textures since the build server does not have DirectX available.
    /// </summary>
    [ContentImporter(".bmp", ".cut", ".dcx", ".dds", ".ico", ".gif", ".jpg", ".lbm", ".lif", ".mdl", ".pcd", ".pcx", ".pic", ".png", ".pnm", ".psd", ".psp", ".raw", ".sgi", ".tga", ".tif", ".wal", ".act", ".pal",
        DisplayName = "DevIL.NET Texture Importer", DefaultProcessor = "TextureProcessor")]
    public class FastTextureImporter : ContentImporter<Texture2DContent>
    {
        public override Texture2DContent Import(string filename, ContentImporterContext context)
        {
            var content = new Texture2DContent
            {
                Identity = new ContentIdentity(new FileInfo(filename).FullName, "DevIL.NET Texture Importer")
            };

            using (var bitmap = DevIL.DevIL.LoadBitmap(filename))
            {
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                                 ImageLockMode.ReadOnly, bitmap.PixelFormat);

                if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                    throw new InvalidContentException();

                int byteCount = bitmapData.Stride * bitmap.Height;
                var bitmapBytes = new byte[byteCount];
                Marshal.Copy(bitmapData.Scan0, bitmapBytes, 0, byteCount);

                bitmap.UnlockBits(bitmapData);

                // Swap red / blue.
                for (int i = 0; i < byteCount; i += 4)
                {
                    // Input format:
                    // BGRA
                    // Output format:
                    // RGBA

                    byte t = bitmapBytes[i + 0];
                    bitmapBytes[i + 0] = bitmapBytes[i + 2];
                    bitmapBytes[i + 2] = t;
                }

                var bitmapContent = new PixelBitmapContent<Color>(bitmap.Width, bitmap.Height);
                bitmapContent.SetPixelData(bitmapBytes);
                content.Mipmaps.Add(bitmapContent);
            }

            content.Validate(null);
            return content;
        }
    }
}