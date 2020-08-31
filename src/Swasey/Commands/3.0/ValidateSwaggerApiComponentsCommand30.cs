using System;
using System.Linq;
using System.Threading.Tasks;

using Swasey.Lifecycle;

namespace Swasey.Commands
{
    internal class ValidateSwaggerApiComponentsCommand30 : ILifecycleCommand
    {

        public Task<ILifecycleContext> Execute(ILifecycleContext context)
        {
            var json = context.ResourceListingJson;

            if (!json.ContainsKey("components") || json.components.length <= 0)
            {
                throw new SwaseyException("components object is empty");
            }

            if (!json.components.ContainsKey("schemas") || json.components.schemas.length <= 0)
            {
                throw new SwaseyException("components.schemas object is empty");
            }

            return Task.FromResult(context);
        }

    }
}
