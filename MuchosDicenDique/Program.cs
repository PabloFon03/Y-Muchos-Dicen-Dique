using System;
using System.Windows.Forms;

namespace MuchosDicenDique
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();
            form.Icon = new System.Drawing.Icon("cool.ico");
            Application.Run(form);
        }
    }
}
