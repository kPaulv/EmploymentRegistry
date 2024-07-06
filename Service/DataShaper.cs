﻿using Contracts.Interfaces;
using System.Dynamic;
using System.Reflection;

namespace Service
{
    public class DataShaper<T> : IDataShaper<T> where T : class
    {
        public PropertyInfo[] Properties { get; set; }

        public DataShaper()
        {
            Properties = typeof(T).GetProperties(BindingFlags.Public | 
                                                 BindingFlags.Instance);
        }

        public ExpandoObject ShapeData(T entity, string fieldsString)
        {
            var requiredProps = GetRequiredProperties(fieldsString);

            return RetrieveShapedDataEntity(entity, requiredProps);
        }

        public IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString)
        {
            var requiredProps = GetRequiredProperties(fieldsString);

            return RetrieveShapedDataCollection(entities, requiredProps);
        }

        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
        {
            var requiredProps = new List<PropertyInfo>();

            // if no props specified in the url than return them all (no shaping)
            if(string.IsNullOrWhiteSpace(fieldsString))
                return Properties.ToList();

            // from url we got: "name,age " -> parse into ["name", "age "]
            var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach( var field in fields)
            {
                // find a property in T that equals ["name", "age"("age ".Trim())]
                var prop = Properties.FirstOrDefault(p =>
                    p.Name.Equals(field.Trim(), StringComparison.InvariantCulture));

                if(prop is null)
                    continue;

                // add received props to list (we must return only Name, Age to controller)
                requiredProps.Add(prop);
            }

            return requiredProps;
        }

        private ExpandoObject RetrieveShapedDataEntity(T entity, IEnumerable<PropertyInfo> props)
        {
            var shapedEntity = new ExpandoObject();

            foreach(var prop in props)
            {
                // use reflection to get Value of T's property with the name stored in prop
                var propValueObject = prop.GetValue(entity);
                // add to dict key = prop.Name, value = property Value from Entity as Object
                shapedEntity.TryAdd(prop.Name, propValueObject);
            }

            return shapedEntity;
        }

        private IEnumerable<ExpandoObject> RetrieveShapedDataCollection
            (IEnumerable<T> entities, IEnumerable<PropertyInfo> props)
        {
            var shapedCollection = new List<ExpandoObject>();

            foreach (var entity in entities)
            {
                shapedCollection.Add(RetrieveShapedDataEntity(entity, props));
            }

            return shapedCollection;
        }
    }
}
