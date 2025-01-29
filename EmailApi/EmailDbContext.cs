using EmailApi;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

public class EmailDbContext : DbContext
{
    public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options) { }

    public DbSet<EmailMessage> EmailMessage { get; set; }
}
