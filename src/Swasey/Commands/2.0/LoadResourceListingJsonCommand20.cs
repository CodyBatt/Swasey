using System;
using System.Linq;
using System.Threading.Tasks;

using Jil;

using Swasey.Lifecycle;

namespace Swasey.Commands
{
    internal class LoadResourceListingJsonCommand20 : ILifecycleCommand
    {

        public async Task<ILifecycleContext> Execute(ILifecycleContext context)
        {
            var json = await context.Loader(context.ResourceListingUri);
            if (string.IsNullOrWhiteSpace(json))
                throw new SwaseyException("Invalid Resource Listing JSON: '{0}'", json);

            var rl = JSON.DeserializeDynamic(json);
            if (rl == null)
                throw new SwaseyException("Unable to parse Resource Listing JSON: '{0}'", context.ResourceListingJson);

            return new LifecycleContext(context)
            {
                State = LifecycleState.Continue,
                ResourceListingJson = rl,
            };
        }

    }
}
