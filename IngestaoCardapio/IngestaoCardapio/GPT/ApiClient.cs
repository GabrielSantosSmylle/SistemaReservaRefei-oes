using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using IngestaoCardapio.Config;
using IngestaoCardapio.Utils;

namespace IngestaoCardapio
{

    public class ApiClient
    {
        private Configuracao Config = Configuracao.ObterInstancia();

        private string ApiKey;
        private string imagePath;
        private string Question;
        private string Endpoint;
        private Log Log;

        public ApiClient(Log log)
        {
            Log = log;
            ApiKey = Config.ObterConfiguracao("ApiKey");
            imagePath = Config.ObterConfiguracao("pdfName") + ".png";
            Question = File.ReadAllText(Config.ObterConfiguracao("PathQuestion")).Replace("YYYY",DateTime.Today.Year.ToString());
            Endpoint = Config.ObterConfiguracao("Endpoint");
        }

        public async Task<string> Api()
        {
            var encodedImage = Convert.ToBase64String(File.ReadAllBytes(imagePath));
            using (var httpClient = new HttpClient())
            {
                 httpClient.DefaultRequestHeaders.Add("api-key", ApiKey);
                var payload = new
                {
                    messages = new object[]
                    {
                  new {
                      role = "user",
                      content = new object[] {
                          new {
                              type = "image_url",
                              image_url = new {
                                  url = $"data:image/jpeg;base64,{encodedImage}"
                              }
                          },
                          new {
                              type = "text",
                              text = Question
                          }
                      }
                  }
                    },
                    temperature = 0.7,
                    top_p = 0.95,
                    max_tokens = 2500,
                    stream = false
                };
                try
                {
                    
                    var response =  httpClient.PostAsync(Endpoint, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));
                    response.Wait();
                    
                    if (response.Result.StatusCode != System.Net.HttpStatusCode.OK)
                    { 
                        throw new Exception("Erro ao chamar a API");
                    }
                    var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Result.Content.ReadAsStringAsync());
                    return responseData.ToString();

                }
                catch (Exception ex) {
                    return null;
                }

            }
        }
    }
}