using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MI.BI;
using MI.BI.Interfaces;
using MI.EN;

namespace MensajesInteresAPI.Controllers
{
    public class ProductoController : ApiController
    {
        [Route("api/Producto/ObtenerProducto")]
        [HttpGet]
        public List<Producto> ObtenerProducto()
        {
            try
            {

                ICatalogo<Producto, InfoJson> productoBI = new ProductoBI();
                Success<Producto> success = new Success<Producto>();

                success.ResponseDataEnumerable = productoBI.Get().ResponseDataEnumerable;

                return success.ResponseDataEnumerable.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Route("api/Producto/InsertarProducto")]
        [HttpPost]
        public Success<Producto> InsertarProducto([FromBody] Producto producto)
        {
            try
            {

                ICatalogo<Producto, InfoJson> productoBI = new ProductoBI();
                Success<Producto> success = new Success<Producto>();

                success = productoBI.Insert(producto);

                if (success.Valor == 1)
                {
                    success.Mensaje = "Se agregó correctamente el registro";
                    success.Exito = true;
                }
                else
                {
                    success.Mensaje = "No se agregó correctamente el resgistro";
                    success.Exito = false;
                }

                return success;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public void Put(int id, [FromBody] string value)
        {
        }


        public void Delete(int id)
        {
        }
    }
}
