using Learun.Util;
using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Learun.Application.TwoDevelopment.SYS_Code

{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2019-01-07 15:01
    /// 描 述：极光推送表
    /// </summary>
    public class SMS_JPushNotificationEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        /// <returns></returns>
        [Column("MID")]
        public int MID { get; set; }
        /// <summary>
        /// 极光messageid
        /// </summary>
        /// <returns></returns>
        [Column("MESSAGEID")]
        public string MessageId { get; set; }
        /// <summary>
        /// 极光推送序号
        /// </summary>
        /// <returns></returns>
        [Column("SENDNO")]
        public int? SendNo { get; set; }
        /// <summary>
        /// 通知消息
        /// </summary>
        /// <returns></returns>
        [Column("ALERT")]
        public string Alert { get; set; }
        /// <summary>
        /// 推送别名（逗号分隔）
        /// </summary>
        /// <returns></returns>
        [Column("ALIAS")]
        public string Alias { get; set; }
        /// <summary>
        /// 推送标签（逗号分隔）
        /// </summary>
        /// <returns></returns>
        [Column("TAG")]
        public string Tag { get; set; }
        /// <summary>
        /// 序列化的数据键值对
        /// </summary>
        /// <returns></returns>
        [Column("EXTRASJSON")]
        public string ExtrasJson { get; set; }
        /// <summary>
        /// 序列化的推送PushPayload对象
        /// </summary>
        /// <returns></returns>
        [Column("PUSHPAYLOADJSON")]
        public string PushPayloadJson { get; set; }
        /// <summary>
        /// 推送时间
        /// </summary>
        /// <returns></returns>
        [Column("PUSHTIME")]
        public DateTime? PushTime { get; set; }
        /// <summary>
        /// 类型（0：公开广播  1：互动别名  2：互动别名）
        /// </summary>
        /// <returns></returns>
        [Column("PTYPE")]
        public int? PType { get; set; }
        /// <summary>
        /// 状态（0：未推送 1：已推送 2：推送失败）
        /// </summary>
        /// <returns></returns>
        [Column("PSTATUS")]
        public int? PStatus { get; set; }
        /// <summary>
        /// 日志
        /// </summary>
        /// <returns></returns>
        [Column("PLOG")]
        public string PLog { get; set; }
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
            this.CreateTime = DateTime.Now;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public void Modify(int? keyValue)
        {
            this.MID = keyValue.Value;
        }
        #endregion
    }
}

