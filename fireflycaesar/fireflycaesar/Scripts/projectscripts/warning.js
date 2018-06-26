function WarningBeforePageLeave() {
    window.onbeforeunload = function (e) {
        e = e || window.event;

        // For IE and Firefox prior to version 4
        if (e) {
            e.returnValue = 'Are you sure?';
        }
        // For Safari
        return 'Are you sure?';
    };
}