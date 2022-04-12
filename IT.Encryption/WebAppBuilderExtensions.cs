using IT.Encryption.Data;
using Microsoft.EntityFrameworkCore;

namespace IT.Encryption;

public static class WebAppBuilderExtensions
{
    public static ServiceProvider MigrateDatabase(this ServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetService<AppDbContext>();
        if (context == null)
            throw new Exception("AppDbContext is not injected");

        context.Database.Migrate();
        return provider;
    }
}