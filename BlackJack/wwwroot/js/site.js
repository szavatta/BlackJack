// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {

    $('#dialogMessage').dialog({
        autoOpen: false,
        buttons: {
            "Ok": function () {

                if (_messageCallback) {
                    _messageCallback();
                }
                $(this).dialog("close");
            }
        },
        modal: true
    });

    $('#dialogConfirm').dialog({
        autoOpen: false,
        buttons: {
            "Si": function () {
                _confirmCallback();
                $(this).dialog("close");
            },
            "No": function () {
                $(this).dialog("close");
            }
        },
        modal: true
    });

});

function showLoading(_message, _title, _height) {
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

function closeLoading() {

    $('#dialogLoading div').hide();
    $('#dialogLoading').dialog('close');
}

function showMessage(_message, _title, _callback, _width, _height, senzaImg, conChiudi) {
    _messageCallback = _callback;

    var img = '<td style="text-align:left;width:50px" valign="top"><img src="/Content/images/information.png" alt="" border="0" /></td>';
    if (senzaImg != null && senzaImg != 'undefined' && senzaImg != '' && senzaImg.toLowerCase() == "true") {
        img = '';
    }

    var buttonChiudi = '';
    if (conChiudi != null && conChiudi != 'undefined' && conChiudi != '' && conChiudi.toLowerCase() == "true") {
        buttonChiudi = '<div style=\"float: right; height: 25px; display: inline-block; padding: 1px;\"><img class=\"closeDialogMessage\" style=\"cursor: pointer;\" src=\"/Content/btn-grigia2.png\" width=\"25px\" title=\"Chiudi\"></div><div style=\"clear: right;\"></div>';
    }

    var testo = '<table style="width:100%">' + buttonChiudi + '<tr>' + img + '<td valign="top" style="text-align:left">' + _message + '</td></tr></table>';

    $('#dialogMessage').html(testo);

    if (_title != undefined) {
        $('#dialogMessage').dialog({
            title: _title
        });
    }

    if (_width != undefined && _width != null && _width > 0) {
        $("#dialogMessage").dialog("option", "width", _width);
    }

    if (_height != undefined && _height != null && _height > 0) {
        $("#dialogMessage").dialog("option", "height", _height);
    }

    $('#dialogMessage').dialog('open');
}

function showConfirm(_message, _callback, _width, _height, senzaImg) {
    _confirmCallback = _callback;

    var img = '<td style="text-align:left;width:50px" valign="top"><img src="/Content/images/question.png" alt="" border="0" /></td>';
    if (senzaImg != null && senzaImg != 'undefined' && senzaImg != '' && senzaImg.toLowerCase() == "true") {
        img = '';
    }
    var testo = '<table style="width:100%"><tr>' + img + '<td valign="top" style="text-align:left">' + _message + '</td></tr></table>';

    $('#dialogConfirm').html(testo);

    $("#dialogConfirm").dialog("option", "z-index", 1000);

    $("#dialogConfirm").dialog({
        open: function () {
            $(this).siblings('.ui-dialog-buttonpane').find('button:eq(1)').focus();
        }
    });

    if (_width != undefined && _width > 0) {
        $("#dialogConfirm").dialog("option", "width", _width);
    }

    if (_height != undefined && _height != null && _height > 0) {
        $("#dialogConfirm").dialog("option", "height", _height);
    }

    $('#dialogConfirm').dialog('open');
}

function getQueryStringValue(key) {
    return decodeURIComponent(window.location.search.replace(new RegExp("^(?:.*[&\\?]" + encodeURIComponent(key).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
}