using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NitelikliBilisim.App.Models
{
    public class CityTownModel
    {
        public bool status { get; set; }
        public List<_City> data { get; set; }
    }

    public class _City
    {
        public string _id { get; set; }
        public string name { get; set; }
        public List<_Town> towns { get; set; }

    }
    public class _Town
    {
        public string _id { get; set; }
        public string name { get; set; }
    }
}
