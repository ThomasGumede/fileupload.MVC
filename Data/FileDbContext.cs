using fileupload.MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace fileupload.MVC.Data;

public class FileDbContext : DbContext
{
    public FileDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<FileModel> Files { get; set; }
}