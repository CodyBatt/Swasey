using System;
using System.Linq;
using System.Threading.Tasks;

using Swasey.Lifecycle;

namespace Swasey.Commands
{
    internal class ValidateSwaggerApiDefinitionsCommand20 : ILifecycleCommand
    {

        public Task<ILifecycleContext> Execute(ILifecycleContext context)
        {
            var json = context.ResourceListingJson;

            if (json.ContainsKey("definitions") && json.definitions.length <= 0)
            {
                throw new SwaseyException("definitions object is empty");
            }

            return Task.FromResult(context);
        }

    }
}
