//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Shared;

//public class JsonIgnoreResolver : DefaultContractResolver
//{
//    protected string[] PropertyNames { get; set; }

//    public JsonIgnoreResolver(string[] propertyNames)
//    {
//        PropertyNames = propertyNames;
//    }

//    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
//    {
//        var props = base.CreateProperties(type, memberSerialization);


//        //var data = props.FirstOrDefault(p => p.PropertyName == "Items"); ;

//        //var axa = data.GetType().GetProperties();

//        //var d = axa.FirstOrDefault(p => p.Name == "Items");

//        //if (_rules.TryGetValue(type, out var allowedProps))
//        //{
//        //    // hanya ambil property yang ada di daftar allowed
//        //    props = props.Where(p => allowedProps.Contains(p.PropertyName)).ToList();
//        //}
//        //return props;
//        return [.. props.Where(p => PropertyNames.Contains(p.PropertyName))];
//    }
//}

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

public class JsonIgnoreResolver : DefaultContractResolver
{
    private readonly Dictionary<Type, HashSet<string>> _ignoreMap;

    public JsonIgnoreResolver()
    {
        _ignoreMap = new Dictionary<Type, HashSet<string>>();
    }

    // daftar property yang mau di-ignore khusus per class
    public void IgnoreProperty(Type type, params string[] propertyNames)
    {
        if (!_ignoreMap.ContainsKey(type))
            _ignoreMap[type] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var name in propertyNames)
            _ignoreMap[type].Add(name);
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var props = base.CreateProperties(type, memberSerialization);

        if (_ignoreMap.ContainsKey(type))
        {
            props = props
                .Where(p => !_ignoreMap[type].Contains(p.PropertyName))
                .ToList();
        }

        return props;
    }
}

//// --- data class ---
//public class Root
//{
//    public string Code { get; set; }
//    public Data Data { get; set; }
//}

//public class Data
//{
//    public List<Item> Items { get; set; }
//}

//public class Item
//{
//    public string Code { get; set; }
//    public string EmployeeCode { get; set; }
//    public string NIK { get; set; }
//    public string OldNIK { get; set; }
//    public string Name { get; set; }
//    public string CompanyCode { get; set; }
//}

//class Program
//{
//    static void Main()
//    {
//        var obj = new Root
//        {
//            Code = "parent-code",
//            Data = new Data
//            {
//                Items = new List<Item>
//                {
//                    new Item {
//                        Code = "a-5",
//                        EmployeeCode = "a-5",
//                        NIK = "x1231",
//                        OldNIK = "xas123",
//                        Name = "MICHAEL",
//                        CompanyCode = "cac1efe3-d"
//                    }
//                }
//            }
//        };

//        var resolver = new JsonIgnoreResolver();
//        resolver.IgnoreProperty(typeof(Item), "Code"); // hanya Code di Item yg di-ignore
//        resolver.IgnoreProperty(typeof(Item), "CompanyCode"); // contoh tambahan

//        var settings = new JsonSerializerSettings
//        {
//            ContractResolver = resolver,
//            Formatting = Formatting.Indented
//        };

//        string json = JsonConvert.SerializeObject(obj, settings);
//        Console.WriteLine(json);
//    }
//}
