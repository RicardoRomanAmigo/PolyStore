using Hangfire;
using PolyStore.Application.Abstractions.Services;

namespace PolyStore.Api.Extensions;

public static class HangfireExtensions
{
    public static void ConfigureRecurringJobs(this IApplicationBuilder app)
    {
        // Resolvemos el servicio desde el proveedor de servicios de la aplicación
        var recurringJobManager = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<IOrderService>(
            "cancel-expired-orders",
            service => service.CancelExpiredOrders(),
            Cron.MinuteInterval(5)
        );
    }
}