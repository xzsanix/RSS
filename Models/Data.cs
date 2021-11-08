using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace RSS.Models
{
    public partial class Data : DbContext
    {
        public Data()
        {
        }

        public Data(DbContextOptions<Data> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Feed> Feeds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\asust\\OneDrive\\Dokumentumok\\database.mdf;Integrated Security=True;Connect Timeout=30");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("Title")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Feed>(entity =>
            {
                entity.ToTable("Feed");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Url)
                    .HasMaxLength(200)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Feeds)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Feed_To_Category");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    }
}
