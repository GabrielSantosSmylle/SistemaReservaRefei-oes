using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace IngestaoCardapio.Utils
{
    public class JsonReader
    {

        private string Json;

        public JsonReader(string json)
        {  
            Json = json;
        }

        public string ReadJsonNode(string nodeName)
        {
            JObject obj = JObject.Parse(Json);
            JToken outputNode = FindNode(obj, nodeName );

            return outputNode.ToString();
        }

        private JToken FindNode(JToken token, string nodeName)
        {
            if (token.Type == JTokenType.Object)
            {
                JObject obj = (JObject)token;

                if (obj.ContainsKey(nodeName))
                {
                    return obj[nodeName];
                }

                // Percorrer os filhos do objeto
                foreach (var child in obj.Children<JProperty>())
                {
                    JToken result = FindNode(child.Value, nodeName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                // Percorrer o array e verificar os itens
                foreach (JToken item in token)
                {
                    JToken result = FindNode(item, nodeName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

    }
}
