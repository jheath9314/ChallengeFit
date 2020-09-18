$(document).ready(function () {
    $('[tool-tip-toggle="tooltip"]').tooltip({
        placement: 'top'
    });
});

// Write your JavaScript code.



function handleScoringTypeChange() {
    
    $(document).on('change', '#scoringType', function () {
        var label = $('#scoringType').find(":selected").text();
        if (label == "Time") {
            label = label + " (Minutes and Seconds)";
            showSecondsField();
        }
        else {
            hideSecondsField();
        }
        $('#timeLabel').text(label);
        
    });
}

function hideSecondsField() {
    var label = $('#scoringType').find(":selected").text();
    if (label != "Time") {
        $('#timeSeconds').hide();
    }
}

function showSecondsField() {
    var label = $('#scoringType').find(":selected").text();
    if (label == "Time") {
        $('#timeSeconds').show();
    }
}
function GetParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'), results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}