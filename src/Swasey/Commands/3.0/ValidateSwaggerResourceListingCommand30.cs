using System;
using System.Linq;
using System.Threading.Tasks;

using Swasey.Lifecycle;
using Swasey.Model;

namespace Swasey.Commands
{
    internal class ValidateSwaggerResourceListingCommand30 : ILifecycleCommand
    {

        public Task<ILifecycleContext> Execute(ILifecycleContext context)
        {
            var json = context.ResourceListingJson;

            if (!json.ContainsKey("openapi") == null || string.IsNullOrWhiteSpace((string)json.openapi))
                throw new SwaseyException("swagger is required");

            //swagger, info, info.version, info.title, path are required
            if (!json.ContainsKey("info") == null)
                throw new SwaseyException("version is required");

            if (!json.info.ContainsKey("version") == null || string.IsNullOrWhiteSpace((string) json.info.version))
                throw new SwaseyException("version is required");

            if (!json.info.ContainsKey("title") == null || string.IsNullOrWhiteSpace((string) json.info.title))
                throw new SwaseyException("title is required");

            if (!json.ContainsKey("paths") == null)
                throw new SwaseyException("paths is required");

            var ctx = new LifecycleContext(context)
            {
                State = LifecycleState.Continue,
                SwaggerVersion = (string) json.openapi
            };
            ctx.ServiceMetadata = new ServiceMetadata(ctx.ServiceMetadata)
            {
                ApiVersion = (string) json.info.version
            };

            foreach (var path in json.paths)
            {
                ctx.ApiPathJsonMapping.Add((string) path.Key, path.Value);
            }

            return Task.FromResult<ILifecycleContext>(ctx);
        }

    }
}
