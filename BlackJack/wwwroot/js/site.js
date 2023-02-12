// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {


});

function ShowLoading(_message, _title, _height) {
    $('#dialogLoading p').html(_message);
    $('#dialogLoading div').show();
    if (_title != undefined) {
        $('#dialogLoading').dialog({
            title: _title
        });
    }
    $('#dialogLoading').dialog('open');
    if (_height != null) {
        $('#dialogLoading').css("height", _height);
    }
}

function CloseLoading() {

    $('#dialogLoading div').hide();
    $('#dialogLoading').dialog('close');
}

function ShowMessage(_message, _callback, _title) {
    _messageCallback = _callback;

    if (_title == undefined) {
        _title = "Black Jack";
    }
    $(".btnMessage").show();
    $(".btnConfirm").hide();
    $("#DialogMessageTitle").html(_title);
    $("#DialogMessageBody").html(_message);
    $("#DialogMessage").modal("show");
}

function ShowConfirm(_message, _callback, _title) {
    _confirmCallback = _callback;

    if (_title == undefined) {
        _title = "Black Jack";
    }
    $(".btnMessage").hide();
    $(".btnConfirm").show();
    $("#DialogMessageTitle").html(_title);
    $("#DialogMessageBody").html(_message);
    $("#DialogMessage").modal("show");
}

function CloseDialogMessage() {
    $('#DialogMessage').modal('hide');
}

function getQueryStringValue(key) {
    return decodeURIComponent(window.location.search.replace(new RegExp("^(?:.*[&\\?]" + encodeURIComponent(key).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
}