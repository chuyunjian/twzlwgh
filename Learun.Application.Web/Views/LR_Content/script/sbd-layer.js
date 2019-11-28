/*
 * 版 本 Learun-ADMS V6.1.6.0 力软敏捷开发框架(http://www.learun.cn)
 * Copyright (c) 2013-2017 上海力软信息技术有限机构
 * 创建人：力软-前端开发组
 * 日 期：2018-07-05
 * 描 述：弹层（基于layer.js-3.0.3）	
 */
(function ($, learun) {
    "use strict";
    $.extend(learun, {
        // 询问框
        layerConfirm: function (_msg, callback) {
            top.learun.language.get(_msg, function (msg) {
                top.layer.confirm(msg, {
                    btn: ['确认', '取消'],
                    title: "提示信息",
                    icon: 0,
                    skin: 'lr-layer',
                    success: function (layero, index) { 
                        layero.find('.layui-layer-btn a').each(function () {
                            var $this = $(this);
                            var _text = $this.text();
                            top.learun.language.get(_text, function (text) {
                                $this.text(text);
                            });

                        });
                        layero.find('.layui-layer-title').each(function () {
                            var $this = $(this);
                            var _text = $this.text();
                            top.learun.language.get(_text, function (text) {
                                $this.text(text);
                            });

                        });
                        document.onkeydown = function (e) { // 回车提交表单
                            // 兼容FF和IE和Opera
                            var theEvent = window.event || e;
                            var code = theEvent.keyCode || theEvent.which || theEvent.charCode;
                            if (code == 13) {
                                callback(true, index);
                            }
                        }
                    },
                }, function (index) {
                    callback(true, index);
                }, function (index) {
                    callback(false, index);
                    top.layer.close(index); //再执行关闭  
                });
            });


        },
        // 自定义表单弹层
        layerForm: function (op) {
            var dfop = {
                id: null,
                title: '系统窗口',
                width: 550,
                height: 400,
                url: 'error',
                btn: ['确认', '关闭'],
                sureClose: true,//是否有确定关闭按钮
                callBack: false,
                maxmin: false,
                end: false,
            };
            $.extend(dfop, op || {});

            /*适应窗口大小*/
            dfop.width = dfop.width > $(top.window).width() ? $(top.window).width() - 10 : dfop.width;
            dfop.height = dfop.height > $(top.window).height() ? $(top.window).height() - 10 : dfop.height;

            var r = top.layer.open({
                id: dfop.id,
                maxmin: dfop.maxmin,
                type: 2,//0（信息框，默认）1（页面层）2（iframe层）3（加载层）4（tips层）
                title: dfop.title,
                area: [dfop.width + 'px', dfop.height + 'px'],
                btn: dfop.btn,
                content: op.url,
                skin: dfop.btn == null ? 'lr-layer-nobtn' : 'lr-layer',
                success: function (layero, index) {
                    top['layer_' + dfop.id] = learun.iframe($(layero).find('iframe').attr('id'), top.frames);
                    layero[0].learun_layerid = 'layer_' + dfop.id;
                    //如果底部有按钮，并且需要确认关闭 -》 添加-确认并关闭窗口勾选按钮
                    if (!!dfop.btn && layero.find('.lr-layer-btn-cb').length == 0 && dfop.sureClose) {
                        top.learun.language.get('确认并关闭窗口', function (text) {
                            layero.find('.layui-layer-btn').append('<div class="checkbox lr-layer-btn-cb" myIframeId="layer_' + dfop.id + '" ><label><input checked="checked" type="checkbox" >' + text + '</label></div>');
                        });
                        layero.find('.layui-layer-btn a').each(function () {
                            var $this = $(this);
                            var _text = $this.text();
                            top.learun.language.get(_text, function (text) {
                                $this.text(text);
                            });

                        });
                    }
                    layero.find('.layui-layer-title').each(function () {
                        var $this = $(this);
                        var _text = $this.text();
                        top.learun.language.get(_text, function (text) {
                            $this.text(text);
                        });

                    });
                    //以下进行了重新
                    try {
                        dfop.success(layero, index);
                    } catch (e) {

                    }
                },
                yes: function (index) {
                    var flag = true;
                    if (!!dfop.callBack) {
                        flag = dfop.callBack('layer_' + dfop.id);
                    }
                    if (!!flag) {
                        learun.layerClose('', index);
                    }
                },
                end: function () {
                    top['layer_' + dfop.id] = null;
                    if (!!dfop.end) {
                        dfop.end();
                    }
                }
            });
        },
        layerIndex: function (op) {
            var dfop = {
                id: null,
                title: '系统窗口',
                width: 550,
                height: 400,
                url: 'error',
                btn: ['关闭'],
                callBack: false,
                maxmin: false,
                end: false,
            };
            $.extend(dfop, op || {});

            /*适应窗口大小*/
            dfop.width = dfop.width > $(top.window).width() ? $(top.window).width() - 10 : dfop.width;
            dfop.height = dfop.height > $(top.window).height() ? $(top.window).height() - 10 : dfop.height;
          
            var r = top.layer.open({
                id: dfop.id,
                maxmin: dfop.maxmin,
                type: 2,//0（信息框，默认）1（页面层）2（iframe层）3（加载层）4（tips层）
                title: dfop.title,
                area: [dfop.width + 'px', dfop.height + 'px'],
                btn: dfop.btn,
                content: op.url,
                skin: dfop.btn == null ? 'lr-layer-nobtn' : 'lr-layer',
                success: function (layero, index) {
                    top['layer_' + dfop.id] = learun.iframe($(layero).find('iframe').attr('id'), top.frames);
                    layero[0].learun_layerid = 'layer_' + dfop.id;
                    //以下进行了重新
                    try {
                        dfop.success(layero, index);
                    } catch (e) {

                    }
                },
                end: function () {
                    top['layer_' + dfop.id] = null;
                    if (!!dfop.end) {
                        dfop.end();
                    }
                }
            });
        },
        layerTwo: function (op) {
            var dfop = {
                id: null,
                title: '系统窗口',
                width: 550,
                height: 400,
                url: 'error',
                btn: ['关闭'],
                callBack: false,
                maxmin: false,
                end: false,
            };
            $.extend(dfop, op || {});

            /*适应窗口大小*/
            dfop.width = dfop.width > $(top.window).width() ? $(top.window).width() - 10 : dfop.width;
            dfop.height = dfop.height > $(top.window).height() ? $(top.window).height() - 10 : dfop.height;

            var r = top.layer.open({
                id: dfop.id,
                maxmin: dfop.maxmin,
                type: 2,//0（信息框，默认）1（页面层）2（iframe层）3（加载层）4（tips层）
                title: dfop.title,
                area: [dfop.width + 'px', dfop.height + 'px'],
                btn: dfop.btn,
                content: op.url,
                skin: dfop.btn == null ? 'lr-layer-nobtn' : 'lr-layer',
                success: function (layero, index) {
                    top['layer_' + dfop.id] = learun.iframe($(layero).find('iframe').attr('id'), top.frames);
                    layero[0].learun_layerid = 'layer_' + dfop.id;
                    //以下进行了重新
                    try {
                        dfop.success(layero, index);
                    } catch (e) {

                    }
                },
                yes: function (index) {
                    var flag = true;
                    if (!!dfop.callBack) {
                        flag = dfop.callBack({ name: 'layer_' + dfop.id, id: 1 });
                    }

                    if (!!flag) {
                        learun.layerClose('', index);
                    }
                },
                btn2: function () {
                    var flag = true;
                    if (!!dfop.callBack) {
                        flag = dfop.callBack({ name: 'layer_' + dfop.id, id: 2 });
                    }

                    if (!!flag) {
                        learun.layerClose('', index);
                    }
                    return false;
                },
                btn3: function () {
                    top['layer_' + dfop.id] = null;
                    if (!!dfop.end) {
                        dfop.end();
                    }
                }
            });
        },
        // 关闭弹层
        layerClose: function (name, index) {
            var _index;
            if (!!index) {
                _index = index;
            }
            else {
                _index = top.layer.getFrameIndex(name);
            }
            var layero = top.$("#layui-layer" + _index);
            var $IsClose = layero.find('.layui-layer-btn').find(".lr-layer-btn-cb input");
            var IsClose = $IsClose.is(":checked");
            if ($IsClose.length == 0) {
                IsClose = true;
            }
            if (IsClose) {
                top.layer.close(_index); //再执行关闭  
            } else {
                top[layero[0].learun_layerid].location.reload();
            }

        },
        // 关闭所有弹层
        layerCloseAll: function () {
            top.layer.closeAll(); //再执行关闭  
        } 
    });


})(window.jQuery, top.learun);