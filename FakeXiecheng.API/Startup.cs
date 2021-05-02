using FakeXiecheng.API.Database;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Formatters;
using FakeXiecheng.API.Profiles;
using AutoMapper;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace FakeXiecheng.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                var secretByte = Encoding.UTF8.GetBytes(Configuration["Authentication:Secretkey"]);
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Authentication:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = Configuration["Authentication:Audience"],

                    ValidateLifetime = true,

                    IssuerSigningKey = new SymmetricSecurityKey(secretByte)

                };
            });
            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                //setupAction.OutputFormatters.Add(
                //    new XmlDataContractSerializerOutputFormatter());//֧��XML
            })
            .AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddXmlDataContractSerializerFormatters()//֧��XML
            .ConfigureApiBehaviorOptions(setupAction =>//���״̬��422
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetail = new ValidationProblemDetails(context.ModelState)
                    {
                        Type = "����ν",
                        Title = "������֤ʧ��",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = "�뿴��ϸ˵��",
                        Instance = context.HttpContext.Request.Path
                    };
                    problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                    return new UnprocessableEntityObjectResult(problemDetail)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });
            services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();
            services.AddDbContext<AppDbContext>(option =>
            {
                //option.UseSqlServer("server=192.168.31.157,1433;Database=FakeXiechengDb;User Id=sa;Password=Sa123456");
                //option.UseSqlServer(Configuration["DbContext:MsSQLConnectionString"]);
                option.UseMySql(Configuration["DbContext:MySQLConnectionString"]);
            });
            //ɨ��profile�ļ�
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //services.AddAutoMapper(typeof(TouristRouteProfile).Assembly, typeof(TouristRoutePictureProfile).Assembly);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FakeXiecheng.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FakeXiecheng.API v1"));
            }
            //�����ģ�
            app.UseRouting();
            //����˭��
            app.UseAuthentication();
            //����Ը�ʲô����ʲôȨ�ޣ�
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                /**
                endpoints.MapGet("/test", async context =>
                {
                    throw new Exception("Test");
                    //await context.Response.WriteAsync("Hello World test!");
                });
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
                */
            });
        }
    }
}
