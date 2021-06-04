using System;
using System.Collections.Generic;
using MI.EN;
using MI.DAO;


namespace MI.PI
{
    public class ResumenComunicadosPI
    {
        public Success<ResumenComunicados> Get(InfoJson parameters)
        {
            Dictionary<string, object> values = null;
            if (parameters != null)
            {
                values = new Dictionary<string, object>
                {
                     { "@rfc", parameters.rfc},
                     { "@visto", parameters.visto}
                };
            }

            Func<
                FunctionDelegate<ResumenComunicados>.ObtenerResultadoDelegate,
                string,
                IDictionary<string, object>,
                Success<ResumenComunicados>> response = FunctionDelegate<ResumenComunicados>.ObtenerListaResultado;


            return response(new SqlFactory().ExecuteList<ResumenComunicados>, "pa_ObtieneEncabezadoComunicados", values ?? null);
        }

        public Success<ResumenComunicados> Insert(ResumenComunicados parameters)
        {

            Dictionary<string, object> values = null;
            if (parameters != null)
            {
                values = new Dictionary<string, object>
                {
                     { "@c_id", parameters.c_id},
                     { "@origen", "SAT Móvil"}
                };
            }

            Func<
                FunctionDelegate<ResumenComunicados>.ObtenerResultadoDelegate,
                string,
                IDictionary<string, object>,
                Success<ResumenComunicados>> response = FunctionDelegate<ResumenComunicados>.ObtenerData;


            return response(new SqlFactory().ExecuteList<ResumenComunicados>, "pa_ObtieneComunicadoVisualizado", values ?? null);
        }

    }
}
