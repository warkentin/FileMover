using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOFAX_Script
{
    class Program
    {
        static void Main(string[] args)
        {            
            // Eingabe von Datumsdifferenzen in Minuten
            int diff = 20, zahl;
            bool canconvert;                        
            DateTime zeit_aktuell, zeit_datei;

            try
            {
                // Eingabe einer Zeitspanne
                if (args.Length > 0)
                {
                    canconvert = int.TryParse(args[0], out zahl);
                    if (canconvert == true) diff = Convert.ToInt32(args[0]);
                }
                diff = Math.Abs(diff) * -1;

                // Eingabe des Quell-Pfads
                string path = @"C:\Temp\KOFAX\";
                if (args.Length > 1) path = args[1].ToString();
                if (path.Substring(path.Length - 1) != @"\") path = path + @"\";

                // Eingabe des Ziel-Pfads
                string path_dest = path + @"\Archiv";
                if (args.Length > 2) path_dest = args[2].ToString();
                if (path_dest.Substring(path_dest.Length - 1) != @"\") path_dest = path_dest + @"\";

                if (Directory.Exists(path) && Directory.Exists(path_dest))
                {
                    Dictionary<string, string> FilesToMove = new Dictionary<string, string>();

                    DirectoryInfo di = new DirectoryInfo(path);
                    //Console.WriteLine(path + " -> " + path_dest);

                    foreach (var fi in di.GetFiles())
                    {                        
                        // Zurückschrauben der Aktuellen Zeit um die eingegebenen Minuten
                        zeit_aktuell = DateTime.Now.AddMinutes(diff);
                        zeit_datei = fi.LastWriteTime;

                        // Betroffene Datei merken
                        if (zeit_datei < zeit_aktuell) FilesToMove.Add(fi.Name, Path.GetFileNameWithoutExtension(fi.Name) + ".lck");
                    }

                    // Betroffene Datei(en) verschieben + LOCK-Datei setzen
                    foreach (KeyValuePair<string, string> pair in FilesToMove)
                    {
                        try
                        {
                            System.IO.File.Create(path_dest + pair.Value).Dispose();
                            System.IO.File.Move(path + pair.Key, path_dest + pair.Key);
                        }
                        catch (Exception err) { }
                    }

                    System.Threading.Thread.Sleep(5000);
                    // LCK-Datei(en) wieder löschen
                    DirectoryInfo did = new DirectoryInfo(path_dest);
                    foreach (var fid in did.GetFiles())
                    {
                        if (fid.Name.Substring(fid.Name.Length - 4) == ".lck") System.IO.File.Delete(path_dest + fid.Name);
                    }
                    if (System.IO.File.Exists(path + "thumbs.db")) System.IO.File.Delete(path + "thumbs.db");
                }
                else
                {
                    // nix!
                }
            }
            catch (Exception err) { }
        }
    }
}