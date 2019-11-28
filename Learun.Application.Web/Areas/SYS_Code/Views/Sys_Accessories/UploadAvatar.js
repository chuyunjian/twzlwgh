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
var aspectRatioWidth = request('aspectRatioWidth');
var aspectRatioHeight = request('aspectRatioHeight');
//Form页对应的窗口，方便传值
var form = null;
var Form = function (_form) {
    form = _form;
};
(function (factory) {
    if (typeof define === 'function' && define.amd) {
        define(['jquery'], factory);
    } else if (typeof exports === 'object') {
        // Node / CommonJS
        factory(require('jquery'));
    } else {
        factory(jQuery);
    }
})
var bootstrap = function ($, learun) {
    "use strict";

    setTimeout(function () {
        //获取Form页数据
        var Files = form.sbdUploaderGetFiles();
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
            $item.append('<div class="lr-tool-bar"><i class="fa fa-cloud-download" title="下载"  data-value="' + item.AttachmentID + '" ></i><i class="fa fa-minus-circle" title="删除"  data-value="' + item.AttachmentID + '" ></i></div>');

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
        //设置Form页数据
        form.sbdUploaderDelete(attachmentId);
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
        $container: null,
        $avatarView: null,
        $avatarForm: null,
        $avatarSrc: null,
        $avatarData: null,
        $avatarInput: null,
        $avatarWrapper: null,
        $avatarPreview: null,
        $avatarBtns: null,
        $saveBtn: null,
        $fileName: '',
        support: {
            fileList: !!$('<input type="file">').prop('files'),
            blobURLs: !!window.URL && URL.createObjectURL,
            formData: !!window.FormData
        },
        init: function () {
            aspectRatioWidth = aspectRatioWidth ? aspectRatioWidth : 96;
            aspectRatioHeight = aspectRatioHeight ? aspectRatioHeight : 130;
            this.$container = $('div.avatar-layout');
            this.$avatarView = this.$container.find('.avatar-view');
            this.$avatarForm = this.$container.find('.avatar-form');
            this.$avatarSrc = this.$avatarForm.find('.avatar-src');
            this.$avatarData = this.$avatarForm.find('.avatar-data');
            this.$avatarInput = this.$avatarForm.find('.avatar-input');
            this.$avatarWrapper = this.$container.find('.avatar-wrapper');
            this.$avatarPreview = this.$container.find('.avatar-preview');
            this.$avatarBtns = this.$container.find('.avatar-btns');
            this.$saveBtn = this.$avatarBtns.find('.avatar-save');

            this.support.datauri = this.support.fileList && this.support.blobURLs;

            if (!this.support.formData) {
                this.initIframe();
            }
            this.initPage();
            this.initTooltip();
            this.addListener();
            $('#lr_form_file_queue').lrscroll();
        },
        addListener: function () {
            //this.$avatarView.on('click', $.proxy(this.click, this));
            this.$avatarInput.on('change', $.proxy(this.change, this));
            //this.$avatarForm.on('submit', $.proxy(this.submit, this));
            this.$avatarBtns.on('click', $.proxy(this.rotate, this));
            this.$saveBtn.on('click', $.proxy(this.submit, this));
        },
        initPage: function () {
            this.$avatarPreview.css({ 'width': aspectRatioWidth + 'px', 'height': aspectRatioHeight + 'px' });
            var parentWidth = parseInt(this.$avatarPreview.parent().css('width'));
            var previewWidth = parseInt(this.$avatarPreview.css('width'));
            this.$avatarPreview.css('margin-left', ((parentWidth - previewWidth - 15) / 2) + 'px');
        },
        initTooltip: function () {
            //this.$avatarView.tooltip({
            //    placement: 'bottom'
            //});
        },
        change: function () {
            var files,
                file;

            if (this.support.datauri) {
                files = this.$avatarInput.prop('files');

                if (files.length > 0) {
                    file = files[0];

                    if (this.isImageFile(file)) {
                        if (this.url) {
                            URL.revokeObjectURL(this.url); // Revoke the old one
                        }
                        var texts = document.querySelector("#avatarInput").value;
                        var teststr = texts;
                        this.$fileName = teststr.match(/[^\\]+\.[^\(]+/i); //直接完整文件名的
                        $('#avatar-name').html(this.$fileName);
                        this.url = URL.createObjectURL(file);
                        this.startCropper();
                    }
                }
            } else {
                file = this.$avatarInput.val();
            }
        },
        rotate: function (e) {
            var data;

            if (this.active) {
                data = $(e.target).data();

                if (data.method) {
                    this.$img.cropper(data.method, data.option);
                }
            }
        },
        isImageFile: function (file) {
            if (file.type) {
                return /^image\/\w+$/.test(file.type);
            } else {
                return /\.(jpg|jpeg|png|gif)$/.test(file);
            }
        },
        startCropper: function () {
            var _this = this;
            if (this.active) {
                this.$img.cropper('replace', this.url);
            } else {
                this.$img = $('<img src="' + this.url + '">');
                this.$avatarWrapper.empty().html(this.$img);
                this.$img.cropper({
                    aspectRatio: ((parseInt(aspectRatioWidth) + 25) / 10) / ((parseInt(aspectRatioHeight) + 28) / 10),
                    preview: this.$avatarPreview.selector,
                    strict: false,
                    autoCropArea: 1,
                });

                this.active = true;
            }
        },
        stopCropper: function () {
            if (this.active) {
                this.$img.cropper('destroy');
                this.$img.remove();
                this.active = false;
            }
        },
        //将base64转换为文件
        dataURLtoFile: function (dataurl, filename) {
            var arr = dataurl.split(','), mime = arr[0].match(/:(.*?);/)[1],
                bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
            while (n--) {
                u8arr[n] = bstr.charCodeAt(n);
            }
            return new File([u8arr], filename, { type: mime });
        },
        submit: function () {
            if ($('#avatar-name').text().length == 0) {
                alert("请先选择图片");
                return false;
            }
            this.$saveBtn.text("请稍后...");
            this.$avatarBtns.find('button').prop('disabled', true);
            this.$container.find('#lr_add_file_btn').prop('disabled', true);
            var img_lg = document.getElementById('imageHead');
            // 截图小的显示框内的内容
            html2canvas(img_lg, {
                allowTaint: true,
                taintTest: false,
                onrendered: function (canvas) {
                    canvas.id = "mycanvas";
                    //生成base64图片数据
                    var dataUrl = canvas.toDataURL("image/jpeg");
                    var newImg = document.createElement("img");
                    newImg.src = dataUrl;
                    //上传
                    var domainUrl = "";//上传地址
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
                    //先后台进行MD5签名（后台调用原因：隐藏加密字符）
                    var param = {};
                    param['Value'] = (UserID + OperationCode);
                    param['__RequestVerificationToken'] = $.lrToken;
                    var res = {};
                    learun.httpPost(top.$.rootUrl + '/Other/GetMD5Token', param, function (resdata) {
                        res = resdata;
                    });
                    //加密后的请求上传地址
                    var serverUrl = domainUrl + "/apiMD5?parameters=Function=UploadFile|UserID=" + UserID + "|OperationCode=" + OperationCode + "|MD5=" + res.data.MD5;
                    var formData = new FormData();
                    formData.append("file", page.dataURLtoFile(dataUrl, $('#avatar-name').text()));
                    formData.append('UserID', UserID);
                    formData.append('OperationCode', OperationCode);
                    formData.append('MD5', res.data.MD5);
                    $.ajax({
                        url: serverUrl,
                        data: formData,
                        async: false,
                        cache: false,
                        contentType: false,
                        processData: false,
                        type: "POST",
                        dataType: 'json',
                        success: function (res) {
                            if (res.result != "1") {
                                page.$saveBtn.text("");
                                page.$avatarBtns.find('button').prop('disabled', false);
                                page.$container.find('#lr_add_file_btn').prop('disabled', false);
                                alert(res.message);
                                return false;
                            } else {
                                page.$saveBtn.text("");
                                page.$avatarBtns.find('button').prop('disabled', false);
                                page.$container.find('#lr_add_file_btn').prop('disabled', false);
                                //设置Form页数据
                                form.sbdUploaderSet(OperationID, res);
                            }
                        }
                    });
                    form.sbdUploaderAvatarSet(dataUrl);
                    learun.layerClose(window.name);
                }
            });
        }
    };
    page.init();
}
