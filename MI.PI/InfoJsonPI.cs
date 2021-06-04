using System;
using System.Collections.Generic;
using MI.EN;
using MI.DAO;

namespace MI.PI
{
    public class InfoJsonPI
    {
        public Success<InfoJson> Get()
        {
            Func<
                FunctionDelegate<InfoJson>.ObtenerResultadoDelegate,
                string,
                IDictionary<string, object>,
                Success<InfoJson>> response = FunctionDelegate<InfoJson>.ObtenerListaResultado;

            
            return response(new SqlFactory().ExecuteList<InfoJson>, "sp_ComunicadosRFC", null);
        }

        public Success<InfoJson> Insert(InfoJson parameters)
        {
            Func<
                FunctionDelegate<InfoJson>.ObtenerResultadoEscalarDelegate,
                string,
                Dictionary<string, object>,
                InfoJson,
                Success<InfoJson>> response = FunctionDelegate<InfoJson>.ObtenerResultado;

            //Dictionary<string, object> values = new Dictionary<string, object>
            //		{
            //			{ "@claveProducto", parameters.ClaveProducto},
            //			{ "@Producto", parameters.ProductoDescripcion},
            //			{ "@Comentario", parameters.Comentario }
            //		};

            return response(new SqlFactory().ExecuteNonQuery, "sp_InsertaComunicadoVisualizado", null, parameters);
        }
    }
}
