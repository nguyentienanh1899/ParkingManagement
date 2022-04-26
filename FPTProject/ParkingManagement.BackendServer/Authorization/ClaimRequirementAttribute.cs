using Microsoft.AspNetCore.Mvc;
using ParkingManagement.BackendServer.Constants;
using System;

namespace ParkingManagement.BackendServer.Authorization
{
    
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(FunctionCode functionId, CommandCode commandId)
            : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { functionId, commandId };
        }
    }
}
