using System;
using MI.BI.Interfaces;
using MI.EN;
using MI.PI;

namespace MI.BI
{
    public class InfoJsonBI : ICatalogo<InfoJson, ResumenComunicados>
    {
        private readonly InfoJsonPI cls = new InfoJsonPI();
        public Success<InfoJson> Get(InfoJson param = null)
        {
            return cls.Get();
        }

        public Success<InfoJson> Insert(InfoJson parameters)
        {
            return cls.Insert(parameters);
        }

        public Success<InfoJson> Update(InfoJson parameters)
        {
            throw new NotImplementedException();
        }

        public Success<InfoJson> Delete(InfoJson parameters)
        {
            throw new NotImplementedException();
        }

        public Success<InfoJson> Get(ResumenComunicados param = null)
        {
            throw new NotImplementedException();
        }
    }
}
