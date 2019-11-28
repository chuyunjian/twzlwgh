/*
 * 创建人：严笛
 * 日 期：2019-05-16
 * 描 述：司机选择插件
 */
(function ($, learun) {
    "use strict";

    $.driverselect = {
        init: function ($self) {
            var dfop = $self[0].dfop;
            $self.addClass('sbd-driverselect');
            $self.attr('type', 'driverselect');
            var $input = $('<div class="input-group">'
                + '             <select class="form-control CDID"><option value="" stype="default">' + dfop.placeholder + '</option></select>'
                + '             <span class="input-group-btn">'
                + '                 <button class="btn btn-default" type="button">选择司机</button>'
                + '             </span>'
                + '         </div>');
            $self.html($input);
            //$self.find("button.btn").on('click', $.driverselect.click($self));
            $.driverselect.bindData($self);
            $.driverselect.bindClick($self);
        },
        bindData: function ($self) {
            var dfop = $self[0].dfop;
            var url = top.$.rootUrl + '/CL_Code/Car_DriverCondition/GetUsableList';
            learun.loading(true, '正在获取司机数据');
            var queryJson = {
                queryJson: JSON.stringify(dfop.param || {})
            };
            var data = learun.httpGet(url, queryJson)
            learun.loading(false);
            if (data.code == learun.httpCode.success) {
                $self.find("select.CDID").filter("option[stype!='default'],option[stype!='click']").remove();
                $.each(data.data, function (i, t) {
                    $self.find("select.CDID").append('<option value="' + t.CDID + '">' + t.CDName + '</option>');
                });
                $self.find("select.CDID").off('change');
                $self.find("select.CDID").on('change', $.driverselect.change);
            } else {
                learun.alert.error(data.info);
            }
        },
        change: function () {
            var $self = $(this);
            var dfop = $self.parents(".sbd-driverselect")[0].dfop;
            var CDID = $self.parents(".sbd-driverselect").find("select.CDID").val();
            var CDName = $self.parents(".sbd-driverselect").find("select.CDID").find("option:selected").text();
            if (CDID.length == 0) { CDName = ""; }
            $self.parents(".sbd-driverselect").attr("CDID", CDID).attr("CDName", CDName);
            if (!!dfop.select) {
                dfop.select({ CDID: CDID, CDName: CDName });
            }
        },
        bindClick: function ($self) {
            var dfop = $self[0].dfop;
            $($self).find("button.btn").on("click", function () {
                learun.layerForm({
                    id: "driver",
                    title: "司机",
                    url: top.$.rootUrl + '/CL_Code/Car_DriverCondition/SelectForm?stype=1&selectValue=' + $self.find("select.CDID").val() + '&CSID=' + dfop.param.CSID + '&BTime=' + dfop.param.BTime + '&ETime=' + dfop.param.ETime,
                    width: 800,
                    height: 520,
                    maxmin: false,
                    callBack: function (id) {
                        return top[id].acceptClick($.driverselect.callback, $self);
                    }
                });
            });
        },
        callback: function (postitem, dfopid, $self) {
            if ($self.find("select.CDID").find("option[value='" + postitem.value + "']").length == 0) {
                var item = $('<option value="' + postitem.value + '">' + postitem.text + '</option>');
                $self.find("select.CDID").append(item);
            }
            $self.find("select.CDID").find("option[value='" + postitem.value + "']").prop("selected", true).trigger("change");
        }
    };
    $.fn.driverselect = function (op) {
        var dfop = {
            param: {},
            placeholder: "请选择",
            select: false,  // 选择事件
        };

        $.extend(dfop, op || {});
        $.extend(dfop.param, dfop.param || {});
        var $self = $(this);
        dfop.id = $self.attr('id');
        if (!dfop.id) {
            return false;
        }
        if (!!$self[0].dfop) {
            return $self;
        }

        $self[0].dfop = dfop

        $.driverselect.init($self);
        return $self;
    };
    $.fn.driverselectGet = function () {
        var $self = $(this);
        return $self.attr("CDID");
    };
    $.fn.driverselectSet = function (value) {
        var $self = $(this);
        var op = $self[0].dfop;
        if (value != undefined && value != null) {
            $self.find("select.CDID").find("option[value='" + value + "']").prop("selected", true).trigger("change");
        }
    };
})(window.jQuery, top.learun);