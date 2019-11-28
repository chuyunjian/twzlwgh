using Dapper;
using Learun.DataBase.Repository;
using Learun.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Learun.Application.TwoDevelopment.SYS_Code
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2019-05-06 08:35
    /// 描 述：APP管理
    /// </summary>
    public class APPService : RepositoryFactory
    {
        #region 构造函数和属性

        private string fieldSql;
        public APPService()
        {
            fieldSql=@"
                t.AGuid,
                t.Version,
                t.Type,
                t.ARemark,
                t.SysCode,
                t.CreateDate,
                t.Name
            ";
        }
        #endregion

        #region 获取数据

        /// <summary>
        /// 获取列表数据
        /// <summary>
        /// <returns></returns>
        public IEnumerable<APPEntity> GetList( string queryJson )
        {
            try
            {
                //参考写法
                //var queryParam = queryJson.ToJObject();
                // 虚拟参数
                //var dp = new DynamicParameters(new { });
                //dp.Add("startTime", queryParam["StartTime"].ToDate(), DbType.DateTime);
                var strSql = new StringBuilder();
                strSql.Append("SELECT ");
                strSql.Append(fieldSql);
                strSql.Append(" FROM APP t ");
                strSql.Append(" where 1=1 ");
                if (!queryJson.IsEmpty())
                {
                    strSql.Append(" and "+queryJson);
                }
                return this.BaseRepository().FindList<APPEntity>(strSql.ToString());
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowServiceException(ex);
                }
            }
        }

        /// <summary>
        /// 获取列表分页数据
        /// <param name="pagination">分页参数</param>
        /// <summary>
        /// <returns></returns>
        public IEnumerable<APPEntity> GetPageList(Pagination pagination, string queryJson)
        {
            try
            {
                var dp = new DynamicParameters(new { });
                var strSql = new StringBuilder();
                strSql.Append("SELECT ");
                strSql.Append(fieldSql);
                strSql.Append(" FROM APP t  WHERE 1=1 ");
                var queryParam = queryJson.ToJObject();
                if (!queryParam["Type"].IsEmpty())
                {
                    strSql.Append(" AND Type = @Type");
                    dp.Add("Type", queryParam["Type"].ToString(), DbType.String);
                }
                if (!queryParam["Name"].IsEmpty())
                {
                    strSql.Append(" AND Name = @Name");
                    dp.Add("Name", queryParam["Name"].ToString(), DbType.String);
                }
                return this.BaseRepository().FindList<APPEntity>(strSql.ToString(),dp, pagination);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowServiceException(ex);
                }
            }
        }

        /// <summary>
        /// 获取实体数据
        /// <param name="keyValue">主键</param>
        /// <summary>
        /// <returns></returns>
        public APPEntity GetEntity(string keyValue)
        {
            try
            {
                return this.BaseRepository().FindEntity<APPEntity>(keyValue);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowServiceException(ex);
                }
            }
        }

        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体数据
        /// <param name="keyValue">主键</param>
        /// <summary>
        /// <returns></returns>
        public void DeleteEntity(string keyValue)
        {
            try
            {
                this.BaseRepository().Delete<APPEntity>(t=>t.AGuid == keyValue);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowServiceException(ex);
                }
            }
        }

        /// <summary>
        /// 保存实体数据（新增、修改）
        /// <param name="keyValue">主键</param>
        /// <summary>
        /// <returns></returns>
        public void SaveEntity(string keyValue, APPEntity entity, string Attachments)
        {
            try
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    entity.Modify(keyValue);
                    this.BaseRepository().Update(entity);
                }
                else
                {
                    entity.Create();
                    this.BaseRepository().Insert(entity);
                }
                WebHelper.BindAttachment(LoginUserInfo.Get().userId, "Version", entity.AGuid, Attachments);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowServiceException(ex);
                }
            }
        }

        #endregion
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表</returns>
        public IEnumerable<APPEntity> GetList(int top, string queryJson)
        {
            var dp = new DynamicParameters(new { });
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT top " + top + " * FROM APP WHERE 1=1 ");
            var queryParam = queryJson.ToJObject();
            if (!queryParam["Type"].IsEmpty())
            {
                strSql.Append(" AND Type = @Type");
                dp.Add("Type", queryParam["Type"].ToString(), DbType.String);
            }
            strSql.Append(" order by CreateDate desc");
            return this.BaseRepository().FindList<APPEntity>(strSql.ToString(), dp);
        }
    }
}
