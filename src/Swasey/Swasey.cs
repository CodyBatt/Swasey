using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Swasey.Commands;
using Swasey.Lifecycle;

namespace Swasey
{
    public class Swasey
    {

        public const SwaggerVersion DefaultSwaggerVersion = SwaggerVersion.Version20;

        public SwaggerVersion SwaggerVersion = DefaultSwaggerVersion;

        public Swasey(GeneratorOptions opts)
        {
            Options = opts;
        }

        public Swasey(GeneratorOptions opts, string ver)
        {
            Options = opts;
            switch (ver)
            {
                case null:
                    throw new Exception("Version is null.");

                case "1.0":
                    throw new SwaseyException(
                        "Howdy! Version 1.0 not implemented - yet. In the mean time check out one of the super cool implemented versions.");

                case "1.1":
                    throw new SwaseyException(
                        "Howdy! Version 1.1 not implemented - yet. In the mean time check out one of the super cool implemented versions.");

                case "1.2":
                    SwaggerVersion = SwaggerVersion.Version12;
                    break;

                case "2.0":
                    SwaggerVersion = SwaggerVersion.Version20;
                    break;

                case "3.0":
                    SwaggerVersion = SwaggerVersion.Version30;
                    break;

                default:
                    throw new Exception("I don't recognize that version. Sure you got it right?");
            }
        }

        public GeneratorOptions Options { get; private set; }

        public async Task Generate(Uri resourceListingUri)
        {
            ILifecycle lifecycle = null;
            if (SwaggerVersion == SwaggerVersion.Version12)
            {
                lifecycle = new GenerationLifecycle()
                    .Enqueue(new LoadResourceListingJsonCommand())
                    .Enqueue(new ValidateSwaggerResourceListingCommand())
                    .Enqueue(new LoadApiJsonCommand())
                    .Enqueue(new ValidateSwaggerApiDefinitionCommand())
                    .Enqueue(new ExtractApiModelsCommand())
                    .Enqueue(new ExtractApiOperationsCommand())
                    .Enqueue(new NormalizeEnumsCommand())
                    .Enqueue(new NormalizeModelsCommand())
                    .Enqueue(new NormalizeOperationsCommand())
                    .Enqueue(new GenerateApiSourcesCommand())
                    .Enqueue(new GenerateModelSourcesCommand())
                    ;
            }
            else if (SwaggerVersion == SwaggerVersion.Version20)
            {
                lifecycle = new GenerationLifecycle()
                    .Enqueue(new LoadResourceListingJsonCommand20())
                    .Enqueue(new ValidateSwaggerResourceListingCommand20())
                    .Enqueue(new ValidateSwaggerApiComponentsCommand30())
                    .Enqueue(new ExtractApiModelsCommand20())
                    .Enqueue(new ExtractApiOperationsCommand20())
                    .Enqueue(new NormalizeEnumsCommand())
                    .Enqueue(new NormalizeModelsCommand20())
                    .Enqueue(new NormalizeOperationsCommand20())
                    .Enqueue(new GenerateApiSourcesCommand20())
                    .Enqueue(new GenerateModelSourcesCommand())
                    ;
            }
            else if(SwaggerVersion == SwaggerVersion.Version30)
            {
                lifecycle = new GenerationLifecycle()
                    .Enqueue(new LoadResourceListingJsonCommand20())
                    .Enqueue(new ValidateSwaggerResourceListingCommand30())
                    .Enqueue(new ValidateSwaggerApiComponentsCommand30())
                    .Enqueue(new ExtractApiModelsCommand30())
                    .Enqueue(new ExtractApiOperationsCommand30())
                    .Enqueue(new NormalizeEnumsCommand())
                    .Enqueue(new NormalizeModelsCommand20())
                    .Enqueue(new NormalizeOperationsCommand20())
                    .Enqueue(new GenerateApiSourcesCommand20())
                    .Enqueue(new GenerateModelSourcesCommand())
                    ;
            }

            await lifecycle.Execute(
                new LifecycleContext(Options)
                {
                    ResourceListingUri = resourceListingUri
                });
        }

    }
}
