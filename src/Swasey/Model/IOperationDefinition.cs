﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Swasey.Model
{
    public interface IOperationDefinition : IOperationDefinitionParent
    {

        IReadOnlyList<IParameterDefinition> BodyParameters { get; }

        bool ConsumesOctetStream { get; }

        string Description { get; }

        IReadOnlyList<IParameterDefinition> FormParameters { get; }

        bool HasBodyParameters { get; }

        bool HasDescription { get; }

        bool HasFormParameters { get; }

        bool HasHeaderParameters { get; }

        bool HasParameters { get; }

        bool HasPathParameters { get; }

        bool HasQueryParameters { get; }

        bool HasRequiredParameters { get; }

        IReadOnlyList<IParameterDefinition> HeaderParameters { get; }

        HttpMethodType HttpMethod { get; }

        IReadOnlyList<IParameterDefinition> Parameters { get; }

        OperationPath Path { get; }

        IReadOnlyList<IParameterDefinition> PathParameters { get; }

        bool ProducesOctetStream { get; }

        IReadOnlyList<IParameterDefinition> QueryParameters { get; }

        IReadOnlyList<IParameterDefinition> RequiredParameters { get; }
    }
}
