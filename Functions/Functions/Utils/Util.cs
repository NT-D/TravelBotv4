using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Functions.Utils
{
    public class Util
    {
        public static T NameValueCollection2Object<T>(NameValueCollection data)
        {
            var dict = data.AllKeys.ToDictionary(p => p, p => data[p]);
            var json = JsonConvert.SerializeObject(dict);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
