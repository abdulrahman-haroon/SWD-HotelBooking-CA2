using HotelBooking_CA2.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotelBooking_CA2.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        OTA_APP_DBContext dbContext();
        DbSet<T> dbset();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetAll();
        T GetById(object id);
        string Insert(T obj);
        string Update(T obj);
        string Delete(object id);
        string SaveChanges();
    }
}
