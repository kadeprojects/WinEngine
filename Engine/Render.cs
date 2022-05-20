using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using WinEngine.Engine.OpenGL;
using OpenTK.Mathematics;
namespace WinEngine.Engine
{
    public struct Rect
    {
        // cordinates
        public float x = 0, y = 0;
        // width, height
        public float w = 0, h = 0;

        public Vector4 color = new Vector4(255,255,255,1);

        public Rect()
        {
        }
    }

    public struct GLVertex
    {
        public float x, y;
        public float u, v;
        public float r, g, b, a;
    };

    public class Render
    {

        public static string generic_shader_vert = @"(
        #version 150 core
        in vec2 v_position;
        in vec2 v_uv;
        in vec4 v_colour;
        out vec2 f_uv;
        out vec4 f_colour;
        uniform mat4 u_projection;
        void main()
            {
                f_uv = v_uv;
                f_colour = v_colour;
                gl_Position = u_projection * vec4(v_position.xy, 0.0, 1.0);
            }";
        public static string generic_shader_frag = @"
        #version 150 core
        uniform sampler2D u_texture;
        in vec2 f_uv;
        in vec4 f_colour;
        out vec4 o_colour;
        void main()
        {
            o_colour = texture(u_texture, f_uv) * f_colour;
            if (o_colour.a == 0.0)
            {
                discard;
            }
        }";



        public static int genericShaderProgram;

        private static int VertexBufferObject;
        private static int VertexArrayObject;

        public static Matrix4 projectionMatrix;


        public static void InitRendering()
        {
            GL.Enable(EnableCap.VertexArray);

            VertexArrayObject = GL.GenVertexArray();

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            VertexBufferObject = GL.GenBuffer();

            int fragShader;
            int vertShader;

            fragShader = GL.CreateShader(ShaderType.FragmentShader);
            vertShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertShader, generic_shader_vert);
            GL.CompileShader(vertShader);
            GL.ShaderSource(fragShader, generic_shader_frag);
            GL.CompileShader(fragShader);

            genericShaderProgram = GL.CreateProgram();
            GL.AttachShader(genericShaderProgram, vertShader);
            GL.AttachShader(genericShaderProgram, fragShader);

            GL.BindAttribLocation(genericShaderProgram, 0, "v_position");
            GL.BindAttribLocation(genericShaderProgram, 1, "v_uv");
            GL.BindAttribLocation(genericShaderProgram, 2, "v_color");

            GL.MatrixMode(MatrixMode.Projection);
            projectionMatrix = Matrix4.CreateOrthographic(1280, 720, -1.0f, 1.0f);
            GL.LoadMatrix(ref projectionMatrix);

            GL.LinkProgram(genericShaderProgram);


            string log = GL.GetProgramInfoLog(genericShaderProgram);
            Console.WriteLine(log);

            double d = projectionMatrix.M11;

            int location = GL.GetUniformLocation(genericShaderProgram, "u_projection");

            GL.UniformMatrix4(location, 1, false,ref d);
        }

        public static List<GLVertex> batchBuffer = new List<GLVertex>();

        public static int batch_texture;
        public static int batch_shader;

        public static void PushQuad(Rect dst, Rect src, Texture tex, int shader)
        {
            if (batch_texture != tex.TextureId || batch_shader != shader)
            {
                PushBatch();
                batch_texture = tex.TextureId;
                batch_shader = shader;
            }

            GLVertex vert;
            vert.x = dst.x;
            vert.y = dst.y;
            vert.u = src.x;
            vert.v = src.y;
            vert.r = dst.color.X / 255;
            vert.g = dst.color.Y / 255;
            vert.b = dst.color.Z / 255;
            vert.a = dst.color.W;
            GLVertex vert1;
            vert1.x = dst.x;
            vert1.y = dst.y + dst.h;
            vert1.u = src.x;
            vert1.v = src.y + src.h;
            vert1.r = dst.color.X / 255;
            vert1.g = dst.color.Y / 255;
            vert1.b = dst.color.Z / 255;
            vert1.a = dst.color.W;
            GLVertex vert2;
            vert2.x = dst.x + dst.w;
            vert2.y = dst.y;
            vert2.u = src.x + src.w;
            vert2.v = src.y;
            vert2.r = dst.color.X / 255;
            vert2.g = dst.color.Y / 255;
            vert2.b = dst.color.Z / 255;
            vert2.a = dst.color.W;
            GLVertex vert3;
            vert3.x = dst.x + dst.w;
            vert3.y = dst.y + dst.h;
            vert3.u = src.x + src.w;
            vert3.v = src.y + src.h;
            vert3.r = dst.color.X / 255;
            vert3.g = dst.color.Y / 255;
            vert3.b = dst.color.Z / 255;
            vert3.a = dst.color.W;

            batchBuffer.Add(vert);
            batchBuffer.Add(vert1);
            batchBuffer.Add(vert2);
            batchBuffer.Add(vert2);
            batchBuffer.Add(vert1);
            batchBuffer.Add(vert3);
        }

        public unsafe static void PushBatch()
        {
            if (batchBuffer.Count != 0)
            {
                GL.BindTexture(TextureTarget.Texture2D, batch_texture);
                GL.UseProgram(batch_shader);

                GL.BindVertexArray(VertexArrayObject);

                GLVertex[] verts = batchBuffer.ToArray();

                fixed (GLVertex* data = verts)
                {
                    var stride = Marshal.SizeOf<GLVertex>();
                    GL.VertexPointer(2, VertexPointerType.Float, stride,
                        IntPtr.Add(new IntPtr(data), 0));
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, stride,
                        IntPtr.Add(new IntPtr(data), 8));
                    GL.ColorPointer(4, ColorPointerType.Float, stride,
                        IntPtr.Add(new IntPtr(data), 16));

                    GL.DrawArrays(PrimitiveType.Triangles, 0, verts.Length);
                }

                Console.WriteLine(GL.GetError());

                batchBuffer.Clear();
            }
        }
    }
}
