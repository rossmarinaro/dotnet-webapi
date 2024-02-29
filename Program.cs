using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDirectoryBrowser();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

//app.UseAuthorization();

app.UseHttpsRedirection();
app.UseRouting();

var sql_handler =  new SQLHandler();

app.Use(async (context, next) => {
    var headers = context.Response.Headers;
    headers.Add("Cross-Origin-Opener-Policy", "same-origin");
    headers.Add("Cross-Origin-Embedder-Policy", "require-corp");
    await next.Invoke(); 
});

app.MapGet("/get", async () => {
   var data = await sql_handler.GetData();
   return data;
});

app.MapGet("/get/{key}", (string key) => {
   return "data " + key + " received.";
});

app.MapPost("/set", async (HttpRequest req) => {

   string content = string.Empty;

   using (var body = new StreamReader(req.Body)) 
        content = await body.ReadToEndAsync();

    Model json = JsonSerializer.Deserialize<Model>(content);

   sql_handler.SetData(json);
   Console.WriteLine("request: " + content);

   return content;
});

app.Run();

