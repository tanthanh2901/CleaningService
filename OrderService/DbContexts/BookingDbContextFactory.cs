using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderService.DbContexts
{
    public class BookingDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
    {
        public BookingDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory()) // đảm bảo chạy từ thư mục đúng
                            .AddJsonFile("appsettings.json") // hoặc appsettings.Development.json nếu bạn dùng
                            .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BookingDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new BookingDbContext(optionsBuilder.Options);
        }
    }
}
