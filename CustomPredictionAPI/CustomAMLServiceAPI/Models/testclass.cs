using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerAMLServiceAPI.Models
{
    public class testclass
    {
        [JsonProperty(PropertyName = "Class")]
        public int Class { get; set; }
        [JsonProperty(PropertyName = "age")]
        public int Age { get; set; }
        [JsonProperty(PropertyName = "menopause")]
        public int Menopause { get; set; }
        [JsonProperty(PropertyName = "tumor-size")]
        public int TumorSize { get; set; }
        [JsonProperty(PropertyName = "node-caps")]
        public int NodeCaps { get; set; }
        [JsonProperty(PropertyName = "deg-malig")]
        public int DegMalig { get; set; }
        [JsonProperty(PropertyName = "breast")]
        public int Breast { get; set; }
        [JsonProperty(PropertyName = "breast-quad")]
        public int BreastQuad { get; set; }
        [JsonProperty(PropertyName = "irradiat")]
        public int Irradiat { get; set; }
    }
}