using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return GetRootPath() + "/graphics/" + filename;
        }

        public static String GetSoundPath(String filename)
        {
            return GetRootPath() + "/sounds/" + filename;
        }

        public static String GetMusicPath(String filename)
        {
            return GetRootPath() + "/music/" + filename;
        }
        public static String GetScriptPath(String filename)
        {
            return GetRootPath() + "/scripts/" + filename;
        }
    }
}
