/* * Copyright (c) 2013-2018 思必达软件技术有限公司
 * 创建人：超级管理员
 * 日  期：2019-03-27 14:48
 * 描  述：单位管理
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
            page.bindForm();
        },
        bind: function () {
            //读取cookie中的token
            $("#Bearer").val(top.$.cookie('BearerToken'));
            // 刷新
            $('#lr-reload').on('click', function () {
                window.location.reload();
            });
            // 打印
            $('#lr-print').on('click', function () {
                $("#gridPanel").jqprint();
            });
            //绑定form
            $('#SourceCode').on('input propertychange', function () {
                page.bindForm();
            });
            //登录
            $('#login').on('click', function () {
                var account = 'system';
                var pwd = '4a7d1ed414474e4033ac29ccb8653d9b';
                var loginType = "0";
                top.layer.prompt({
                    value: account,
                    title: '账号'
                }, function (value, index, elem) {
                    account = value;
                    top.layer.close(index);
                    top.layer.prompt({
                        value: pwd,
                        title: '密码'
                    }, function (value, index, elem) {
                        pwd = value;
                        top.layer.close(index);
                        top.layer.prompt({
                            value: loginType,
                            title: '登录类型'
                        }, function (value, index, elem) {
                            loginType = value;
                            top.layer.close(index);
                            page.login(account, pwd, loginType);
                        });
                    });
                });
            });
        },
        initData: function () {
            if (!!selectedRow) {
            }
        },
        bindForm: function () {
            $("#SourceForm").html('');
            var datas = $('#SourceCode').val().split('\n')
            $.each(datas, function (index, item) {
                if (item.trim().replace(" ", "").length > 0) {
                    var element = $(' <div class="col-xs-12 lr-form-item">'
                        + '     <div class="lr-form-item-title key"></div>'
                        + '     <input type="text" class="form-control value" />'
                        + ' </div>');
                    var data = item.split(':');
                    if (data.length > 0) {
                        element.find('.key').text(data[0]);
                    }
                    if (data.length > 1) {
                        element.find('.value').attr('name', data[0]).val(data[1]);
                    }
                    $("#SourceForm").append(element);
                }
            });
            $("#SourceForm .lr-form-item input.value").on('input propertychange', function () {
                page.bindSource();
            });
        },
        bindSource: function () {
            $('#SourceCode').val("");
            var str = [];
            $.each($("#SourceForm .lr-form-item"), function (index, item) {
                str.push($(item).find('.key').text() + ":" + $(item).find('.value').val());
            });
            $('#SourceCode').val(str.join('\n'));
        },
        login: function (account, pwd, loginType) {
            var index = top.layer.load();
            $.ajax({
                url: top.$.rootUrl + '/rest/user/login',
                type: 'POST',
                datatype: "json",
                data: {
                    Account: account,
                    Password: pwd,
                    LoginType: loginType
                },
                headers: {

                },
                success: function (data) {
                    if (data.code == 200) {
                        top.$.cookie('BearerToken', data.data[0].Token, { expires: 365 });
                        $("#Bearer").val(data.data[0].Token);
                    } else {
                        top.layer.msg(data.info);
                    }
                    top.layer.close(index);
                },
                error: function (err) {
                    top.layer.msg("服务器请求错误");
                    top.layer.close(index);
                }
            });
        },
        //将对象转为FormData键值对的方法
        makeFormData: function (obj, form_data) {
            var data = [];
            if (obj instanceof File) {
                data.push({ key: "", value: obj });
            }
            else if (obj instanceof Array) {
                for (var j = 0, len = obj.length; j < len; j++) {
                    var arr = makeFormData(obj[j]);
                    for (var k = 0, l = arr.length; k < l; k++) {
                        var key = !!form_data ? j + arr[k].key : "[" + j + "]" + arr[k].key;
                        data.push({ key: key, value: arr[k].value })
                    }
                }
            }
            else if (typeof obj == 'object') {
                for (var j in obj) {
                    var arr = makeFormData(obj[j]);
                    for (var k = 0, l = arr.length; k < l; k++) {
                        var key = !!form_data ? j + arr[k].key : "[" + j + "]" + arr[k].key;
                        data.push({ key: key, value: arr[k].value })
                    }
                }
            }
            else {
                data.push({ key: "", value: obj });
            }
            if (!!form_data) {
                // 封装
                for (var i = 0, len = data.length; i < len; i++) {
                    form_data.append(data[i].key, data[i].value)
                }
            }
            else {
                return data;
            }
        }
    };
    // 保存数据
    acceptClick = function (callBack) {
        $("#preData").html("数据提交中...");
        $.ajax({
            url: top.$.rootUrl + '/' + $('input.route').val(),
            type: $("#Method").text(),
            datatype: "json",
            data: $("#SourceForm").serialize(),
            headers: {
                'Authorization': 'Bearer ' + $("#Bearer").val(),
            },
            success: function (data) {
                $('#preData').jsonViewer(data);
            },
            error: function (err) {
                $('#preData').jsonViewer(err);
            }
        });
    };
    page.init();
}
