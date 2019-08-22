using ASProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ASProject.DAL
{
    public class SystemContext : DbContext, IDbContext
    {
        //IDbContext has an abstract method "Dispose" and "SaveChanges". 
        //They're implemented in DbContext from which SystemContext inherits from.
        public SystemContext() 
            : base("DefaultConnection")
        {

        }
        public DbSet<T> GetEntitySet<T>() where T: class,IEntity 
        {
            return base.Set<T>(); //returns a DbSet<EntityType> instance; 
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Database.SetInitializer<SystemContext>(null);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //ToTable(string) is for a preferred table name aside from the standard class name 
            modelBuilder.Entity<Pet>().ToTable("Pet"); 
            modelBuilder.Entity<PetImage>().ToTable("PetImage");

            base.OnModelCreating(modelBuilder);
        }
    }
}