/* * Copyright (c) 2013-2018 思必达软件技术有限公司
 * 创建人：超级管理员
 * 日  期：2019-05-06 08:35
 * 描  述：APP管理
 */
var acceptClick;
var keyValue = request('keyValue');
var bootstrap = function ($, learun) {
    "use strict";
    var selectedRow = learun.frameTab.currentIframe().selectedRow;
    var page = {
        init: function () {
            page.initData();
            page.bind();
        },
        bind: function () {
             // 刷新
             $('#lr-reload').on('click', function () {
                 window.location.reload();
             });
             // 打印
             $('#lr-print').on('click', function () {
                 $("#gridPanel").jqprint();
             });
        },
        initData: function () {
            if (!!selectedRow) {
            }
        }
    };
    page.init();
}
