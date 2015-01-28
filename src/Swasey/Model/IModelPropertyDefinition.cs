﻿using System;
using System.Linq;

namespace Swasey.Model
{
    public interface IModelPropertyDefinition
    {
        QualifiedName Name { get; }

        DataType Type { get; }

        string Description { get; }

        bool HasDescription { get; }

        bool IsKey { get; }

        bool IsRequired { get; }

    }
}
