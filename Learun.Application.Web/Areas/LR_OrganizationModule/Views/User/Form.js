/*
 * 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架(http://www.learun.cn)
 * Copyright (c) 2013-2018 上海力软信息技术有限公司
 * 创建人：力软-前端开发组
 * 日 期：2017.04.18
 * 描 述：账号添加	
 */
var companyId = request('companyId');


var acceptClick;
var keyValue = '';
var bootstrap = function ($, learun) {
    "use strict";
    var selectedRow = learun.frameTab.currentIframe().selectedRow;
    var page = {
        init: function () {
            page.bind();
            page.initData();
        },
        bind: function () {
            // 部门
            $('#F_DepartmentId').lrDepartmentSelect({ companyId: companyId });
            // 性别
            $('#F_Gender').lrselect();
            /*检测重复项*/
            $('#F_Account').on('blur', function () {
                $.lrExistField(keyValue, 'F_Account', top.$.rootUrl + '/LR_OrganizationModule/User/ExistAccount');
            });
            //角色
            $('#RoleIDs').lrselect({
                url: top.$.rootUrl + '/LR_OrganizationModule/Role/GetList',
                value: "F_RoleId",
                text: "F_FullName",
                title: "title",
                type: "multiple"
            });
        },
        initData: function () {
            if (!!selectedRow) {
                keyValue = selectedRow.F_UserId;
                selectedRow.F_Password = "******";
                $('#form').lrSetFormData(selectedRow);
                $('#F_Password').attr('readonly', 'readonly');
                $('#F_Account').attr('readonly', 'readonly');

                $('#F_Password').attr('unselectable', 'on');
                $('#F_Account').attr('unselectable', 'on');

                learun.httpAsyncGet(top.$.rootUrl + '/LR_AuthorizeModule/UserRelation/GetObjectIdList?userId=' + keyValue + '&category=1', function (res) {
                    if (res.code == 200) {
                        setTimeout(function () {
                            var F_ObjectId = [];
                            $.each(res.data, function (id, item) {
                                F_ObjectId.push(item.F_ObjectId);
                            });
                            $('#RoleIDs').lrselectSet(F_ObjectId.join(','));
                        }, 10);
                    }
                });
            }
            else {
                selectedRow = {};
                selectedRow.F_Password = "111111";
                $('#form').lrSetFormData(selectedRow);
                $('#F_CompanyId').val(companyId);
            }
        }
    };
    // 保存数据
    acceptClick = function (callBack) {
        if (!$('#form').lrValidform()) {
            return false;
        }
        var postData = $('#form').lrGetFormData(keyValue);
        if (!keyValue) {
            postData.F_Password = $.md5(postData.F_Password);
        }
        $.lrSaveForm(top.$.rootUrl + '/LR_OrganizationModule/User/SaveForm?keyValue=' + keyValue, postData, function (res) {
            // 保存成功后才回调
            if (!!callBack) {
                callBack();
            }
        });
    };
    page.init();
}