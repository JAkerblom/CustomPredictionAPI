using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerAMLServiceAPI.Models
{
    public class TrainingModel : BaseModel
    {
        public string StorageAccountName { get; set; }
        public string StorageAccountKey { get; set; }
        public string RelPathToContainer { get; set; }
        public string NameOfOutput { get; set; }

        //public string BatchExecutionURI_Submit { get { return BaseURI + "jobs?api-version=2.0"; } }
        //public string BatchExecutionURI_Start { get { return BaseURI + "jobs/" + _job_id + "/start?api-version=2.0"; } }
        //public string BatchExecutionURI_Status { get { return BaseURI + "jobs/" + _job_id + "?api-version=2.0"; } }
        //public string BatchExecutionURI_Delete { get { return BaseURI + "jobs/" + _job_id; } }
        
        //private string _job_id;
        //public void SetJobId(string id)
        //{
        //    _job_id = id;
        //}
    }

    
}