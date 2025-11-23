using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class UniversityDbContext : DbContext
    {
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options)
        {
        }

        public DbSet<Enrollment> Enrollmenty => Set<Enrollment>();

        public DbSet<Gabinet> Gabinety => Set<Gabinet>(); // opcjonalnie, tabela będzie tak czy siak. DbSet ułatwi dostęp. Na razie zostaje

        public DbSet<Kurs> Kursy => Set<Kurs>();
        public DbSet<LicznikIndeksow> LicznikiIndeksow => Set<LicznikIndeksow>();
        public DbSet<Profesor> Profesorzy => Set<Profesor>();
        public DbSet<Student> Studenci => Set<Student>();
        public DbSet<Wydzial> Wydzialy => Set<Wydzial>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Owned Types =====
            modelBuilder.Entity<Student>().OwnsOne(s => s.AdresZamieszkania);
            modelBuilder.Entity<Profesor>().OwnsOne(p => p.AdresZamieszkania);

            // ===== Enrollment (many-to-many z payload) =====
            modelBuilder.Entity<Enrollment>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollmenty)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Kurs)
                .WithMany(k => k.Enrollmenty)
                .HasForeignKey(e => e.KursId);

            // ===== Kurs - Prerequisites (self-referencing many-to-many) =====
            modelBuilder.Entity<Kurs>()
                .HasMany(k => k.Prerequisites)
                .WithMany(k => k.IsPrerequisiteFor)
                .UsingEntity<Dictionary<string, object>>(
                    "KursPrerequisite",
                    j => j.HasOne<Kurs>().WithMany().HasForeignKey("PrerequisiteId").OnDelete(DeleteBehavior.Restrict),
                    j => j.HasOne<Kurs>().WithMany().HasForeignKey("KursId").OnDelete(DeleteBehavior.Cascade)
                );

            // ===== Kurs - Profesor (one-to-many) =====
            modelBuilder.Entity<Kurs>()
                .HasOne(k => k.Prowadzacy)
                .WithMany(p => p.ProwadzoneKursy)
                .HasForeignKey(k => k.ProfesorId)
                .OnDelete(DeleteBehavior.Restrict); // <- zmienione z CASCADE

            // ===== Kurs - Wydzial (one-to-many) =====
            modelBuilder.Entity<Kurs>()
                .HasOne(k => k.Wydzial)
                .WithMany(w => w.Kursy)
                .HasForeignKey(k => k.WydzialId)
                .OnDelete(DeleteBehavior.Restrict); // <- zmienione z CASCADE

            // ===== Profesor - Wydzial (one-to-many) =====
            modelBuilder.Entity<Profesor>()
                .HasOne(p => p.Wydzial)
                .WithMany(w => w.Profesorzy)
                .HasForeignKey(p => p.WydzialId);

            // ===== Profesor - Gabinet (one-to-one) =====
            modelBuilder.Entity<Profesor>()
                .HasOne(p => p.Gabinet)
                .WithOne(g => g.Profesor)
                .HasForeignKey<Gabinet>(g => g.ProfesorId);

            // ===== StudentStudiowMgr - Promotor (one-to-many) =====
            modelBuilder.Entity<StudentStudiowMgr>()
                .HasOne(s => s.Promotor)
                .WithMany() // brak kolekcji w Profesorze
                .HasForeignKey(s => s.PromotorId)
                .OnDelete(DeleteBehavior.SetNull);

            // ===== Indeksy unikalne =====
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.IndeksUczelniany)
                .IsUnique();

            modelBuilder.Entity<Profesor>()
                .HasIndex(p => p.IndeksUczelniany)
                .IsUnique();

            // ===== LicznikIndeksow (Prefix jako PK) =====
            modelBuilder.Entity<LicznikIndeksow>()
                .HasKey(l => l.Prefix);

            modelBuilder.Entity<LicznikIndeksow>()
                .Property(l => l.AktualnaWartosc)
                .IsRequired();
        }
    }
}
