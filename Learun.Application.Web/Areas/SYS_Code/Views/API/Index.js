/* * 创建人：超级管理员
 * 日  期：2019-03-27 14:48
 * 描  述：单位管理
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
            }, 220, 400);
            $('#CType').lrDataItemSelect({ code: 'CType' });
            $('#CMemberShip').lrDataItemSelect({ code: 'CMemberShip' });
            // 查询
            $('#btn_Search').on('click', function () {
                var keyword = $('#txt_Keyword').val();
                page.search({ keyword: keyword });
            });
            // 刷新
            $('#lr_refresh').on('click', function () {
                location.reload();
            });
            // 调试
            $('#lr_debug').on('click', function () {
                var keyValue = $('#gridtable').jfGridValue('ID');
                selectedRow = $('#gridtable').jfGridGet('rowdata');
                if (learun.checkrow(keyValue)) {
                    learun.layerForm({
                        id: 'debug',
                        title: '调试',
                        url: top.$.rootUrl + '/SYS_Code/API/Debug?keyValue=' + encodeURIComponent(encodeURIComponent(keyValue)),
                        width: 960,
                        height: 580,
                        sureClose: false,
                        btn: ['提交', '关闭'],
                        callBack: function (id) {
                            return top[id].acceptClick(refreshGirdData);
                        }
                    });
                }
            });
            // 调用说明
            $('#lr_detail').on('click', function () {
                learun.layerIndex({
                    id: 'detail',
                    title: '调用说明',
                    url: top.$.rootUrl + '/SYS_Code/API/Explain?keyValue=',
                    width: 1024,
                    height: 600,
                });
            });
        },
        initGird: function () {
            $('#gridtable').jfGrid({
                url: top.$.rootUrl + '/SYS_Code/API/GetPageList',
                headData: [
                    {
                        label: '请求方式', name: 'Method', width: 80, align: "left", formatterAsync: function (callback, value, row, op, $cell) {
                            var span = "";
                            switch (value) {
                                case "GET":
                                    span = '<span class="label label-info" style="display: inline-block;width: 50px;">GET</span>'
                                    break;
                                case "POST":
                                    span = '<span class="label label-success" style="display: inline-block;width: 50px;">POST</span>'
                                    break;
                                case "PUT":
                                    span = '<span class="label label-warning" style="display: inline-block;width: 50px;">PUT</span>'
                                    break;
                                case "DELETE":
                                    span = '<span class="label label-danger" style="display: inline-block;width: 50px;">DELETE</span>'
                                    break;
                                default:
                                    break;
                            }
                            callback(span);
                        }
                    },
                    { label: '所属控制器', name: 'Controller', width: 100, align: "left" },
                    { label: '接口描述', name: 'Name', width: 200, align: "left" },
                    { label: '参数说明', name: 'ParamDesc', width: 200, align: "left" },
                    { label: '路由', name: 'Route', width: 400, align: "left" },
                ],
                mainId: 'ID',
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
