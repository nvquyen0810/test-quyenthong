using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FptEcommerce.API.Caching
{
    public interface IRedisCacheService
    {
        T Get<T>(string key);
        T Set<T>(string key, T value, int m1, int m2);
        void Remove(string key);
    }
}
