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
    /// 日 期：2018-12-11 23:06
    /// 描 述：附件管理
    /// </summary>
    public class Sys_AccessoriesService : RepositoryFactory
    {
        #region 构造函数和属性

        private string fieldSql;
        public Sys_AccessoriesService()
        {
            fieldSql = @"
                t.ID,
                t.PhyFileName,
                t.SysFileName,
                t.OperationCode,
                t.OperationID,
                t.FileSize,
                t.CreateTime,
                t.CreatePerson,
                t.State,
                t.FileType,
                t.SavePath,
                t.Remark
            ";
        }
        #endregion

        #region 获取数据

        /// <summary>
        /// 获取列表数据
        /// <summary>
        /// <returns></returns>
        public IEnumerable<Sys_AccessoriesEntity> GetList(string queryJson)
        {
            try
            {
                //参考写法
                var queryParam = queryJson.ToJObject();
                // 虚拟参数
                var dp = new DynamicParameters(new { });
                var strSql = new StringBuilder();
                strSql.Append("SELECT ");
                strSql.Append(fieldSql);
                strSql.Append(" FROM Sys_Accessories t WHERE 1=1 ");
                if (!queryParam["OperationCode"].IsEmpty())
                {
                    strSql.Append(" AND OperationCode=@OperationCode");
                    dp.Add("OperationCode", queryParam["OperationCode"].ToString(), DbType.String);
                }
                if (!queryParam["OperationID"].IsEmpty())
                {
                    strSql.Append(" AND OperationID=@OperationID");
                    dp.Add("OperationID", queryParam["OperationID"].ToString(), DbType.String);
                }
                else
                {
                    //必须有业务主键才执行查询
                    return new List<Sys_AccessoriesEntity>();
                }
                strSql.Append(" ORDER BY CreateTime ASC");
                return this.BaseRepository().FindList<Sys_AccessoriesEntity>(strSql.ToString(), dp);
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
        public IEnumerable<Sys_AccessoriesEntity> GetPageList(Pagination pagination, string queryJson)
        {
            try
            {
                //参考写法
                var queryParam = queryJson.ToJObject();
                // 虚拟参数
                var dp = new DynamicParameters(new { });
                var strSql = new StringBuilder();
                strSql.Append("SELECT ");
                strSql.Append(fieldSql);
                strSql.Append(" FROM Sys_Accessories t WHERE 1=1 ");
                if (!queryParam["OperationCode"].IsEmpty())
                {
                    strSql.Append(" AND t.OperationCode LIKE @OperationCode");
                    dp.Add("OperationCode", "%" + queryParam["OperationCode"].ToString() + "%", DbType.String);
                }
                if (!queryParam["OperationID"].IsEmpty())
                {
                    strSql.Append(" AND t.OperationID LIKE @OperationID");
                    dp.Add("OperationID", "%" + queryParam["OperationID"].ToString() + "%", DbType.String);
                }
                return this.BaseRepository().FindList<Sys_AccessoriesEntity>(strSql.ToString(), dp, pagination);
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
        public Sys_AccessoriesEntity GetEntity(string keyValue)
        {
            try
            {
                return this.BaseRepository().FindEntity<Sys_AccessoriesEntity>(keyValue);
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
                this.BaseRepository().Delete<Sys_AccessoriesEntity>(t => t.ID == keyValue);
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
        public void SaveEntity(string keyValue, Sys_AccessoriesEntity entity)
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

    }
}
