using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdGen;
namespace TBlog.Common
{
    /// <summary>
    /// ID生成器
    /// </summary>
    public static class IdBuilder
    {
        private static IdGenerator idBulider;
        static IdBuilder()
        {
            idBulider = new IdGenerator(0);
        }

        /// <summary>
        /// 生成一个ID
        /// </summary>
        /// <returns></returns>
        public static long CreateId()
        {
            return idBulider.CreateId();
        }
    }
}
