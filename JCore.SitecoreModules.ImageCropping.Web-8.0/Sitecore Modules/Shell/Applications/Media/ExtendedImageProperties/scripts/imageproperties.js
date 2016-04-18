var $jQuery = jQuery.noConflict();
var imageId = "img[id$=Img]";
var ratio = "";
var scalingFactor = 1;

$jQuery(function () {
    var media = $jQuery(imageId);

    CalculateScalingFactor();

    var x1 = adjustToCoordinate($jQuery("input[coordinate='x1']").val());
    var y1 = adjustToCoordinate($jQuery("input[coordinate='y1']").val());

    var x2 = adjustToCoordinate($jQuery("input[coordinate='x2']").val());
    var y2 = adjustToCoordinate($jQuery("input[coordinate='y2']").val());

    if (ratio) {
        $jQuery("#clear").hide();
    }

    if (x1 >= 0 || y1 >= 0 || x2 > 0 || y2 > 0) {
        if (ratio) {
            media.imgAreaSelect({
                x1: x1,
                y1: y1,
                x2: x2,
                y2: y2,
                aspectRatio: ratio,
                onSelectEnd: updateCoordinates,
                handles: true,
                parent: ".cropImageParent"
            });
        } else {
            media.imgAreaSelect({
                x1: x1,
                y1: y1,
                x2: x2,
                y2: y2,
                onSelectEnd: updateCoordinates,
                handles: true,
                parent: ".cropImageParent"
            });
        }
    } else {
        if (ratio) {
            media.imgAreaSelect({
                onSelectEnd: updateCoordinates,
                handles: true,
                aspectRatio: ratio,
                parent: ".cropImageParent"
            });
        } else {
            media.imgAreaSelect({
                onSelectEnd: updateCoordinates,
                handles: true,
                parent: ".cropImageParent"
            });
        }
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
        if (!ratio) {
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
        }
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
    if (!value && value !== 0) return "";
    return Math.round(value * scalingFactor);
}

function adjustToCoordinate(value) {
    if (!value && value !== 0) return "";
    return Math.round(value / scalingFactor);
}

function CalculateScalingFactor() {
    var originalWidth = parseInt($jQuery("input[id$=OriginalWidth]").val());
    var originalHeight = parseInt($jQuery("input[id$=OriginalHeight]").val());
    var newWidth;
    var newHeight;
    var image = $jQuery(imageId);

    if (originalWidth > originalHeight) {
        newWidth = 500;
        newHeight = originalHeight * newWidth / originalWidth;
    } else {
        newHeight = 300;
        newWidth = originalWidth * newHeight / originalHeight;
    }

    image.width(newWidth);
    image.height(newHeight);
    ratio = parseInt(newWidth) + ":" + parseInt(newHeight);
    scalingFactor = originalWidth / newWidth;
}


