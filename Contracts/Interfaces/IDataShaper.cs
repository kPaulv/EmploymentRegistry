using Entities.Entities;
using System.Dynamic;

namespace Contracts.Interfaces
{
    public interface IDataShaper<T>
    {
        ShapedEntity ShapeData(T entity, string fieldsString);
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString);
    }
}
