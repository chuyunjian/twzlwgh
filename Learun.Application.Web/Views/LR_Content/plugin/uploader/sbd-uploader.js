/*
 * 创建人：思必达-严笛
 * 日 期：2018-12-11
 * 描 述：sbd-uploader 表单附件选择插件
 */

(function ($, learun) {
    "use strict";

    $.sbdUploader = {
        layerFrameIndex: null,
        init: function ($self) {
            var dfop = $self[0]._sbdUploader.dfop;
            $.sbdUploader.initRender($self, dfop);
            //得到当前上传插件所在窗体对象ID
            $.sbdUploader.layerFrameIndex = top.layer.getFrameIndex(window.name);
        },
        initRender: function ($self, dfop) {
            switch (dfop.UpType) {
                case 1://附件模式
                    $self.attr('OperationCode', dfop.OperationCode);
                    $self.attr('type', 'sbd-Uploader').addClass('sbdUploader-wrap');
                    var $wrap = $('<div class="sbdUploader-input" ></div>');
                    var $uploadBtn = $('<a id="sbdUploader_uploadBtn_' + dfop.id + '" class="btn btn-success sbdUploader-input-btn">上传</a>');
                    var $btnGroup = $('<div class="sbdUploader-btn-group"></div>');
                    var $uploadMsg = $('<span class="extensions"></span>');
                    $btnGroup.append($uploadBtn);
                    $btnGroup.append($uploadMsg);
                    //$btnGroup.append($downBtn);
                    $self.append($btnGroup);
                    $self.append($wrap);
                    $uploadBtn.on('click', $.sbdUploader.openUploadForm);
                    //$downBtn.on('click', $.sbdUploader.openDownForm);
                    break;
                case 2://单张头像模式
                    $self.attr('type', 'sbd-Uploader').addClass('sbdUploader-wrap sbdUploader-InchPhoto');
                    var $input = $('<div class="sbdUploader-input" ></div>');
                    $self.append($input);
                    var $wrap = $('<a href="javascript:;" class="thumbnail InchPhoto " title="点击上传" style="width:' + dfop.Width + 'px;height:' + dfop.Height + 'px;">'
                        //+ '<i class="fa fa-plus" aria-hidden="true"></i>'
                        + '              <img src="/Content/images/head/avatar.png?v=20181229" alt="...">'
                        + '      </a>');
                    var $btnGroup = $('<div class="sbdUploader-btn-group"></div>');
                    $self.append($wrap);

                    $wrap.on('click', $.sbdUploader.openUploadAvatar);
                    break;
                default:
                    break;
            }

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
            //加密后请求附件
            $('#' + dfop.id).find('.sbdUploader-input').text("文件数据获取中...");
            $('#sbdUploader_uploadBtn_' + dfop.id).addClass("disabled");
            $('#sbdUploader_downBtn_' + dfop.id).addClass("disabled");
            var serverUrl = domainUrl + "/apiMD5?parameters=Function=AccessoriesList|UserID=" + UserID + "|OperationCode=" + dfop.OperationCode + "|OperationID=" + dfop.OperationID + "|MD5=" + res.data.MD5;
            learun.httpAsyncGet(serverUrl, function (res) {
                $.sbdUploader.filesCallBack(res.list, $self);
                //$self.sbdUploaderSet(dfop.OperationID, res);
            });
            //附件编码信息
            param = {};
            param['Value'] = (UserID + dfop.OperationCode);
            param['__RequestVerificationToken'] = $.lrToken;
            learun.httpPost(top.$.rootUrl + '/Other/GetMD5Token', param, function (resdata) {
                res = resdata;
            });
            var serverUrl = domainUrl + "/apiMD5?parameters=Function=AccOperationList|UserID=" + UserID + "|OperationCode=" + dfop.OperationCode + "|MD5=" + res.data.MD5;
            learun.httpAsyncGet(serverUrl, function (res) {
                if (res.result == "1") {
                    $.each(res.list, function (index,item) {
                        $('#' + dfop.id).find("span.extensions").text("上传类型：" + item.FileType);
                    });
                }
            });
        },
        openUploadForm: function () {
            var $btn = $(this);
            var $self = $btn.parents('.sbdUploader-wrap');
            var dfop = $self[0]._sbdUploader.dfop;
            learun.layerForm({
                id: dfop.id,
                title: dfop.placeholder,
                url: top.$.rootUrl + '/SYS_Code/Sys_Accessories/UploadForm?keyVaule=' + dfop.value + "&extensions=" + dfop.extensions + '&OperationCode=' + dfop.OperationCode + '&OperationID=' + dfop.OperationID + '&multiple=' + dfop.multiple + '&fileNumLimit=' + dfop.fileNumLimit,
                width: 600,
                height: 450,
                maxmin: false,
                callBack: function (id) {
                    return top[id].acceptClick($.sbdUploader.filesCallBack, $self);
                },
                success: function (layero, index) {
                    var iframeWin = top.window[layero.find('iframe')[0]['name']]; //得到iframe页的窗口对象，执行iframe页的方法：iframeWin.SetWinID();
                    //告诉上传页面，当前form页面对象
                    iframeWin.Form($self);
                }
            });
        },
        openDownForm: function () {
            var $btn = $(this);
            var $self = $btn.parents('.sbdUploader-wrap');
            var dfop = $self[0]._sbdUploader.dfop;
            learun.layerForm({
                id: dfop.id,
                title: dfop.placeholder,
                url: top.$.rootUrl + '/SYS_Code/Sys_Accessories/DownForm?keyVaule=' + dfop.value,
                width: 600,
                height: 400,
                maxmin: false,
                btn: null
            });
        },
        openUploadAvatar: function () {
            var $btn = $(this);
            var $self = $btn.parents('.sbdUploader-wrap');
            var dfop = $self[0]._sbdUploader.dfop;
            learun.layerForm({
                id: dfop.id,
                title: dfop.placeholder,
                url: top.$.rootUrl + '/SYS_Code/Sys_Accessories/UploadAvatar?keyVaule=' + dfop.value + "&extensions=" + dfop.extensions + '&OperationCode=' + dfop.OperationCode + '&OperationID=' + dfop.OperationID + '&aspectRatioWidth=' + dfop.Width + '&aspectRatioHeight=' + dfop.Height,
                width: 600,
                height: 402,
                maxmin: false,
                btn: null,
                end: function () {
                    var serverUrl = "";//上传地址
                    switch (top.$.uploadFlag) {
                        case "1"://自定义服务器

                            break;
                        default://本地

                            break;
                    }
                },
                success: function (layero, index) {
                    var iframeWin = top.window[layero.find('iframe')[0]['name']]; //得到iframe页的窗口对象，执行iframe页的方法：iframeWin.SetWinID();
                    //告诉上传页面，当前form页面对象
                    iframeWin.Form($self);
                }
            });
        },
        //保存文件回调
        filesCallBack: function (Files, $self) {
            var dfop = $self[0]._sbdUploader.dfop;
            switch (top.$.uploadFlag) {
                case "1"://自定义服务器

                    break;
                default://本地

                    break;
            }
            $('#' + dfop.id).find('.sbdUploader-input').html('');
            $('#sbdUploader_uploadBtn_' + dfop.id).removeClass("disabled");
            $('#sbdUploader_downBtn_' + dfop.id).removeClass("disabled");
            var SysFileName = [], Url = [];
            $.each(Files, function (index, item) {
                SysFileName.push(item.SysFileName);
                Url.push(item.HttpPath);
                //$ul.append("<li>" + item.SysFileName + '<a class="delete" href="javascript:;">删除</a>' + "</li>");
                var $item = $('<div class="lr-form-file-queue-item" id="lr_filequeue_' + item.AttachmentID + '" ></div>');
                $item.append('<div class="lr-file-image"><img src="' + top.$.rootUrl + '/Content/images/filetype/' + item.FileType + '.png"></div>');
                $item.append('<span class="lr-file-name">' + item.SysFileName + '(' + learun.countFileSize(item.FileSize) + ')</span>');
                $item.append('<div class="lr-tool-bar"><i class="fa fa-minus-circle" title="删除"  data-value="' + item.AttachmentID + '" ></i></div>');

                $item.find('.lr-tool-bar .fa-minus-circle').on('click', function () {
                    var fileId = $(this).attr('data-value');
                    $.sbdUploader.deleteFile(fileId, $self);
                });
                $('#' + dfop.id).find('.sbdUploader-input').append($item);
            });

            switch (dfop.UpType) {
                case 1://附件模式

                    break;
                case 2://头像模式
                    if (Url.length > 0) {
                        $('#' + dfop.id).find('.sbdUploader-input').parents(".sbdUploader-InchPhoto").find('.InchPhoto img').attr('src', Url[0]);
                    }
                    break;
                default:
                    break;
            }
            $('#' + dfop.id).find('.sbdUploader-input').attr('Files', JSON.stringify(Files));
        },
        //删除文件
        deleteFile: function (fileId, $self) {
            var dfop = $self[0]._sbdUploader.dfop;
            var Files = $('#' + dfop.id).find('.sbdUploader-input').attr('Files');
            if (Files.length > 0) {
                Files = JSON.parse(Files);
            } else {
                Files = [];
            }
            $('#' + dfop.id).find('.sbdUploader-input').find('#lr_filequeue_' + fileId).remove();
            var temp = [];
            $.each(Files, function (index, item) {
                if (item.AttachmentID != fileId) {
                    temp.push(item);
                }
            });
            Files = temp;
            $('#' + dfop.id).find('.sbdUploader-input').attr('Files', JSON.stringify(Files));
        },
    };

    $.fn.sbdUploader = function (op) {
        var $this = $(this);
        if (!!$this[0]._sbdUploader) {
            return $this;
        }
        var dfop = {
            placeholder: '上传附件',
            isUpload: true,
            isDown: true,
            extensions: '',
            UpType: 1,//上传类型（1：附件上传模式 2：头像上传模式）
            OperationCode: '',//附件编码
            OperationID: '',//业务主键
            Width: 90,//头像模式选中框宽
            Height: 132,//头像模式选中框高
            multiple: true,//true多选 false单选
            fileNumLimit: 0,//上传文件数量（默认0，不限制）
        }

        $.extend(dfop, op || {});
        dfop.id = $this.attr('id');
        dfop.value = learun.newGuid();

        $this[0]._sbdUploader = { dfop: dfop };
        $.sbdUploader.init($this);
    };

    $.fn.sbdUploaderSet = function (value, res) {
        var $self = $(this);
        var dfop = $self[0]._sbdUploader.dfop;
        dfop.value = value;
        switch (top.$.uploadFlag) {
            case "1"://自定义服务器

                break;
            default://本地

                break;
        }
        $('#sbdUploader_uploadBtn_' + dfop.id).removeClass("disabled");
        $('#sbdUploader_downBtn_' + dfop.id).removeClass("disabled");
        if (res.result == "1") {
            var FilesStr = "";
            //附件模式要读取已上传记录
            if (dfop.UpType == "1") {
                FilesStr = $('#' + dfop.id).find('.sbdUploader-input').attr('Files') ? $('#' + dfop.id).find('.sbdUploader-input').attr('Files') : "";
            }
            var Files = FilesStr ? JSON.parse(FilesStr) : [];
            $.each(res.list, function (index, item) {
                Files.push(item);
            });
            var SysFileName = [], Url = [];
            $.each(Files, function (index, item) {
                SysFileName.push(item.SysFileName);
                Url.push(item.HttpPath);
            });
            switch (dfop.UpType) {
                case 1://附件模式

                    break;
                case 2://头像模式
                    if (Url.length > 0) {
                        $('#' + dfop.id).find('.sbdUploader-input').parents(".sbdUploader-InchPhoto").find('.InchPhoto img').attr('src', Url[0]);
                    }
                    break;
                default:
                    break;
            }
            $('#' + dfop.id).find('.sbdUploader-input').attr('Files', JSON.stringify(Files)).text(SysFileName.join('、'));
        } else {
            $('#' + dfop.id).find('.sbdUploader-input').text('');
        }
    }

    $.fn.sbdUploaderAvatarSet = function (url) {
        var $self = $(this);
        var dfop = $self[0]._sbdUploader.dfop;
        $self.find('img').attr('src', '');
        $self.find('img').attr('src', url);
    }

    $.fn.sbdUploaderGet = function () {
        switch (top.$.uploadFlag) {
            case "1"://自定义服务器

                break;
            default:

                break;
        }
        var $this = $(this);
        var dfop = $this[0]._sbdUploader.dfop;
        //return dfop.value;
        var FilesStr = $('#' + dfop.id).find('.sbdUploader-input').attr('Files');
        var Files = FilesStr ? JSON.parse(FilesStr) : [];
        var AttachmentID = [];
        $.each(Files, function (index, item) {
            AttachmentID.push(item.AttachmentID);
        });
        if (AttachmentID.length > 0) {
            return AttachmentID.join(',');
        }
        else {
            return '';
        }
    }

    $.fn.sbdUploaderGetFiles = function () {
        var $this = $(this);
        var dfop = $this[0]._sbdUploader.dfop;
        //return dfop.value;
        var FilesStr = $('#' + dfop.id).find('.sbdUploader-input').attr('Files');
        var Files = FilesStr ? JSON.parse(FilesStr) : [];
        return Files;
    }

    $.fn.sbdUploaderDelete = function (fileId) {
        var $self = $(this);
        var dfop = $self[0]._sbdUploader.dfop;
        dfop.value = fileId;
        var FilesStr = $('#' + dfop.id).find('.sbdUploader-input').attr('Files') ? $('#' + dfop.id).find('.sbdUploader-input').attr('Files') : "";
        var Files = FilesStr ? JSON.parse(FilesStr) : [];
        var FilesTemp = [];
        $.each(Files, function (index, item) {
            if (item.AttachmentID != fileId) {
                FilesTemp.push(item);
            }
        });
        var SysFileName = [];
        $.each(FilesTemp, function (index, item) {
            SysFileName.push(item.SysFileName);
        });
        $('#' + dfop.id).find('.sbdUploader-input').attr('Files', JSON.stringify(FilesTemp)).text(SysFileName.join('、'));
    }
})(jQuery, top.learun);