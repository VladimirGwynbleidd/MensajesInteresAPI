using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Data.SqlClient;

namespace MI.DAO
{
    public class SqlFactoryConnection
    {
        public ILogger Logger { get; }

        public SqlFactoryConnection(ILogger logger = null)
        {
            Logger = logger;
        }

        private static string DesEncripta(string strCad)
        {
            CryptographyAes Cryp = new CryptographyAes();

            return Cryp.DecryptString(strCad);
        }

        private static string Connection_String
        {
            get
            {
                string cadenaConexion = string.Empty;

                cadenaConexion = DesEncripta(ConfigurationManager.ConnectionStrings["MSJ_INTERES"].ConnectionString);

                return cadenaConexion;
            }
        }

        public SqlConnection Connection()
        {
            SqlConnection conn = new SqlConnection(Connection_String);
            Logger?.LogInformation("Abriendo conexión de base de datos en modo sincrono");
            conn.Open();
            return conn;
        }
    }
}
