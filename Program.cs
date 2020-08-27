using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CM3D2_DLC_CheckerSharp.Properties;
using Microsoft.Win32;

namespace CM3D2_DLC_CheckerSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('=', 101));
            Console.WriteLine("CM3D2_DLC_Checker");
            Console.WriteLine(new string('=', 101));

            RegistryKey registry = Registry.CurrentUser;
            registry = registry.OpenSubKey("SOFTWARE");
            registry = registry.OpenSubKey("KISS");
            registry = registry.OpenSubKey("カスタムメイド3D2");
            string installPath = registry.GetValue("InstallPath", ".").ToString();
            DirectoryInfo cm3d2InstallPath = new DirectoryInfo(installPath);
            if (!cm3d2InstallPath.Exists)
            {
                MessageBox.Show(String.Format("{0} does not exist.", cm3d2InstallPath.FullName));
                return;
            }

            FileInfo cm3d2Exe = new FileInfo(Path.Combine(cm3d2InstallPath.FullName, "CM3D2x64.exe"));
            if (!cm3d2Exe.Exists)
            {
                MessageBox.Show(String.Format("{0} was not found!", cm3d2Exe.FullName));
                return;
            }

            StreamReader lstReader;
            if (File.Exists("CM_ListDLC.lst"))
                lstReader = new StreamReader(File.OpenRead("CM_ListDLC.lst"), Encoding.UTF8);
            else
                lstReader = new StreamReader(new MemoryStream(Resources.CM_ListDLC, false), Encoding.UTF8);
            
            List<DLC> installed = new List<DLC>();
            List<DLC> uninstalled = new List<DLC>();
            while (!lstReader.EndOfStream)
            {
                string cline = lstReader.ReadLine();
                if (!cline.Contains(","))
                    continue;

                args = cline.Split(',');
                DLC dlc = new DLC();
                dlc.filename = args[0];
                dlc.dlcName = args[1];
                dlc.targetFile = new FileInfo(Path.Combine(cm3d2InstallPath.FullName, dlc.filename));

                if (dlc.targetFile.Exists)
                    installed.Add(dlc);
                else
                    uninstalled.Add(dlc);
            }

            installed.Sort((x, y) => x.dlcName.CompareTo(y.dlcName));
            uninstalled.Sort((x, y) => x.dlcName.CompareTo(y.dlcName));

            Console.WriteLine("Already Installed:");
            installed.ForEach(x => Console.WriteLine(x.dlcName));

            Console.WriteLine("\nNot Installed:");
            uninstalled.ForEach(x => Console.WriteLine(x.dlcName));

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Press any key to end program");
            Console.ReadKey();
        }
    }

    class DLC
    {
        public string filename;
        public FileInfo targetFile;
        public string dlcName;

        public override string ToString()
        {
            return dlcName;
        }
    }
}
