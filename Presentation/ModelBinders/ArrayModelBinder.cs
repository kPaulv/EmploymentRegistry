using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Reflection;

namespace Presentation.ModelBinders
{
    public class ArrayModelBinder : IModelBinder
    {
        // method for binding array of GUIDs separated with ',' from request to API
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // provided data is not enumerable collection
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            // retrieve provided data as a string
            var providedValue = bindingContext.ValueProvider
                                                .GetValue(bindingContext.ModelName)
                                                .ToString();
            if (string.IsNullOrEmpty(providedValue))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            string[] arr = new[] { "a", "b", "c"};

            // generic type for provided data
            var genericType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];

            var converter = TypeDescriptor.GetConverter(genericType);

            // split provided data string into array
            var providedValuesArray = providedValue
                                        .Split(new[] { "," }, 
                                                StringSplitOptions.RemoveEmptyEntries);

            // convert provided data array to array of objects of generic type
            var providedObjectsArray = providedValuesArray
                                        .Select(v => converter.ConvertFromString(v.Trim()))
                                        .ToArray();

            // pass data via guid array
            var guidArray = Array.CreateInstance(genericType, providedObjectsArray.Length);
            providedObjectsArray.CopyTo(guidArray, 0);
            bindingContext.Model = guidArray;

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
