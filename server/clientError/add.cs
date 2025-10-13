using System.Threading.Tasks;
using db.Models;
using db.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace server.clientError;

public class add : RequestHandler
{
    protected override async Task HandleRequest()
    {
        var text = Query["text"];
        var guid = Query["guid"];
        
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(guid))
        {
            Context.Response.StatusCode = 400;
            return;
        }
        
        await using var scope = Program.Services.CreateAsyncScope();
        var clientErrorService = scope.ServiceProvider.GetRequiredService<IClientErrorRepository>();

        var clientError = new ClientError()
        {
            Message = text,
            Uuid = guid
        };
        await clientErrorService.AddAsync(clientError);
        await clientErrorService.SaveChangesAsync();
        Context.Response.StatusCode = 200;
        
        Program.Logger.LogInformation("Client error logged from {Guid}. Inserted ID: {ClientErrorId}", guid, clientError.Id);
    }
}