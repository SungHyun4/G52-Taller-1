using System;

[System.Serializable]
public class Producto
{
    public int id;
    public string nombre;
    public string tipo;   // Basico, Fragil, Pesado
    public double peso;
    public double precio;
    public double tiempo;  // tiempo de despacho

    // Constructor
    public Producto(int id, string nombre, string tipo, double peso, double precio, double tiempo)
    {
        this.id = id;
        this.nombre = nombre;
        this.tipo = tipo;
        this.peso = peso;
        this.precio = precio;
        this.tiempo = tiempo;
    }

    // Constructor vacío para JSON
    public Producto() { }

    public override string ToString()
    {
        return $"ID: {id}, Nombre: {nombre}, Tipo: {tipo}, Peso: {peso}kg, Precio: ${precio}, Tiempo: {tiempo}s";
    }
}
