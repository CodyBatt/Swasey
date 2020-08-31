using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Swasey.Model
{
    [DebuggerDisplay("[{ResourceName, nq}] {HttpMethod} {Name}")]
    internal class OperationDefinition20 : BaseDefinition, IOperationDefinition20
    {

        private ResponseDefinition _response;

        public OperationDefinition20(OperationPath20 path, IServiceMetadata meta) : base(meta)
//        public OperationDefinition20(IServiceMetadata meta) : base(meta)
        {
            ConsumesOctetStream = false;
            Path = path;
            Parameters = new List<ParameterDefinition>();
            ProducesOctetStream = false;
        }

        public OperationDefinition20(IOperationDefinition20 copyFrom)
//            : this(copyFrom.Metadata)
            : this(copyFrom.Path, copyFrom.Metadata)
        {
            ConsumesOctetStream = copyFrom.ConsumesOctetStream;
            Context = copyFrom.Context;
            Description = copyFrom.Description;
            HttpMethod = copyFrom.HttpMethod;
            Name = copyFrom.Name;
            ProducesOctetStream = copyFrom.ProducesOctetStream;

            copyFrom.Parameters
                .Select(x => new ParameterDefinition(x))
                .ToList()
                .ForEach(x => AddParameter(x));
            SetResponse(new ResponseDefinition(copyFrom.Response));
        }

        public string Namespace
        {
            get { return ApiNamespace; }
        }

        public List<ParameterDefinition> Parameters { get; private set; }

        public ResponseDefinition Response
        {
            get { return _response; }
            set
            {
                if (value != null)
                {
                    value.Context = this;
                }
                _response = value;
            }
        }

        public IReadOnlyList<IParameterDefinition> BodyParameters
        {
            get { return Parameters.Where(x => x.Type == ParameterType.Body).ToList(); }
        }

        public bool ConsumesOctetStream { get; set; }

        public IServiceDefinition Context { get; set; }

        public string Description { get; set; }

        public IReadOnlyList<IParameterDefinition> FormParameters
        {
            get { return Parameters.Where(x => x.Type == ParameterType.Form).ToList(); }
        }

        public bool HasBodyParameters
        {
            get { return Parameters.Any(x => x.Type == ParameterType.Body); }
        }

        public bool HasDescription
        {
            get { return !string.IsNullOrWhiteSpace(Description); }
        }

        public bool HasFormParameters
        {
            get { return Parameters.Any(x => x.Type == ParameterType.Form); }
        }

        public bool HasHeaderParameters
        {
            get { return Parameters.Any(x => x.Type == ParameterType.Header); }
        }

        public bool HasParameters
        {
            get { return Parameters.Any(); }
        }

        public bool HasPathParameters
        {
            get { return Parameters.Any(x => x.Type == ParameterType.Path); }
        }

        public bool HasQueryParameters
        {
            get { return Parameters.Any(x => x.Type == ParameterType.Query); }
        }

        public bool HasRequiredParameters
        {
            get { return Parameters.Any(x => x.IsRequired); }
        }

        public IReadOnlyList<IParameterDefinition> HeaderParameters
        {
            get { return Parameters.Where(x => x.Type == ParameterType.Header).ToList(); }
        }

        public HttpMethodType HttpMethod { get; set; }

        public QualifiedName Name { get; set; }

        IReadOnlyList<IParameterDefinition> IOperationDefinition20.Parameters
        {
            get { return Parameters; }
        }

        public OperationPath20 Path { get; private set; }

        public IReadOnlyList<IParameterDefinition> PathParameters
        {
            get { return Parameters.Where(x => x.Type == ParameterType.Path).ToList(); }
        }

        public bool ProducesOctetStream { get; set; }

        public IReadOnlyList<IParameterDefinition> QueryParameters
        {
            get { return Parameters.Where(x => x.Type == ParameterType.Query).ToList(); }
        }

        public IReadOnlyList<IParameterDefinition> RequiredParameters
        {
            get { return Parameters.Where(x => x.IsRequired).ToList(); }
        }

        public QualifiedName ResourceName { get; set; }

        IResponseDefinition IOperationDefinitionParent.Response
        {
            get { return Response; }
        }

        public OperationDefinition20 AddParameter(ParameterDefinition param)
        {
            param.Context = this;
            Parameters.Add(param);

//            if (param.Type == ParameterType.Path)
//            {
//                Path.AddPathParameter(param);
//            }

            return this;
        }

        public OperationDefinition20 SetResponse(ResponseDefinition response)
        {
            response.Context = this;
            Response = response;
            return this;
        }


    }
}
