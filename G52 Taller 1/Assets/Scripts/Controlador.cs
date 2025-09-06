using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ControladorSimulacion : MonoBehaviour
{
    public string archivoProductosTxt = "productos.txt"; // Archivo con los productos
    public GestorPila pila; // Referencia a la pila

    private List<Producto> productosDisponibles = new List<Producto>();
    private bool simulacionActiva = false;

  
    private int productosGenerados = 0;
    private int productosDespachados = 0;
    private float tiempoInicioSimulacion;
    private float tiempoTotalDespacho = 0f;
    private List<float> tiemposDespacho = new List<float>();
    private Dictionary<string, int> despachadosPorTipo = new Dictionary<string, int>();

    
    private List<string> historialGenerados = new List<string>();

  
    public TMP_InputField inputGenerados; 
    public TMP_Text txtTotalGenerados;   
    public TMP_Text txtDespachados;
    public TMP_Text txtNoDespachados;
    public TMP_Text txtTiempoTotal;
    public TMP_Text txtTiempoTotalDespacho;
    public TMP_Text txtTiempoPromedioDespacho;
    public TMP_Text txtDespachadosPorTipo;
    public TMP_Text txtTipoMasDespachado;

   
    public Button botonIniciar;
    public Button botonDetener;

    void Start()
    {
        CargarProductosDesdeTxt();

        // Conectar botones a sus funciones
        if (botonIniciar != null)
            botonIniciar.onClick.AddListener(IniciarSimulacion);

        if (botonDetener != null)
            botonDetener.onClick.AddListener(CerrarSimulacion);
    }

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
                    int.Parse(datos[0].Substring(2)),
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

    public void IniciarSimulacion()
    {
        if (!simulacionActiva)
        {
            simulacionActiva = true;
            productosGenerados = 0;
            productosDespachados = 0;
            tiempoTotalDespacho = 0f;
            tiemposDespacho.Clear();
            despachadosPorTipo.Clear();
            historialGenerados.Clear();

            tiempoInicioSimulacion = Time.time;

            // Resetear UI
            if (txtTotalGenerados != null)
                txtTotalGenerados.text = "Total generados: 0";
            if (inputGenerados != null)
                inputGenerados.text = "";

            StartCoroutine(GenerarProductos());
            StartCoroutine(DespacharProductos());
        }
    }

    public void CerrarSimulacion()
    {
        simulacionActiva = false;
        float tiempoTotal = Time.time - tiempoInicioSimulacion;

        int noDespachados = productosGenerados - productosDespachados;
        float promedioDespacho = tiemposDespacho.Count > 0 ? tiemposDespacho.Average() : 0f;
        string tipoMasDespachado = despachadosPorTipo.Count > 0 ?
            despachadosPorTipo.OrderByDescending(x => x.Value).First().Key : "Ninguno";

        // Mostrar métricas
        if (txtDespachados != null) txtDespachados.text = "Despachados: " + productosDespachados;
        if (txtNoDespachados != null) txtNoDespachados.text = "No despachados: " + noDespachados;
        if (txtTiempoTotal != null) txtTiempoTotal.text = "Tiempo total: " + tiempoTotal.ToString("F2") + " s";
        if (txtTiempoTotalDespacho != null) txtTiempoTotalDespacho.text = "Tiempo total despacho: " + tiempoTotalDespacho.ToString("F2") + " s";
        if (txtTiempoPromedioDespacho != null) txtTiempoPromedioDespacho.text = "Promedio despacho: " + promedioDespacho.ToString("F2") + " s";
        if (txtDespachadosPorTipo != null) txtDespachadosPorTipo.text = string.Join("\n", despachadosPorTipo.Select(x => x.Key + ": " + x.Value));
        if (txtTipoMasDespachado != null) txtTipoMasDespachado.text = "Más despachado: " + tipoMasDespachado;

        ExportarResultadosJSON();
    }

    IEnumerator GenerarProductos()
    {
        while (simulacionActiva)
        {
            int cantidad = Random.Range(1, 4);
            for (int i = 0; i < cantidad; i++)
            {
                Producto aleatorio = productosDisponibles[Random.Range(0, productosDisponibles.Count)];
                pila.apilar(aleatorio);

                productosGenerados++;

                // Guardar info completa del producto en historial
                string infoProducto = $"ID: {aleatorio.id} | Nombre: {aleatorio.nombre} | Tipo: {aleatorio.tipo} | Peso: {aleatorio.peso} | Precio: {aleatorio.precio} | Tiempo: {aleatorio.tiempo}";
                historialGenerados.Add(infoProducto);

                // Actualizar UI
                if (txtTotalGenerados != null)
                    txtTotalGenerados.text = "Total generados: " + productosGenerados;
                if (inputGenerados != null)
                    inputGenerados.text = string.Join("\n", historialGenerados);
            }
            yield return new WaitForSeconds(2.5f); 
        }
    }

    IEnumerator DespacharProductos()
    {
        while (simulacionActiva)
        {
            if (pila.isPilaVacia())
            {
                Producto p = pila.desapilar();
                productosDespachados++;

                tiempoTotalDespacho += (float)p.tiempo;
                tiemposDespacho.Add((float)p.tiempo);

                if (!despachadosPorTipo.ContainsKey(p.tipo))
                    despachadosPorTipo[p.tipo] = 0;
                despachadosPorTipo[p.tipo]++;

                yield return new WaitForSeconds((float)p.tiempo);
            }
            else
            {
                yield return null;
            }
        }
    }

    void ExportarResultadosJSON()
    {
        List<Producto> resultados = pila.obtenerTodosLosProductos();
        string json = JsonUtility.ToJson(new Wrapper<Producto> { items = resultados }, true);

        string ruta = Path.Combine(Application.dataPath, "resultados.json");
        File.WriteAllText(ruta, json);

        Debug.Log("Resultados exportados a: " + ruta);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
    }
}