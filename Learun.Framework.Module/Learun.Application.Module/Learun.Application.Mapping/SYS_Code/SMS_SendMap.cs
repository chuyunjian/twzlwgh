using Learun.Application.TwoDevelopment.SYS_Code;
using System.Data.Entity.ModelConfiguration;

namespace  Learun.Application.Mapping
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2019-01-29 14:40
    /// 描 述：短信
    /// </summary>
    public class SMS_SendMap : EntityTypeConfiguration<SMS_SendEntity>
    {
        public SMS_SendMap()
        {
            #region 表、主键
            //表
            this.ToTable("SMS_SEND");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}

