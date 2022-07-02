using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBlog.Common
{
    public class TBlogApiException : Exception
    {
        public override string Source { get { return "TBlogApiException"; } }

        public TBlogApiException(string msg) : base(msg) 
        {
        }
    }
}
