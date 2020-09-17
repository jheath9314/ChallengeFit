// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>



function handleScoringTypeChange() {
    
    $(document).on('change', '#scoringType', function () {
        var label = $('#scoringType').find(":selected").text();
        $('#timeLabel').text(label);
    });
}