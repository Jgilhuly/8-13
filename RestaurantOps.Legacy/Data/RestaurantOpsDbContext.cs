using Microsoft.EntityFrameworkCore;
using RestaurantOps.Legacy.Models;

namespace RestaurantOps.Legacy.Data
{
	public class RestaurantOpsDbContext : DbContext
	{
		public RestaurantOpsDbContext(DbContextOptions<RestaurantOpsDbContext> options) : base(options)
		{
		}

		public DbSet<Category> Categories => Set<Category>();
		public DbSet<MenuItem> MenuItems => Set<MenuItem>();
		public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
		public DbSet<Order> Orders => Set<Order>();
		public DbSet<OrderLine> OrderLines => Set<OrderLine>();
		public DbSet<Ingredient> Ingredients => Set<Ingredient>();
		public DbSet<InventoryTx> InventoryTx => Set<InventoryTx>();
		public DbSet<Employee> Employees => Set<Employee>();
		public DbSet<Shift> Shifts => Set<Shift>();
		public DbSet<TimeOff> TimeOff => Set<TimeOff>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Table names
			modelBuilder.Entity<Category>().ToTable("Categories").HasKey(c => c.CategoryId);
			modelBuilder.Entity<MenuItem>().ToTable("MenuItems").HasKey(m => m.MenuItemId);
			modelBuilder.Entity<RestaurantTable>().ToTable("RestaurantTables").HasKey(t => t.TableId);
			modelBuilder.Entity<Order>().ToTable("Orders").HasKey(o => o.OrderId);
			modelBuilder.Entity<OrderLine>().ToTable("OrderLines").HasKey(ol => ol.OrderLineId);
			modelBuilder.Entity<Ingredient>().ToTable("Ingredients").HasKey(i => i.IngredientId);
			modelBuilder.Entity<InventoryTx>().ToTable("InventoryTx").HasKey(tx => tx.TxId);
			modelBuilder.Entity<Employee>().ToTable("Employees").HasKey(e => e.EmployeeId);
			modelBuilder.Entity<Shift>().ToTable("Shifts").HasKey(s => s.ShiftId);
			modelBuilder.Entity<TimeOff>().ToTable("TimeOff").HasKey(t => t.TimeOffId);

			// Relationships (minimal, align with existing schema)
			modelBuilder.Entity<MenuItem>()
				.HasOne<Category>()
				.WithMany()
				.HasForeignKey(m => m.CategoryId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Order>()
				.HasOne<RestaurantTable>()
				.WithMany()
				.HasForeignKey(o => o.TableId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Order>()
				.HasMany(o => o.Lines)
				.WithOne()
				.HasForeignKey(l => l.OrderId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<OrderLine>()
				.HasOne<MenuItem>()
				.WithMany()
				.HasForeignKey(l => l.MenuItemId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<InventoryTx>()
				.HasOne<Ingredient>()
				.WithMany()
				.HasForeignKey(tx => tx.IngredientId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Shift>()
				.HasOne<Employee>()
				.WithMany()
				.HasForeignKey(s => s.EmployeeId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<TimeOff>()
				.HasOne<Employee>()
				.WithMany()
				.HasForeignKey(t => t.EmployeeId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Order>()
				.Property(o => o.Status)
				.HasMaxLength(20)
				.HasDefaultValue("Open");

			// Column configuration to match existing schema
			modelBuilder.Entity<Category>()
				.Property(c => c.Name).HasMaxLength(100);
			modelBuilder.Entity<Category>()
				.Property(c => c.Description).HasMaxLength(500);

			modelBuilder.Entity<MenuItem>()
				.Property(m => m.Name).HasMaxLength(150);
			modelBuilder.Entity<MenuItem>()
				.Property(m => m.Description).HasMaxLength(1000);
			modelBuilder.Entity<MenuItem>()
				.Property(m => m.Price).HasPrecision(10, 2);
			modelBuilder.Entity<MenuItem>()
				.Property(m => m.IsAvailable).HasDefaultValue(true);

			modelBuilder.Entity<RestaurantTable>()
				.Property(t => t.Name).HasMaxLength(50);
			modelBuilder.Entity<RestaurantTable>()
				.Property(t => t.IsOccupied).HasDefaultValue(false);

			modelBuilder.Entity<OrderLine>()
				.Property(ol => ol.PriceEach).HasPrecision(10, 2);

			modelBuilder.Entity<Ingredient>()
				.Property(i => i.Unit).HasMaxLength(20);
			modelBuilder.Entity<Ingredient>()
				.Property(i => i.QuantityOnHand).HasPrecision(10, 2);
			modelBuilder.Entity<Ingredient>()
				.Property(i => i.ReorderThreshold).HasPrecision(10, 2);

			modelBuilder.Entity<InventoryTx>()
				.Property(tx => tx.QuantityChange).HasPrecision(10, 2);
			modelBuilder.Entity<InventoryTx>()
				.Property(tx => tx.Notes).HasMaxLength(255);

			modelBuilder.Entity<Employee>()
				.Property(e => e.FirstName).HasMaxLength(50);
			modelBuilder.Entity<Employee>()
				.Property(e => e.LastName).HasMaxLength(50);
			modelBuilder.Entity<Employee>()
				.Property(e => e.Role).HasMaxLength(30);
			modelBuilder.Entity<Employee>()
				.Property(e => e.HireDate).HasColumnType("date");

			modelBuilder.Entity<Shift>()
				.Property(s => s.ShiftDate).HasColumnType("date");
			modelBuilder.Entity<Shift>()
				.Property(s => s.StartTime).HasColumnType("time(0)");
			modelBuilder.Entity<Shift>()
				.Property(s => s.EndTime).HasColumnType("time(0)");
			modelBuilder.Entity<Shift>()
				.Property(s => s.Role).HasMaxLength(30);

			modelBuilder.Entity<TimeOff>()
				.Property(t => t.StartDate).HasColumnType("date");
			modelBuilder.Entity<TimeOff>()
				.Property(t => t.EndDate).HasColumnType("date");
			modelBuilder.Entity<TimeOff>()
				.Property(t => t.Status).HasMaxLength(20).HasDefaultValue("Pending");

			// Ignore convenience/non-mapped properties
			modelBuilder.Entity<MenuItem>().Ignore(m => m.CategoryName);
			modelBuilder.Entity<OrderLine>().Ignore(l => l.LineTotal).Ignore(l => l.MenuItemName);
			modelBuilder.Entity<Ingredient>().Ignore(i => i.NeedsReorder);
			modelBuilder.Entity<InventoryTx>().Ignore(tx => tx.IngredientName);
			modelBuilder.Entity<Employee>().Ignore(e => e.FullName);
			modelBuilder.Entity<Shift>().Ignore(s => s.EmployeeName);
			modelBuilder.Entity<TimeOff>().Ignore(t => t.EmployeeName);

			base.OnModelCreating(modelBuilder);
		}
	}
}


