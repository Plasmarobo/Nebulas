using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Nebulas
{
    public class Config
    {

        protected static String GetRootPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static String GetGraphicPath(String filename)
        {
            return Path.Combine(GetRootPath(), "graphics", filename);
        }

        public static String GetSoundPath(String filename)
        {
            return Path.Combine(GetRootPath(),"sounds", filename);
        }

        public static String GetMusicPath(String filename)
        {
            return Path.Combine(GetRootPath(),"music",filename);
        }
        public static String GetScriptPath(String filename)
        {
            return Path.Combine(GetRootPath(),"scripts", filename);
        }
    }
}
