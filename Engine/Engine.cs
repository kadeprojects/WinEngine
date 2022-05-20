using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
namespace WinEngine.Engine
{
    public class Engine
    {
        public static void EngineLoop()
        {
            using (Game game = new Game(new GameWindowSettings() { IsMultiThreaded = false, RenderFrequency = 60, UpdateFrequency = 60}, new NativeWindowSettings() { Title = "WinEngine Prototype", Size = new Vector2i(1280,720), API = OpenTK.Windowing.Common.ContextAPI.OpenGL, APIVersion = new Version(3,2)}))
            {
                game.Run();
            }
        }
    }
}
