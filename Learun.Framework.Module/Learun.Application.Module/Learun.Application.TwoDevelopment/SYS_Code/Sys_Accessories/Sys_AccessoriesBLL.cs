using Learun.Util;
using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using Learun.Cache.Base;
using Learun.Cache.Factory;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Learun.Application.TwoDevelopment.SYS_Code
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2018-12-11 23:06
    /// 描 述：附件管理
    /// </summary>
    public class Sys_AccessoriesBLL : Sys_AccessoriesIBLL
    {
        private Sys_AccessoriesService sys_AccessoriesService = new Sys_AccessoriesService();
        /*缓存文件分片信息*/
        private ICache cache = CacheFactory.CaChe();
        private string cacheKey = "Sys_Accessories_";

        #region 获取数据

        /// <summary>
        /// 获取列表数据
        /// <summary>
        /// <returns></returns>
        public IEnumerable<Sys_AccessoriesEntity> GetList(string queryJson)
        {
            try
            {
                return sys_AccessoriesService.GetList(queryJson);
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
        public IEnumerable<Sys_AccessoriesEntity> GetPageList(Pagination pagination, string queryJson)
        {
            try
            {
                return sys_AccessoriesService.GetPageList(pagination, queryJson);
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
        public Sys_AccessoriesEntity GetEntity(string keyValue)
        {
            try
            {
                return sys_AccessoriesService.GetEntity(keyValue);
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
                sys_AccessoriesService.DeleteEntity(keyValue);
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
        public void SaveEntity(string keyValue, Sys_AccessoriesEntity entity)
        {
            try
            {
                sys_AccessoriesService.SaveEntity(keyValue, entity);
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
        /// 保存分片附件（文件缓存2小时）
        /// </summary>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="chunk">分片文件序号</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="AccsInfo">文件对象信息</param>
        public void SaveChunkAnnexes(string fileGuid, Stream fileStream, Sys_AccessoriesEntity AccsInfo)
        {
            try
            {
                int RedisFileTime = Config.GetValue("RedisFileTime").IsEmpty() ? 21600 : Config.GetValue("RedisFileTime").ToInt();
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                //文件
                cache.Write<byte[]>(cacheKey + fileGuid, bytes, DateTime.Now.AddSeconds(RedisFileTime), CacheId.files);
                //文件信息
                cache.Write<Sys_AccessoriesEntity>(cacheKey + fileGuid + "_Info", AccsInfo, DateTime.Now.AddSeconds(RedisFileTime), CacheId.files);
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
        /// 保存附件（支持大文件分片传输）
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="chunks">文件总共分多少片</param>
        /// <param name="fileStream">文件二进制流</param>
        /// <returns></returns>
        public bool SaveAnnexes(string UserID, string fileGuid, string OperationCode)
        {
            try
            {
                string fileName = "";
                Sys_AccessoriesEntity AccsInfo = cache.Read<Sys_AccessoriesEntity>(cacheKey + fileGuid + "_Info", CacheId.files);
                if (AccsInfo == null)
                {
                    AccsInfo = GetEntity(fileGuid);
                    if (AccsInfo == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    fileName = AccsInfo.SysFileName;
                }
                //获取文件完整文件名(包含绝对路径)
                //文件存放路径格式：/Resource/ResourceFile/{userId}/{date}/{guid}.{后缀名}
                string filePath = AccsInfo.SavePath;
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                string FileEextension = Path.GetExtension(fileName);
                string virtualPath = Path.Combine("/" + filePath + "/", (fileGuid + FileEextension));
                virtualPath = virtualPath.Replace("//", "/");
                //创建文件夹
                string path = Path.GetDirectoryName(virtualPath);
                Directory.CreateDirectory(path);
                if (!System.IO.File.Exists(virtualPath))
                {
                    long filesize = SaveAnnexesToFile(fileGuid, virtualPath);
                    if (filesize == -1)// 表示保存失败
                    {
                        RemoveChunkAnnexes(fileGuid);
                        return false;
                    }
                    //文件信息写入数据库
                    AccsInfo.FileSize = filesize;
                    AccsInfo.FileType = FileEextension.Replace(".", "");
                    SaveEntity("", AccsInfo);
                }
                return true;
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
        /// 保存附件到文件中
        /// </summary>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="chunks">总共分片数</param>
        /// <param name="buffer">文件二进制流</param>
        /// <returns>-1:表示保存失败</returns>
        public long SaveAnnexesToFile(string fileGuid, string filePath)
        {
            try
            {
                string ServerPath = WebHelper.BasePath;
                string MapPath = ServerPath + filePath;
                long filesize = 0;
                //创建一个FileInfo对象
                FileInfo file = new FileInfo(MapPath);
                //创建文件
                FileStream fs = file.Create();
                byte[] bufferByRedis = cache.Read<byte[]>(cacheKey + fileGuid, CacheId.files);
                if (bufferByRedis == null)
                {
                    return -1;
                }
                //写入二进制流
                fs.Write(bufferByRedis, 0, bufferByRedis.Length);
                filesize += bufferByRedis.Length;
                cache.Remove(cacheKey + fileGuid, CacheId.files);
                cache.Remove(cacheKey + fileGuid + "_Info", CacheId.files);
                //关闭文件流
                fs.Close();

                return filesize;
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
        /// 移除文件分片数据
        /// </summary>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="chunks">文件分片数</param>
        public void RemoveChunkAnnexes(string fileGuid)
        {
            try
            {
                cache.Remove(cacheKey + fileGuid, CacheId.files);
                cache.Remove(cacheKey + fileGuid + "_Info", CacheId.files);
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
        //删除附件及附件记录
        public void DeleteFile(Sys_AccessoriesEntity file)
        {
            string virtualPath = Path.Combine("/" + file.SavePath + "/", (file.PhyFileName));
            virtualPath = virtualPath.Replace("//", "/");
            string ServerPath = WebHelper.BasePath;
            string MapPath = ServerPath + virtualPath;
            if (File.Exists(MapPath))
            {
                //删除硬盘中的附件
                File.Delete(MapPath);
            }
            DeleteEntity(file.ID);
        }
        /// <summary>
        /// 将业务关联附件
        /// </summary>
        /// <param name="OperationCode">附加编码</param>
        /// <param name="OperationID">业务主键</param>
        /// <param name="AttachmentID">附件ID列表，以逗号分隔</param>
        public void BindAttachment(string UserID, string OperationCode, string OperationID, string AttachmentID)
        {
            string[] AttachmentIDList = AttachmentID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in AttachmentIDList)
            {
                SaveAnnexes(UserID, item, OperationCode);
            }
            List<Sys_AccessoriesEntity> ListAccessory = GetList(new { OperationCode = OperationCode, OperationID = OperationID }.ToJson()).ToList();
            if (AttachmentIDList.Length != 0)
            {
                //数据库已经存在附件ID
                List<string> listOld = new List<string>();
                foreach (Sys_AccessoriesEntity modA in ListAccessory)
                {
                    if (!AttachmentIDList.Contains(modA.ID))
                    {
                        DeleteFile(modA);
                    }
                    else
                    {
                        listOld.Add(modA.ID);
                    }
                }
                //将新附件关联业务数据
                foreach (string item in AttachmentIDList)
                {
                    //不存在的附件才进行业务关联
                    if (!listOld.Contains(item))
                    {
                        Sys_AccessoriesEntity temp = GetEntity(item);
                        if (temp.OperationID.IsEmpty())
                        {
                            temp.OperationID = OperationID;
                            SaveEntity(temp.ID, temp);
                        }
                    }

                }
            }
            else
            {
                //删除该记录所有附件
                foreach (Sys_AccessoriesEntity modA in ListAccessory)
                {
                    DeleteFile(modA);
                }
            }
        }
        #endregion

        #region 扩展方法
        #region 黄尊明
        #region 根据业务编码与业务ID获取对应的附件列表
        /// <summary>
        /// 根据业务编码与业务ID获取对应的附件列表
        /// </summary>
        /// <param name="strOperationCode">业务编码</param>
        /// <param name="strOperationID">业务ID</param>
        /// <returns></returns>
        public List<Sys_AccessoriesEntity> GetModelListByOperationCode(string strOperationCode, string strOperationID)
        {
            JObject jb = new JObject();
            jb.Add("OperationCode", strOperationCode);
            jb.Add("OperationID", strOperationID);
            return GetList(jb.ToJson()).ToList<Sys_AccessoriesEntity>();
        }
        #endregion
        #region 通过业务主键编号和附件编码得到第一张附件路径
        /// <summary>
        /// 通过业务主键编号和附件编码得到第一张附件路径
        /// </summary>
        /// <param name="OperationID">业务主键编号</param>
        /// <param name="OperationCode">业务附件编码</param>
        /// <returns>返回第一张附件路径</returns>
        public string GetFileUrl(string OperationID, string OperationCode)
        {
            string Url = string.Empty;
            Sys_AccessoriesBLL bllA = new Sys_AccessoriesBLL();
            var accessories = GetModelListByOperationCode(OperationCode, OperationID).OrderByDescending(t => t.CreateTime).FirstOrDefault();
            if (accessories != null)
            {
                Url = accessories.getHttpPath();
            }
            return Url;
        }
        #endregion

        #endregion
        #endregion
    }
}
