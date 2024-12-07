using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sys = Cosmos.System;
using Cosmos.System.Graphics;
using System.Drawing;
using Cosmos.System.Graphics.Fonts;
using System.Diagnostics;
using IL2CPU.API.Attribs;
using Cosmos.HAL.Drivers.Audio;
using Cosmos.System.Audio.IO;
using Cosmos.System.Audio;
using Cosmos.HAL.Audio;



namespace CosmosKernel1
{
    public class Kernel : Sys.Kernel
    {
        //[ManifestResourceStream(ResourceName = "LZW_OS.macarena.wav")] public static byte[] music;

        Canvas canvas;

        static string ASC16Base64 = "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AAB4zMzM2MzGxsbMAAAAAAAA/sbGwMDAwMDAwAAAAAAAAAAA/mxsbGxsbGwAAAAAAAAA/sZgMBgwYMb+AAAAAAAAAAAAftjY2NjYcAAAAAAAAAAAZmZmZmZ8YGDAAAAAAAAAAHbcGBgYGBgYAAAAAAAAAH4YPGZmZjwYfgAAAAAAAAA4bMbG/sbGbDgAAAAAAAA4bMbGxmxsbGzuAAAAAAAAHjAYDD5mZmZmPAAAAAAAAAAAAH7b29t+AAAAAAAAAAAAAwZ+29vzfmDAAAAAAAAAHDBgYHxgYGAwHAAAAAAAAAB8xsbGxsbGxsYAAAAAAAAAAP4AAP4AAP4AAAAAAAAAAAAYGH4YGAAA/wAAAAAAAAAwGAwGDBgwAH4AAAAAAAAADBgwYDAYDAB+AAAAAAAADhsbGBgYGBgYGBgYGBgYGBgYGBgYGNjY2HAAAAAAAAAAABgYAH4AGBgAAAAAAAAAAAAAdtwAdtwAAAAAAAAAOGxsOAAAAAAAAAAAAAAAAAAAAAAAABgYAAAAAAAAAAAAAAAAAAAAGAAAAAAAAAAADwwMDAwM7GxsPBwAAAAAANhsbGxsbAAAAAAAAAAAAABw2DBgyPgAAAAAAAAAAAAAAAAAfHx8fHx8fAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";
        static MemoryStream ASC16FontMS = new MemoryStream(Convert.FromBase64String(ASC16Base64));

        public void DrawACSIIString(Canvas canvas, Color color, string s, int x, int y, int scale)
        {
            string[] lines = s.Split('\n');
            for (int l = 0; l < lines.Length; l++)
            {
                for (int c = 0; c < lines[l].Length; c++)
                {
                    int offset = (Encoding.ASCII.GetBytes(lines[l][c].ToString())[0] & 0xFF) * 16;
                    ASC16FontMS.Seek(offset, SeekOrigin.Begin);
                    byte[] fontbuf = new byte[16];
                    ASC16FontMS.Read(fontbuf, 0, fontbuf.Length);

                    for (int i = 0; i < 16 * scale; i++)
                    {
                        for (int j = 0; j < 8 * scale; j++)
                        {
                            if ((fontbuf[i / scale] & (0x80 >> (j / scale))) != 0)
                            {
                                canvas.DrawPoint(color, (x + j) + (c * 8 * scale), y + i + (l * 16 * scale));
                            }
                        }
                    }
                }
            }
        }

        private readonly Sys.Graphics.Bitmap bitmap = new Sys.Graphics.Bitmap(10, 10,
                new byte[] { 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0,
                    255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255,
                    0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255,
                    0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 23, 59, 88, 255,
                    23, 59, 88, 255, 0, 255, 243, 255, 0, 255, 243, 255, 23, 59, 88, 255, 23, 59, 88, 255, 0, 255, 243, 255, 0,
                    255, 243, 255, 0, 255, 243, 255, 23, 59, 88, 255, 153, 57, 12, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255,
                    243, 255, 0, 255, 243, 255, 153, 57, 12, 255, 23, 59, 88, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243,
                    255, 0, 255, 243, 255, 0, 255, 243, 255, 72, 72, 72, 255, 72, 72, 72, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0,
                    255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 72, 72,
                    72, 255, 72, 72, 72, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255,
                    10, 66, 148, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255,
                    243, 255, 10, 66, 148, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 10, 66, 148, 255, 10, 66, 148, 255,
                    10, 66, 148, 255, 10, 66, 148, 255, 10, 66, 148, 255, 10, 66, 148, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255,
                    243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 10, 66, 148, 255, 10, 66, 148, 255, 10, 66, 148, 255, 10, 66, 148,
                    255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255,
                    0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, 0, 255, 243, 255, }, ColorDepth.ColorDepth32);

        Sys.FileSystem.CosmosVFS fs = new Cosmos.System.FileSystem.CosmosVFS();


