/*
 * Downloads yaml from textfield
 */
function downloadyaml() {
    if ($('input[id="yamlfilename"]').val() != '') {
        $("<a />", {
            download: $('#yamlfilename').val() + ".yaml",
            href: URL.createObjectURL(new Blob([$("textarea").val()], {
                type: "text/vnd.yaml"
            }))
        })
            .appendTo("body")[0].click();
        $(window).one("focus", function () {
            $("a").last().remove()
        })
    } else {
        window.alert("Please fill in the Yaml filename");
    }
}