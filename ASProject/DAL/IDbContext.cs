using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASProject.Models;
namespace ASProject.DAL
{
    public interface IDbContext:IDisposable
    {
        DbSet<T> GetEntitySet<T>() where T : class, IEntity;
        int SaveChanges();
    }
}
