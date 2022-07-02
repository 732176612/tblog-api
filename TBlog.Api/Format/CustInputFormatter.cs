using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Text;
using System.Threading.Tasks;

namespace TBlog.Api
{
    public sealed class CustInputFormatter : InputFormatter
    {
        public CustInputFormatter()
        {
            SupportedMediaTypes.Add("text/plain");
        }

        protected override bool CanReadType(Type type)
        {
            return (type == typeof(DateTime)) || (type == typeof(int));
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            string val;
            using (var reader = context.ReaderFactory(context.HttpContext.Request.Body, Encoding.UTF8))
            {
                val = await reader.ReadToEndAsync();
            }
            InputFormatterResult result = null;
            if (context.ModelType == typeof(DateTime))
            {
                result = InputFormatterResult.Success(DateTime.Parse(val));
            }
            else
            {
                result = InputFormatterResult.Success(int.Parse(val));
            }
            return result;
        }
    }
}
