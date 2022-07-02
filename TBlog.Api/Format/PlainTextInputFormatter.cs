using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace TBlog.Api
{
    public sealed class PlainTextInputFormatter : TextInputFormatter
    {
        public PlainTextInputFormatter()
        {
            SupportedMediaTypes.Add("text/plain");
            SupportedEncodings.Add(System.Text.Encoding.UTF8);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            string content;
            using (var reader = context.ReaderFactory(context.HttpContext.Request.Body, encoding))
            {
                content = await reader.ReadToEndAsync();
            }
            return await InputFormatterResult.SuccessAsync(content);
        }
    }
}
