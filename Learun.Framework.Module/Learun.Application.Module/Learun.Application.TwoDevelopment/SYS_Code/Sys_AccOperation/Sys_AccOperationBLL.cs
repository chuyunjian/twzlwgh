using Learun.Util;
using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Learun.Cache.Base;
using Learun.Cache.Factory;
using Newtonsoft.Json.Linq;

namespace Learun.Application.TwoDevelopment.SYS_Code
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2018-12-11 22:49
    /// 描 述：附件注册
    /// </summary>
    public class Sys_AccOperationBLL : Sys_AccOperationIBLL
    {
        private Sys_AccOperationService sys_AccOperationService = new Sys_AccOperationService();
        /*缓存文件分片信息*/
        private ICache cache = CacheFactory.CaChe();
        private string cacheKey = "Sys_AccOperation_";
        #region 获取数据

        /// <summary>
        /// 获取列表数据
        /// <summary>
        /// <returns></returns>
        public IEnumerable<Sys_AccOperationEntity> GetList(string queryJson)
        {
            try
            {
                return sys_AccOperationService.GetList(queryJson);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
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
                return sys_AccOperationService.GetPageList(pagination, queryJson);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
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
                return sys_AccOperationService.GetEntity(keyValue);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
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
                sys_AccOperationService.DeleteEntity(keyValue);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
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
                if (keyValue.IsEmpty())
                {
                    var mod = sys_AccOperationService.GetEntity(entity.OperationCode);
                    if (mod != null)
                    {
                        throw new Exception("附件编码已存在，保存失败");
                    }
                }
                sys_AccOperationService.SaveEntity(keyValue, entity);
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }

        #endregion

        #region 扩展
        public IEnumerable<Sys_AccOperationEntity> GetListByID(string OperationCode)
        {
            try
            {
                List<Sys_AccOperationEntity> AccsInfo = cache.Read<List<Sys_AccOperationEntity>>(cacheKey + OperationCode + "_Info", CacheId.files);
                if (AccsInfo == null)
                {
                    var param = new JObject();
                    param["OperationCode"] = OperationCode;
                    AccsInfo = sys_AccOperationService.GetList(param.ToJson()).ToList();
                    cache.Write<List<Sys_AccOperationEntity>>(cacheKey + OperationCode + "_Info", AccsInfo, CacheId.files);
                }
                return AccsInfo;
            }
            catch (Exception ex)
            {
                if (ex is ExceptionEx)
                {
                    throw;
                }
                else
                {
                    throw ExceptionEx.ThrowBusinessException(ex);
                }
            }
        }
        #endregion
    }
}
