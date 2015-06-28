using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard
{
    public static class StringHelper
    {
        /// <summary>
        /// 手机正则表达式格式
        /// </summary>
        public static string MobilePattern = @"^((13[0-9])|(15[^4,\D])|(17[0-9])|(18[0-9]))\d{8}$";

        /// <summary>
        /// 身份证正则表达式格式
        /// </summary>
        public static string IdnoPattern = @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$";

        /// <summary>
        /// 汉字正则表达式格式
        /// </summary>
        public static string ChinesePattern = @"^[\\u4e00-\\u9fa5]+$";

    }
}
