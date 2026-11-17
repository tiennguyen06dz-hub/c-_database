using Bai6._2;
using Bai6_2.GUI;
using System;
using System.Windows.Forms;

namespace Bai6_2;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}