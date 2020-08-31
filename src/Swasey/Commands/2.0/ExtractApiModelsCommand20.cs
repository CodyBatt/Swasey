using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Swasey.Lifecycle;
using Swasey.Normalization;

namespace Swasey.Commands
{
    internal class ExtractApiModelsCommand20 : ILifecycleCommand
    {
        private ILifecycleContext ctx;

        public virtual Task<ILifecycleContext> Execute(ILifecycleContext context)
        {
            ctx = new LifecycleContext(context)
            {
                State = LifecycleState.Continue
            };

            var json = context.ResourceListingJson;

            foreach (var definition in json.definitions)
            {
                var definitionType = (string) definition.Value.type;
                var isEnum = "enum".Equals(definitionType, StringComparison.InvariantCultureIgnoreCase);

                if (isEnum)
                {
                    var model = ParseEnumData(definition);
                    model.ApiNamespace = context.ApiNamespace;
                    model.ModelNamespace = context.ModelNamespace;
                    ctx.NormalizationContext.Enums.Add(model);
                }
                else
                {
                    var model = ParseModelData(definition);
                    model.ApiNamespace = context.ApiNamespace;
                    model.ModelNamespace = context.ModelNamespace;

                    ctx.NormalizationContext.Models.Add(model);
                }
            }

            foreach (var model in ctx.NormalizationContext.Models.Where(x => x.RawSubTypes.Any()))
            {
                foreach (var st in model.RawSubTypes)
                {
                    var sm = ctx.NormalizationContext.Models.FirstOrDefault(x => x.Name.Equals(st, StringComparison.InvariantCultureIgnoreCase));
                    if (sm == null) continue;
                    model.SubTypes.Add(sm);
                }
            }

            var enumNames = ctx.NormalizationContext.Enums.Select(x => x.Name).ToList();
            var modelNames = ctx.NormalizationContext.Models.Select(x => x.Name).ToList();

            // Ensure that Enum Properties are properly indicated
            ctx.NormalizationContext.Models
                .SelectMany(x => x.Properties)
                .Where(x => enumNames.Contains(x.TypeName) && !modelNames.Contains(x.TypeName))
                .ToList()
                .ForEach(x => x.IsEnum = true);

            return Task.FromResult<ILifecycleContext>(ctx);
        }

        protected NormalizationApiModelEnum ParseEnumData(dynamic item)
        {
            dynamic model = item.Value;

            var normEnum = new NormalizationApiModelEnum
            {
                Name = (string) item.Key,
            };

            if (model.ContainsKey("description") && !string.IsNullOrWhiteSpace((string) model.description))
            {
                normEnum.Description = (string) model.description;
            }

            var type = new SimpleNormalizationApiDataType(model);
            type.TryParseEnumFromJObject(model);

            var values = type.EnumValues;
            if (values.Any()) normEnum.Values.AddRange(values);

            return normEnum;
        }

        protected virtual string GetModelName(dynamic model) => model.Value.title;

        protected NormalizationApiModel ParseModelData(dynamic item)
        {
           dynamic model = item.Value;

            var normModel = new NormalizationApiModel
            {
                Name = GetModelName(item),
            };

            if (model.ContainsKey("subTypes"))
            {
                foreach (var sto in model.subTypes)
                {
                    if (sto == null) continue;
                    var st = (string) sto;
                    if (string.IsNullOrWhiteSpace(st)) continue;

                    normModel.RawSubTypes.Add(st);
                }
            }

            if (model.ContainsKey("description") && !string.IsNullOrWhiteSpace((string) model.description))
            {
                normModel.Description = (string) model.description;
            }

            var requiredProperties = ParseRequiredProperties(model);

            foreach (NormalizationApiModelProperty prop in ParseProperties(model))
            {
                if (requiredProperties.Contains(prop.Name))
                {
                    prop.IsRequired = true;
                }
                normModel.Properties.Add(prop);
            }

            return normModel;
        }

        protected IEnumerable<NormalizationApiModelProperty> ParseProperties(dynamic model)
        {
            if (!model.ContainsKey("properties")) goto NoMoreProperties;

            foreach (var propKv in model.properties)
            {

                var obj = propKv.Value;

                var prop = new NormalizationApiModelProperty
                {
                    Name = propKv.Key
                };

                prop.CopyFrom(SimpleNormalizationApiDataType.ParseFromJObject(obj));
                
                if (obj.ContainsKey("description"))
                {
                    prop.Description = obj.description;
                }
                if (obj.ContainsKey("key"))
                {
                    prop.IsKey = (bool) obj.key;
                }

                yield return prop;
            }

        NoMoreProperties:
            yield break;
        }

        protected HashSet<string> ParseRequiredProperties(dynamic model)
        {
            var requiredProperties = new HashSet<string>();

            if (model.ContainsKey("required"))
            {
                foreach (var n in model.required)
                {
                    var val = (string) n;
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        requiredProperties.Add(val);
                    }
                }
            }
            return requiredProperties;
        }
    }
}