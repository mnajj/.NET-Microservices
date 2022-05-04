using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
  public class Startup
  {
    public IConfiguration Configuration { get; }
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
      Configuration = configuration;
      _env = env;
    }


    public void ConfigureServices(IServiceCollection services)
    {
      if (_env.IsProduction())
      {
        Console.WriteLine("--> Using SqlServer Db");
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(Configuration.GetConnectionString("PlatformsConnectionString")));
      }
      else
      {
        Console.WriteLine("--> Using InMem Db");
        services.AddDbContext<AppDbContext>(opt =>
             opt.UseInMemoryDatabase("InMem"));
      }

      services.AddScoped<IPlatformRepo, PlatformRepo>();

      services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
      services.AddControllers();
      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
      });

      Console.WriteLine($"--> CommandService Endpoint {Configuration["CommandService"]}");

    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
        app.UseHttpsRedirection();
      }

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapGet("/protos/platforms.proto", async context =>
        {
          await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
        });
      });

      PrepDb.PrepPopulation(app, env.IsProduction());
    }
  }
}