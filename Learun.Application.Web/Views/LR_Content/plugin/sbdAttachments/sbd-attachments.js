/*
 * 创建人：严笛
 * 日 期：2019-01-12
 * 描 述：附件展示
 */
(function ($, learun) {
    "use strict";

    $.sbdAttachments = {
        init: function ($self) {
            var dfop = $self[0]._sbdAttachments.dfop;
            $.sbdAttachments.initRender($self, dfop);
        },
        initRender: function ($self, dfop) {
            $self.addClass('sbdAttachments-wrap').attr('ID', 'sbdAttachments-' + dfop.OperationCode + '-' + dfop.OperationID);
            $self.html('<p class="init">文件数据获取中...</p>');
            //获得附件数据
            var domainUrl = "";
            switch (top.$.uploadFlag) {
                case "1"://自定义服务器
                    domainUrl = top.$.uploadUrl;
                    break;
                default://本地服务器
                    domainUrl = top.$.rootUrl;
                    break;
            }
            var loginInfo = learun.clientdata.get(['userinfo']);
            var UserID = loginInfo.userId;
            //先后台进行MD5签名（后台调用原因：隐藏加密字符）
            var param = {};
            param['Value'] = (UserID + dfop.OperationCode + dfop.OperationID);
            param['__RequestVerificationToken'] = $.lrToken;
            var res = {};
            learun.httpPost(top.$.rootUrl + '/Other/GetMD5Token', param, function (resdata) {
                res = resdata;
            });
            var serverUrl = domainUrl + "/apiMD5?parameters=Function=AccessoriesList|UserID=" + UserID + "|OperationCode=" + dfop.OperationCode + "|OperationID=" + dfop.OperationID + "|MD5=" + res.data.MD5;
            learun.httpAsyncGet(serverUrl, function (res) {
                $self.sbdAttachmentsSet(res);
            });
        }
    };

    $.fn.sbdAttachments = function (op) {
        var $this = $(this);
        if (!!$this[0]._sbdAttachments) {
            return $this;
        }
        var dfop = {
            OperationCode: '',//附件编码
            OperationID: '',//业务主键
            UpType: 1,//上传类型（1：附件上传模式 2：头像上传模式）
            ShowName:0,//0,展示附件名称，1，不展示
            Width: 90,//头像模式选中框宽
            Height: 132,//头像模式选中框高
        }

        $.extend(dfop, op || {});

        $this[0]._sbdAttachments = { dfop: dfop };
        $.sbdAttachments.init($this);
    };

    $.fn.sbdAttachmentsSet = function (res) {
        var $self = $(this);
        var dfop = $self[0]._sbdAttachments.dfop;
        var images = [];
        if (res.result == "1") {
            switch (dfop.UpType) {
                case 1://附件模式
                    var $ul = $('<ul></ul>')
                    $('#sbdAttachments-' + dfop.OperationCode + '-' + dfop.OperationID).html($ul);
                    $.each(res.list, function (index, item) {
                        switch (item.FileType.toLowerCase()) {
                            case "jpg":
                            case "jpeg":
                            case "png":
                            case "gif":
                                images.push(item);
                                if (dfop.ShowName == 1) {
                                    $ul.append('<li><a href="javascript:;" class="thumbnail" onclick="top.viewer(this,\'' + item.AttachmentID + '\');"><img src="' + item.HttpPath + '"/></a><p><a href="' + item.HttpPath + '" download="' + item.SysFileName + '" class="download">点击下载</a></p></li>');

                                } else {
                                    $ul.append('<li><a href="javascript:;" class="thumbnail" onclick="top.viewer(this,\'' + item.AttachmentID + '\');"><img src="' + item.HttpPath + '"/></a><p>' + item.SysFileName + '<a href="' + item.HttpPath + '" download="' + item.SysFileName + '" class="download">点击下载</a></p></li>');
                                }
                                break;
                            default:
                                if (dfop.ShowName == 1) {

                                    $ul.append('<li><a href="' + item.HttpPath + '" download="' + item.SysFileName + '" class="thumbnail"><img src="' + top.$.rootUrl + '/Content/images/filetype/' + item.FileType + '.png"/></a><p><a href="' + item.HttpPath + '" download="' + item.SysFileName + '" class="download">点击下载</a></p></li>');
                                } else {
                                    $ul.append('<li><a href="' + item.HttpPath + '" download="' + item.SysFileName + '" class="thumbnail"><img src="' + top.$.rootUrl + '/Content/images/filetype/' + item.FileType + '.png"/></a><p>' + item.SysFileName + '<a href="' + item.HttpPath + '" download="' + item.SysFileName + '" class="download">点击下载</a></p></li>');
                                }
                                break;
                        }
                    });
                    break;
                case 2://头像
                    $.each(res.list, function (index, item) {
                        switch (item.FileType.toLowerCase()) {
                            case "jpg":
                            case "jpeg":
                            case "png":
                            case "gif":
                                images.push(item);
                                var $wrap = $('<a href="javascript:;" class="thumbnail InchPhoto " onclick="top.viewer(this,\'' + item.AttachmentID + '\');">'
                                    + '              <img src="' + item.HttpPath + '" alt="">'
                                    + '      </a>');
                                $('#sbdAttachments-' + dfop.OperationCode + '-' + dfop.OperationID).html($wrap);
                                break;
                            default:
                                break;
                        }
                    });
                    break;
                default:
                    break;
            }
        } else {
            $('#sbdAttachments-' + dfop.OperationCode + '-' + dfop.OperationID).html('<p class="error">' + '数据获取失败：' + res.message + '</p>');
        }
        $('#sbdAttachments-' + dfop.OperationCode + '-' + dfop.OperationID).attr('datas', JSON.stringify(images));
    }
})(jQuery, top.learun);