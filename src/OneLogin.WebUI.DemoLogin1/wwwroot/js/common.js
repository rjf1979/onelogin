Request = {
    QueryString: function (item) {
        var val = location.search.match(new RegExp("[\?\&]" + item + "=([^\&]*)(\&?)", "i"));
        return val ? val[1] : val;
    }
}

msg = function (result, okCallFun, time = 1000) {
    if (result.isError) {
        if (result.reloadCode === "403") {
            msgFail("无权操作", null, time);
        } else {
            msgFail(result.message, null, time);
        }
    } else {
        msgOk(result.message, okCallFun, time);
    }
}

msgOk = function (msg, callFun, time = 1000) {
    layer.closeAll();
    if (time <= 0) {
        callFun();
    } else {
        layer.msg(msg, { icon: 1, time: time, shade: 0.2 }, callFun);
    }
}

msgFail = function (msg, callFun, time = 1000) {
    layer.closeAll();
    if (time <= 0) {
        callFun();
    } else {
        layer.msg(msg, { icon: 5, time: time, shade: 0.2 }, callFun);
    }
}