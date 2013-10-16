var $jQuery = jQuery.noConflict();
var scalingFactor = 1;

$jQuery(function () {
    var media = $jQuery("#ctl00_ctl00_ctl00_ctl00_ctl04_Img");

    scalingFactor = CalculateScalingFactor();

    var x1 = adjustToCoordinate($jQuery("input[coordinate='x1']").val());
    var y1 = adjustToCoordinate($jQuery("input[coordinate='y1']").val());

    var x2 = adjustToCoordinate($jQuery("input[coordinate='x2']").val());
    var y2 = adjustToCoordinate($jQuery("input[coordinate='y2']").val());

    if (x1 > 0 && y1 > 0 && x2 > 0 && y2 > 0) {
        media.imgAreaSelect({
            x1: x1,
            y1: y1,
            x2: x2,
            y2: y2,
            onSelectEnd: updateCoordinates, 
            handles: true
        });
    } else {
        media.imgAreaSelect({
            onSelectEnd: updateCoordinates,
            handles: true
        });
    }
});

function updateCoordinates(img, selection) {
    $jQuery("input[coordinate='x1']").val(adjustToSize(selection.x1));
    $jQuery("input[coordinate='y1']").val(adjustToSize(selection.y1));
    $jQuery("input[coordinate='x2']").val(adjustToSize(selection.x2));
    $jQuery("input[coordinate='y2']").val(adjustToSize(selection.y2));
    scForm.postEvent(this, event, 'ChangeDimentions');
}

function adjustToSize(value) {   
    return Math.round(value * scalingFactor);
}

function adjustToCoordinate(value) {
    return Math.round(value / scalingFactor);
}

function CalculateScalingFactor() {
    var originalHeight = $jQuery("#ctl00_ctl00_ctl00_ctl00_ctl04_OriginalHeight").val();
    var newHeight = 300;
    return originalHeight / 300;
}