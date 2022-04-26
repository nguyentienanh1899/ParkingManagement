using System.Collections.Generic;

namespace ParkingManagement.BackendServer.Constants
{
    public enum FunctionCode
    {
        DASHBOARD,

        MANAGEMENT,
        MANAGEMENT_CATEGORY,
        MANAGEMENT_PARKINGLOT,
        MANAGEMENT_TICKET,
        MANAGEMENT_BOOKINGOFFICE,
        MANAGEMENT_CAR,
        MANAGEMENT_TRIP,

        STATISTIC,
        STATISTIC_MONTHLY_NEWMEMBER,
        STATISTIC_MONTHLY_TICKETSALE,
        STATISTIC_MONTHLY_TRIP,

        SYSTEM,
        SYSTEM_USER,
        SYSTEM_ROLE,
        SYSTEM_FUNCTION,
        SYSTEM_PERMISSION,
    }
}
