var $jQuery = jQuery.noConflict();
var scalingFactor = 1;
var imageId = "img[id$=Img]";

$jQuery(function () {
    var media = $jQuery(imageId);

    scalingFactor = CalculateScalingFactor();

    var x1 = adjustToCoordinate($jQuery("input[coordinate='x1']").val());
    var y1 = adjustToCoordinate($jQuery("input[coordinate='y1']").val());

    var x2 = adjustToCoordinate($jQuery("input[coordinate='x2']").val());
    var y2 = adjustToCoordinate($jQuery("input[coordinate='y2']").val());

    if (x1 >= 0 && y1 >= 0 && x2 > 0 && y2 > 0) {
        media.imgAreaSelect({
            x1: x1,
            y1: y1,
            x2: x2,
            y2: y2,
            onSelectEnd: updateCoordinates,
            handles: true,
            parent: ".cropImageParent"
        });
    } else {
        media.imgAreaSelect({
            onSelectEnd: updateCoordinates,
            handles: true,
            parent: ".cropImageParent"
        });
    }

    $jQuery("#square").click(function () {
        var media = $jQuery(imageId);
        var ias = media.imgAreaSelect({
            aspectRatio: "1:1",
            onSelectEnd: updateCoordinates,
            handles: true,
            parent: ".cropImageParent"
        });
    });
    $jQuery("#free").click(function () {
        var media = $jQuery(imageId);
        var ias = media.imgAreaSelect({
            aspectRatio: false,
            onSelectEnd: updateCoordinates,
            handles: true,
            parent: ".cropImageParent"
        });
    });
    $jQuery("#clear").click(function () {
        var media = $jQuery(imageId);
        var ias = media.imgAreaSelect({
            remove: true,
            onSelectEnd: updateCoordinates,
            handles: true,
            parent: ".cropImageParent"
        });
        ias = media.imgAreaSelect({
            onSelectEnd: updateCoordinates,
            handles: true,
            parent: ".cropImageParent"
        });
        updateCoordinates(null, { x1: "", y1: "", x2: "", y2: "" });
    });
});

function updateCoordinates(img, selection) {
    $jQuery("input[coordinate='x1']").val(adjustToSize(selection.x1));
    $jQuery("input[coordinate='y1']").val(adjustToSize(selection.y1));
    $jQuery("input[coordinate='x2']").val(adjustToSize(selection.x2));
    $jQuery("input[coordinate='y2']").val(adjustToSize(selection.y2));
    scForm.postEvent(this, event, 'ChangeDimentions');
}

function adjustToSize(value) {
    if (!value && value != 0) return "";
    return Math.round(value * scalingFactor);
}

function adjustToCoordinate(value) {
    if (!value && value != 0) return "";
    return Math.round(value / scalingFactor);
}

function CalculateScalingFactor() {
    var originalHeight = $jQuery("input[id$=OriginalHeight]").val();
    var newHeight = 300;
    return originalHeight / 300;
}