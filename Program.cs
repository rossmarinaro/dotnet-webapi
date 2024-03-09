using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
   Args = args,
   WebRootPath = "wwwroot/meatball-madness"
});

builder.Services.AddDirectoryBrowser();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithViews();

var app = builder.Build();

//app.UseDirectoryBrowser();
app.UseHttpsRedirection();

app.UseDefaultFiles(); 
app.UseStaticFiles(new StaticFileOptions {  
      OnPrepareResponse = packet => {
         packet.Context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");
         packet.Context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "require-corp");
         packet.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*"); 
      },
      ServeUnknownFileTypes = true
   }
);

app.Use(async (context, next) => { 
   
   context.Response.OnStarting(() => {

      Console.WriteLine("Context: " + context.ToString());
      return Task.CompletedTask;
   });

   await next(); 

});

//app.UseStaticFiles();
//app.UseRouting();

var sql_handler = new SQLHandler();

app.MapDefaultControllerRoute();

app.MapGet("/get/{key}", async (HttpRequest req) => {
   Console.WriteLine("Request: " + req.ToString());
   var data = await sql_handler.GetData();
   return data;
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


//ASPNETCORE_URLS="http://localhost:8000"
//var PORT = Document.GetEnvironmentVariable("PORT") ?? "http://localhost:5024";

app.Run(/* PORT */);

