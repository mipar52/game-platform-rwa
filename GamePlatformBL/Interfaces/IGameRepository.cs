using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePlatformBL.Interfaces
{
    public interface IGameRepository<TEntity, TViewModel>
    {
        public IEnumerable<TEntity> GetAll();
        public TEntity? Get(int id);
        public TEntity Add(TEntity entity, TViewModel vm);
        public TEntity Update(int id, TViewModel model);
        public void Remove(int id);
    }
}
