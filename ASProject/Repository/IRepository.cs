using ASProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ASProject.Repository
{
    interface IRepository<T> where T : IEntity
    {
        string Add(T entity);
        string Update(T entity);
        T GetEntityById(int id);
        IQueryable<T> GetAll();
        Dictionary<string,object> Delete(int id);

    }
}
