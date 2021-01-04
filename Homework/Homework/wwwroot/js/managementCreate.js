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
            hiddenName: true
        })
    }

    function selectTagInit() {
        var url = $('#tagCloudTextUrl').val();
        $.ajax({
            url: url,
            method: 'GET',
            dataType: 'json',
            success: function (res) {
                console.log(res);
                setTagCloudSelect(res);
                //console.log(res);
            },
            error: function (err) { console.log(err) },
        });
    }

    function setTagCloudSelect(tagArr) {
        $('#TagsArray').select2({
            tags: true,
            data: tagArr,
            tokenSeparators: [',']
        });
    }

    function ckeditorInit() {
        CKEDITOR.replace('Body');
    }

    // call init
    inputFileInit();
    datePickerInit();
    selectTagInit();
    ckeditorInit();
});