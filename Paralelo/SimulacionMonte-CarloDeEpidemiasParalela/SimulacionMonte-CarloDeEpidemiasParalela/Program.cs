using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;

class Program
{
    static void Main()
    {
        string archivoCSV = "resultados_tiempos.csv";
        File.WriteAllText(archivoCSV, "Nucleos,Tiempo_ms\n");

        int[] hilosAProbar = { 1, 2, 4, 8 };

        foreach (int numCores in hilosAProbar)
        {
            Console.WriteLine($"\n--- INICIANDO PRUEBA CON {numCores} NÚCLEOS ---");

            char[,] mapa = new char[1002, 1002]; 

            for (int i = 1; i <= 1000; i++)
            {
                for (int j = 1; j <= 1000; j++) { mapa[i, j] = 'S'; }
            }

            // Paciente cero en el centro
            mapa[500, 500] = 'I';

            ParallelOptions opciones = new ParallelOptions { MaxDegreeOfParallelism = numCores };
            Stopwatch cronometro = Stopwatch.StartNew();

            for (int dia = 1; dia <= 365; dia++)
            {
                int totalS = 0, totalI = 0, totalR = 0, totalM = 0;
                char[,] mapaManana = new char[1002, 1002];

                Parallel.For(1, 1001, opciones, i =>
                {
                    int filaS = 0, filaI = 0, filaR = 0, filaM = 0;

                    for (int j = 1; j <= 1000; j++)
                    {
                        // REGLA 1: SANOS
                        if (mapa[i, j] == 'S')
                        {
                            if (mapa[i - 1, j] == 'I' || mapa[i + 1, j] == 'I' || mapa[i, j - 1] == 'I' || mapa[i, j + 1] == 'I')
                            {
                                int tiradaContagio = Random.Shared.Next(1, 101);
                                if (tiradaContagio <= 50) { mapaManana[i, j] = 'I'; filaI++; }
                                else { mapaManana[i, j] = 'S'; filaS++; }
                            }
                            else { mapaManana[i, j] = 'S'; filaS++; }
                        }
                        // REGLA 2: INFECTADOS
                        else if (mapa[i, j] == 'I')
                        {
                            int tiradaInfectado = Random.Shared.Next(1, 101);

                            if (tiradaInfectado <= 35) { mapaManana[i, j] = 'R'; filaR++; }
                            else if (tiradaInfectado <= 60) { mapaManana[i, j] = 'M'; filaM++; }
                            else { mapaManana[i, j] = 'I'; filaI++; }
                        }
                        // REGLA 3: RECUPERADOS Y MUERTOS
                        else
                        {
                            mapaManana[i, j] = mapa[i, j];
                            if (mapa[i, j] == 'R') filaR++;
                            else if (mapa[i, j] == 'M') filaM++;
                        }
                    }

                    Interlocked.Add(ref totalS, filaS);
                    Interlocked.Add(ref totalI, filaI);
                    Interlocked.Add(ref totalR, filaR);
                    Interlocked.Add(ref totalM, filaM);
                });

                mapa = mapaManana;

                Console.WriteLine($"Día {dia} ({numCores} Núcleos): {totalS} Sanos | {totalI} Infectados | {totalR} Recuperados | {totalM} Muertos");
            }

            cronometro.Stop();

            string lineaCSV = $"{numCores},{cronometro.ElapsedMilliseconds}\n";
            File.AppendAllText(archivoCSV, lineaCSV);

            Console.WriteLine($"\n--- Tiempo total con {numCores} núcleos: {cronometro.ElapsedMilliseconds} ms ---\n");
        }

        Console.WriteLine($"\n Listo. El archivo '{archivoCSV}' ha sido creado exitosamente.");


        Console.WriteLine("\n--- INICIANDO RENDERIZADO DE IMÁGENES (Tardará unos minutos) ---");

        // Reiniciamos el mapa para la película
        char[,] mapaRender = new char[1002, 1002];
        for (int i = 1; i <= 1000; i++)
        {
            for (int j = 1; j <= 1000; j++) { mapaRender[i, j] = 'S'; }
        }
        mapaRender[500, 500] = 'I';

        ParallelOptions opcionesRender = new ParallelOptions { MaxDegreeOfParallelism = 8 };

        for (int dia = 1; dia <= 365; dia++)
        {
            char[,] mapaManana = new char[1002, 1002];

            Parallel.For(1, 1001, opcionesRender, i =>
            {
                for (int j = 1; j <= 1000; j++)
                {
                    if (mapaRender[i, j] == 'S')
                    {
                        if (mapaRender[i - 1, j] == 'I' || mapaRender[i + 1, j] == 'I' || mapaRender[i, j - 1] == 'I' || mapaRender[i, j + 1] == 'I')
                        {
                            if (Random.Shared.Next(1, 101) <= 50) mapaManana[i, j] = 'I';
                            else mapaManana[i, j] = 'S';
                        }
                        else mapaManana[i, j] = 'S';
                    }
                    else if (mapaRender[i, j] == 'I')
                    {
                        int tirada = Random.Shared.Next(1, 101);
                        if (tirada <= 35) mapaManana[i, j] = 'R';
                        else if (tirada <= 60) mapaManana[i, j] = 'M';
                        else mapaManana[i, j] = 'I';
                    }
                    else
                    {
                        mapaManana[i, j] = mapaRender[i, j];
                    }
                }
            });

            mapaRender = mapaManana;

            // Foto del dia
            GuardarFrame(mapaRender, dia);

            // Un pequeño aviso para no sentir que el programa se ha quedado colgado. Solo cada 10 días.
            if (dia % 10 == 0) Console.WriteLine($"Renderizando fotogramas... {dia}/365 completados.");
        }

        Console.WriteLine("\n RENDERIZADO TERMINADO. Revisa la carpeta 'Frames_Paralelo'.");
    }
    
    #pragma warning disable CA1416 
    static void GuardarFrame(char[,] mapa, int dia)
    {
        if (!Directory.Exists("Frames_Paralelo")) Directory.CreateDirectory("Frames_Paralelo");

        using (Bitmap foto = new Bitmap(1000, 1000))
        {
            for (int i = 1; i <= 1000; i++)
            {
                for (int j = 1; j <= 1000; j++)
                {
                    Color colorPixel = Color.White;

                    if (mapa[i, j] == 'S') colorPixel = Color.LightGray;     // Sanos
                    else if (mapa[i, j] == 'I') colorPixel = Color.Red;      // Infectados
                    else if (mapa[i, j] == 'R') colorPixel = Color.LimeGreen;// Recuperados
                    else if (mapa[i, j] == 'M') colorPixel = Color.Black;    // Muertos

                    // Pintamos el píxel (Le restamos 1 a 'i' y 'j' porque el Bitmap empieza en 0,0)
                    foto.SetPixel(j - 1, i - 1, colorPixel);
                }
            }

            // Guardamos la foto con formato de 3 dígitos (ej. dia_001.png, dia_015.png) 
            // Esto es para que al hacer el GIF se ordenen correctamente.
            foto.Save($"Frames_Paralelo/dia_{dia:D3}.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}