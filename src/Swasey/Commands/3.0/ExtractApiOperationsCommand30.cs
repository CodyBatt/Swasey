using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

using Swasey.Lifecycle;
using Swasey.Model;
using Swasey.Normalization;

namespace Swasey.Commands
{
    internal class ExtractApiOperationsCommand30 : ExtractApiOperationsCommand20
    {
        protected override IEnumerable<dynamic> ExtractApiOperations(ILifecycleContext context)
        {
            foreach (var apiKv in context.ApiPathJsonMapping)
            {
                var basePath = (string)context.ResourceListingJson.servers[0].url;
                var opPath = apiKv.Key;

                var ops = apiKv.Value;
                if (ops == null) { continue; }

                foreach (var op in ops)
                {
                    yield return new
                    {
                        BasePath = basePath,
                        OperationPath = opPath,
                        JObject = op
                    };
                }
            }
        }

        protected override NormalizationApiOperation ParseOperationData(object obj)
        {
            dynamic extractedOp = obj;
            var opObj = extractedOp.JObject;

            //I should really iterate over opObj.Value.tags here and concatenate all tags but I'm not going to.
            //My excuse is "minimum viable product."
            var op = new NormalizationApiOperation
            {
                BasePath = (string)extractedOp.BasePath,
                Path = (string)extractedOp.OperationPath,
                HttpMethod = ((string)opObj.Key).ParseHttpMethodType(),
                Description = opObj.Value.ContainsKey("summary") ? (string)opObj.Value.summary : string.Empty,
                Name = opObj.Value.ContainsKey("operationId") ? (string)opObj.Value.operationId : string.Empty,
                ResourcePath = opObj.Value.ContainsKey("tags") ? (string)opObj.Value.tags[0] : string.Empty
            };

            op.Parameters.AddRange(ParseParameters(opObj));
            op.Response = ParseResponse(opObj);
            op.SupportsStreamingUpload = ParseSupportsStreamingUpload(opObj.Value);
            op.SupportsStreamingDownload = ParseSupportsStreamingDownload(opObj.Value);

            return op;
        }

        private bool ParseSupportsStreamingUpload(dynamic op)
        {
            if (!op.ContainsKey("requestBody")) return false;
            if (!op.requestBody.ContainsKey("content")) return false;
            if (!op.requestBody.content.ContainsKey("application/octet-stream")) return false;

            return true;
        }

        private bool ParseSupportsStreamingDownload(dynamic op)
        {
            if (!op.ContainsKey("responses")) { goto ReturnFalse; }

            try
            {
                foreach (var responseKvp in op.responses)
                {
                    var response = responseKvp.Value;
                    if (response.ContainsKey("content"))
                    {
                        foreach (var ct in response.content)
                        {
                            if (ct.Key == "application/octet-stream")
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            ReturnFalse:
            return false;
        }

        NormalizationApiOperationParameter ParseBody(dynamic op)
        {
            if (!op.ContainsKey("requestBody")) return null;

            var bodyObj = op.requestBody;
            if (!bodyObj.ContainsKey("content")) return null;

            var param = new NormalizationApiOperationParameter();

            foreach(var kvp in bodyObj.content)
            {
                var ct = kvp.Key;
                if (ct != "application/json") continue;

                var pinfo = kvp.Value;
                param.CopyFrom(SimpleNormalizationApiDataType.ParseFromJObject(pinfo));
                param.ParameterType = ParameterType.Body;
                param.Name = "body";
                param.Description = bodyObj.ContainsKey("description") ? (string)bodyObj.description : string.Empty;
                param.IsRequired = pinfo.ContainsKey("required") && (bool)pinfo.required;
                return param;
            }
            return null;
        }

        private IEnumerable<NormalizationApiOperationParameter> ParseParameters(dynamic opKvp)
        {
            var op = opKvp.Value;

            var body = ParseBody(op);
            if(body != null)
            {
                yield return body;
            }

            if (!op.ContainsKey("parameters")) { goto NoMoreParameters; }


            foreach (var paramObj in op.parameters)
            {
                if (!OperationParameterFilter(paramObj)) continue;
                if (paramObj.ContainsKey("type"))
                {
                    throw new InvalidOperationException($"Unexpected type property in {paramObj.ToString()}");
                }

                if(!paramObj.ContainsKey("schema")) continue;

                var param = new NormalizationApiOperationParameter();
                param.CopyFrom(SimpleNormalizationApiDataType.ParseFromJObject(paramObj));

                // in: path, body, query etc..
                param.ParameterType = GetParamType(paramObj);
                param.AllowsMultiple = paramObj.ContainsKey("allowMultiple") && (bool) paramObj.allowMultiple;
                param.Name = paramObj.ContainsKey("name") ? (string) paramObj.name : string.Empty;
                param.IsRequired = paramObj.ContainsKey("required") && (bool)paramObj.required;

                yield return param;
            }

            NoMoreParameters:
            ;
        }

        private NormalizationApiOperationResponse ParseResponse(dynamic op)
        {
            //Not sure if this is the best way to go about handling response. 
            //In Swagger 2.0 Response type seems to be tied to the
            //response element whereas it was not in Swagger 1.2.
            dynamic dataType = op.Value;
            if (dataType.responses.ContainsKey("200"))
                dataType = dataType.responses["200"];
            else if (dataType.responses.ContainsKey("201"))
                dataType = op.Value.responses["201"];
            else if (dataType.responses.ContainsKey("204"))
                dataType = op.Value.responses["204"];
            else if (dataType.responses.ContainsKey("202"))
                dataType = op.Value.responses["202"];

            if(dataType.ContainsKey("content"))
            {
                foreach(var ct in dataType.content)
                {
                    // Take the first one
                    dataType = ct.Value;
                    break;
                }
            }
            else
            {
                return new VoidApiOperationResponse();
            }

            dataType = SimpleNormalizationApiDataType.ParseFromJObject(dataType);

            var resp = new NormalizationApiOperationResponse();
            resp.CopyFrom(dataType);
            resp.Title20 = resp.TypeName;

            return resp;
        }
    }
}
