using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ProjectAppBackgroundServer
{    
    static class Program
    {
        public static string LOGDIR = Path.GetFullPath("..\\Log\\");

        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!Directory.Exists(Program.LOGDIR))
                Directory.CreateDirectory(Program.LOGDIR);
            
            Application.Run(new ProjectServerApp());
        }
    }
}
