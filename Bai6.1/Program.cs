
using System;
using System.Windows.Forms;
using Bai6._1.GUI;

namespace Bai6._1
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new RetailForm());
        }
    }
}
