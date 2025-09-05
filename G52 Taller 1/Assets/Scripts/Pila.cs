using System.Collections.Generic;
using UnityEngine;

public class GestorPila : MonoBehaviour
{
    [Header("Configuración de la Pila")]
    public int capacidadMaxima = 100;

    private Stack<Producto> pilaProductos;

    // Getters
    public int getTamanioActual()
    {
        return pilaProductos.Count;
    }

    public Producto getProductoEnTope()
    {
        return pilaProductos.Count > 0 ? pilaProductos.Peek() : null;
    }

    public bool isPilaVacia()
    {
        return pilaProductos.Count == 0;
    }

    public bool isPilaLlena()
    {
        return pilaProductos.Count >= capacidadMaxima;
    }

    private void Awake()
    {
        pilaProductos = new Stack<Producto>();
    }

    public bool apilar(Producto producto)
    {
        if (isPilaLlena())
        {
            Debug.LogWarning("No se puede apilar: Pila llena");
            return false;
        }
        pilaProductos.Push(producto);
        Debug.Log("Producto apilado: " + producto.nombre);
        return true;
    }

    public Producto desapilar()
    {
        if (isPilaVacia())
        {
            Debug.LogWarning("No se puede desapilar: Pila vacía");
            return null;
        }
        Producto producto = pilaProductos.Pop();
        Debug.Log("Producto desapilado: " + producto.nombre);
        return producto;
    }

    public void vaciarPila()
    {
        pilaProductos.Clear();
        Debug.Log("Pila vaciada");
    }

    public List<Producto> obtenerTodosLosProductos()
    {
        return new List<Producto>(pilaProductos);
    }
}
