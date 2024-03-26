//常州项目保留的附件上传脚本
//单据上传页面需要引入
//异步上传文件的方法
function ajaxFileUpload(url, code, success) {
    $.ajaxFileUpload({
        url: url, //用于文件上传的服务器端请求地址
        secureuri: false, //一般设置为false
        fileElementId: 'fileUpload_' + code, //文件上传控件的id属性  
        dataType: 'json', //返回值类型 一般设置为json
        success: success,
        error: function (data, status, e)//服务器响应失败处理函数
        {
            alert(status);
        }
    });
    return false;
}

//刷新关联单据的数据
function refleshRelatedBill(exeCountSql){
    $("#containerRelatedBill").pager("reflesh", exeCountSql);
}

//刷新所需附件数据
function refleshAttachNeed(exeCountSql) {
    $("#containerAttachNeed").pager("reflesh", exeCountSql);
}

//初始化分页
$(function () {
    $("#containerAttach").pager();
    $("#containerRelatedBill").pager();
    $("#containerAttachNeed").pager();
});

//上传附件，上传成功后，刷新附件列表
$(document).ready(function () {
    //查看附件列表
    $(".showData").on("click", ".showAttach", function () {
        var url = $(this).attr("href");
        OpenSelect("附件列表", url);
        return false;
    });

    //上传附件
    $(".showData").on("click", ".uploadAttach", function () {
        var resourceCode = $(this).attr("data-resourceCode");
        var prjSysCode = $(this).attr("data-prjsyscode");
        if (isNull(prjSysCode)) {
            showError("必须选择项目后才允许上传附件");
            return false;
        }

        var keyCode = $(this).attr("data-keyCode");
        var attachTypeCode = $(this).attr("data-attachtypecode");
        console.info(attachTypeCode); 
        if ($("#fileUpload_" + attachTypeCode).val().length > 0) {
            var url = "/admin/sys_BillAttach/fileUpload?resourceCode=" + resourceCode + "&prjSysCode=" + prjSysCode + "&keyCode=" + keyCode + "&attachTypeCode=" + attachTypeCode;
            ajaxFileUpload(url, attachTypeCode, function (data, status) {
                showJsonResult(data, function () {
                    //刷新附件列表
                    //refleshAttachData(true);
                    refleshAttachNeed(false);
                });
            });
        } else {
            alert("请选择文件");
        }
    });

    //删除附件
    $(".showData").on("click",".fileDelete", function () {
        if (confirm("确认删除")) {
            var rowID = $(this).attr("data-rowID");
            ajaxPost("/admin/sys_BillAttach/fileDelete", { rowID: rowID }, function (data) {
                showJsonResult(data, function () {
                    //刷新本页面
                    refleshData(true);
                    //刷新父页面
                    window.parent.refleshAttachNeed(false);
                });
            });
        }
        return false;
    });
});