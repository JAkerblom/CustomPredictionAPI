namespace CustomerAMLServiceAPI.Models
{
    public interface IModel
    {
        string ApiKey { get; set; }
        
        string BaseURI { get; set; } 
        string DashboardURL { get; set; }

        
        //string RequestResponseURI_Classic { get; }
        //string RequestResponseURI_Swagger { get; }
        //string BatchExecutionURI_Submit { get; }
        //string BatchExecutionURI_Start { get; }   
        //string BatchExecutionURI_Status { get; }
        //string BatchExecutionURI_Delete { get; }
    }
}