using System;
using System.Collections.Generic;
using MI.EN;
using MI.DAO;


namespace MI.PI
{
	public class ProductoProcess
	{
		public Success<Producto> Get()
		{
			Func<
				FunctionDelegate<Producto>.ObtenerResultadoDelegate,
				string,
				IDictionary<string, object>,
				Success<Producto>> response = FunctionDelegate<Producto>.ObtenerListaResultado;
			return response(new SqlFactory().ExecuteList<Producto>, "sp_ObtenerTipoProducto", null);
		}

		public Success<Producto> Insert(Producto parameters)
		{
			Func<
				FunctionDelegate<Producto>.ObtenerResultadoEscalarDelegate,
				string,
				Dictionary<string, object>,
				Producto,
				Success<Producto>> response = FunctionDelegate<Producto>.ObtenerResultado;

			Dictionary<string, object> values = new Dictionary<string, object>
					{
						{ "@claveProducto", parameters.ClaveProducto},
						{ "@Producto", parameters.ProductoDescripcion},
						{ "@Comentario", parameters.Comentario }
					};

			return response(new SqlFactory().ExecuteNonQuery, "sp_AgregarTipoProducto", values, parameters);
		}

		public Success<Producto> Update(Producto parameters)
		{
			Func<
				FunctionDelegate<Producto>.ObtenerResultadoEscalarDelegate,
				string,
				Dictionary<string, object>,
				Producto,
				Success<Producto>> response = FunctionDelegate<Producto>.ObtenerResultado;

			Dictionary<string, object> values = new Dictionary<string, object>
					{
						{ "@idProductos", parameters.IdProducto },
						{ "@claveProducto", parameters.ClaveProducto},
						{ "@Producto", parameters.ProductoDescripcion},
						{ "@Comentario", parameters.Comentario},
					};

			return response(new SqlFactory().ExecuteNonQuery, "sp_ActualizarProducto", values, parameters);
		}

		public Success<Producto> Delete(Producto parameters)
		{
			Func<
				FunctionDelegate<Producto>.ObtenerResultadoEscalarDelegate,
				string,
				Dictionary<string, object>,
				Producto,
				Success<Producto>> response = FunctionDelegate<Producto>.ObtenerResultado;

			Dictionary<string, object> values = new Dictionary<string, object>
					{
						{ "@idProductos", parameters.IdProducto }
					};

			return response(new SqlFactory().ExecuteNonQuery, "sp_EliminarProducto", values, parameters);
		}
	}
}
