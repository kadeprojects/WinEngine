﻿using OpenTK.Graphics.OpenGL;
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
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0f, 0f, 0f, 1f);

            Render.InitRendering();

            testTex = new Texture(new GLImage("test.png"));
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Rect r = new Rect();
            r.x = 200;
            r.y = 200;
            r.w = testTex.ImageData.w;
            r.h = testTex.ImageData.h;

            Rect src = new Rect();
            src.x = 0;
            src.y = 0;
            src.w = 1;
            src.h = 1;

            Render.PushQuad(r, src, testTex, Render.genericShaderProgram);

            Render.PushBatch();

            Context.SwapBuffers();

            base.OnUpdateFrame(e);
        }
    }
}
