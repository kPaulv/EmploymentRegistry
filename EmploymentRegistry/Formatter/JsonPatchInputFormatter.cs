using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using System.Text;

namespace EmploymentRegistry.Formatter
{
    // Helper formatter class to add NewtonsoftJson formatter instead of System.Text.Json
    public class JsonPatchInputFormatter
    {
        public JsonPatchInputFormatter() { }

        // Local function to add one more JSON formatter strictly
        // for Controllers(PATCH request)
        public NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            return new ServiceCollection().AddLogging()
                                          .AddMvc()
                                          .AddNewtonsoftJson()
                                          .Services
                                          .BuildServiceProvider()
                                          .GetRequiredService<IOptions<MvcOptions>>()
                                          .Value
                                          .InputFormatters
                                          .OfType<NewtonsoftJsonPatchInputFormatter>()
                                          .First();
        }
    }
}
