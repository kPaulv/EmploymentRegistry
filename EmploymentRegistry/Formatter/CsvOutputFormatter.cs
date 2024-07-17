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
            if(typeof(CompanyOutputDto).IsAssignableFrom(type) ||
                        typeof(IEnumerable<CompanyOutputDto>).IsAssignableFrom(type)) {
                return base.CanWriteType(type);
            }

            return false;
        }

        private static void FormatCsv(StringBuilder buff, CompanyOutputDto company)
        {
            buff.AppendLine($"{company.Id},\"{company.Name},\"{company.FullAddress}\"");
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, 
                                                        Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var buff = new StringBuilder();

            if(context.Object is IEnumerable<CompanyOutputDto>)
            {
                foreach(CompanyOutputDto company in (IEnumerable<CompanyOutputDto>)context.Object)
                {
                    FormatCsv(buff, company);
                }
            } else
            {
                FormatCsv(buff, (CompanyOutputDto)context.Object);
            }

            await response.WriteAsync(buff.ToString());
        }
    }
}
