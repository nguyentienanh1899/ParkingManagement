using Microsoft.AspNetCore.Builder;
using ParkingManagement.BackendServer.Helpers;

namespace ParkingManagement.BackendServer.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorWrapping(
           this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorWrappingMiddleware>();
        }
    }
}
