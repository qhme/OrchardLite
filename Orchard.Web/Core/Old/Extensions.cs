using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard;
using System.ComponentModel;
using System.Reflection;

namespace Orchard.Core
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 从枚举中获取Description
        /// </summary>
        /// <param name="enumName">需要获取枚举描述的枚举</param>
        /// <returns>描述内容</returns>
        public static string GetDescription<TEnum>(this TEnum Enum) where TEnum : struct
        {
            var em = Enum.ToString();
            FieldInfo fieldInfo = Enum.GetType().GetField(em);
            if (fieldInfo == null) return em;
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length < 1) return em;
            return attributes[0].Description;
        }
        /// <summary>
        /// 从枚举中获取Description列表,生成页面下拉框
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumObj"></param>
        /// <param name="markCurrentAsSelected"></param>
        /// <param name="valuesToExclude"></param>
        /// <returns></returns>
        public static SelectList ToSelectDescriptionList<TEnum>(this TEnum enumObj, bool markCurrentAsSelected = true,
            int[] valuesToExclude = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new ArgumentException("An Enumeration type is required.", "enumObj");


            var values = from TEnum enumValue in Enum.GetValues(typeof(TEnum))
                         where valuesToExclude == null || !valuesToExclude.Contains(Convert.ToInt32(enumValue))
                         select new { ID = Convert.ToInt32(enumValue), Name = ((TEnum)System.Enum.Parse(typeof(TEnum), enumValue.ToString())).GetDescription() };

            object selectedValue = null;
            if (markCurrentAsSelected)
                selectedValue = Convert.ToInt32(enumObj);
            return new SelectList(values, "ID", "Name", selectedValue);
        }
    }
}
