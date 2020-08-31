using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Swasey.Lifecycle;
using Swasey.Model;
using Swasey.Normalization;

namespace Swasey.Commands
{
    internal class NormalizeModelsCommand20 : ILifecycleCommand
    {

        private readonly List<NormalModelName> _modelNameMaps = new List<NormalModelName>();

        private readonly Dictionary<string, int> _usedNames = new Dictionary<string, int>();

        public Task<ILifecycleContext> Execute(ILifecycleContext context)
        {
            var serviceDefinition = new ServiceDefinition(context.ServiceDefinition);

            foreach (var normalModel in context.NormalizationContext.Models)
            {
                var normalModelName = new NormalModelName(normalModel.ResourcePath, normalModel.Name);
                _modelNameMaps.Add(normalModelName);

                var modelDef = new ModelDefinition(normalModel.AsMetadata())
                {
                    ContextName = normalModel.ResourceName,
                    Description = normalModel.Description,
                    Name = normalModel.Name.MapDataTypeName(),
                };
                normalModelName.New = modelDef.Name;

                modelDef.Properties.AddRange(ExtractModelProperties(normalModel.Properties));

                serviceDefinition.AddModel(modelDef);
            }

            var resourceModelLookup = _modelNameMaps.ToLookup(x => x.ResourcePath);

            // Ensure all Operations refer to the proper model names
            foreach (var op in context.NormalizationContext.Operations)
            {
                if (!resourceModelLookup.Contains(op.ResourcePath)) { continue; }

                var resourceModels = resourceModelLookup[op.ResourcePath].ToList();

                var returnItem = resourceModels.SingleOrDefault(x => op.Response.TypeName.Equals(x.Old));
                if (returnItem != null)
                {
                    op.Response.SetTypeName(returnItem.New);
                }

                foreach (var opParam in op.Parameters)
                {
                    var paramItem = resourceModels.SingleOrDefault(x => opParam.TypeName.Equals(x.Old));
                    if (paramItem != null)
                    {
                        opParam.SetTypeName(paramItem.New);
                    }
                }
            }


            var ctx = new LifecycleContext(context)
            {
                ServiceDefinition = serviceDefinition,
                State = LifecycleState.Continue
            };
            return Task.FromResult<ILifecycleContext>(ctx);
        }

        private IEnumerable<ModelPropertyDefinition> ExtractModelProperties(IEnumerable<NormalizationApiModelProperty> properties)
        {
            foreach (var normalProp in properties)
            {
                yield return new ModelPropertyDefinition(normalProp.AsMetadata())
                {
                    Name = normalProp.Name.MapDataTypeName(),
                    Description = normalProp.Description,
                    IsKey = normalProp.IsKey,
                    IsRequired = normalProp.IsRequired,
                    Type = normalProp.AsDataType()
                };
            }
        }

        private string NormalizeModelName(ModelDefinition def, string desiredName)
        {
            int count;
            if (!_usedNames.TryGetValue(desiredName, out count))
            {
                _usedNames.Add(desiredName, ++count);
                return desiredName;
            }

            var isFirstAttempt = !desiredName.StartsWith(def.ContextName);
            desiredName = def.ContextName + def.Name;
            if (!isFirstAttempt)
            {
                desiredName += count;
            }
            return NormalizeModelName(def, desiredName);
        }

        private class NormalModelName
        {

            public NormalModelName(string resourcePath, string old)
            {
                ResourcePath = resourcePath;
                Old = old;
                New = old;
            }

            public string New { get; set; }

            public string Old { get; private set; }

            public string ResourcePath { get; private set; }

        }

    }
}
