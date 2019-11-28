/* * 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架(http://www.learun.cn)
 * Copyright (c) 2013-2018 上海力软信息技术有限公司
 * 创建人：超级管理员
 * 日  期：2018-12-11 22:49
 * 描  述：附件注册
 */
var acceptClick;
var keyValue = request('keyValue');
var bootstrap = function ($, learun) {
    "use strict";
    var selectedRow = learun.frameTab.currentIframe().selectedRow;
    var page = {
        init: function () {
            page.initData();
        },
        bind: function () {
        },
        initData: function () {
            if (!!selectedRow) {
                $('#form').lrSetFormData(selectedRow);
            } else {
                selectedRow = {};
                selectedRow.FileType = "jpg,jpeg,gif,png,mp4,rar,zip,doc,docx,xls,xlsx";
                selectedRow.SavePath = "/FileUpload/TTT/";
                $('#form').lrSetFormData(selectedRow);
            }
        }
    };
    // 保存数据
    acceptClick = function (callBack) {
        if (!$('#form').lrValidform()) {
            return false;
        }
        var postData = $('#form').lrGetFormData();
        $.lrSaveForm(top.$.rootUrl + '/SYS_Code/Sys_AccOperation/SaveForm?keyValue=' + keyValue, postData, function (res) {
            // 保存成功后才回调
            if (!!callBack) {
                callBack();
            }
        });
    };
    page.init();
}
