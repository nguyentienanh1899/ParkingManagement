using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParkingManagement.BackendServer.Data;
using ParkingManagement.BackendServer.Data.Entities;

[assembly: HostingStartup(typeof(ParkingManagement.BackendServer.Areas.Identity.IdentityHostingStartup))]
namespace ParkingManagement.BackendServer.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}