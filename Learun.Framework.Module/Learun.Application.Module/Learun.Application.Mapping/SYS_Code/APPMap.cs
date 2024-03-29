﻿using Learun.Application.TwoDevelopment.SYS_Code;
using System.Data.Entity.ModelConfiguration;

namespace  Learun.Application.Mapping
{
    /// <summary>
    /// 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架
    /// Copyright (c) 2013-2018 上海力软信息技术有限公司
    /// 创 建：超级管理员
    /// 日 期：2019-05-06 08:35
    /// 描 述：APP管理
    /// </summary>
    public class APPMap : EntityTypeConfiguration<APPEntity>
    {
        public APPMap()
        {
            #region 表、主键
            //表
            this.ToTable("APP");
            //主键
            this.HasKey(t => t.AGuid);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}

