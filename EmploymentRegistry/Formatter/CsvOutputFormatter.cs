using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects;
using System.Text;

namespace EmploymentRegistry.Formatter
{
    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter() {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type? type)
        {
            if(typeof(CompanyDto).IsAssignableFrom(type) ||
                        typeof(IEnumerable<CompanyDto>).IsAssignableFrom(type)) {
                return base.CanWriteType(type);
            }

            return false;
        }

        private static void FormatCsv(StringBuilder buff, CompanyDto company)
        {
            buff.AppendLine($"{company.Id},\"{company.Name},\"{company.FullAddress}\"");
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, 
                                                        Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var buff = new StringBuilder();

            if(context.Object is IEnumerable<CompanyDto>)
            {
                foreach(CompanyDto company in (IEnumerable<CompanyDto>)context.Object)
                {
                    FormatCsv(buff, company);
                }
            } else
            {
                FormatCsv(buff, (CompanyDto)context.Object);
            }

            await response.WriteAsync(buff.ToString());
        }
    }
}
