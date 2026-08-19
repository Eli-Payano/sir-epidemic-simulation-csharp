using System.Diagnostics;
using System;
using System.IO;
using System.Drawing;

class Program
{
    static void Main()
    {
        char[,] mapa = new char[1002, 1002];
        Random dado = new Random();

        // Inicializar a todos como Sanos ('S')
        for (int i = 1; i <= 1000; i++)
        {
            for (int j = 1; j <= 1000; j++)
            {
                mapa[i, j] = 'S';
            }
        }

        // Paciente cero en el centro
        mapa[500, 500] = 'I';

        // Iniciamos el cronómetro para el reporte secuencial
        Stopwatch cronometro = Stopwatch.StartNew();

        for (int dia = 1; dia <= 365; dia++)
        {
            int totalS = 0;
            int totalI = 0;
            int totalR = 0;
            int totalM = 0;

            char[,] mapaManana = new char[1002, 1002];

            for (int i = 1; i <= 1000; i++)
            {
                for (int j = 1; j <= 1000; j++)
                {
                    // REGLA 1: SANOS
                    if (mapa[i, j] == 'S')
                    {
                        if (mapa[i - 1, j] == 'I' || mapa[i + 1, j] == 'I' || mapa[i, j - 1] == 'I' || mapa[i, j + 1] == 'I')
                        {
                            int tiradaContagio = dado.Next(1, 101);
                            if (tiradaContagio <= 50) // 50% de contagio
                            {
                                mapaManana[i, j] = 'I';
                                totalI++;
                            }
                            else
                            {
                                mapaManana[i, j] = 'S';
                                totalS++;
                            }
                        }
                        else
                        {
                            mapaManana[i, j] = 'S';
                            totalS++;
                        }
                    }
                    // REGLA 2: INFECTADOS
                    else if (mapa[i, j] == 'I')
                    {
                        int tiradaInfectado = dado.Next(1, 101);

                        if (tiradaInfectado <= 35) // 35% se recuperan
                        {
                            mapaManana[i, j] = 'R';
                            totalR++;
                        }
                        else if (tiradaInfectado <= 60) // 25% mueren
                        {
                            mapaManana[i, j] = 'M';
                            totalM++;
                        }
                        else // Se quedan infectados
                        {
                            mapaManana[i, j] = 'I';
                            totalI++;
                        }
                    }
                    // REGLA 3: RECUPERADOS Y MUERTOS
                    else
                    {
                        mapaManana[i, j] = mapa[i, j];
                        if (mapa[i, j] == 'R') totalR++;
                        else if (mapa[i, j] == 'M') totalM++;
                    }
                }
            }

            mapa = mapaManana;

            Console.WriteLine($"Día {dia}: {totalS} Sanos | {totalI} Infectados | {totalR} Recuperados | {totalM} Muertos");
        }

        cronometro.Stop();
        Console.WriteLine($"\nTiempo total secuencial: {cronometro.ElapsedMilliseconds} ms");


        Console.WriteLine("\n--- INICIANDO RENDERIZADO SECUENCIAL ---");
        char[,] mapaRender = new char[1002, 1002];
        for (int i = 1; i <= 1000; i++)
        {
            for (int j = 1; j <= 1000; j++) { mapaRender[i, j] = 'S'; }
        }
        mapaRender[500, 500] = 'I';

        for (int dia = 1; dia <= 365; dia++)
        {
            char[,] mapaManana = new char[1002, 1002];

            // BUCLES SECUENCIALES 
            for (int i = 1; i <= 1000; i++)
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
            }
            mapaRender = mapaManana;
            GuardarFrame(mapaRender, dia);
            if (dia % 10 == 0) Console.WriteLine($"Renderizando fotogramas... {dia}/365");
        }
        Console.WriteLine("\n RENDERIZADO TERMINADO. Revisa la carpeta 'Frames_Secuencial'.");

    }
    
    #pragma warning disable CA1416
    static void GuardarFrame(char[,] mapa, int dia)
    {
        if (!Directory.Exists("Frames_Secuencial")) Directory.CreateDirectory("Frames_Secuencial");
        
        using (Bitmap foto = new Bitmap(1000, 1000))
        {
            for (int i = 1; i <= 1000; i++)
            {
                for (int j = 1; j <= 1000; j++)
                {
                    Color colorPixel = Color.White;
                    if (mapa[i, j] == 'S') colorPixel = Color.LightGray;
                    else if (mapa[i, j] == 'I') colorPixel = Color.Red;
                    else if (mapa[i, j] == 'R') colorPixel = Color.LimeGreen;
                    else if (mapa[i, j] == 'M') colorPixel = Color.Black;
                    
                    foto.SetPixel(j - 1, i - 1, colorPixel); 
                }
            }
            foto.Save($"Frames_Secuencial/dia_{dia:D3}.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }

}