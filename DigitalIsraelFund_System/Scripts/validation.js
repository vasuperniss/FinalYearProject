function validateText(id, type) {
    _validate(document.getElementById(id).innerText, type, id);
}

function validateInput(id, type) {
    _validate(document.getElementById(id).value, type, id);
}

function _validate(value, type, id) {
    var data = {
        "value": value,
        "type": type
    };
    $.ajax({
        url: "/Request/ValidateField",
        type: "POST",
        data: JSON.stringify(data),
        dataType: "json",
        contentType: "application/json",
        success: function (response) {
            if (response.Success) {
                document.getElementById(id).style.borderRightColor = '#38ACEC';
            }
            else {
                document.getElementById(id).style.borderRightColor = '#ff0000';
            }
        },
        error: function () {
            alert('בעיה בשרת');
        }
    });
}