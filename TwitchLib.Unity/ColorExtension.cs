using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchLib.Unity
{
    public static class ColorExtension
    {
        public static UnityEngine.Color ToUnity(this System.Drawing.Color color)
        {
            return new UnityEngine.Color(color.R, color.G, color.B, color.A);
        }
    }
}
