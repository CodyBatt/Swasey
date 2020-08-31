using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Swasey
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static partial class Constants
    {

        public const string MimeType_ApplicationOctetStream = "application/octet-stream";

        public const string ParameterName_Version = "version";

        public static readonly Dictionary<string, string> DataTypeMapping = new Dictionary<string, string>
        {
            {"boolean", "bool"}
        };

        public const string DataType_DateTime = "System.DateTime";

        public const string DataType_TimeSpan = "System.TimeSpan";
    }
}
