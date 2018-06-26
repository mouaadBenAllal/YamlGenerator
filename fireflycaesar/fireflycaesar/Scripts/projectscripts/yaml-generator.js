// <summary>
// Gemerates yaml textfield
// </summary>
function GenerateYamlTextField() {
    var textfield = $('#yamltextfield').val();
    if (!textfield) {
        textfield = "---\n";
    }
    var formLength = $("#moduleform *").filter(":input").length;

    var yaml = '\n[{ \"' + $("#moduleform").find('h1').text() + '\":{';
    var i = 0;
    $("#moduleform *").filter(":input").each(function (i, child) {
        if ($(child).val() != '') {
            if (!$(child).is('button')) {
                yaml += '\"' + $(child).attr('name') + '\"' + ":" + "\"" + $(child).val() + "\"" + ',' + "\n";
            }
        }
        i++;
    });
    yaml = yaml.slice(0, -1);
    yaml = yaml.slice(0, -1);
    yaml += '\n}}]';

    console.log(yaml);
    var data = JSON.parse(yaml);
    var yaml = json2yaml(data);

    yaml += '\n\n';
    textfield += yaml;

    console.log(yaml);

    $('#generatedYamlTextField').removeAttr("hidden");
    $('#yamltextfield').val(textfield);
}

