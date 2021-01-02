$(document).ready(function () {
    function inputFileInit() {
        $('#CoverPhotoImg').on('change', function () {
            //get the file name
            var fileName = $(this).val();
            //replace the "Choose a file" label
            $(this).next('.custom-file-label').html(fileName);
        })
    }

    function datePickerInit() {
        // ref: https://amsul.ca/pickadate.js/date/
        $('#CreateDate').pickadate({
            showMonthsShort: true,
            format: 'yyyy/mm/dd',
            formatSubmit: 'yyyy-mm-dd',
            hiddenName: true
        })
    }

    // call init
    inputFileInit();
    datePickerInit();
});