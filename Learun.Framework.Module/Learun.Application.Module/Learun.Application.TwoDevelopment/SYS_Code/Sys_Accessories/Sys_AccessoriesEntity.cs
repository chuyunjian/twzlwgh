using Learun.Util;
using System;
using System.ComponentModel.DataAnnotations.Schema;
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
    public class Sys_AccessoriesEntity
    {
        #region 实体成员
        /// <summary>
        /// 记录编码
        /// </summary>
        /// <returns></returns>
        [Column("ID")]
        public string ID { get; set; }
        /// <summary>
        /// 物理文件名
        /// </summary>
        /// <returns></returns>
        [Column("PHYFILENAME")]
        public string PhyFileName { get; set; }
        /// <summary>
        /// 系统文件名
        /// </summary>
        /// <returns></returns>
        [Column("SYSFILENAME")]
        public string SysFileName { get; set; }
        /// <summary>
        /// 附件编码
        /// </summary>
        /// <returns></returns>
        [Column("OPERATIONCODE")]
        public string OperationCode { get; set; }
        /// <summary>
        /// 业务编码
        /// </summary>
        /// <returns></returns>
        [Column("OPERATIONID")]
        public string OperationID { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        /// <returns></returns>
        [Column("FILESIZE")]
        public double? FileSize { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        [Column("CREATETIME")]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        /// <returns></returns>
        [Column("CREATEPERSON")]
        public string CreatePerson { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        /// <returns></returns>
        [Column("STATE")]
        public int? State { get; set; }
        /// <summary>
        /// 文件类型
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
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public void Create()
        {

        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public void Modify(string keyValue)
        {
            this.ID = keyValue;
        }
        /// <summary>
        /// 文件大小转汉字
        /// </summary>
        [NotMapped]
        public string FileSizeText
        {
            get
            {
                return WebHelper.HumanReadableFilesize(this.FileSize.HasValue ? this.FileSize.Value : 0);
            }
        }

        //文件路径+文件名
        [NotMapped]
        public string FilePath
        {
            get
            {
                return SavePath + PhyFileName;
            }
        }
        //获得网络路径
        public string getHttpPath()
        {
            string UploadUrl = "";
            switch (Config.GetValue("UploadFlag"))
            {
                case "0":
                    UploadUrl = WebHelper.WebUrl;
                    break;
                case "1":
                    UploadUrl = Config.GetValue("UploadUrl");
                    break;
                default:
                    UploadUrl = WebHelper.WebUrl;
                    break;
            }
            string httpUrl = UploadUrl + this.FilePath;
            return httpUrl;
        }
        #endregion
    }
}

