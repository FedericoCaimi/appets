
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Appets.DataAccess.Interface;
using Appets.Domain;
using Appets.Exceptions;
using System.Linq;


namespace Appets.DataAccess
{
    public abstract class BaseRepository<T> : IRepository<T> where T : DomainEntity
    {
        protected DbContext Context { get; set; }

        public virtual void Add(T entity)
        {
            if (exists(entity.Id)) throw new DBKeyAlreadyExistsException();
            Context.Set<T>().Add(entity);
        }

        public virtual void Remove(T entity)
        {
            if (!exists(entity.Id)) throw new DBKeyNotFoundException();
            entity.IsDeleted = true;
            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update(T entity)
        {
            if (!exists(entity.Id)) throw new DBKeyNotFoundException();
            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Context.Set<T>();
        }

        public virtual T Get(Guid id)
        {
            if (!exists(id)) throw new DBKeyNotFoundException();
            return Context.Set<T>().FirstOrDefault(x => x.Id == id);
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        protected virtual bool exists(Guid id)
        {
            return Context.Set<T>().FirstOrDefault(x => x.Id == id) != null;
        }

        #region IDisposable Support
        private bool disposingValue = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposingValue)
            {
                if (disposing)
                {
                    this.Context.Dispose();
                }
            }
        }
        #endregion
    }
}