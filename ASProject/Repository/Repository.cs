using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASProject.Models;
using ASProject.DAL;
using System.Data.Entity;

namespace ASProject.Repository
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly IDbContext dbContext;
        protected DbSet<T> EntitySet { get; }

        public Repository(IDbContext dbContext)
        {
            this.dbContext = dbContext;
            EntitySet = dbContext.GetEntitySet<T>();
        }




        //implemented methods from interface:
        public string Add(T entity)
        {
            try
            {
                EntitySet.Add(entity);
                dbContext.SaveChanges(); //returns rows affected (int)

            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return "success";

        }

        public Dictionary<string,object> Delete(int id)
        {
            Dictionary<string, object> statusDict = new Dictionary<string, object>();
            T entity = GetEntityById(id);
            try
            { 
                System.Diagnostics.Debug.WriteLine("Was successfully able to retrieve :" + entity.Id);
                EntitySet.Remove(entity);
                dbContext.SaveChanges();

            }catch(Exception ex)
            {
                statusDict.Add("status", ex.ToString());
            }
            statusDict.Add("status", "success");
            statusDict.Add("removedObj", entity);
            return statusDict;
        }


        public IQueryable<T> GetAll()
        {
            return EntitySet.AsQueryable();

        }

        public T GetEntityById(int id)
        {
            return EntitySet.Where(t => t.Id == id).FirstOrDefault();
        }

        public string[] Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}