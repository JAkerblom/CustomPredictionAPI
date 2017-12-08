using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CustomerAMLServiceAPI.Models;
using CustomerAMLServiceAPI.Services;

namespace CustomerAMLServiceAPI.Controllers
{
    public class ValuesController : ApiController
    {
        private IAzureService _azure;

        public ValuesController(IAzureService azure)
        {
            _azure = azure;
        }

        /// <summary>
        ///     GET api/values/GetStaticPrediction() 
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage GetStaticPrediction() //Switched return from string to HttpResponseMessage
        {
            JObject result = _azure.GetPrediction(model: "dummy_v1.0");
            if (result != null)
            {
                dynamic value = result["error"];
                if (value is JArray)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                // This is hardcoded so should return 9.02010034467746E-09 in the Values array
                var predictionValues = result["Results"]["output1"]["value"]["Values"];
                return Request.CreateResponse(HttpStatusCode.OK, predictionValues);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);

            //return "value"; // Comment out all others and include this instead if error checking just the Get method
        }

        /// <summary>
        ///     POST api/values/PostDummyPredictionWClass    
        ///     POST api/values (Not any more, changed the method name to PostDummyPrediction (from Post())
        ///     DEPRECATED! Decided to just take in JObject, which made it easier to map key strings with values.
        /// </summary>
        /// <param name="tc">
        ///     Put in the following in the body (no headers needed):
        ///     {
        ///	        "Class": 0,
        ///	        "age": 0,
        ///	        "menopause": 0,
        ///         "tumor-size": 0,
        ///         "inv-nodes": 0,
        ///         "node-caps": 0,
        ///         "deg-malig": 0,
        ///         "breast": 0,
        ///         "breast-quad": 0,
        ///         "irradiat": 0
        ///     }
        /// </param>
        /// <returns>
        ///     Should return:
        ///     "{\"Class\":0,\"age\":0,\"menopause\":0,\"tumor-size\":0,\"node-caps\":0,\"deg-malig\":0,\"breast\":0,\"breast-quad\":0,\"irradiat\":0}"
        /// </returns>
        public HttpResponseMessage PostDummyPredictionWClass([FromBody]testclass tc)
        {
            JObject result = _azure.GetPredictionFromClass(model: "dummy_v1.0", obj: tc);
            if (result != null)
            {
                dynamic value = result["error"];
                if (value is JArray)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                var predictionValues = result["Results"]["output1"]["value"]["Values"];
                return Request.CreateResponse(HttpStatusCode.OK, predictionValues);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        /// <summary>
        ///     POST api/values/PostDummyPrediction    
        ///     POST api/values (Not any more, changed the method name to PostDummyPrediction (from Post())
        /// </summary>
        /// <param name="tc">
        ///     Put in the following in the body (no headers needed):
        ///     {
        ///	        "Class": 0,
        ///	        "age": 0,
        ///	        "menopause": 0,
        ///         "tumor-size": 0,
        ///         "inv-nodes": 0,
        ///         "node-caps": 0,
        ///         "deg-malig": 0,
        ///         "breast": 0,
        ///         "breast-quad": 0,
        ///         "irradiat": 0
        ///     }
        /// </param>
        /// <returns>
        ///     Should return:
        ///     [
        ///         [
        ///            "0",
        ///            "0",
        ///            "0",
        ///            "0",
        ///            "0",
        ///            "0",
        ///            "0",
        ///            "0",
        ///            "0",
        ///            "0",
        ///            "0",
        ///            "9.02010034467746E-09"
        ///         ]
        ///     ]
        /// </returns>
        public HttpResponseMessage PostDummyPredictionWJson([FromBody]JObject jsonobj)
        {
            JObject result = _azure.GetPredictionFromJson(model: "dummy_v1.0", obj: jsonobj);
            if (result != null)
            {
                dynamic value = result["error"];
                if (value is JArray)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                var predictionValues = result["Results"]["output1"]["value"]["Values"];
                return Request.CreateResponse(HttpStatusCode.OK, predictionValues);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        /// <summary>
        ///     POST api/values/PostRetrainingOfModel     
        /// </summary>
        /// <param name="jsonobj">
        ///     When testing, assign mymodelname to 'dummy_v1.0'
        ///     {
        ///         "model": "mymodelname"
        ///     }
        /// </param>
        /// <returns>
        ///     {
        ///         "StatusCode": 4,
        ///         "Results": {
        ///             "output2": {
        ///                 "RelativeLocation": "mycontainer/output2results.ilearner",
        ///                 "BaseLocation": "https://mystorageacct.blob.core.windows.net/",
        ///                 "SasBlobToken": "?sv=yyyy-mm-dd&sr=c&sig=somesignature&st=yyyy-mm-ddT20%3A31%3A40Z"
        ///             },
        ///             "output1": {
        ///                 "RelativeLocation": "mycontainer/output1results.csv",
        ///                 "BaseLocation": "https://mystorageacct.blob.core.windows.net/",
        ///                 "SasBlobToken": "?sv=yyyy-mm-dd&sr=c&sig=somesignature&st=yyyy-mm-ddT20%3A31%3A40Z"
        ///             }
        ///         }
        ///     }
        /// </returns>
        public HttpResponseMessage PostRetrainingOfModel([FromBody]JObject jsonobj)
        {
            string modelName = jsonobj.GetValue("model").Value<string>();
            if (!_azure.IsValidModel(modelName))
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Error: Model not defined in application.");
            }

            var response = _azure.RetrainModel(modelName);

            if (response.Result.IsSuccessStatusCode)
            {
                return Request.CreateResponse(HttpStatusCode.OK, response.Result.Content.ReadAsAsync<BatchExecutionRequest>());
            }

            return Request.CreateResponse(response.Result.StatusCode, response);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
