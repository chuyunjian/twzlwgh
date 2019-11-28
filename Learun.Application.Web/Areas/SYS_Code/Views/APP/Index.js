/* * 版 本 Learun-ADMS V6.1.6.0 力软敏捷开发框架(http://www.learun.cn)
 * Copyright (c) 2013-2017 上海力软信息技术有限公司
 * 创建人：超级管理员
 * 日  期：2018-11-28 16:34
 * 描  述：app包管理
 */
var selectedRow;
var refreshGirdData;
var bootstrap = function ($, learun) {
    "use strict";
    var page = {
        init: function () {
            page.initGird();
            page.bind();
        },
        bind: function () {
            $('#multiple_condition_query').lrMultipleQuery(function (queryJson) {
                page.search(queryJson);
            }, 150, 350);
            //类型
            $('#Type').lrselect({
                maxHeight: 100
            });
            // 查询
            $('#btn_Search').on('click', function () {
                page.search();
            });
            // 刷新
            $('#lr_refresh').on('click', function () {
                location.reload();
            });
            // 新增
            $('#lr_add').on('click', function () {
                selectedRow = null;
                learun.layerForm({
                    id: 'form',
                    title: '新增',
                    url: top.$.rootUrl + '/SYS_Code/App/Form',
                    width: 700,
                    height: 500,
                    callBack: function (id) {
                        return top[id].acceptClick(refreshGirdData);
                    }
                });
            });
            // 编辑
            $('#lr_edit').on('click', function () {
                var keyValue = $('#girdtable').jfGridValue('AGuid');
                selectedRow = $('#girdtable').jfGridGet('rowdata');
                if (learun.checkrow(keyValue)) {
                    learun.layerForm({
                        id: 'form',
                        title: '编辑',
                        url: top.$.rootUrl + '/SYS_Code/App/Form?keyValue=' + keyValue,
                        width: 700,
                        height: 500,
                        callBack: function (id) {
                            return top[id].acceptClick(refreshGirdData);
                        }
                    });
                }
            });
            // 删除
            $('#lr_delete').on('click', function () {
                var keyValue = $('#girdtable').jfGridValue('AGuid');
                if (learun.checkrow(keyValue)) {
                    learun.layerConfirm('是否确认删除该项！', function (res) {
                        if (res) {
                            learun.deleteForm(top.$.rootUrl + '/SYS_Code/App/DeleteForm', { keyValue: keyValue }, function () {
                            });
                        }
                    });
                }
            });
            // 详情
            //$('#lr_details').on('click', function () {
            //    var keyValue = $('#girdtable').jfGridValue('AGuid');
            //    if (learun.checkrow(keyValue)) {
            //        learun.layerIndex({
            //            id: 'form',
            //            title: '详情',
            //            url: top.$.rootUrl + '/SYS_Code/App/Details?keyValue=' + keyValue,
            //            width: 700,
            //            height: 400,
            //        });
            //    }
            //});
        },
        initGird: function () {
            $('#girdtable').lrAuthorizeJfGrid({
                url: top.$.rootUrl + '/SYS_Code/App/GetPageList',
                headData: [
                    { label: '名称', name: 'Name', width: 200, align: "left" },
                    { label: '版本', name: 'Version', width: 200, align: "left" },
                    {
                        label: '类型', name: 'Type', width: 200, align: "left",
                        formatter: function (cellvalue, options, rowObject) {
                            if (options.Type == "0") {
                                return "Android";
                            } else {
                                return "IOS";
                            }
                        }
                    },
                    { label: '更新日期', name: 'CreateDate', width: 200, align: "left" },
                    { label: '备注', name: 'ARemark', width: 400, align: "left" },


                ],
                mainId: 'AGuid',
                sidx: "CreateDate",
                sord: "desc",
                reloadSelected: true,
                isPage: true
            });
            page.search();
        },
        search: function (param) {
            var queryJson = JSON.stringify(param);
            param = param || {};
            $('#girdtable').jfGridSet('reload', { param: { queryJson: JSON.stringify(param) } });
        }
    };
    refreshGirdData = function () {
        page.search();
    };
    page.init();
}
