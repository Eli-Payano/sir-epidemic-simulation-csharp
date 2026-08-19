# Simulación Espacial SIR (Secuencial vs. Paralelo)

Este proyecto implementa una simulación bidimensional de epidemias basada en el modelo matemático SIR (Susceptibles, Infectados, Recuperados). Su objetivo principal es demostrar la optimización de algoritmos de alto costo computacional a través del multithreading en C#.

## Estructura del Entregable

```text
Proyecto_Epidemia_Final/
├── Secuencial/
│   └── Program.cs          # Código fuente (Algoritmo original un solo hilo)
├── Paralelo/
│   └── Program.cs          # Código optimizado (TPL, Parallel.For, Interlocked)
├── Datos_y_Graficas/
│   ├── resultados_tiempos.csv
│   └── speedup_grafica.png # Gráfica de Strong Scaling
├── Documentacion/
│   └── Informe_Epidemia_Final.pdf # Análisis formal de escalabilidad
└── Animaciones/
    ├── secuencial_brote.mp4
    ├── paralelo_brote.mp4
    └── output_side_by_side.mp4    # Visualización lado a lado
```

## Tecnologías Utilizadas

* **C# (.NET):** Lógica del sistema y motor de concurrencia.
* **Task Parallel Library (TPL):** Paralelización de bucles matriciales.
* **System.Drawing.Common:** Rasterización diaria de fotogramas.
* **FFmpeg:** Composición y encodeado de video H.264 para la animación final.

## Instalación y Uso

1. El entorno requiere **.NET SDK**.
2. Clonar este repositorio y restaurar los paquetes mediante `dotnet restore` para asegurar la inclusión de `System.Drawing.Common`.
3. Para ejecutar cualquier versión, navegue a la carpeta correspondiente e ingrese `dotnet run`.
4. El programa ejecutará las pruebas de estrés automáticamente, exportando el CSV y los fotogramas en su propio directorio.

---
*Desarrollado como proyecto de validación de calidad de software, reducción paralela e ingeniería de rendimiento.*
