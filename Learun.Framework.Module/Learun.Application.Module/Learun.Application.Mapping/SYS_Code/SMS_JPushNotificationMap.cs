using Learun.Application.TwoDevelopment.SYS_Code;
using System.Data.Entity.ModelConfiguration;

namespace  Learun.Application.Mapping
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2019-01-07 15:01
    /// 描 述：极光推送表
    /// </summary>
    public class SMS_JPushNotificationMap : EntityTypeConfiguration<SMS_JPushNotificationEntity>
    {
        public SMS_JPushNotificationMap()
        {
            #region 表、主键
            //表
            this.ToTable("SMS_JPUSHNOTIFICATION");
            //主键
            this.HasKey(t => t.MID);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}

