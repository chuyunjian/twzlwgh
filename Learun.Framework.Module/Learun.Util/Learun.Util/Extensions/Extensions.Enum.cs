using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Learun.Util
{
    /// <summary>
    /// 枚举扩展
    /// 创建人：思必达-严笛
    /// 日 期：2019-04-18
    /// 描 述：枚举扩展
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 获取枚举值的Description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription<T>(this T value) where T : struct
        {
            string result = value.ToString();
            Type type = typeof(T);
            FieldInfo info = type.GetField(value.ToString());
            var attributes = info.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attributes != null && attributes.FirstOrDefault() != null)
            {
                result = (attributes.First() as DescriptionAttribute).Description;
            }

            return result;
        }
        /// <summary>
        /// 获取对应INT值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetValue<T>(this T value) where T : struct
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("{0} 未能找到对应的枚举值.", value), "Value");
            }
        }
    }
}
