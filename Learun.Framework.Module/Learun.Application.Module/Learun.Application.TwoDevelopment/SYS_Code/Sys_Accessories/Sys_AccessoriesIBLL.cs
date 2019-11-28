using Learun.Util;
using System.Data;
using System.Collections.Generic;
using System.IO;

namespace Learun.Application.TwoDevelopment.SYS_Code
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2018-12-11 23:06
    /// 描 述：附件管理
    /// </summary>
    public interface Sys_AccessoriesIBLL
    {
        #region 获取数据

        /// <summary>
        /// 获取列表数据
        /// <summary>
        /// <returns></returns>
        IEnumerable<Sys_AccessoriesEntity> GetList(string queryJson);
        /// <summary>
        /// 获取列表分页数据
        /// <param name="pagination">分页参数</param>
        /// <summary>
        /// <returns></returns>
        IEnumerable<Sys_AccessoriesEntity> GetPageList(Pagination pagination, string queryJson);
        /// <summary>
        /// 获取实体数据
        /// <param name="keyValue">主键</param>
        /// <summary>
        /// <returns></returns>
        Sys_AccessoriesEntity GetEntity(string keyValue);
        /// <summary>
        /// 通过业务主键编号和附件编码得到第一张附件路径
        /// </summary>
        /// <param name="OperationID">业务主键编号</param>
        /// <param name="OperationCode">业务附件编码</param>
        /// <returns>返回第一张附件路径</returns>
        string GetFileUrl(string OperationID, string OperationCode);
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体数据
        /// <param name="keyValue">主键</param>
        /// <summary>
        /// <returns></returns>
        void DeleteEntity(string keyValue);
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// <param name="keyValue">主键</param>
        /// <summary>
        /// <returns></returns>
        void SaveEntity(string keyValue, Sys_AccessoriesEntity entity);


        /// <summary>
        /// 保存分片附件（文件缓存2小时）
        /// </summary>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="chunk">分片文件序号</param>
        /// <param name="fileStream">文件流</param>
        /// <param name="AccsInfo">文件对象信息</param>
        void SaveChunkAnnexes(string fileGuid, Stream fileStream, Sys_AccessoriesEntity AccsInfo);
        /// <summary>
        /// 保存附件（支持大文件分片传输）
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="chunks">文件总共分多少片</param>
        /// <param name="fileStream">文件二进制流</param>
        /// <returns></returns>
        bool SaveAnnexes(string UserID, string fileGuid, string OperationCode);
        /// <summary>
        /// 保存附件到文件中
        /// </summary>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="chunks">总共分片数</param>
        /// <param name="buffer">文件二进制流</param>
        /// <returns>-1:表示保存失败</returns>
        long SaveAnnexesToFile(string fileGuid, string filePath);
        /// <summary>
        /// 移除文件分片数据
        /// </summary>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="chunks">文件分片数</param>
        void RemoveChunkAnnexes(string fileGuid);
        /// <summary>
        /// 删除附件及附件记录
        /// </summary>
        /// <param name="file"></param>
        void DeleteFile(Sys_AccessoriesEntity file);
        /// <summary>
        /// 将业务关联附件
        /// </summary>
        /// <param name="OperationCode">附加编码</param>
        /// <param name="OperationID">业务主键</param>
        /// <param name="AttachmentID">附件ID列表，以逗号分隔</param>
        void BindAttachment(string UserID, string OperationCode, string OperationID, string AttachmentID);
        #endregion

    }
}