        protected override void BeforeRun()
        {
            //var mixer = new AudioMixer();
            //var audioStream = MemoryAudioStream.FromWave(music);
            //var audioStream = new MemoryAudioStream(new SampleFormat(AudioBitDepth.Bits16, 2, true), 48000, music);
            //var driver = AC97.Initialize(bufferSize: 4096);
            //mixer.Streams.Add(audioStream);

            //var audioManager = new AudioManager()
            //{
            //Stream = mixer,
            //Output = driver
            //};
            //audioManager.Enable();

            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            Sys.KeyboardManager.SetKeyLayout(new Sys.ScanMaps.ESStandardLayout());
            Console.WriteLine("Cosmos booted successfully. Let's go in Graphical Mode");

            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1920, 1080, ColorDepth.ColorDepth32));
            canvas.Clear(Color.Black);
        }
        protected override void Run()
        {
            // Mostrar missatge de benvinguda
            int conty = 0;
            int contx = 5;
            int xfix = contx;

            canvas.Clear(Color.Black);
            DrawACSIIString(canvas, Color.Green, "LZW_OS", 10, conty += 100, 4);

            var available_space = fs.GetAvailableFreeSpace(@"0:");
            DrawACSIIString(canvas, Color.Green, "Available Free Space", 10, conty += 70, 2);

            var fs_type = fs.GetFileSystemType(@"0:\");
            DrawACSIIString(canvas, Color.Green, "File System Type: " + fs_type, 10, conty += 35, 2);

            var files_list = Directory.GetFiles(@"0:\");
            canvas.Display();

            foreach (var file in files_list)
            {
                DrawACSIIString(canvas, Color.Green, fs_type, 10, conty += 35, 2);


            }
            DrawACSIIString(canvas, Color.Green, "Presiona cualquier tecla para seguir", 10, conty += 60, 2);
            canvas.Display();
            Console.ReadKey(true);
            canvas.Clear(Color.Black);


            while (true)
            {
                string input = "";
                while (true)
                {
                    canvas.Clear(Color.Black);
                    Console.Clear();
                    DrawACSIIString(canvas, Color.Green, "Escribe el nombre del comando: " + input, 10, 10, 2);
                    canvas.Display();
                    canvas.Clear(Color.Black);

                    var keyInfo = Console.ReadKey(intercept: true);

                    if (keyInfo.Key == ConsoleKey.Enter)
                        break;

                    if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input = input.Substring(0, input.Length - 1);
                    }
                    else if (keyInfo.Key != ConsoleKey.Backspace)
                    {
                        input += keyInfo.KeyChar;
                    }
                }
                canvas.Clear(Color.Black);
                switch (input.ToLower())
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "about":
                        ShowAbout();
                        break;
                    case "reboot":
                        RebootSystem();
                        break;
                    case "shutdown":
                        ShutDownSystem();
                        break;
                    case "gestionar fitxers":
                        GestionarFitxers();
                        break;
                    default:
                        DrawACSIIString(canvas, Color.Green, "Comanda desconeguda. Prova amb 'help' per veure les opcions: ", 10, conty += 35, 2);
                        canvas.Display();

                        break;
                }
            }
        }

        private void ShowHelp()
        {
            canvas.Clear(Color.Black);

            int conty = 0;
            DrawACSIIString(canvas, Color.Green, "Comandes disponibles: ", 10, conty += 35, 2);
            DrawACSIIString(canvas, Color.Green, "help - Mostra aquesta ajuda: ", 10, conty += 35, 2);
            DrawACSIIString(canvas, Color.Green, "about - Mostra informació sobre el sistema: ", 10, conty += 35, 2);
            DrawACSIIString(canvas, Color.Green, "reboot - Reinicia el sistema: ", 10, conty += 35, 2);
            DrawACSIIString(canvas, Color.Green, "shutdown - Apaga el sistema: ", 10, conty += 35, 2);
            DrawACSIIString(canvas, Color.Green, "gestionar fitxers - Crear, modificar o veure fitxers i carpetes: ", 10, conty += 35, 2);
            DrawACSIIString(canvas, Color.Green, "Presiona cualquier tecla para seguir", 10, conty += 60, 2);
            canvas.Display();
            Console.ReadKey(true);
            canvas.Clear(Color.Black);
        }

        private void ShowAbout()
        {
            canvas.Clear(Color.Black);

            int conty = 0;
            DrawACSIIString(canvas, Color.Green, "Sistema Operatiu creat amb Cosmos Kernel ", 10, conty += 35, 2);
            DrawACSIIString(canvas, Color.Green, "Versió 1.0 ", 10, conty += 35, 2);
            DrawACSIIString(canvas, Color.Green, "Desenvolupat per [El teu nom] ", 10, conty += 35, 2);
            DrawACSIIString(canvas, Color.Green, "Presiona cualquier tecla para seguir", 10, conty += 60, 2);
            canvas.Display();
            Console.ReadKey(true);
            canvas.Clear(Color.Black);
        }

        private void RebootSystem()
        {
            canvas.Clear(Color.Black);

            int conty = 0;
            DrawACSIIString(canvas, Color.Green, "Reiniciant el sistema... ", 10, conty += 35, 2);
            canvas.Display();

            Cosmos.System.Power.Reboot();

        }

        private void ShutDownSystem()
        {
            canvas.Clear(Color.Black);

            int conty = 0;
            DrawACSIIString(canvas, Color.Green, "Apagant el sistema... ", 10, conty += 35, 2);
            canvas.Display();

            Cosmos.System.Power.Shutdown();
        }

        private void GestionarFitxers()
        {
            canvas.Clear(Color.Black);

            int conty = 0;
            DrawACSIIString(canvas, Color.Green, "Que vols gestionar? (Fitxer o carpeta):", 10, conty += 35, 2);
            string tipus = Console.ReadLine()?.ToLower();
            canvas.Display();


            switch (tipus)
            {
                case "fitxer":
                    GestionarFitxer();
                    break;
                case "carpeta":
                    GestionarCarpeta();
                    break;
                default:
                    DrawACSIIString(canvas, Color.Green, "Opció desconeguda. Prova amb 'fitxer' o 'carpeta'.", 10, conty += 35, 2);
                    canvas.Display();

                    break;
            }
        }

        private void GestionarFitxer()
        {
            canvas.Clear(Color.Black);

            int conty = 0;
            DrawACSIIString(canvas, Color.Green, "Nom del fitxer (amb extensió, p. ex. document.txt):", 10, conty += 35, 2);
            canvas.Display();

            string nomFitxer = Console.ReadLine();

            string ruta = @"0:\" + nomFitxer;

            if (File.Exists(ruta))
            {
                DrawACSIIString(canvas, Color.Green, "El fitxer ja existeix. Vols veure el contingut? (sí/no):", 10, conty += 35, 2);
                canvas.Display();

                string veureContingut = Console.ReadLine()?.ToLower();

                if (veureContingut == "sí" || veureContingut == "si")
                {
                    string contingut = File.ReadAllText(ruta);
                    DrawACSIIString(canvas, Color.Green, "Contingut del fitxer:", 10, conty += 35, 2);
                    DrawACSIIString(canvas, Color.Green, "---------------------------------", 10, conty += 35, 2);
                    DrawACSIIString(canvas, Color.Green, contingut, 10, conty += 35, 2);
                    DrawACSIIString(canvas, Color.Green, "---------------------------------", 10, conty += 35, 2);
                    canvas.Display();


                }
                DrawACSIIString(canvas, Color.Green, "Vols (1) modificar o (2) afegir contingut? [1/2]:", 10, conty += 35, 2);
                string accio = Console.ReadLine();
                canvas.Display();

                if (accio == "1")
                {
                    DrawACSIIString(canvas, Color.Green, "Vols (1) modificar o (2) afegir contingut? [1/2]:", 10, conty += 35, 2);

                    Console.WriteLine("Escriu el nou contingut per substituir l'existent:");
                    string nouContingut = Console.ReadLine();
                    canvas.Display();

                    File.WriteAllText(ruta, nouContingut);
                    DrawACSIIString(canvas, Color.Green, "El fitxer s'ha modificat correctament.", 10, conty += 35, 2);
                    canvas.Display();

                }
                else if (accio == "2")
                {
                    DrawACSIIString(canvas, Color.Green, "Escriu el contingut per afegir al fitxer:", 10, conty += 35, 2);
                    canvas.Display();

                    string afegirContingut = Console.ReadLine();
                    File.AppendAllText(ruta, afegirContingut + Environment.NewLine);
                    DrawACSIIString(canvas, Color.Green, "El contingut s'ha afegit correctament.", 10, conty += 35, 2);
                    canvas.Display();

                }
                else
                {
                    DrawACSIIString(canvas, Color.Green, "Opció no vàlida.", 10, conty += 35, 2);
                    canvas.Display();

                }
            }
            else
            {
                DrawACSIIString(canvas, Color.Green, "El fitxer no existeix. Escriu el contingut per crear-lo:", 10, conty += 35, 2);
                string contingut = Console.ReadLine();
                canvas.Display();

                File.WriteAllText(ruta, contingut);
                DrawACSIIString(canvas, Color.Green, "El fitxer s'ha creat correctament.", 10, conty += 35, 2);
                canvas.Display();

            }
        }

        private void GestionarCarpeta()
        {
            canvas.Clear(Color.Black);

            int conty = 0;
            DrawACSIIString(canvas, Color.Green, "Nom de la carpeta:", 10, conty += 35, 2);
            string nomCarpeta = Console.ReadLine();
            canvas.Display();


            string ruta = @"0:\" + nomCarpeta;

            if (Directory.Exists(ruta))
            {
                DrawACSIIString(canvas, Color.Green, "La carpeta ja existeix. No cal crear-la de nou.", 10, conty += 35, 2);
                canvas.Display();

            }
            else
            {
                Directory.CreateDirectory(ruta);
                DrawACSIIString(canvas, Color.Green, "La carpeta s'ha creat correctament.", 10, conty += 35, 2);
                canvas.Display();

            }
        }
    }
}