using HotelBooking_CA2.Context;
using HotelBooking_CA2.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotelBooking_CA2.Services
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private OTA_APP_DBContext _context = null;
        private DbSet<T> table = null;
        public GenericRepository()
        {
            this._context = new OTA_APP_DBContext();
            table = _context.Set<T>();
        }
        public GenericRepository(OTA_APP_DBContext _context)
        {
            this._context = _context;
            table = _context.Set<T>();
        }


        public OTA_APP_DBContext dbContext()
        {
            return _context;
        }

        public DbSet<T> dbset()
        {
            return table;
        }




        public IEnumerable<T> GetAll()
        {
            return table.ToList();
        }


        public T GetById(object id)
        {
            return table.Find(id);
        }


        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return table.Where(expression);
        }

        public string Insert(T obj)
        {
            try
            {


                table.Add(obj);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";

        }
        public string Update(T obj)
        {
            try
            {

                table.Attach(obj);
                _context.Entry(obj).State = EntityState.Modified;
                //try
                //{
                //    _context.Entry(obj).Property("CreatedBy").IsModified = false;
                //    _context.Entry(obj).Property("CreatedDate").IsModified = false;
                //}
                //catch(Exception ex) { }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }


        public string Delete(object id)
        {
            try
            {
                T existing = table.Find(id);
                table.Remove(existing);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }

        public string SaveChanges()
        {
            try
            {
                _context.SaveChanges(true);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "";
        }
    }
}
