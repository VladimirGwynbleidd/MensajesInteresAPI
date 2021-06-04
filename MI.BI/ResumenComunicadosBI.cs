using System;
using MI.BI.Interfaces;
using MI.EN;
using MI.PI;


namespace MI.BI
{
    public class ResumenComunicadosBI : ICatalogo<ResumenComunicados, InfoJson>
    {
        private readonly ResumenComunicadosPI cls = new ResumenComunicadosPI();
        public Success<ResumenComunicados> Get(InfoJson param = null)
        {
            return cls.Get(param);
        }

        public Success<ResumenComunicados> Insert(ResumenComunicados parameters)
        {
            return cls.Insert(parameters);
        }

        public Success<ResumenComunicados> Update(ResumenComunicados parameters)
        {
            throw new NotImplementedException();
        }

        public Success<ResumenComunicados> Delete(ResumenComunicados parameters)
        {
            throw new NotImplementedException();
        }

        public Success<ResumenComunicados> Get(ResumenComunicados param = null)
        {
            throw new NotImplementedException();
        }
    }
}
