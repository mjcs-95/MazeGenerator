import os
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns

def generar_diagramas_caja(archivo_csv):
    # 1. Validar que el archivo existe
    if not os.path.exists(archivo_csv):
        print(f"Error: El archivo '{archivo_csv}' no se encuentra en el directorio.")
        return

    # 2. Cargar los datos del CSV
    df = pd.read_csv(archivo_csv)
    
    # 3. Definir las características numéricas a graficar
    # Excluimos la columna 'Algorithm' que usaremos para el eje X
    caracteristicas = ['DeadEnds', 'Intersection', 'LongestPath', 'Directness', 'Twistiness']
    
    # 4. Configurar el estilo visual de Seaborn
    sns.set_theme(style="whitegrid")
    
    # 5. Inicializar la cuadrícula de gráficos (2 filas, 3 columnas para albergar las 5 métricas)
    fig, axes = plt.subplots(2, 3, figsize=(18, 11))
    axes = axes.flatten() # Aplanamos la matriz de ejes a 1D para iterar fácilmente

    # 6. Dibujar un diagrama de caja por cada característica
    for i, columna in enumerate(caracteristicas):
        # Creamos el boxplot asignando un color distinto a cada algoritmo automáticamente
        sns.boxplot(ax=axes[i], x='Algorithm', y=columna, data=df, hue='Algorithm', legend=False)
        
        # Estética de cada gráfica individual
        axes[i].set_title(f'Distribución de {columna} por Algoritmo', fontsize=13, fontweight='bold')
        axes[i].set_xlabel('Algoritmo', fontsize=10)
        axes[i].set_ylabel('Porcentaje (%)', fontsize=10)
        
        # Rotamos las etiquetas del eje X para que no se pisen entre sí
        axes[i].set_xticklabels(axes[i].get_xticklabels(), rotation=40, ha='right', fontsize=9)

    # 7. Limpieza: Como tenemos 5 características y espacio para 6 gráficos,
    # eliminamos el último cuadrante que queda vacío.
    fig.delaxes(axes[-1])

    # 8. Ajustar espacios y guardar/mostrar el resultado
    plt.tight_layout()
    
    # Guarda la imagen en alta calidad en el mismo directorio
    nombre_salida = 'boxplots_caracteristicas.png'
    plt.savefig(nombre_salida, dpi=300)
    print(f"¡Éxito! El diagrama de cajas agrupado se ha guardado como '{nombre_salida}'.")
    
    # Muestra el gráfico en pantalla
    plt.show()

# Ejecutar el script apuntando a tu archivo
if __name__ == "__main__":
    generar_diagramas_caja('analisis.csv')