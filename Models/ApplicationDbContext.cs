using Microsoft.EntityFrameworkCore;

namespace user_service_api.Models;

public class ApplicationDbContext:DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

 public DbSet<UserInfo?> User { get; set; } 
 public DbSet<Role?> Role { get; set; } 
 public DbSet<Permission?> Permission { get; set; } 
 public DbSet<RolePermission?> RolePermission { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 在这里定义实体与数据库表之间的映射关系
        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.ToTable("users"); // 指定实体映射到的表名
            entity.HasKey(e => e.Id); // 指定主键
        });
        
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles"); // 指定实体映射到的表名
            entity.HasKey(e => e.Id); // 指定主键
        });
        
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("permissions"); // 指定实体映射到的表名
            entity.HasKey(e => e.Id); // 指定主键
        });
        
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("role_permissions"); // 指定实体映射到的表名
            entity.HasKey(e => e.Id); // 指定主键
        });
        // 其他的 entity
        base.OnModelCreating(modelBuilder);
    }
}