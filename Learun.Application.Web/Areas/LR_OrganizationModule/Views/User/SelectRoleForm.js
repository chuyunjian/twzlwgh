/*
 * 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架(http://www.learun.cn)
 * Copyright (c) 2013-2018 上海力软信息技术有限公司
 * 创建人：力软-前端开发组
 * 日 期：2017.04.18
 * 描 述：管理区域	
 */
var acceptClick;
var keyValue = request('keyValue');//用户主键
var bootstrap = function ($, learun) {
    "use strict";


    var selectedRow = learun.frameTab.currentIframe().selectedRow;
    var page = {
        init: function () {
            page.bind();
            page.initData();
        },
        bind: function () {
            //角色列表
            learun.httpAsyncGet(top.$.rootUrl + '/LR_OrganizationModule/Role/GetTree', function (res) {
                if (res.code == 200) {
                    setTimeout(function () {
                        $('#RoleTree').lrtree({
                            data: res.data,
                        });
                        page.bindData();
                    }, 10);
                }
            });
        },
        initData: function () {
            if (!!selectedRow) {

            }
        },
        bindData: function () {
            learun.httpAsyncGet(top.$.rootUrl + '/LR_AuthorizeModule/UserRelation/GetObjectIdList?userId=' + keyValue + '&category=1', function (res) {
                if (res.code == 200) {
                    $.each(res.data, function (id, item) {
                        var divId = 'RoleTree_' + item.F_ObjectId.replace(/-/g, '_');
                        var $div = $('#RoleTree').find('#' + divId);
                        $div.find('.lr-tree-node-cb').trigger('click');
                    });
                }
            });
        }
    };
    // 保存数据
    acceptClick = function (callBack) {
        if (!$('#form').lrValidform()) {
            return false;
        }
        var postData = $('#form').lrGetFormData(keyValue);
        var Nodes = [];
        var tt = $('#RoleTree').lrtreeSet('getCheckNodeIds');
        $.each(tt, function (id, item) {
            if (item.indexOf('_learun_moduleId') == -1) {
                Nodes.push(item);
            }
        });
        postData["userId"] = keyValue;
        postData["objectIds"] = Nodes.join(',');
        postData["category"] = 1;
        $.lrSaveForm(top.$.rootUrl + '/LR_AuthorizeModule/UserRelation/SaveObjectList', postData, function (res) {
            // 保存成功后才回调
            if (!!callBack) {
                callBack();
            }
        });
    };
    page.init();
}