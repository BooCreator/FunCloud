function getCaretPos(elem) {
    elem.focus();
    if (elem.selectionStart) return elem.selectionStart;
    else if (document.selection) {
        var sel = document.selection.createRange();
        var clone = sel.duplicate();
        sel.collapse(true);
        clone.moveToElementText(elem);
        clone.setEndPoint('EndToEnd', sel);
        return clone.text.length;
    }
    return 0;
}

function getStr(elem, pos_elem) {
    var pos = $(pos_elem).val();
    var str = $(elem).val();
    var start = pos;
    var end = pos;
    var c = str[start];
    while (c != ',' && start > 0) {
        start--;
        c = str[start];
    }
    с = str[end];
    while (c != ',' && end <= str.length) {
        c = str[end];
        end++;
    }
    start = (start > 0) ? start + 1 : start;
    console.log(str.substr(start, end - start) + "|" + start + "|" + end + "|" + pos);
    return str.substr(start, end - start);
}

function msg(message) {
    var i_class = "alert alert-dismissible fade show ";
    var text = "";
    switch (message) {
        case 'ok':
            i_class += "alert-success";
            text = "Операция выполнена успешно!";
            break;
        case 'exist':
            i_class += "alert-danger";
            text = "Объект уже существует!";
            break;
        case 'empty':
            i_class += "alert-danger";
            text = "Объект пуст!";
            break;
        case 'not_access':
            i_class += "alert-danger";
            text = "Доступ запрещен!";
            break;
        case 'unknown_error':
            i_class += "alert-danger";
            text = "Произошла неизвестная ошибка!";
            break;
        default:
            i_class += "alert-warning"; break;
    }

    $("#msg").html("");
    $("#msg").append(
        '<div class="' + i_class + '" role="alert">' +
            '<strong>Сообщение!</strong> ' + text + 
            '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
                '<span aria-hidden="true">&times;</span>' +
            '</button>' +
        '</div>'
    );
    $('html, body').animate({ scrollTop: 0 }, 'fast');
}

function post_msg(action, value) {
    $.post(action, { id: value }).done(
        function (data) {
            msg(data);
            location.reload();
        });
}

function post(action, value) {
    $.post(action, { id: value }).done(
        function (data) {
            if (data == "ok") {
                location.reload();
            } else {
                msg(data);
            }
        });
}

function post_form(action, form_id) {
    var data = $("#" + form_id).serialize();
    $.post(action, data).done(
        function (data) {
            if (data == "ok") {
                location.reload();
            } else {
                msg(data);
            }
        });
}

function post_data(action, item_id, data_id) {
    var value = "";
    if (data_id != 0)
        value = $("#" + data_id).val();
    $.post(action, { id: item_id, title: value }).done(
        function (data) {
            if (data == "ok") {
                location.reload();
            } else {
                msg(data);
            }
        });
}

function like(action, item_id, item_author, is_like) {
    $.post(action, { id: item_id, author: item_author, isLike: is_like }).done(
        function (data) {
            location.reload();
        });
}

function set(elem, value, point) {
    $("#" + elem).val(value);
    $("#" + elem + "Name").val($(point).text());
}

function FindValue(action, text, this_id, dropdown, param, default_val) {
    $.post(action, { Text: text, ThisIs: this_id }).done(
        function (data) {
            var items = data;
            $(dropdown).html("");
            $(dropdown).append(
                '<button class="dropdown-item bg-red-hover" type="button" onclick="set(\'' + param + '\', -1, this)">' + default_val + '</button>');
            for (var i = 0; i < items.length; i++) {
                $(dropdown).append(
                    '<button class="dropdown-item bg-red-hover" type="button" onclick="set(\'' + param + '\',' + items[i]["Value"] + ', this)">' + items[i]["Name"] + '</button>');
            }
        });
}
function FindValue2(action, text, this_id, dropdown, param) {
    $.post(action, { Text: text, ThisIs: this_id }).done(
        function (data) {
            var items = data;
            $(dropdown).html("");
            for (var i = 0; i < items.length; i++) {
                $(dropdown).append(
                    '<button class="dropdown-item bg-red-hover" type="button" onclick="add(\'' + param + '\', this)">' + items[i]["Name"] + '</button>');
            }
        });
}

function add(elem, btn) {
    var str = getStr($(elem), elem + "Pos");
    console.log(str);
    $(elem).val($(elem).val().replace(str, $(btn).text()) + ",");
}

// -- Profile

function removeFromList(action, item_id, work_id, user_id) {
    $.post(action, { id: item_id, WorkID: work_id, Author: user_id }).done(
        function (data) {
            if (data == "ok") {
                location.reload();
            } else {
                msg(data);
            }
        });
}

// -- Request



// -- Work

function addToList(action, list_id, work_id, user_id) {
    $.post(action, { id: list_id, WorkID: work_id, Author: user_id }).done(
        function (data) {
            msg(data);
        });
}

function removefile(action, item_id, item_name) {
    $.post(action, { WorkID: item_id, FileName: item_name }).done(
        function (data) {
            location.reload();
        });
}

function sendfile(action, item_id) {
    var data = new FormData();
    data.append("WorkID", item_id);
    var imagefile = document.querySelector('#file');
    data.append("file", imagefile.files[0]);

    $.ajax({
        url: action,
        type: 'POST',
        data: data,
        cache: false,
        dataType: 'json',
        processData: false,
        contentType: false,
        success: function (respond, textStatus) {
            location.reload();
        }
    });

}
function updFileList() {
    var tmp = [];
    for (var i = 0; i < fields.length; i++) {
        if (fields[i].value !== '') {
            tmp.push(fields[i].value);
        }
    }
    fileListOutput.textContent = tmp.join(', ')
}

// -- Comment

function answer(elem, comment_id) {
    $("[name=Answer]").val(comment_id);
    $("#title").text("Ответ к комментарию " + comment_id + "!");
}