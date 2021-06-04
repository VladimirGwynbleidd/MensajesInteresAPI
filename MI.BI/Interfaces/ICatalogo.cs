using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MI.EN;

namespace MI.BI.Interfaces
{
    public interface ICatalogo<T, U> where T : class
        where U : class
    {
        Success<T> Get(U param = null);
        Success<T> Insert(T parameters);
        Success<T> Update(T parameters);
        Success<T> Delete(T parameters);
    }
}
