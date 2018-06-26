$('#moduleform').on('click',
    function () {
        $('#yaml').append($('#a').attr('name'));
    });


var i = 0;

$(document).ready(function () {
    var options = $.getJSON('/Scripts/projectscripts/csharpmodules.json', function (data) {
        $.each(data, function (key, val) {
            $.each(val,
                function (objectKey, objectValue) {
                    $("#selectbox" + i).append($('<option style="width: 100%; color:#ff9800 !important; background-color:#374046 !important;"></option>').attr("value", objectKey).text(objectKey));
                });
        });
    });
});

// <summary>
// generates form, from selected module
// </summary>
function generateform(select) {
    var selected = $(select).val();
    var obj = $.getJSON("/Scripts/projectscripts/csharpmodules.json", function (data) {
        $.each(data, function (datakey, dataval) {
            $.each(dataval,
                function (objectKey, objectValue) {
                    if (selected == objectKey) {
                        var form = '<button class="btn btn-primary" id="closeicon" + i +" aria-hidden="true" style="background: #ff9800; border-radius: 0px;" onclick="hide(this,form' + i + ')">hide ' + selected +'</button><br>';
                        form += '<form class="form-control" style="color:#ff9800;" id="form' + i + '"><div class="form-group"><h1>' + selected + '</h1><br>';

                        $.each(objectValue, function (modulenames, moduleparams) {

                            form += '<label name="' + modulenames + '" for="' + modulenames + i + '">' + modulenames + '</label>';
                            var req = false;
                            $.each(moduleparams, function (paramattr, attrvalue) {
                                if (paramattr == 'required' && attrvalue == true) {
                                    req = true;
                                }
                                if (paramattr == 'options' && attrvalue != false) {

                                    form += '<select style="width: 100%; color:#7edc06 !important;"  class="form-control" required=' + req + ' id="' + modulenames + i + '" name="' + modulenames + '">';
                                    $.each(attrvalue, function (index, optionsval) {
                                        form += '<option style="color:#7edc06 !important;background-color: #374046;" >' + optionsval + '</option>';
                                    });
                                    form += '</select><br>';
                                } else if (paramattr == 'options') {
                                    form += '<input class="form-control required=' + req + ' id="' + modulenames + i + '" name="' + modulenames + '" style="height:26px; color:#ff9800;"><br>';
                                }
                            });
                        });
                        form += '<br><button type="button" onclick="GenerateYamlTextField()" class="btn btn-warning" style="background: #ff9800; border-radius: 0px;"><i class="fas fa-cogs"></i> Generate</button>';
                        form += '<br></div><i class="fa fa-plus text-success" id="plusicon' + i + '" aria-hidden="true" style="font-size:36px;" onclick="generateselect()"></i></form><br>';

                        $('#moduleform').append(form);
                        $('#selectbox' + i + '').remove();
                        i++;
                    }
                });
        });
    });
}

function hide(btn,form) {
    $(form).toggle();
}
// <summary>
// generates plus button for adding new modules
// </summary>
function generateselect() {
    var a = i - 1;
    $('#plusicon' + a).remove();


    $.getJSON("/Scripts/projectscripts/csharpmodules.json",
        function(optiondata) {
            var select = '<select class="form-control" style="width:100%;" id="selectbox' +
                i +
                '" onchange="generateform(this)">' +
                '<option selected = "selected" disabled = "disabled" > Choose a module...</option>';
            $.each(optiondata,
                function(keys, values) {
                    $.each(values,
                        function(objectkey, objectvalue) {
                            select += '<option style="width: 100%; color:#ff9800 !important; background-color:#374046 !important;" value="' + objectkey + '">' + objectkey + '</option>';
                        });
                });
            select += '</select>';
            $('#selectarea').append(select);
        });
}