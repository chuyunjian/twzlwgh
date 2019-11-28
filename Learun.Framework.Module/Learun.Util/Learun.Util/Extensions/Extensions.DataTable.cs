using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Learun.Util
{
    /// <summary>
    /// 创建人：严笛
    /// 日 期：2019-03-19
    /// 描 述：Table扩展帮助类
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// DataTable 转 List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt) where T : new()
        {
            var list = new List<T>();
            try
            {
                var columnNames = new List<string>();
                foreach (DataColumn col in dt.Columns)
                {
                    columnNames.Add(col.ColumnName);
                }
                PropertyInfo[] Properties;
                Properties = typeof(T).GetProperties();
                list = dt.AsEnumerable().ToList().ConvertAll<T>(x => GetObject<T>(x, columnNames, Properties));
            }
            catch { }
            return list;
        }

        private static T GetObject<T>(DataRow row, List<string> columnsName, PropertyInfo[] properties) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                foreach (PropertyInfo objProperty in properties)
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (columnname == "ccstatus")
                    {
                        var aa = "";
                    }
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!objProperty.PropertyType.IsGenericType)
                        {
                            //非泛型
                            objProperty.SetValue(obj, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, objProperty.PropertyType), null);
                        }
                        else
                        {
                            //泛型Nullable<>
                            Type genericTypeDefinition = objProperty.PropertyType.GetGenericTypeDefinition();
                            if (genericTypeDefinition == typeof(Nullable<>))
                            {
                                Type type = objProperty.PropertyType;
                                Type underlyingType = Nullable.GetUnderlyingType(type);
                                if (underlyingType.IsEnum)
                                {
                                    objProperty.SetValue(obj, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(Enum.Parse(underlyingType, value), Nullable.GetUnderlyingType(objProperty.PropertyType)), null);
                                }
                                else
                                {
                                    objProperty.SetValue(obj, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, Nullable.GetUnderlyingType(objProperty.PropertyType)), null);
                                }
                            }
                        }
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                return obj;
            }
        }
    }
}
