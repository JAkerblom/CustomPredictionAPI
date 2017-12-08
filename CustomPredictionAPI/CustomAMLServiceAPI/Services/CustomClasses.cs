using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerAMLServiceAPI.Models;

namespace CustomerAMLServiceAPI.Services
{
    public class ModelTable
    {
        public string ApiKey { get; set; }
        public string Uri { get; set; }
        public string[] ColumnNames { get; set; }
        public string[][] Values { get; set; }
    }

    public class ModelFixture
    {
        //public List<TrainingModel> Models { get; set; }
        //public List<BaseModel> Models { get; set; }
        public BaseModel[] Models { get; set; } 
        //public IDictionary<string, > MyProperty { get; set; }
    }

    public class ModelTestFixture
    {
        public string ApiKey { get; set; }
        public string Uri { get; set; }
        public string[] ColumnNames { get; set; }
        public string[][] Values { get; set; }
        public IDictionary<string, RequestColValTable> Inputs { get; set; }
    }

    public class RequestColValTable
    {
        public string[] ColumnTable { get; set; }
        public string[][] ValuesTable { get; set; }
        // This should be the table to use when a swagger service is used
        public string[] ColumnsAndValues { get; set; } 
    }
}