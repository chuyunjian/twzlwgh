/* * 创建人：超级管理员
 * 日  期：2018-12-11 22:49
 * 描  述：附件注册
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
            // 查询
            $('#btn_Search').on('click', function () {
                var keyword = $('#txt_Keyword').val();
                page.search({ keyword: keyword });
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
                    url: top.$.rootUrl + '/SYS_Code/Sys_AccOperation/Form',
                    width: 700,
                    height: 400,
                    callBack: function (id) {
                        return top[id].acceptClick(refreshGirdData);
                    }
                });
            });
            // 编辑
            $('#lr_edit').on('click', function () {
                var keyValue = $('#gridtable').jfGridValue('OperationCode');
                selectedRow = $('#gridtable').jfGridGet('rowdata');
                if (learun.checkrow(keyValue)) {
                    learun.layerForm({
                        id: 'form',
                        title: '编辑',
                        url: top.$.rootUrl + '/SYS_Code/Sys_AccOperation/Form?keyValue=' + keyValue,
                        width: 700,
                        height: 400,
                        callBack: function (id) {
                            return top[id].acceptClick(refreshGirdData);
                        }
                    });
                }
            });
            // 删除
            $('#lr_delete').on('click', function () {
                var keyValue = $('#gridtable').jfGridValue('OperationCode');
                if (learun.checkrow(keyValue)) {
                    learun.layerConfirm('是否确认删除该项！', function (res) {
                        if (res) {
                            learun.deleteForm(top.$.rootUrl + '/SYS_Code/Sys_AccOperation/DeleteForm', { keyValue: keyValue }, function () {
                                $('#btn_Search').trigger('click');
                            });
                        }
                    });
                }
            });
            // 详情
            $('#lr_detail').on('click', function () {
                var keyValue = $('#gridtable').jfGridValue('OperationCode');
                if (learun.checkrow(keyValue)) {
                    learun.layerIndex({
                        id: 'detail',
                        title: '详情',
                        url: top.$.rootUrl + '/SYS_Code/Sys_AccOperation/Detail?keyValue=' + keyValue,
                        width: 700,
                        height: 400,
                    });
                }
            });
        },
        initGird: function () {
            $('#gridtable').lrAuthorizeJfGrid({
                url: top.$.rootUrl + '/SYS_Code/Sys_AccOperation/GetPageList',
                headData: [
                    { label: '附件编码', name: 'OperationCode', width: 200, align: "left" },
                    { label: '附件名称', name: 'OperationName', width: 200, align: "left" },
                    { label: '文件大小', name: 'LimitTotalSize', width: 200, align: "left" },
                    { label: '单个文件大小', name: 'LimitFileSize', width: 200, align: "left" },
                    { label: '上传文件类型', name: 'FileType', width: 200, align: "left" },
                    { label: '保存路径', name: 'SavePath', width: 200, align: "left" },
                    { label: '备注', name: 'Remark', width: 200, align: "left" },
                    { label: '创建时间', name: 'CreateTime', width: 200, align: "left" },
                ],
                mainId: 'OperationCode',
                isPage: true
            });
            page.search();
        },
        search: function (param) {
            param = param || {};
            $('#gridtable').jfGridSet('reload', { queryJson: JSON.stringify(param) });
        }
    };
    refreshGirdData = function () {
        $('#gridtable').jfGridSet('reload');
    };
    page.init();
}
