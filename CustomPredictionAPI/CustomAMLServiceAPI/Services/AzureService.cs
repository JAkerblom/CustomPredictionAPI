using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CustomerAMLServiceAPI.Models;

//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Formatting;
//using System.Net.Http.Headers;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Auth;
//using Microsoft.WindowsAzure.Storage.Blob;

namespace CustomerAMLServiceAPI.Services
{
    public class AzureService : IAzureService
    {
        IDictionary<string, ModelTable> _models;
        //IDictionary<string, ModelFixture> _modelFixtures;
        //IDictionary<string, IModel> _modelFixtures;
        IDictionary<string, ModelFixture> _modelFixtures;

        public JObject GetPrediction(string model)
        {
            ModelTable mt = _models[model];

            string result = PostJsonAsync(mt).Result;

            return JObject.Parse(result);
        }

        public JObject GetPredictionFromClass(string model, testclass obj)
        {
            ModelTable mt = _models[model];

            string result = PostJsonAsync(mt).Result;

            return JObject.Parse(result);
        }

        public JObject GetPredictionFromJson(string model, JObject obj)
        {
            ModelTable mt = _models[model];
            mt.ColumnNames = BuildColumnNames(obj);
            mt.Values = BuildSingleValueMatrix(obj);

            string result = PostJsonAsync(mt).Result;

            return JObject.Parse(result);
        }

