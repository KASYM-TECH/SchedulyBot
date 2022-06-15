using TechExtensions.SchedyBot.DLL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechExtensions.SchedyBot.BLL.Services
{
    public static class TimeZoneService
    {
        /// <summary>
        /// Устанавливает подходящий TimeZone 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity">Сущность</param>
        /// <param name="offset">Разница в часах</param>
        /// <returns></returns>
        public static TEntity SetProperTimeZone<TEntity>(TEntity entity, TimeSpan offset) where TEntity : BaseEntity
        {
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
                if (property.GetValue(entity) is DateTime)
                    property.SetValue(entity, ((DateTime)property.GetValue(entity)!).Add(offset.Negate()));
            return entity;
        }
    }
}
