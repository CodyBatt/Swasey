using System;
using System.Linq;

namespace Swasey.Normalization
{
    internal class NormalizationApiOperationResponse : NormalizationApiDataType
    {

        public NormalizationApiOperationResponse() {}

        public NormalizationApiOperationResponse(NormalizationApiOperationResponse copyFrom) : base(copyFrom)
        {
            if (copyFrom == null) return;
            CopyFrom(copyFrom);
        }

        public new string Title20 { get; set; }
    }
}
