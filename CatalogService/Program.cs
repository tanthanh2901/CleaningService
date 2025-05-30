using CatalogService.DbContexts;
using CatalogService.Interface;
using CatalogService.Repositories;
using Microsoft.EntityFrameworkCore;
using CatalogService.AWSS3;
using Microsoft.Extensions.Options;
using Amazon.S3;
using Amazon;
using CatalogService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

builder.Services.AddDbContext<CatalogDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var assembly = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.AddAutoMapper(assembly);

builder.Services.AddControllers();

builder.Services.Configure<S3Settings>(builder.Configuration.GetSection("S3Settings"));
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var s3Settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
    var config = new AmazonS3Config
    {
        RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region)
    };

    return new AmazonS3Client(s3Settings.AccessKey, s3Settings.SecretKey, config);
});
builder.Services.AddScoped<IS3Service, S3Service>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();

