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
    /// 日 期：2018-12-11 22:49
    /// 描 述：附件注册
    /// </summary>
    public class Sys_AccOperationService : RepositoryFactory
    {
        #region 构造函数和属性

        private string fieldSql;
        public Sys_AccOperationService()
        {
            fieldSql = @"
                t.OperationCode,
                t.OperationName,
                t.LimitTotalSize,
                t.LimitFileSize,
                t.FileType,
                t.SavePath,
                t.Remark,
                t.CreateTime
            ";
        }
        #endregion

        #region 获取数据

        /// <summary>
        /// 获取列表数据
        /// <summary>
        /// <returns></returns>
        public IEnumerable<Sys_AccOperationEntity> GetList(string queryJson)
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
                strSql.Append(" FROM Sys_AccOperation t WHERE 1=1");
                if (!queryParam["OperationCode"].IsEmpty())
                {
                    strSql.Append(" AND OperationCode=@OperationCode");
                    dp.Add("OperationCode", queryParam["OperationCode"].ToString(), DbType.String);
                }
                return this.BaseRepository().FindList<Sys_AccOperationEntity>(strSql.ToString(), dp);
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
        public IEnumerable<Sys_AccOperationEntity> GetPageList(Pagination pagination, string queryJson)
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
                strSql.Append(" FROM Sys_AccOperation t WHERE 1=1 ");
                if (!queryParam["keyword"].IsEmpty())
                {
                    strSql.Append(" AND t.OperationCode LIKE @OperationCode");
                    dp.Add("OperationCode", "%" + queryParam["keyword"].ToString() + "%", DbType.String);
                }
                return this.BaseRepository().FindList<Sys_AccOperationEntity>(strSql.ToString(), dp, pagination);
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
        public Sys_AccOperationEntity GetEntity(string keyValue)
        {
            try
            {
                return this.BaseRepository().FindEntity<Sys_AccOperationEntity>(keyValue);
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
                this.BaseRepository().Delete<Sys_AccOperationEntity>(t => t.OperationCode == keyValue);
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
        public void SaveEntity(string keyValue, Sys_AccOperationEntity entity)
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
