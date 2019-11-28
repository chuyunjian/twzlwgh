using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learun.Util.Web.Model
{
    /// <summary>
    /// SMS_JPushNotification（极光推送-通知）
    /// </summary>
    [Serializable]
    public class SMS_JPushNotificationInfo
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public SMS_JPushNotificationInfo()
        {




            Alert = string.Empty;
            ExtrasJson = string.Empty;
            PushPayloadJson = string.Empty;
            PushTime = DateTime.Parse("1990/1/1 00:00:00");


            PLog = string.Empty;
            CreateTime = DateTime.Now;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public int MID { get; set; }

        /// <summary>
        /// 极光messageid
        /// </summary>
        public long MessageId { get; set; }

        /// <summary>
        /// 极光推送序号
        /// </summary>
        public int SendNo { get; set; }

        /// <summary>
        /// 通知消息
        /// </summary>
        public string Alert { get; set; }

        /// <summary>
        /// 推送别名（逗号分隔）
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 推送标签（逗号分隔）
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 序列化的数据键值对
        /// </summary>
        public string ExtrasJson { get; set; }

        /// <summary>
        /// 序列化的推送PushPayload对象
        /// </summary>
        public string PushPayloadJson { get; set; }

        /// <summary>
        /// 推送时间
        /// </summary>
        public DateTime PushTime { get; set; }

        /// <summary>
        /// 类型（0：公开广播  1：互动别名  2：互动别名）
        /// </summary>
        public int PType { get; set; }

        /// <summary>
        /// 状态（0：未推送 1：已推送 2：推送失败）
        /// </summary>
        public int PStatus { get; set; }

        /// <summary>
        /// 日志
        /// </summary>
        public string PLog { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        #endregion

        #region 扩展属性

        #endregion
    }
}
