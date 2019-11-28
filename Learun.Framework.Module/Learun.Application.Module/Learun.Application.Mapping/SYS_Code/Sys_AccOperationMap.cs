using Learun.Application.TwoDevelopment.SYS_Code;
using System.Data.Entity.ModelConfiguration;

namespace  Learun.Application.Mapping
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2018-12-26 23:33
    /// 描 述：附件业务注册
    /// </summary>
    public class Sys_AccOperationMap : EntityTypeConfiguration<Sys_AccOperationEntity>
    {
        public Sys_AccOperationMap()
        {
            #region 表、主键
            //表
            this.ToTable("SYS_ACCOPERATION");
            //主键
            this.HasKey(t => t.OperationCode);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}

