using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using db.Models;
using Microsoft.Extensions.DependencyInjection;

namespace server.inGameNews;

public class getNews : RequestHandler
{
    protected override async Task HandleRequest()
    {
        await using var scope = Program.Services.CreateAsyncScope();
        var newsService = scope.ServiceProvider.GetRequiredService<db.Repositories.INewsRepository>();
        
        // return json with empty array
        List<News> newsItems = [];
        
        var json = System.Text.Json.JsonSerializer.Serialize(newsItems);
        Context.Response.ContentType = "application/json";
        
        await using var writer = new StreamWriter(Context.Response.OutputStream, Encoding.UTF8);
        await writer.WriteAsync(json);
        await writer.FlushAsync();
        await Context.Response.OutputStream.FlushAsync();
        Context.Response.Close();
    }
}