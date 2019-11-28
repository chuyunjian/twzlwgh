using Learun.Util;
using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Learun.Application.TwoDevelopment.SYS_Code

{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2019-05-06 08:35
    /// 描 述：APP管理
    /// </summary>
    public class APPEntity 
    {
        #region 实体成员
        /// <summary>
        /// AGuid
        /// </summary>
        /// <returns></returns>
        [Column("AGUID")]
        public string AGuid { get; set; }
        /// <summary>
        /// Version
        /// </summary>
        /// <returns></returns>
        [Column("VERSION")]
        public string Version { get; set; }
        /// <summary>
        /// Type
        /// </summary>
        /// <returns></returns>
        [Column("TYPE")]
        public int? Type { get; set; }
        /// <summary>
        /// ARemark
        /// </summary>
        /// <returns></returns>
        [Column("AREMARK")]
        public string ARemark { get; set; }
        /// <summary>
        /// SysCode
        /// </summary>
        /// <returns></returns>
        [Column("SYSCODE")]
        public string SysCode { get; set; }
        /// <summary>
        /// CreateDate
        /// </summary>
        /// <returns></returns>
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        /// <returns></returns>
        [Column("NAME")]
        public string Name { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public void Create()
        {
            this.CreateDate = DateTime.Now;
            this.AGuid = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public void Modify(string keyValue)
        {
            this.AGuid = keyValue;
        }
        #endregion
        [NotMapped]
        public string FileUrl { get; set; }
    }
}

