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
        
        public SystemContext() 
            : base("DefaultConnection")
        {
        //Disepose and SaveChanges() are implemented in DbContext that is parent class
            
        }
        public DbSet<T> GetEntitySet<T>() where T: class,IEntity 
        {
            return base.Set<T>(); //returns a DbSet<EntityType> instance; 
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //ToTable(string) is for a specific table name instead of standard class name 
            modelBuilder.Entity<Pet>().ToTable("Pet"); 
            modelBuilder.Entity<PetImage>().ToTable("PetImage");

            base.OnModelCreating(modelBuilder);
        }
    }
}