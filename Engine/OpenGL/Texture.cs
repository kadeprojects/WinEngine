using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace WinEngine.Engine.OpenGL
{
    public class GLImage
    {
        public List<byte> PixelData;
        public int w, h;

        public GLImage(string path)
        {
            Image<Rgba32> image = Image.Load<Rgba32>(path);
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            w = image.Width;
            h = image.Height;

            var pixels = new List<byte>(4 * image.Width * image.Height);

            image.ProcessPixelRows(accessor =>
            {
                Rgba32 transparent = Color.Transparent;

                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

                    for (int x = 0; x < pixelRow.Length; x++)
                    {
                        ref Rgba32 pixel = ref pixelRow[x];
                        pixels.Add(pixel.R);
                        pixels.Add(pixel.G);
                        pixels.Add(pixel.B);
                        pixels.Add(pixel.A);
                    }
                }
            });

            PixelData = pixels;
        }
    }

    public class Texture
    {
        public int TextureId;

        public GLImage ImageData;

        public Texture(GLImage image)
        {
            ImageData = image;

            GL.GenTextures(1, out TextureId);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ImageData.w, ImageData.h, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.PixelData.ToArray());
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
            GL.Enable(EnableCap.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}
