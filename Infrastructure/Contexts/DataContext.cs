
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<UserEntity>(options)
{
    public DbSet<AddressEntity> Addresses { get; set; }
    public DbSet<RideEntity> Rides { get; set; }
    public DbSet<BookingEntity> Bookings { get; set; }
    public DbSet<MessageEntity> Messages { get; set; }
    public DbSet<ReviewEntity> Reviews { get; set; }


}
