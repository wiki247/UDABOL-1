﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Modelo;
using Negocio;

namespace Dao
{
    public class ConexionTexto : IConexion
    {
        private String _archivo;
        private String _contenido;
        private Type _tipo;

        public bool Conectar(Type tipo)
        {
            try
            {
                _tipo = tipo;
                _archivo = tipo.Name+".txt";
                if (File.Exists(_archivo))
                {
                    _contenido = File.ReadAllText(_archivo);
                }
                else _contenido = "";
                return true;
            }
            catch (Exception ex) {
                System.Console.WriteLine("Ha ocurrido un Error: "+ex.Message);
            }
            return false;
        }

        /* ROL
         * 1    Usuario Comun
         * 2    Super Usuario
         * 
         * Usuario
         * 
         * 1    Fulanito de tal 1
         * 2    SuperUsuario    2
         * 
         * 
         */

        public Boolean EliminarRegistro(KeyValuePair<String,String> condicion)
        {

            List<IObjetoTexto> listaAEliminar = LeerTabla();
            PropertyInfo[] _propiedadesClase = _tipo.GetProperties();

            foreach (IObjetoTexto objeto in listaAEliminar)
            {
                Type tipoObjeto = objeto.GetType();
                PropertyInfo _identificadorCondicion = tipoObjeto.GetProperty(condicion.Key);
                String _valor = _identificadorCondicion.GetValue(objeto).ToString();
                if (_valor.Equals(condicion.Value))
                {
                    int nroLinea = listaAEliminar.IndexOf(objeto);

                    String[] lineas = _contenido.Split("\n");
                    _contenido = "";

                    for (int i = 0; i < lineas.Length; i++)
                    {
                        if (i != nroLinea && lineas[i] != null && !lineas[i].Trim().Equals(""))
                        {
                            _contenido += lineas[i] + "\n";
                        }
                    }

                    return true;
                }
            }

            return true;
        }

        public bool EscribirTabla( List<IObjetoTexto> lista)
        {
            try
            {
                if (lista != null )
                {
                    String contenido = "";
                    foreach (IObjetoTexto _objeto in lista) {
                        if (_objeto != null)
                        {
                            contenido += _objeto.guardarTexto() + "\n";
                        }
                    }
                    File.WriteAllText(_archivo, contenido);
                    _contenido = contenido;
                    return true;
                }
            }
            catch (Exception ex) {
                System.Console.WriteLine("Ha ocurrido un Error: " + ex.Message);
            }
            return false;
        }

        public bool Guardar()
        {
            try
            {
                File.WriteAllText(_archivo, _contenido, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Ha ocurrido un Error: " + ex.Message);
            }
            return false;
        }

        public List<IObjetoTexto> LeerTabla()
        {
            List<IObjetoTexto> lista = new List<IObjetoTexto>();
            try
            {
                String[] lineas = _contenido.Split("\n");
                for (int i=0;i<lineas.Length;i++) {
                    lineas[i] = lineas[i].Trim();
                    if (!lineas[i].Trim().Equals(""))
                    {
                        IObjetoTexto _objeto = ModeloFactory.darInstancia(_tipo);
                        lista.Add(_objeto.leerTexto(lineas[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Ha ocurrido un Error: " + ex.Message);
            }


            return lista;
        }

        public List<T> LeerTabla<T>()
        {
            List<T> lista = new List<T>();
            try
            {
                String[] lineas = _contenido.Split("\n");
                for (int i = 0; i < lineas.Length; i++)
                {
                    lineas[i] = lineas[i].Trim();
                    if (!lineas[i].Equals(""))
                    {
                        IObjetoTexto _objeto = ModeloFactory.darInstancia(typeof(T));
                        lista.Add((T)_objeto.leerTexto(lineas[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Ha ocurrido un Error: " + ex.Message);
            }
            return lista;
        }
    }
}