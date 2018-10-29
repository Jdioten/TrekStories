////validate min and max durations on Search Form		
$(document).ready(function () {
    $('#SearchForm').on('submit', function (e) {

        // Check min <= max
        if ($('#MinDuration').val() != '' && $('#MaxDuration').val() != '' && $('#MinDuration').val() > $('#MaxDuration').val())
        {
            $("#minDurationValidation").append("The minimum duration cannot be bigger than the maximum duration.");

        // Stop submission of the form
        e.preventDefault();
        }
    });
});