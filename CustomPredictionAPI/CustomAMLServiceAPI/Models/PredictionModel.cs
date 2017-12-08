using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerAMLServiceAPI.Models
{
    public class PredictionModel : BaseModel
    {
        public string[] ColumnNames { get; set; }
        public string[][] Values { get; set; }

        public string RequestResponseURI_Classic { get { return BaseURI + "execute?api-version=2.0&details=true"; } }
        public string RequestResponseURI_Swagger { get { return BaseURI + "execute?api-version=2.0&format=swagger"; } }
        public string BatchExecutionURI_Submit { get { return BaseURI + "jobs?api-version=2.0"; } }
        public string BatchExecutionURI_Start { get { return BaseURI + "jobs/" + _job_id + "/start?api-version=2.0"; } }
        public string BatchExecutionURI_Status { get { return BaseURI + "jobs/" + _job_id + "?api-version=2.0"; } }
        public string BatchExecutionURI_Delete { get { return BaseURI + "jobs/" + _job_id; } }

        private string _job_id;
        public void SetJobId(string id)
        {
            _job_id = id;
        }
    }
}