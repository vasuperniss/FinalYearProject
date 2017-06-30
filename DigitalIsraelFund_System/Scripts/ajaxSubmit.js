function ajaxSbmt(data, url, onSuccess, onFail) {
    $.ajax({
        url: url,
        type: "POST",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json",
        success: function (response) {
            if (response.Success) {
                onSuccess(response);
            }
            else {
                onFail(response.ErrMsg);
            }
        },
        error: function () {
        }
    });
}