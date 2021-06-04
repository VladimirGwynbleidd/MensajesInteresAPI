using System;
using System.Collections.Generic;
using System.Linq;
using MI.DAO;
using MI.EN;
using Newtonsoft.Json;

namespace MI.PI
{
    class FunctionDelegate<T> where T : class
    {
        public delegate IEnumerable<T> ObtenerResultadoDelegate(string sp, IDictionary<string, object> parametros);
        public delegate int ObtenerResultadoEscalarDelegate(string sp, Dictionary<string, object> parametros);

        public static Success<T> ObtenerListaResultado(ObtenerResultadoDelegate metodo,
            string sp, IDictionary<string, object> parametros)
        {
            Success<T> success = new Success<T>();

            try
            {
                success.Exito = true;
                success.ResponseDataEnumerable = metodo(sp, parametros).ToList();
                success.ResponseData = new List<T>();

                return success;
            }
            catch (Exception ex)
            {
                success.Exito = false;
                success.Mensaje = ex.Message;
                success.ResponseData = new List<T>();
                InsertaLogComunicado(1, ex.Message);
                throw new ArgumentException(JsonConvert.SerializeObject(success), ex);
            }
        }

        public static Success<T> ObtenerResultado(ObtenerResultadoEscalarDelegate metodo, string sp, Dictionary<string, object> parametros, T arg)
        {
            Success<T> success = new Success<T>();

            try
            {
                int valor = metodo(sp, parametros);

                success.Exito = true;
                success.Valor = valor;

                return success;
            }
            catch (Exception ex)
            {
                success.Exito = false;
                success.Mensaje = ex.Message;
                success.Data = arg;
                InsertaLogComunicado(1, ex.Message);
                throw new ArgumentException(JsonConvert.SerializeObject(success), ex);
            }

        }

        public static Success<T> ObtenerData(ObtenerResultadoDelegate metodo,
            string sp, IDictionary<string, object> parametros)
        {
            Success<T> success = new Success<T>();

            try
            {
                success.Exito = true;
                success.Data = metodo(sp, parametros).Single();

                return success;
            }
            catch (Exception ex)
            {
                success.Exito = false;
                success.Mensaje = ex.Message;
                success.ResponseData = new List<T>();
                InsertaLogComunicado(1, ex.Message);
                throw new ArgumentException(JsonConvert.SerializeObject(success), ex);
            }
        }

        public static void InsertaLogComunicado(int valor, string message)
        {
            string HostActual = System.Net.Dns.GetHostName();

            Func<
            FunctionDelegate<ResumenComunicados>.ObtenerResultadoEscalarDelegate,
            string,
            Dictionary<string, object>,
            ResumenComunicados,
            Success<ResumenComunicados>> response = FunctionDelegate<ResumenComunicados>.ObtenerResultado;

            Dictionary<string, object> values = new Dictionary<string, object>
                    {
                        { "@clave", valor},
                        { "@descripcion", "[" + HostActual + "]-" + message}
                    };

            response(new SqlFactory().ExecuteNonQuery, "sp_RegistraLogComunicado", values, null);

        }
    }
}
