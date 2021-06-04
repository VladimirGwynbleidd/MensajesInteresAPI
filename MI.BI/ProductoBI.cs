using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MI.BI.Interfaces;
using MI.EN;
using MI.PI;


namespace MI.BI
{
    public class ProductoBI : ICatalogo<Producto, InfoJson>
    {
        private readonly ProductoProcess cls = new ProductoProcess();
        public Success<Producto> Get(Producto param = null)
        {
            return cls.Get();
        }

        public Success<Producto> Insert(Producto parameters)
        {
            return cls.Insert(parameters);
        }

        public Success<Producto> Update(Producto parameters)
        {
            throw new NotImplementedException();
        }

        public Success<Producto> Delete(Producto parameters)
        {
            throw new NotImplementedException();
        }

        public Success<Producto> Get(InfoJson param = null)
        {
            throw new NotImplementedException();
        }
    }
}
