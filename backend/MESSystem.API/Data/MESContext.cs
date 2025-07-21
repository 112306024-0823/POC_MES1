using Microsoft.EntityFrameworkCore;
using MESSystem.API.Models;

namespace MESSystem.API.Data;

/// <summary>
/// MES 系統資料庫上下文
/// </summary>
public class MESContext : DbContext
{
    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="options">資料庫選項</param>
    public MESContext(DbContextOptions<MESContext> options) : base(options)
    {
    }

    /// <summary>
    /// 使用者資料表
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// 進貨到廠預估表
    /// </summary>
    public DbSet<DeliveryOverview> DeliveryOverviews { get; set; }

    /// <summary>
    /// 模型建立時的配置
    /// </summary>
    /// <param name="modelBuilder">模型建構器</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 設定 User 實體
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Factory).HasConversion<int>();
            entity.HasIndex(e => new { e.Username, e.Factory }).IsUnique();
        });

        // 設定 DeliveryOverview 實體
        modelBuilder.Entity<DeliveryOverview>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BlNo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Customer).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Style).HasMaxLength(50);
            entity.Property(e => e.PoNo).HasMaxLength(50);
            entity.Property(e => e.Rolls).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ArriveStatus).HasConversion<int>();
            entity.Property(e => e.Factory).HasConversion<int>();
        });

        // 種子資料
        SeedData(modelBuilder);
    }

    /// <summary>
    /// 種子資料
    /// </summary>
    /// <param name="modelBuilder">模型建構器</param>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        // 建立測試使用者
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Factory = Factory.TPL
            },
            new User
            {
                Id = 2,
                Username = "user1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Factory = Factory.NVN
            },
            new User
            {
                Id = 3,
                Username = "user2",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Factory = Factory.LR
            }
        );

        // 建立測試進貨資料
        modelBuilder.Entity<DeliveryOverview>().HasData(
            new DeliveryOverview
            {
                Id = 1,
                BlNo = "DFS067185048",
                Customer = "KOH",
                Style = "EX5HA401, MXSHJ431",
                PoNo = "TMKT-25-04335, TM...",
                Rolls = 715.00m,
                Etd = new DateTime(2025, 7, 13),
                Eta = new DateTime(2025, 7, 20),
                FtyEta = new DateTime(2025, 7, 24),
                ArriveStatus = ArriveStatus.OnTime,
                Factory = Factory.TPL
            },
            new DeliveryOverview
            {
                Id = 2,
                BlNo = "DFS067185059",
                Customer = "KOH",
                Style = "WX51A302, HO25, W...",
                PoNo = "TMKT-25-04390, TM...",
                Rolls = 371.00m,
                Etd = new DateTime(2025, 7, 12),
                Eta = new DateTime(2025, 7, 19),
                FtyEta = new DateTime(2025, 7, 24),
                ArriveStatus = ArriveStatus.OnTime,
                Factory = Factory.TPL
            },
            new DeliveryOverview
            {
                Id = 3,
                BlNo = "DFS067184961",
                Customer = "ONY",
                Style = "804760_HO25, 82639...",
                PoNo = "TMKT-25-03503, TM...",
                Rolls = 896.00m,
                Etd = new DateTime(2025, 7, 10),
                Eta = new DateTime(2025, 7, 17),
                FtyEta = new DateTime(2025, 7, 23),
                ArriveStatus = ArriveStatus.OnTime,
                Factory = Factory.TPL
            },
            new DeliveryOverview
            {
                Id = 4,
                BlNo = "DFS067184975",
                Customer = "KOH",
                Style = "WX5HA124",
                PoNo = "TMKT-25-04096",
                Rolls = 39.00m,
                Etd = new DateTime(2025, 7, 9),
                Eta = new DateTime(2025, 7, 16),
                FtyEta = new DateTime(2025, 7, 22),
                ArriveStatus = ArriveStatus.OnTime,
                Factory = Factory.TPL
            },
            new DeliveryOverview
            {
                Id = 5,
                BlNo = "978-22774452",
                Customer = "KOH",
                Style = "DXSFA011",
                PoNo = "TMKT-25-02706",
                Rolls = 3.00m,
                Etd = new DateTime(2025, 7, 16),
                Eta = new DateTime(2025, 7, 17),
                FtyEta = new DateTime(2025, 7, 21),
                ArriveStatus = ArriveStatus.OnTime,
                Factory = Factory.TPL
            },
            new DeliveryOverview
            {
                Id = 6,
                BlNo = "DFS067184970",
                Customer = "ONY",
                Style = "804760_FA25",
                PoNo = "TMKT-25-01914/TM...",
                Rolls = 113.00m,
                Etd = new DateTime(2025, 7, 4),
                Eta = new DateTime(2025, 7, 11),
                FtyEta = new DateTime(2025, 7, 17),
                ArriveStatus = ArriveStatus.Delayed,
                Factory = Factory.TPL
            },
            new DeliveryOverview
            {
                Id = 7,
                BlNo = "EXTC/2025-00...",
                Customer = "ONY",
                Style = "842197_HO25",
                PoNo = "TMKT-25-03351",
                Rolls = 87.00m,
                Etd = new DateTime(2025, 7, 16),
                Eta = new DateTime(2025, 7, 16),
                FtyEta = new DateTime(2025, 7, 16),
                ArriveStatus = ArriveStatus.Delayed,
                Factory = Factory.TPL
            },
            new DeliveryOverview
            {
                Id = 8,
                BlNo = "25TRUCKS36",
                Customer = "TGT",
                Style = "M9232P_C425",
                PoNo = "TMKT-25-01181",
                Rolls = 480.00m,
                Etd = new DateTime(2025, 7, 14),
                Eta = new DateTime(2025, 7, 10),
                FtyEta = new DateTime(2025, 7, 10),
                ArriveStatus = ArriveStatus.Delayed,
                Factory = Factory.TPL
            },
            new DeliveryOverview
            {
                Id = 9,
                BlNo = "GT2506AE0013",
                Customer = "NET",
                Style = "273841_ALL,2/3846...",
                PoNo = "TMKT-25-00955, TM...",
                Rolls = 75.00m,
                Etd = new DateTime(2025, 6, 17),
                Eta = new DateTime(2025, 6, 18),
                FtyEta = new DateTime(2025, 6, 21),
                ArriveStatus = ArriveStatus.Delayed,
                Factory = Factory.TPL
            }
        );
    }
} 