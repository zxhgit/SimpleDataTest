
using System.Xml.Serialization;
namespace Zx.Web.ApiContainer.model.Enum
{
    public enum ServiceErrorCodeEnum
    {
        [XmlEnum("0")]
        NoError = 0,

        [XmlEnum("10")]
        PathMissing = 10,

        [XmlEnum("11")]
        MethodMissing = 11,

        [XmlEnum("20")]
        InternalServerError = 20,

        [XmlEnum("30")]
        InvalidToken = 30,

        [XmlEnum("40")]
        ArgumentError = 40,

        [XmlEnum("41")]
        NullParameter = 41,

        [XmlEnum("42")]
        ParameterTypeErr = 42,

        [XmlEnum("43")]
        ParameterFormatErr = 43,

    }
}