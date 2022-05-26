using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinEngine.Engine.OpenGL;

namespace WinEngine.Engine
{
    public class Game : GameWindow
    {
        Texture testTex;

        public Game(GameWindowSettings set, NativeWindowSettings nset) : base(set, nset)
        {
            Console.WriteLine("Graphics API: " + nset.API + ", Version: " + nset.APIVersion);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.4f, 0.4f, 0.4f, 1f);

            Render.InitRendering();

            GL.Enable(EnableCap.CullFace);

            testTex = new Texture(new GLImage("test.png"));
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            GL.Viewport(new System.Drawing.Rectangle(0, 0, 1280, 720));
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(0.4f, 0.4f, 0.4f, 1f);

            Rect r = new Rect();
            r.x = 0;
            r.y = 0;
            r.w = (int)(testTex.ImageData.w * 0.2);
            r.h = (int)(testTex.ImageData.h * 0.2);

            Rect src = new Rect();
            src.x = 0;
            src.y = 0;
            src.w = 1;
            src.h = 1;

            Render.PushQuad(r, src, testTex, Render.genericShaderProgram);

            Render.PushBatch();

            base.OnUpdateFrame(e);

            Context.SwapBuffers();

        }
    }
}
