using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace TaskerService.DbContexts
{
    public class TaskerDbContextFactory : IDesignTimeDbContextFactory<TaskerDbContext>
    {
        public TaskerDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory()) // đảm bảo chạy từ thư mục đúng
                            .AddJsonFile("appsettings.json") // hoặc appsettings.Development.json nếu bạn dùng
                            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TaskerDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new TaskerDbContext(optionsBuilder.Options);
        }
    }

}
