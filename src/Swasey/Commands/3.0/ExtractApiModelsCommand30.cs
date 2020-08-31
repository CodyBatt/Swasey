using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Swasey.Lifecycle;
using Swasey.Normalization;

namespace Swasey.Commands
{
    internal class ExtractApiModelsCommand30 : ExtractApiModelsCommand20
    {
        private ILifecycleContext ctx;

        public override Task<ILifecycleContext> Execute(ILifecycleContext context)
        {
            ctx = new LifecycleContext(context)
            {
                State = LifecycleState.Continue
            };

            var json = context.ResourceListingJson;

            foreach (var schema in json.components.schemas)
            {
                var definitionType = (string)schema.Value.type;
                var isEnum = schema.Value.ContainsKey("enum");

                if (isEnum)
                {
                    var model = ParseEnumData(schema);
                    model.ApiNamespace = context.ApiNamespace;
                    model.ModelNamespace = context.ModelNamespace;
                    ctx.NormalizationContext.Enums.Add(model);
                }
                else
                {
                    var model = ParseModelData(schema);
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

        protected override string GetModelName(dynamic model) => model.Key;
    }
}