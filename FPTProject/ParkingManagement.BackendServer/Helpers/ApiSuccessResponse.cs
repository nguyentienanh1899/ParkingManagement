namespace ParkingManagement.BackendServer.Helpers
{
    public class ApiSuccessResponse :ApiResponse
    {
        public object Respone { get; }
        public ApiSuccessResponse(object respone) :base(200)
        {
            Respone = respone;
        }
    }
}
