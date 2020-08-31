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
    internal class ExtractApiOperationsCommand20 : ILifecycleCommand
    {
        public Func<dynamic, bool> OperationFilter { get; protected set; }

        public Func<dynamic, bool> OperationParameterFilter { get; protected set; }

        public Task<ILifecycleContext> Execute(ILifecycleContext context)
        {
            OperationFilter = context.OperationFilter ?? Defaults.DefaultOperationFilter;
            OperationParameterFilter = context.OperationParameterFilter ?? Defaults.DefaultOperationParameterFilter;

            var ctx = new LifecycleContext(context)
            {
                State = LifecycleState.Continue
            };

            foreach (var apiOp in ExtractApiOperations(context))
            {
                if (!OperationFilter(apiOp.JObject)) continue;

                NormalizationApiOperation op = ParseOperationData(apiOp);
                op.ApiNamespace = context.ApiNamespace;
                op.ModelNamespace = context.ModelNamespace;

                op.Response.ApiNamespace = context.ApiNamespace;
                op.Response.ModelNamespace = context.ModelNamespace;

                foreach (var param in op.Parameters)
                {
                    param.ApiNamespace = context.ApiNamespace;
                    param.ModelNamespace = context.ModelNamespace;
                }

                ctx.NormalizationContext.Operations.Add(op);
            }

            return Task.FromResult<ILifecycleContext>(ctx);
        }

        protected virtual IEnumerable<dynamic> ExtractApiOperations(ILifecycleContext context)
        {
            foreach (var apiKv in context.ApiPathJsonMapping)
            {
                var basePath = (string) context.ResourceListingJson.basePath;
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
//                }
            }
        }

        protected virtual NormalizationApiOperation ParseOperationData(object obj)
        {
            dynamic extractedOp = obj;
            var opObj = extractedOp.JObject;

            //I should really iterate over opObj.Value.tags here and concatenate all tags but I'm not going to.
            //My excuse is "minimum viable product."
            var op = new NormalizationApiOperation
            {
                BasePath = (string) extractedOp.BasePath,
                Path = (string) extractedOp.OperationPath,
                HttpMethod = ((string)opObj.Key).ParseHttpMethodType(),
                Description = opObj.Value.ContainsKey("summary") ? (string) opObj.Value.summary : string.Empty,
                Name = opObj.Value.ContainsKey("operationId") ? (string) opObj.Value.operationId : string.Empty,
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
            if (!op.ContainsKey("consumes")) { goto ReturnFalse; }

            try
            {
                foreach (var ctype in op.consumes)
                {
                    var ct = (string) ctype;
                    if (Constants.MimeType_ApplicationOctetStream.Equals(ct, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
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

        private bool ParseSupportsStreamingDownload(dynamic op)
        {
            if (!op.ContainsKey("produces")) { goto ReturnFalse; }

            try
            {
                foreach (var ctype in op.produces)
                {
                    var ct = (string) ctype;
                    if (Constants.MimeType_ApplicationOctetStream.Equals(ct, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
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

        private IEnumerable<NormalizationApiOperationParameter> ParseParameters(dynamic opKvp)
        {
            var op = opKvp.Value;

            if (!op.ContainsKey("parameters")) { goto NoMoreParameters; }

            foreach (var paramObj in op.parameters)
            {
                if (!OperationParameterFilter(paramObj)) continue;
                if (!paramObj.ContainsKey("type") && !paramObj.ContainsKey("schema")) continue;

                var param = new NormalizationApiOperationParameter();
                param.CopyFrom(SimpleNormalizationApiDataType.ParseFromJObject(paramObj));

                param.AllowsMultiple = paramObj.ContainsKey("allowMultiple") && (bool) paramObj.allowMultiple;
                param.Description = paramObj.ContainsKey("description") ? (string) paramObj.description : string.Empty;
                param.Name = paramObj.ContainsKey("name") ? (string) paramObj.name : string.Empty;
                param.ParameterType = GetParamType(paramObj);
                param.IsRequired = paramObj.ContainsKey("required") && (bool) paramObj.required;

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
            if(dataType.responses.ContainsKey("200"))
                dataType = dataType.responses["200"];
            else if(dataType.responses.ContainsKey("201"))
                dataType = op.Value.responses["201"];
            else if (dataType.responses.ContainsKey("204"))
                dataType = op.Value.responses["204"];
            else if (dataType.responses.ContainsKey("202"))
                dataType = op.Value.responses["202"];

            string title = "";
            if(dataType.ContainsKey("schema"))
                if(dataType.schema.ContainsKey("title"))
                    title = dataType.schema["title"];

            dataType = SimpleNormalizationApiDataType.ParseFromJObject(dataType);

            var resp = new NormalizationApiOperationResponse();
            resp.CopyFrom(dataType);
            resp.Title20 = title;

            return resp;
        }

        protected ParameterType GetParamType(dynamic op)
        {
            var p = ((string) op["in"]).Trim().ToLowerInvariant();
            return p.ParseParameterType();
        }

    }
}