        private Task<string> PostJsonAsync(ModelTable model)
        {
            string apiKey = model.ApiKey;
            string uri = model.Uri;
            string[] columnNames = model.ColumnNames;
            string[][] values = model.Values;

            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, RequestColValTable>() {
                    {
                        "input1",
                        new RequestColValTable()
                        {
                            ColumnTable = columnNames,
                            ValuesTable = values
                        }
                    }
                },
                    GlobalParameters = new Dictionary<string, string>() { }
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri(uri);
                client.Timeout = new TimeSpan(0, 0, 40);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var obj = JsonConvert.SerializeObject(
                    scoreRequest,
                    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                request.Content = new StringContent(obj, Encoding.UTF8, "application/json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                Task<HttpResponseMessage> task = client.SendAsync(request);

                return task.Result.Content.ReadAsStringAsync();
            }
        }

        private Task<string> PostJsonAsyncTmp(ModelTestFixture model)
        {
            string apiKey = model.ApiKey;
            string uri = model.Uri;
            string[] columnNames = model.ColumnNames;
            string[][] values = model.Values;

            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, RequestColValTable>() {
                    {
                        "input1",
                        new RequestColValTable()
                        {
                            ColumnTable = columnNames,
                            ValuesTable = values
                        }
                    }
                },
                    GlobalParameters = new Dictionary<string, string>() { }
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri(uri);
                client.Timeout = new TimeSpan(0, 0, 40);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var obj = JsonConvert.SerializeObject(
                    scoreRequest,
                    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                request.Content = new StringContent(obj, Encoding.UTF8, "application/json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                Task<HttpResponseMessage> task = client.SendAsync(request);

                return task.Result.Content.ReadAsStringAsync();
            }
        }

        public async Task<HttpResponseMessage> RetrainModel(string modelName)
        {
            //ModelFixture model = _modelFixtures.Where(x => x.Key == modelName).First().Value;
            //TrainingModel model = _modelFixtures.Where(x => x.Key == modelName).First().Value;

            string apiKey = "";
            string BaseUrl = "";
            //string apiKey = model.ApiKey; // Replace this with the API key for the web service
            //string BaseUrl = model.BaseURI + "/jobs"; // URI + /jobs (This example assumes the common string for BES, but I assume instead both RRS and BES, which makes it sufficient to append /jobs later instead.

            string StorageAccountName = ""; // Replace this with your Azure Storage Account name
            string StorageAccountKey = "a_storage_account_key"; // Replace this with your Azure Storage Key
            string StorageContainerName = "mycontainer"; // Replace this with your Azure Storage Container name
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);


            // Input file and blob:: Assumes that we have a file locally and want to first store it in a blob, which we direct the 
            //const string inputDataFile = "input1data.file_extension"; /*Replace this with the location of your input file, and valid file extension (usually .csv)*/
            const string inputBlobName = "input1datablob.file_extension"; /*Replace this with the name you would like to use for your Azure blob; this needs to have the same extension as the input file */
            const string outputResults = "output1results.file_extension"; /* Replace this with the location you would like to use for your output file, and valid file extension (usually .csv for scoring results, or .ilearner for trained models)*/
            const string outputModel = "output2results.ilearner";

            // Don't know if this is to be used by us at all.
            //UploadFileToBlob(
            //    inputDataFile, 
            //    inputBlobName,
            //    StorageContainerName, 
            //    storageConnectionString);

            // set a time out for polling status
            const int TimeOutInMilliseconds = 120 * 1000; // Set a timeout of 2 minutes

            using (HttpClient client = new HttpClient())
            {
                var request = new BatchExecutionRequest()
                {
                    Inputs = new Dictionary<string, AzureBlobDataReference>() {
                        {
                            "input1",
                             new AzureBlobDataReference()
                             {
                                 ConnectionString = storageConnectionString,
                                 RelativeLocation = string.Format("{0}/{1}", StorageContainerName, inputBlobName)
                             }
                        },
                    },

                    Outputs = new Dictionary<string, AzureBlobDataReference>() {
                        {
                            "output1",
                            new AzureBlobDataReference()
                            {
                                ConnectionString = storageConnectionString,
                                RelativeLocation = string.Format("{0}/{1}", StorageContainerName, outputResults)
                            }
                        },
                        {
                            "output2",
                            new AzureBlobDataReference()
                            {
                                ConnectionString = storageConnectionString,
                                RelativeLocation = string.Format("{0}/{1}", StorageContainerName, outputModel)
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>() { }
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                // 1.a Submit job
                //Task<HttpResponseMessage> task = client.SendAsync(request);
                var response = await client.PostAsJsonAsync(BaseUrl + "?api-version=2.0", request);

                // 1.b Validate job id is fetched
                if (!response.IsSuccessStatusCode)
                {
                    response.ReasonPhrase = "Retraining failed. Error occurred at job submition step.";
                    //response.StatusCode = HttpStatusCode.PreconditionFailed;
                    return response;
                }

                var jobId = response.Content.ReadAsStringAsync();
 
                // 2. Start job
                var responseFromStart = await client.PostAsync(BaseUrl + "/" + jobId + "/start?api-version=2.0", null);
                if (!response.IsSuccessStatusCode)
                {
                    response.ReasonPhrase = "Retraining failed. Error occurred at job starting step.";
                    //response.StatusCode = HttpStatusCode.;
                    return response;
                }

                // 3. See status of job and validate success
                string jobLocation = BaseUrl + "/" + jobId + "?api-version=2.0";
                Stopwatch watch = Stopwatch.StartNew();
                bool done = false;
                while (!done)
                {
                    response = await client.GetAsync(jobLocation);
                    if (!response.IsSuccessStatusCode)
                    {
                        response.ReasonPhrase = "Retraining failed. Error occurred at getting status step.";
                        return response;
                    }

                    BatchScoreStatus status = await response.Content.ReadAsAsync<BatchScoreStatus>();
                    
                    // X. Delete job in case it is timed-out
                    if (watch.ElapsedMilliseconds > TimeOutInMilliseconds)
                    {
                        done = true;
                        response = await client.DeleteAsync(jobLocation);
                        if (!(response.StatusCode == HttpStatusCode.NoContent))
                        {
                            response.ReasonPhrase = string.Format("Retraining failed. It timed out at getting status step. Tried deleting job {0}, but this request failed as well...", jobId);
                            return response;
                        }
                        response.ReasonPhrase = string.Format("Retraining failed. It timed out at getting status step. Deleted job {0} ...", jobId);
                        return response;
                    }
                    switch (status.StatusCode)
                    {
                        case BatchScoreStatusCode.NotStarted: //Job not yet started...
                            break;
                        case BatchScoreStatusCode.Running: // Job running...
                            break;
                        case BatchScoreStatusCode.Failed: // Job failed! Can get status.Details
                            done = true;
                            break;
                        case BatchScoreStatusCode.Cancelled: // Job cancelled!
                            done = true;
                            break;
                        case BatchScoreStatusCode.Finished: // Job finished!
                            done = true;
                            break;
                    }

                    if (!done)
                    {
                        Thread.Sleep(1000); // Wait one second
                    }
                }

                return response;
            }
        }

        private string[] BuildColumnNames(JObject jsonstr)
        {
            var jsonobj = jsonstr;
            int len = jsonobj.Count;
            var columns =  new string[len];
            int i = 0;
            foreach (var item in jsonobj)
            {
                columns[i] = item.Key;
                i++;
            }

            return columns;
        }

        private string[][] BuildSingleValueMatrix(JObject jsonstr)
        {
            var jsonobj = jsonstr;
            int len = jsonobj.Count;
            var values = new string[][] { new string[len] };
            int i = 0;
            foreach (var item in jsonobj)
            {
                values[0][i] = item.Value.ToString();
                i++;
            }

            return values;
        }

        private string[] BuildTableWithColumnsAndValues()
        {
            throw new NotImplementedException();
        }

        public AzureService()
        {
            //RequestColValTable reqTable = new RequestColValTable() { ColumnsAndValues = new string[] { } };
            //IDictionary<string, RequestColValTable> valueBodyOfRequest = new Dictionary<string, RequestColValTable>() { "input1", reqTable };
            _modelFixtures = new Dictionary<string, ModelFixture>()
            {
                { 
                    "someModel",
                    new ModelFixture()
                    {
                        Models = new BaseModel[]
                         //Models = new List<TrainingModel>()
                        {
                            new TrainingModel()
                            {
                                ApiKey = "",
                                BaseURI = ""
                            },
                            new PredictionModel()
                            {
                                ApiKey = "",
                                BaseURI = ""
                            }
                        }
                    }
                }
            };
            //_modelFixtures = new Dictionary<string, IModel>()
            //{
            //    {
            //        "someModel",
            //        new TrainingModel()
            //        {
            //            ApiKey = "",
            //            BaseURI = ""
            //            //Inputs = new Dictionary<string, RequestColValTable>()
            //            //{
            //            //    "input1",
            //            //    new RequestColValTable()
            //            //    {
            //            //        ColumnsAndValues = new string[] { "" }
            //            //        //ColumnsAndValues = BuildTableWithColumnsAndValues()
            //            //    }
            //            //}
            //        }
            //    }
            //};

            _models = new Dictionary<string, ModelTable>()
            {
                {
                    "Create R Model",
                    new ModelTable()
                    {
                        ApiKey = "WUEOWmgR7n5wxVOaM0Pi9d8pXGsDbLRJs0JRkWKjqE9D7ILekzfBJom+eh04QjaICdSLq9QdBQcRFrryjHhiAQ==",
                        Uri = "https://europewest.services.azureml.net/workspaces/3fadc55951da46828a105146f089a9f5/services/10e036c01f54418d9bb5f42cf42697a4/execute?api-version=2.0&details=true",
                        ColumnNames = new string[] {
                            "Class",
                            "age",
                            "menopause",
                            "tumor-size",
                            "inv-nodes",
                            "node-caps",
                            "deg-malig",
                            "breast",
                            "breast-quad",
                            "irradiat"
                        },
                        Values = new string[][] { new string[] { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" } }
                    }
                },
                {
                    "dummy_v1.0",
                    new ModelTable()
                    {
                        ApiKey = "VnUOD/22PvgGT4MNKSFYGFhpnIYi37nmoLmxbsrg38C7tOV9mVZ9TRaY4mRiTSPgH/6GbrutFFp9l2CzEWmK2g==",
                        Uri = "https://europewest.services.azureml.net/workspaces/3fadc55951da46828a105146f089a9f5/services/88b729e66ce44db6adb2b179c2effbf1/execute?api-version=2.0&details=true",
                        ColumnNames = new string[] {
                            "Class",
                            "age",
                            "menopause",
                            "tumor-size",
                            "inv-nodes",
                            "node-caps",
                            "deg-malig",
                            "breast",
                            "breast-quad",
                            "irradiat"
                        },
                        Values = new string[][] { new string[] { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" } }
                    }
                }
            };
        }

        public bool IsValidModel(string modelName)
        {
            return _modelFixtures.ContainsKey(modelName);
        }

    }
}