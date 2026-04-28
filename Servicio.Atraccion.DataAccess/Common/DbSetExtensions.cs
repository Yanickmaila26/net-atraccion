using Microsoft.EntityFrameworkCore;

namespace Servicio.Atraccion.DataAccess.Common 
{ 
    public static class DbSetExtensions { 
        public static IQueryable<T> AsQueryContext<T>(this DbSet<T> set) where T : class => set.AsNoTracking().AsQueryable(); 
    } 
}