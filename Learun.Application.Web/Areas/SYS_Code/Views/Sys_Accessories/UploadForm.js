/*
 * 版 本 Learun-ADMS V7.0.0 力软敏捷开发框架(http://www.learun.cn)
 * Copyright (c) 2013-2018 上海力软信息技术有限机构
 * 创建人：力软-前端开发组
 * 日 期：2017.03.22
 * 描 述：附件上传管理	
 */
var keyVaule = request('keyVaule');
var extensions = request('extensions');
var OperationCode = request('OperationCode');
var OperationID = request('OperationID');
var multiple = request("multiple");
var fileNumLimit = request("fileNumLimit");
if (multiple == "false") { multiple = false; } else { multiple = true; }
//Form页对应的窗口，方便传值
var form = null;
var Form = function (_form) {
    form = _form;
};
//数据
var Files = [];
var acceptClick;
var bootstrap = function ($, learun) {
    "use strict";
    setTimeout(function () {
        //获取Form页数据
        Files = form.sbdUploaderGetFiles();
        for (var i = 0, l = Files.length; i < l; i++) {
            $('#lr_form_file_queue .lr-form-file-queue-bg').hide();
            var item = Files[i];
            fileInfo[item.AttachmentID] = {
                fileGuid: item.AttachmentID,
                chunks: 1
            }
            var $item = $('<div class="lr-form-file-queue-item" id="lr_filequeue_' + item.AttachmentID + '" ></div>');
            $item.append('<div class="lr-file-image"><img src="' + top.$.rootUrl + '/Content/images/filetype/' + item.FileType + '.png"></div>');
            $item.append('<span class="lr-file-name">' + item.SysFileName + '(' + learun.countFileSize(item.FileSize) + ')</span>');

            $item.append('<div class="lr-msg"><i class="fa fa-check-circle"></i></div>');
            //$item.append('<div class="lr-tool-bar"><i class="fa fa-cloud-download" title="下载"  data-value="' + item.AttachmentID + '" ></i><i class="fa fa-minus-circle" title="删除"  data-value="' + item.AttachmentID + '" ></i></div>');
            $item.append('<div class="lr-tool-bar"><i class="fa fa-minus-circle" title="删除"  data-value="' + item.AttachmentID + '" ></i></div>');

            $item.find('.lr-tool-bar .fa-minus-circle').on('click', function () {
                var fileId = $(this).attr('data-value');
                DeleteFile(fileId, fileId);
            });

            $item.find('.lr-tool-bar .fa-cloud-download').on('click', function () {
                var fileId = $(this).attr('data-value');
                DownFile(fileId);
            });

            $('#lr_form_file_queue_list').append($item);
        }
    }, 100)


    var fileInfo = {};

    // 触发合并文件碎片
    var mergeFileChunks = function (file, res) {
        var param = {};
        var serverUrl = "";//上传地址
        switch (top.$.uploadFlag) {
            case "1"://自定义服务器

                break;
            default://本地文件系统

                break;
        }
        // 文件保存成功后
        if (form == null) {
            alert("文件保存失败");
        }
        var $fileItem = $('#lr_form_file_queue_list').find('#lr_filequeue_' + file.id);
        $fileItem.find('.lr-uploader-progress').remove();
        $fileItem.append('<div class="lr-msg"><i class="fa fa-check-circle"></i></div>');
        $fileItem.append('<div class="lr-tool-bar"><i class="fa fa-minus-circle" title="删除" data-attachment="' + res.list[0].AttachmentID + '" data-value="' + file.id + '" ></i></div>');

        $fileItem.find('.lr-tool-bar .fa-minus-circle').on('click', function () {
            var fileId = $(this).attr('data-value');
            var attachmentId = $(this).attr('data-attachment');
            DeleteFile(fileId, attachmentId);
        });
        $.each(res.list, function (index, item) {
            Files.push(item);
        });
    }
    // 触发清楚文件碎片
    var reomveFileChunks = function (file) {
        //var param = {};
        //param['__RequestVerificationToken'] = $.lrToken;
        //param['fileGuid'] = fileInfo[file.id].fileGuid;
        //param['chunks'] = fileInfo[file.id].chunks;

        //learun.httpAsyncPost(top.$.fileUrl + "/SYS_Code/Sys_Accessories/MergeFile", param, function (res) { });
        var $fileItem = $('#lr_form_file_queue_list').find('#lr_filequeue_' + file.id);
        $fileItem.find('.lr-uploader-progress').remove();
        $fileItem.append('<div class="lr-msg"><i class="fa fa-exclamation-circle"></i></div>');
    }
    // 删除文件
    var DeleteFile = function (fileId, attachmentId) {
        var param = {};
        switch (top.$.uploadFlag) {
            case "1"://自定义服务器

                break;
            default://本地文件系统

                break;
        }
        var file = page.uploader.getFile(fileId);
        if (!!file) {
            page.uploader.removeFile(file);
        }
        delete fileInfo[fileId];
        var FilesTemp = [];
        $.each(Files, function (index, item) {
            if (item.AttachmentID != attachmentId) {
                FilesTemp.push(item);
            }
        });
        Files = FilesTemp;
        var $fileItem = $('#lr_form_file_queue_list').find('#lr_filequeue_' + fileId);
        $fileItem.remove();
        if ($('#lr_form_file_queue_list>div').length == 0) {
            $('#lr_form_file_queue .lr-form-file-queue-bg').show();
        }
    }
    // 下载文件
    var DownFile = function (fileId) {
        switch (top.$.uploadFlag) {
            case "1"://自定义服务器
                //设置Form页数据
                form.sbdUploaderSet(OperationID, res);
                break;
            default:
                learun.download({ url: top.$.fileUrl + '/SYS_Code/Sys_Accessories/DownFile', param: { fileId: fileId, __RequestVerificationToken: $.lrToken }, method: 'POST' });
                break;
        }
    }

    var page = {
        uploader: null,
        init: function () {
            if (!WebUploader.Uploader.support()) {
                alert('Web Uploader 不支持您的浏览器！如果你使用的是IE浏览器，请尝试升级 flash 播放器');
                throw new Error('WebUploader does not support the browser you are using.');
            }
            var domainUrl = "";//上传地址
            var chunked = true;//分片上传
            switch (top.$.uploadFlag) {
                case "1"://自定义服务器
                    domainUrl = top.$.uploadUrl;
                    break;
                default://本地文件系统
                    domainUrl = top.$.rootUrl;
                    break;
            }
            var loginInfo = learun.clientdata.get(['userinfo']);
            var UserID = loginInfo.userId;
            //附件编码信息
            if (extensions.length > 0) {
                $('.lr-form-layout-header').find("span.extensions").text("上传类型：" + extensions);
            } else {
                var domainUrl = "";
                switch (top.$.uploadFlag) {
                    case "1"://自定义服务器
                        domainUrl = top.$.uploadUrl;
                        break;
                    default://本地服务器
                        domainUrl = top.$.rootUrl;
                        break;
                }

                var param = {};
                var res = {};
                param['Value'] = (UserID + OperationCode);
                param['__RequestVerificationToken'] = $.lrToken;
                learun.httpPost(top.$.rootUrl + '/Other/GetMD5Token', param, function (resdata) {
                    res = resdata;
                });
                var serverUrl = domainUrl + "/apiMD5?parameters=Function=AccOperationList|UserID=" + UserID + "|OperationCode=" + OperationCode + "|MD5=" + res.data.MD5;
                res = learun.httpGet(serverUrl);
                if (res.result == "1") {
                    $.each(res.list, function (index, item) {
                        $('.lr-form-layout-header').find("span.extensions").text("上传类型：" + item.FileType);
                        extensions = item.FileType;
                    });
                }
            }
            //先后台进行MD5签名（后台调用原因：隐藏加密字符）
            param = {};
            param['Value'] = (UserID + OperationCode);
            param['__RequestVerificationToken'] = $.lrToken;
            res = {};
            learun.httpPost(top.$.rootUrl + '/Other/GetMD5Token', param, function (resdata) {
                res = resdata;
            });
            //加密后的请求上传地址
            var serverUrl = domainUrl + "/apiMD5?parameters=Function=UploadFile|UserID=" + UserID + "|OperationCode=" + OperationCode + "|MD5=" + res.data.MD5;
            chunked = false;
            page.uploader = WebUploader.create({
                auto: true,
                swf: top.$.rootUrl + '/Content/webuploader/Uploader.swf',
                // 文件接收服务端。
                server: serverUrl,
                // 选择文件的按钮。可选。
                // 内部根据当前运行是创建，可能是input元素，也可能是flash.
                pick: '#lr_add_file_btn',
                dnd: '#lr_form_file_queue',
                paste: 'document.body',
                disableGlobalDnd: true,
                accept: {
                    extensions: extensions || "gif,jpeg,jpg,png,psd,rar,zip,pdf,doc,docx,ppt,pptx,txt,xls,xlsx"
                },
                multiple: multiple,
                //fileNumLimit: fileNumLimit,
                // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
                resize: false,
                // 文件分片上传
                chunked: chunked,
                chunkRetry: 3,
                prepareNextFile: true,
                chunkSize: '1048576',
                // 上传参数
                formData: {
                    __RequestVerificationToken: $.lrToken,
                    OperationCode: OperationCode,
                    OperationID: OperationID
                }
            });
            page.uploader.on('fileQueued', page.fileQueued);
            page.uploader.on('uploadStart', page.uploadStart);
            page.uploader.on('uploadBeforeSend', page.uploadBeforeSend);
            page.uploader.on('uploadProgress', page.uploadProgress);
            page.uploader.on('uploadSuccess', page.uploadSuccess);
            page.uploader.on('uploadError', page.uploadError);
            page.uploader.on('uploadComplete', page.uploadComplete);
            page.uploader.on('error', page.error);


            $('#lr_form_file_queue').lrscroll();

        },
        fileQueued: function (file) {// 文件加载到队列
            if (fileNumLimit > 0 && $('#lr_form_file_queue_list .lr-form-file-queue-item').length >= fileNumLimit) {
                learun.alert.error('超出上传数量限制');
                return false;
            }
            if (!multiple && $('#lr_form_file_queue_list .lr-form-file-queue-item').length >= 1) {
                learun.alert.error('超出上传数量限制');
                return false;
            } else {
                fileInfo[file.id] = { name: file.name };
                $('#lr_form_file_queue .lr-form-file-queue-bg').hide();
                // 添加一条文件记录
                var $item = $('<div class="lr-form-file-queue-item" id="lr_filequeue_' + file.id + '" ></div>');
                $item.append('<div class="lr-file-image"><img src="' + top.$.rootUrl + '/Content/images/filetype/' + file.ext + '.png"></div>');
                $item.append('<span class="lr-file-name">' + file.name + '(' + learun.countFileSize(file.size) + ')</span>');

                $('#lr_form_file_queue_list').append($item);
            }
        },
        uploadStart: function (file) {
            var $fileItem = $('#lr_form_file_queue_list').find('#lr_filequeue_' + file.id);
            $fileItem.append('<div class="lr-uploader-progress"><div class="lr-uploader-progress-bar" style="width:0%;"></div></div>');
        },
        uploadBeforeSend: function (object, data, headers) {
            if (typeof (fileInfo[data.id]) != "undefined") {
                data.chunk = data.chunk || 0;
                data.chunks = data.chunks || 1;
                fileInfo[data.id].fileGuid = fileInfo[data.id].fileGuid || WebUploader.Base.guid();
                data.fileGuid = fileInfo[data.id].fileGuid;
                fileInfo[data.id].chunks = data.chunks;
            }
        },
        uploadProgress: function (file, percentage) {
            var $fileItem = $('#lr_form_file_queue_list').find('#lr_filequeue_' + file.id);
            $fileItem.find('.lr-uploader-progress-bar').css('width', (percentage * 100 + '%'));
        },
        uploadSuccess: function (file, res) {
            if (res.code == 200 || res.result == "1") {// 上传成功
                mergeFileChunks(file, res);
            }
            else {// 上传失败
                reomveFileChunks(file);
            }
        },
        uploadError: function (file, code) {
            reomveFileChunks(file);
        },
        uploadComplete: function (file) {
        },
        error: function (type) {
            switch (type) {
                case 'Q_TYPE_DENIED':
                    learun.alert.error('不支持的文件类型');
                    break;
            };
        }
    };
    // 保存数据
    acceptClick = function (callBack, $self) {
        if (!!callBack) {
            callBack(Files, $self);
        }
        return true;
    };
    page.init();
}
