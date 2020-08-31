using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Swasey
{
    public enum SwaggerVersion
    {
        [EnumMember(Value = "1.0")]
        Version10,
        
        [EnumMember(Value = "1.1")]
        Version11,

        [EnumMember(Value = "1.2")]
        Version12,

        [EnumMember(Value = "2.0")]
        Version20,

        [EnumMember(Value = "3.0")]
        Version30

    }
}
