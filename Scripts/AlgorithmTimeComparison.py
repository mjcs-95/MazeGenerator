import os
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

def generar_grafico_tiempos(archivo_csv):
    # 1. Validar que el archivo existe
    if not os.path.exists(archivo_csv):
        print(f"Error: El archivo '{archivo_csv}' no se encuentra.")
        return

    # 2. Cargar los datos
    df = pd.read_csv(archivo_csv)
    
    # Agrupamos por algoritmo y tamaño calculando la media de tiempo
    # (Esto evita problemas si repetiste mediciones para un mismo tamaño)
    df_grouped = df.groupby(['Algorithm', 'Size'])['Time(ms)'].mean().reset_index()

    # 3. Configurar dimensiones y estilo limpio (fondo blanco)
    plt.figure(figsize=(22, 10))
    sns.set_theme(style="white")

    # 4. Crear el gráfico de barras agrupadas usando el tamaño ('Size') como matiz (hue)
    # He seleccionado la paleta 'deep', pero puedes usar 'Set2' o 'muted' si quieres variar los colores
    ax = sns.barplot(
        x='Algorithm', 
        y='Time(ms)', 
        hue='Size', 
        data=df_grouped, 
        palette='deep'
    )

    # 5. CAMBIO CLAVE: Escala Exponencial / Logarítmica para el Eje Y
    ax.set_yscale('log')

    # 6. Añadir las etiquetas numéricas verticales en la parte superior de cada barra
    for bar in ax.patches:
        altura = bar.get_height()
        # Evitamos pintar etiquetas en barras vacías o con valores erróneos
        if altura > 0:
            ax.annotate(
                f'{altura:.2f}',                               # Texto (2 decimales)
                (bar.get_x() + bar.get_width() / 2., altura),  # Coordenadas (X, Y)
                ha='center',                                   # Alineación horizontal
                va='bottom',                                   # Alineación vertical
                rotation=90,                                   # Rotación vertical a 90 grados
                xytext=(0, 6),                                 # Separación en píxeles respecto al borde de la barra
                textcoords='offset points', 
                fontsize=9
            )

    # 7. Personalización de etiquetas y títulos
    ax.set_title('Comparativa de Tiempos de Ejecución por Algoritmo y Tamaño', fontsize=14, fontweight='bold', pad=15)
    ax.set_xlabel('Algorithm', fontsize=12, labelpad=10)
    ax.set_ylabel('Tiempo(ms)', fontsize=12, labelpad=10)
    
    # Ajustar los ticks del eje X para que se vean bien espaciados
    ax.tick_params(axis='x', labelsize=11)

    # 8. Colocar la leyenda de tamaños de forma horizontal en la parte superior (como en tu imagen)
    plt.legend(
        title='Tamaño del Laberinto', 
        loc='upper center', 
        bbox_to_anchor=(0.5, 0.98), 
        ncol=6, 
        frameon=True, 
        fontsize=10
    )

    # 9. Guardar la imagen en alta definición y mostrarla
    plt.tight_layout()
    nombre_imagen = 'TimeComparison_LogScale.png'
    plt.savefig(nombre_imagen, dpi=300)
    print(f"¡Gráfico generado con éxito! Guardado como '{nombre_imagen}'.")
    plt.show()

if __name__ == "__main__":
    generar_grafico_tiempos('Tiempos.csv')