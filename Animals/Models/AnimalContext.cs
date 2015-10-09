using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Animals.Models
{
    // По идее для облегчения тестирования не мешало бы добавить выделенный репозиторий...
    // но раз в задании об этом не сказано, то иТакСойдёт.jpg
    public class AnimalContext : DbContext
    {
        public AnimalContext() : base ("Database1")
    {

    }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Type> Types { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // чтобы атрибуты в таблицах БД назывались также как и имена полей классов,
            // иначе будут во множественном числе (%attrName%+'s' - то, что называется плюразацией)
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}