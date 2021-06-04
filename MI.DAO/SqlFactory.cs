using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;

namespace MI.DAO
{
	public enum ExecuteType
	{
		ExecuteReader,
		ExecuteNonQuery,
		ExecuteScalar
	}

	public class SqlFactory : IDisposable
	{
		private bool disposed = false;
		private readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
		public ILogger Logger { get; }

		public SqlFactory(ILogger logger = null)
		{
			Logger = logger;
		}

		public IEnumerable<T> ExecuteList<T>(string sql, IDictionary<string, object> parameters) where T : class, new()
		{
			Type TypeT = typeof(T);
			ConstructorInfo ctor = TypeT.GetConstructor(Type.EmptyTypes);
			if (ctor == null)
			{
				throw new InvalidOperationException($"Type {TypeT.Name} no contiene un constructor por default");
			}
			using (SqlConnection _conn = new SqlFactoryConnection().Connection())
			{
				using (SqlCommand Command = new SqlCommand(sql, _conn))
				{
					Command.CommandType = CommandType.StoredProcedure;
					if (parameters != null)
					{
						Command.Parameters.Clear();
						foreach (KeyValuePair<string, object> kvp in parameters)
						{
							Command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value != null ? kvp.Value : DBNull.Value));
						}
					}
					using (SqlDataReader dr = Command.ExecuteReader())
					{
						T obj = new T();
						while (dr.Read())
						{
							obj = DelegadoAccion<T>(dr);

							yield return obj;
						}
					}
				}
			}
		}


		public object ExecuteProcedure(string procedureName, ExecuteType executeType, IDictionary<string, object> parameters)
		{
			using (SqlConnection _conn = new SqlFactoryConnection().Connection())
			{
				try
				{
					object returnObject = null;
					SqlCommand Command = new SqlCommand(procedureName, _conn)
					{
						CommandType = CommandType.StoredProcedure,
					};
					if (parameters != null)
					{
						Command.Parameters.Clear();
						foreach (KeyValuePair<string, object> kvp in parameters)
						{
							Command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
						}
					}
					switch (executeType)
					{
						case ExecuteType.ExecuteNonQuery:
							returnObject = Command.ExecuteNonQuery();
							break;
						case ExecuteType.ExecuteScalar:
							returnObject = Command.ExecuteScalar();
							break;
					}
					return returnObject;
				}
				catch (Exception ex)
				{
					Logger?.LogError("Error de ejecución en modo sincrono ExecuteProcedure");
					throw new ArgumentException(ex.Message, ex);
				}
				finally
				{
					if (_conn.State == ConnectionState.Open)
					{
						Logger?.LogInformation("Cerrando conexión en modo sincrono ExecuteProcedure");
						_conn.Close();
					}
				}
			}
		}


		public object Execute(string procedureName, ExecuteType executeType, Dictionary<string, object> parameters)
		{
			using (SqlConnection _conn = new SqlFactoryConnection().Connection())
			{
				try
				{
					object returnObject = null;
					using (SqlCommand Command = new SqlCommand(procedureName, _conn))
					{
						Command.CommandType = CommandType.StoredProcedure;
						if (parameters != null)
						{
							Command.Parameters.Clear();
							foreach (KeyValuePair<string, object> kvp in parameters)
							{
								Command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
							}
						}
						switch (executeType)
						{
							case ExecuteType.ExecuteNonQuery:
								returnObject = Command.ExecuteNonQuery();
								break;
							case ExecuteType.ExecuteScalar:
								returnObject = Command.ExecuteScalar();
								break;
						}
					}
					return returnObject;
				}
				catch (Exception ex)
				{
					Logger?.LogError($"Error en ejecución del siguiente Procedimiento {procedureName} se produjo el siguiente error: {ex.Message}");
					throw new ArgumentException(ex.Message, ex);

				}
				finally
				{
					if (_conn.State == ConnectionState.Open)
					{
						Logger?.LogInformation("Cerrando conexión en modo sincrono Execute");
						_conn.Close();
					}
				}
			}
		}

		
		public static T DelegadoAccion<T>(SqlDataReader dr) where T : class, new()
		{
			T obj = new T();
			for (int i = 0; i < dr.FieldCount; i++)
			{
				PropertyInfo property = obj.GetType().GetProperty(dr.GetName(i), BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
				if (!dr.IsDBNull(i) && property != null)
				{
					switch (Type.GetTypeCode(property.PropertyType))
					{
						case TypeCode.String:
							property.SetValue(obj, dr.GetString(i), null);
							break;
						case TypeCode.Int32:
							property.SetValue(obj, (object)dr.GetFieldType(i) == typeof(int) ? dr.GetInt32(i) : (int)dr.GetInt64(i), null);
							break;
						case TypeCode.Int64:
							property.SetValue(obj, (int)dr.GetInt64(i), null);
							break;
						case TypeCode.Boolean:
							property.SetValue(obj, dr.GetBoolean(i), null);
							break;
						case TypeCode.Double:
							property.SetValue(obj, dr.GetDouble(i), null);
							break;
						case TypeCode.Decimal:
							property.SetValue(obj, dr.GetDecimal(i), null);
							break;
						case TypeCode.DateTime:
							property.SetValue(obj, Convert.ToDateTime(dr.GetDateTime(i).ToString("yyyy-MM-ddTHH:mm:ss")), null);
							break;
						default:
							property.SetValue(obj, dr.GetFloat(i), null);
							break;
					}
				}
			}
			return obj;
		}

		public int ExecuteNonQuery(string procedureName, Dictionary<string, object> parameters)
		{
			int returnObject = 0;
			using (SqlConnection _conn = new SqlFactoryConnection().Connection())
			{
				using (SqlCommand Command = new SqlCommand(procedureName, _conn))
				{
					try
					{
						Command.CommandType = CommandType.StoredProcedure;
						if (parameters != null)
						{
							Command.Parameters.Clear();
							foreach (KeyValuePair<string, object> kvp in parameters)
							{
								Command.Parameters.Add(new SqlParameter(kvp.Key, kvp.Value));
							}
						}
						returnObject = Command.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						Logger?.LogError($"Error en ejecución del siguiente Procedimiento {procedureName} se produjo el siguiente error: {ex.Message}");
						throw new ArgumentException(ex.Message, ex);
					}
					finally
					{
						if (_conn.State == ConnectionState.Open)
						{
							Logger?.LogInformation("Cerrando conexión en modo sincrono ExecuteNonQuery");
							_conn.Close();
						}
					}
				}
				return returnObject;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			if (disposing)
			{
				handle.Dispose();
			}
			disposed = true;
		}


		~SqlFactory()
		{
			Dispose(false);
		}
	}
}
