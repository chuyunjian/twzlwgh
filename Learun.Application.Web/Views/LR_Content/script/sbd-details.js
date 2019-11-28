/*
 * Copyright (c) 思必达
 * 创建人：严笛
 * 日 期：2018.10.24
 * 描 述：详情处理方法
 */
(function ($, learun) {
    "use strict";

    $.fn.sbdSetDetails = function (data) {// 设置页面数据
        for (var id in data) {
            var value = data[id];
            var $obj = $(this).find('#' + id);
            if ($obj.length == 0 && value != null) {
                $obj = $('[name="' + id + '"][value="' + value + '"]');
                if ($obj.length > 0) {
                    if (!$obj.is(":checked")) {
                        $obj.trigger('click');
                    }
                }
            }
            else {
                value = value == null ? "" : value;
                var type = $obj.attr('type');
                if ($obj.hasClass("lr-input-wdatepicker")) {
                    type = "datepicker";
                }
                switch (type) {
                    case "checkbox":
                        var isck = 0;
                        if ($obj.is(":checked")) {
                            isck = 1;
                        } else {
                            isck = 0;
                        }
                        if (isck != parseInt(value)) {
                            $obj.trigger('click');
                        }
                        break;
                    case "lrselect":
                        $obj.lrselectSet(value);
                        break;
                    case "formselect":
                        $obj.lrformselectSet(value);
                        break;
                    case "lrGirdSelect":
                        $obj.lrGirdSelectSet(value);
                        break;
                    case "text":
                        if ($obj.hasClass("form-control")) {

                            $obj.val(value);
                        } else {
                            $obj.text(value);
                        }
                        break;
                    case "html":
                        if (typeof $obj.attr("format") != "undefined") {
                            $obj.html(learun.formatDate(value, $obj.attr("format")));
                        }
                        else {
                            $obj.html(value);
                        }
                        break;
                    case "datepicker":
                        $obj.val(learun.formatDate(value, 'yyyy-MM-dd'));
                        break;
                    case "lr-Uploader":
                        $obj.lrUploaderSet(value);
                        break;
                    case "lr-radio":
                        if (!$obj.find('input[value="' + value + '"]').is(":checked")) {
                            $obj.find('input[value="' + value + '"]').trigger('click');
                        }
                        break;
                    case "lr-checkbox":
                        var valueArray = value.split(',');
                        $.each(valueArray, function (index) {
                            if (!$obj.find('input:checkbox[value="' + this + '"][name="' + id + '"]').is(":checked")) {
                                $obj.find('input:checkbox[value="' + this + '"][name="' + id + '"]').trigger('click');
                            }
                        })
                        break;
                    default:
                        try {
                            var reg = /^[0-9]{4}-[0-1]?[0-9]{1}-[0-3]?[0-9]{1}$/;
                            if (reg.test(learun.formatDate(value, 'yyyy-MM-dd'))) {
                                //日期格式参考lr-date.js中定义
                                var format = $obj.attr("data-date-format");
                                if (typeof (format) == "undefined") {
                                    format = "yyyy-MM-dd";
                                }
                                value = learun.formatDate(value, format);
                            }
                        } catch (e) {

                        }
                        if ($obj.prop("tagName") == "DIV" || $obj.prop("tagName") == "SPAN" || $obj.prop("tagName") == "P") {
                            $obj.text(value);
                        } else if ($obj.prop("tagName") == "INPUT" || $obj.prop("tagName") == "SELECT") {
                            $obj.val(value);
                        }
                        break;
                }
            }
        }
    };
})(jQuery, top.learun);