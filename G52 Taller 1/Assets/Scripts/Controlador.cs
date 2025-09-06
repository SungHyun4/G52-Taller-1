using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ControladorSimulacion : MonoBehaviour
{
    public string archivoProductosTxt = "productos.txt"; // Archivo con productos
    public GestorPila pila; // Aquí se referencia tu GestorPila

    private List<Producto> productosDisponibles = new List<Producto>();
    private bool simulacionActiva = false;

    void Start()
    {
        CargarProductosDesdeTxt();
    }

    // Cargar productos desde archivo .txt
    void CargarProductosDesdeTxt()
    {
        string ruta = Path.Combine(Application.dataPath, archivoProductosTxt);

        if (!File.Exists(ruta))
        {
            Debug.LogError("No se encontró el archivo: " + ruta);
            return;
        }

        string[] lineas = File.ReadAllLines(ruta);
        foreach (string linea in lineas)
        {
            string[] datos = linea.Split('|');
            if (datos.Length == 6)
            {
                Producto p = new Producto(
                    int.Parse(datos[0].Substring(2)), // quitar "P-"
                    datos[1],
                    datos[2],
                    float.Parse(datos[3]),
                    float.Parse(datos[4]),
                    int.Parse(datos[5])
                );
                productosDisponibles.Add(p);
            }
        }

        Debug.Log("Productos cargados: " + productosDisponibles.Count);
    }

    // Botón "Iniciar"
    public void IniciarSimulacion()
    {
        if (!simulacionActiva)
        {
            simulacionActiva = true;
            StartCoroutine(GenerarProductos());
            StartCoroutine(DespacharProductos());
        }
    }

    // Botón "Cerrar Interacción"
    public void CerrarSimulacion()
    {
        simulacionActiva = false;
        ExportarResultadosJSON();
    }

    // Corrutina: genera productos aleatoriamente
    IEnumerator GenerarProductos()
    {
        while (simulacionActiva)
        {
            int cantidad = Random.Range(1, 4); // 1 a 3 productos
            for (int i = 0; i < cantidad; i++)
            {
                Producto aleatorio = productosDisponibles[Random.Range(0, productosDisponibles.Count)];
                pila.apilar(aleatorio);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // Corrutina: despacha productos desde el tope
    IEnumerator DespacharProductos()
    {
        while (simulacionActiva)
        {
            if (pila.isPilaVacia())
            {
                Producto p = pila.desapilar();
                yield return new WaitForSeconds((float)p.tiempo);

            }
            else
            {
                yield return null;
            }
        }
    }

    // Guardar resultados en JSON (usando JsonUtility)
    void ExportarResultadosJSON()
    {
        List<Producto> resultados = pila.obtenerTodosLosProductos();
        string json = JsonUtility.ToJson(new Wrapper<Producto> { items = resultados }, true);

        string ruta = Path.Combine(Application.dataPath, "resultados.json");
        File.WriteAllText(ruta, json);

        Debug.Log("Resultados exportados a: " + ruta);
    }

    // Clase auxiliar porque JsonUtility no serializa listas directamente
    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
    }
}
