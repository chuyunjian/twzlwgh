using Learun.Util;
using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Learun.Application.TwoDevelopment.SYS_Code

{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2019-01-29 14:40
    /// 描 述：短信
    /// </summary>
    public class SMS_SendEntity
    {
        #region 实体成员
        /// <summary>
        /// ID
        /// </summary>
        /// <returns></returns>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public int? ID { get; set; }
        /// <summary>
        /// 接收号码
        /// </summary>
        /// <returns></returns>
        [Column("RECEIVEMOBILE")]
        public string ReceiveMobile { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        /// <returns></returns>
        [Column("VERIFYCODE")]
        public string VerifyCode { get; set; }
        /// <summary>
        /// 发送内容
        /// </summary>
        /// <returns></returns>
        [Column("CONTENT")]
        public string Content { get; set; }
        /// <summary>
        /// 状态:0未使用,1已使用
        /// </summary>
        /// <returns></returns>
        [Column("STATUS")]
        public int? Status { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        /// <returns></returns>
        [Column("VALIDTIME")]
        public DateTime? ValidTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        [Column("CREATETIME")]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 发送状态:0失败,1成功
        /// </summary>
        /// <returns></returns>
        [Column("SENDSTATUS")]
        public int? SendStatus { get; set; }
        /// <summary>
        /// 发送日志
        /// </summary>
        /// <returns></returns>
        [Column("SENDLOG")]
        public string SendLog { get; set; }
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
        public void Modify(int? keyValue)
        {
            this.ID = keyValue;
        }
        #endregion
    }
}

