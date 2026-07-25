using System.ComponentModel;

namespace CorporateServiceDesk.Infrastructure.Pagination.Enums.Utils
{
    public enum EnumResultType
    {
        [Description("Ok")]
        Ok = 200,

        [Description("Created")]
        Created = 201,

        [Description("Accepted")]
        Accepted = 202,

        [Description("No Content")]
        NoContent = 204,

        [Description("Unauthorized")]
        Unauthorized = 401,

        [Description("Bad Request")]
        InvalidInput = 400,

        [Description("Forbidden")]
        Forbidden = 403,

        [Description("Not Found")]
        NotFound = 404,

        [Description("Method Not Allowed")]
        MethodNotAllowed = 405,

        [Description("Request Timeout")]
        RequestTimeout = 408,

        [Description("Conflict")]
        Conflict = 409,

        [Description("Internal Server Error")]
        InternalServerError = 500,

        [Description("BadGateway")]
        BadGateway = 502,

        [Description("Service Unavailable")]
        ServiceUnavailable = 503
    }
}
