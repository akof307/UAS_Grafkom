using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pertemuan1
{
    class Program
    {
        static void Main(string[] args)
        {
            var nativewindowSettings = new NativeWindowSettings()
            {
                Size = new OpenTK.Mathematics.Vector2i(1080, 720),
                Title = "pertemuan 1"
            };
            using (var Window = new Window(GameWindowSettings.Default, nativewindowSettings))
            {
                Window.Run();
            }
        }
    }
}
