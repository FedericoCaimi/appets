using System;
using System.Collections.Generic;

namespace Appets.BusinessLogic.Interface
{
    public interface ICrud<T>
    {
        T Create(T entity);
        void Remove(Guid id);
        T Update(Guid id, T entity);
        T Get(Guid id);
        IEnumerable<T> GetAll();
    }
}