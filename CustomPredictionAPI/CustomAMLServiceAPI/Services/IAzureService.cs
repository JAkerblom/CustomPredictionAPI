using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CustomerAMLServiceAPI.Models;

namespace CustomerAMLServiceAPI.Services
{
    public interface IAzureService
    {
        JObject GetPrediction(string model);
        JObject GetPredictionFromClass(string model, testclass obj);
        JObject GetPredictionFromJson(string model, JObject obj);
        bool IsValidModel(string modelName);
        Task<HttpResponseMessage> RetrainModel(string modelName);
    }
}