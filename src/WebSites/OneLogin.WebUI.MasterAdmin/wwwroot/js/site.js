//axios
// 请求拦截器
axios.interceptors.request.use(function (config) {
    // 放置业务逻辑代码
    if (config.url.toLowerCase().indexOf('menuid=') < 0) {
        if (config.url.indexOf('?') >= 0) {
            config.url += '&menuId=' + Request.QueryString('menuId');
        } else {
            config.url += '?menuId=' + Request.QueryString('menuId');
        }
    }
    return config;
}, function (error) {
    // axios发生错误的处理
    return window.Promise.reject(error);
});
// 响应拦截器
axios.interceptors.response.use(function (response) {
    // 放置业务逻辑代码
    // response是服务器端返回来的数据信息，与Promise获得数据一致
    //var result = response.data;
    //if (result.Code === "403") {
    //    msgFail('无权操作');
    //}
    return response;
}, function (error) {
    // axios请求服务器端发生错误的处理
    if (error.response.status === 403) {
        msgFail('无权操作');
    }
    return window.Promise.reject(error);
});

//date
function formatDate(time, fmt="yyyy-MM-dd") {
    var dateTime = new Date(time);
    var o = {
        "M+": dateTime.getMonth() + 1, //月份
        "d+": dateTime.getDate(), //日
        "h+": dateTime.getHours(), //小时
        "m+": dateTime.getMinutes(), //分
        "s+": dateTime.getSeconds(), //秒
        "q+": Math.floor((dateTime.getMonth() + 3) / 3), //季度
        "S": dateTime.getMilliseconds() //毫秒
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (dateTime.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) {
             fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        }
    return fmt;
}

function formatMoney(money, symbol = '￥') {
    if (!money) return '<span style="color:green">￥0.00</span>';
    symbol = symbol || '￥';
    if (money == typeof(string)) {
        money = parseFloat(money);
    }
    var str = money.toFixed(2);  // 只取2位小数
    var l = str.split('.')[0]; // 获取整数位
    var r = str.split('.')[1]; // 获取小数位
    var arr = [] // 用于保存结果

    var len = Math.ceil(l.length / 3); // 3位数一个 `,`
    for (let i = 0; i < len; i++) {
        arr.unshift(l.slice(-3 * (i + 1), -3 * i || undefined)); // 如果传(-3,0)获取不到参数，将0换成undefined相当于没传
        if (i !== len - 1) { // 最后一次截取不加 `,`了
            arr.unshift(',');
        }
    }
    if (money > 0) {
        return '<span style="color:green">' + symbol + arr.join('') + '.' + r + '</span>';
    } else if (money < 0) {
        return '<span style="color:red">' + symbol + arr.join('') + '.' + r + '</span>';
    }
    return '<span style="color:gray">' + symbol + arr.join('') + '.' + r + '</span>';
}

//axios的配置
axios.default.header = { 'Content-type': 'application/json; charset=utf-8' }

function closeMe() {
    var index = parent.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
    parent.layer.close(index); //再执行关闭
}

function parentTableSearch(btnId = "btn_search") {
    parent.layui.$('#' + btnId).click();
}

function parentTableReload() {
    if (parent.window.layui.table != undefined) {
        parent.window.layui.table.reload();
    }
}

Request = {
    QueryString: function (item) {
        var val = location.search.match(new RegExp("[\?\&]" + item + "=([^\&]*)(\&?)", "i"));
        return val ? val[1] : val;
    }
}

//扩展方法
toInt = function (value) {
    //debugger;
    if (value == null || !value) return 0;
    try {
        return parseInt(value);
    } catch (e) {
        return 0;
    }
}

msg = function (result, okCallFun, time = 1000) {
    if (result.IsError) {
        if (result.reloadCode === "403") {
            msgFail("无权操作", null, time);
        } else {
            msgFail(result.Message, null, time);
        }
    } else {
        msgOk(result.Message, okCallFun, time);
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

showLoader = function () {
    return layer.load({ shade: 0.2 });
}

closeLoader = function () {
    layer.closeAll();
}

treeTableRender = function (options) {
    options = processOptions(options);
    treeTable.render(options);
}

tableRender = function (options) {
    options = processOptions(options);
    table.render(options);
}

function processOptions(options) {
    if (options.url.toLowerCase().indexOf('menuid=') < 0) {
        if (options.url.indexOf('?') >= 0) {
            options.url += '&menuId=' + Request.QueryString('menuId');
        } else {
            options.url += '?menuId=' + Request.QueryString('menuId');
        }
    }
    return options;
}

//menuId
layerOpen = function (options) {
    if (options.content[0].toLowerCase().indexOf('menuid=') < 0) {
        if (options.content[0].indexOf('?') >= 0) {
            options.content[0] += '&menuId=' + Request.QueryString('menuId');
        } else {
            options.content[0] += '?menuId=' + Request.QueryString('menuId');
        }
    }
    layer.open(options);
}

//格式化显示
function formatShowBool(isTrue, trueText, falseText) {
    if (isTrue) {
        return '<span class="layui-badge layui-bg-blue">' + trueText + '</span>';
    } else {
        return '<span class="layui-badge layui-bg-gray">' + falseText + '</span>';
    }
}

layuiSelectRender = function (form, selectJQueryObj, emptyName, items, selectedItemId) {
    selectJQueryObj.empty();
    if (emptyName) selectJQueryObj.append('<option value="">' + emptyName + '</option>');
    $(items).each(function (index, item) {
        var selectOpt = "";
        if (selectedItemId == item.Value) {
            selectOpt = 'selected="selected"';
        }
        var option = '<option value="' + item.Value + '" ' + selectOpt + '>' + item.Text + '</option>';
        selectJQueryObj.append(option);
    });
    form.render(selectJQueryObj);
}

