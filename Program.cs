using System;
using System.Windows.Forms;

namespace RabinKeyExchange.WinForms {
    static class Program {
        [STAThread]
        static void Main() {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
