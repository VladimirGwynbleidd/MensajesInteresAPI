using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MI.BI;
using MI.BI.Interfaces;
using MI.EN;

namespace MensajesInteresAPI.Controllers
{
    public class ResumenComunicadosController : ApiController
    {
        // GET: api/ResumenComunicados/ObtenerResumenComunicados
        [Route("api/ResumenComunicados/ObtieneEncabezadoComunicados")]
        [HttpGet]

        public List<ResumenComunicados> ObtieneEncabezadoComunicados([FromBody] InfoJson resumenComunicados)
        {
            try
            {
                ICatalogo<ResumenComunicados, InfoJson> resumenComunicadosBI = new ResumenComunicadosBI();
                Success<ResumenComunicados> success = new Success<ResumenComunicados>
                {
                    ResponseDataEnumerable = resumenComunicadosBI.Get(resumenComunicados).ResponseDataEnumerable
                };

                return success.ResponseDataEnumerable.ToList();
            }
            catch (Exception ex)
            {
                return new List<ResumenComunicados>
                { new ResumenComunicados
                    {
                        Encabezado =string.Empty,
                        Detalle=string.Empty,
                        f_VigenciaIni = string.Empty,
                        f_VigenciaFin = string.Empty,
                        DescError = ex.Message
                    }
                };
            }
        }

        // POST: api/ResumenComunicados/RegistraComunicadoVisualizado
        [Route("api/ResumenComunicados/ObtieneComunicadoVisualizado")]
        [HttpPost]
        public ResumenComunicados ObtieneComunicadoVisualizado([FromBody] ResumenComunicados resumenComunicados)
        {
            try
            {

                ICatalogo<ResumenComunicados, InfoJson> resumenComunicadosBI = new ResumenComunicadosBI();
                Success<ResumenComunicados> success = new Success<ResumenComunicados>
                {
                    Data = resumenComunicadosBI.Insert(resumenComunicados).Data
                };
                return success.Data;
            }
            catch (Exception ex)
            {
                return new ResumenComunicados {
                    Encabezado=string.Empty,
                    Detalle=string.Empty,
                    f_VigenciaIni = string.Empty,
                    f_VigenciaFin = string.Empty,
                    DescError = ex.Message
                };
            }
        }
    }
}
