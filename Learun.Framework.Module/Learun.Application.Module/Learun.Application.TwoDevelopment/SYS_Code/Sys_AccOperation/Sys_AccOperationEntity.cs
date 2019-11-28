using Learun.Util;
using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Learun.Application.TwoDevelopment.SYS_Code

{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2018-12-11 22:49
    /// 描 述：附件注册
    /// </summary>
    public class Sys_AccOperationEntity 
    {
        #region 实体成员
        /// <summary>
        /// 附件编码
        /// </summary>
        /// <returns></returns>
        [Column("OPERATIONCODE")]
        public string OperationCode { get; set; }
        /// <summary>
        /// 附件名称
        /// </summary>
        /// <returns></returns>
        [Column("OPERATIONNAME")]
        public string OperationName { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        /// <returns></returns>
        [Column("LIMITTOTALSIZE")]
        public int? LimitTotalSize { get; set; }
        /// <summary>
        /// 单个文件大小
        /// </summary>
        /// <returns></returns>
        [Column("LIMITFILESIZE")]
        public int? LimitFileSize { get; set; }
        /// <summary>
        /// 上传文件类型
        /// </summary>
        /// <returns></returns>
        [Column("FILETYPE")]
        public string FileType { get; set; }
        /// <summary>
        /// 保存路径
        /// </summary>
        /// <returns></returns>
        [Column("SAVEPATH")]
        public string SavePath { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        [Column("REMARK")]
        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        [Column("CREATETIME")]
        public DateTime? CreateTime { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public void Create()
        {
            //this.OperationCode = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public void Modify(string keyValue)
        {
            this.OperationCode = keyValue;
        }
        #endregion
    }
}

