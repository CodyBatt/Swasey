using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Swasey.Lifecycle;

namespace Swasey.Commands
{
    internal class GenerateApiSourcesCommand20 : ILifecycleCommand
    {

        public Task<ILifecycleContext> Execute(ILifecycleContext context)
        {
            var svcDef = context.ServiceDefinition;
            var template = context.ApiOperationTemplate;
            var swaseyWriter = context.OperationWriter;

            var sb = new StringBuilder();

            foreach (var op in svcDef.ResourceOperations20.SelectMany(x => x))
            {
                using (var sw = new StringWriter(sb.Clear()))
                {
                    template(sw, op);
                }
                swaseyWriter(op.Name, sb.ToString(), op);
            }

            return Task.FromResult(context);
        }

    }
}
