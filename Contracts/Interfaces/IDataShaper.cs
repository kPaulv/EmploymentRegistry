using System.Dynamic;

namespace Contracts.Interfaces
{
    public interface IDataShaper<T>
    {
        ExpandoObject ShapeData(T entity, string fieldsString);
        IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString);
    }
}
