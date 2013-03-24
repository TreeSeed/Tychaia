"use strict";

/// <summary>
/// Deals with exporting various image sizes.
/// </summary>
function MMAWSizes()
{
    this._generateSize = function(width, height, name) {
        var id = width + "x" + height;
        if (name != null) {
            id += " (" + name + ")";
        }
        this.sizes[id] = {
            "width": width,
            "height": height,
            "css": "s" + width + "x" + height
        };
    };

    this.sizes = {};
    this._generateSize(1920, 1080, "1080p");
    this._generateSize(1600, 1200);
    this._generateSize(1680, 1050);
    this._generateSize(1400, 1050);
    this._generateSize(1600, 900);
    this._generateSize(1280, 800, "ASUS Transformer");
    this._generateSize(720, 1080, "Samsung Galaxy Nexus");
    this._generateSize(480, 800, "Lumia 800");
    this._generateSize(640, 960, "iPhone 4s");
    this._generateSize(768, 1024, "iPad");
    this._generateSize(600, 1024, "Samsung Galaxy Tab");
    this._generateSize(480, 800, "Samsung Galaxy S");
    this._generateSize(320, 480, "HTC Magic");
    this._generateSize(240, 320);
    
    this.activeSize = "1920x1080";
    
    this.initializeOutputSizeOption = function() { 
        for (var idx in this.sizes) {
            if (!this.sizes.hasOwnProperty(idx)) {
                continue;
            }
            var option = document.createElement("option");
            option.innerText = idx;
            option.value = idx;
            $("#outputSize")[0].appendChild(option);
        }
        $("#outputSize").change(function() {
            this.setSize($("#outputSize")[0].value);
        }.bind(this));
    };
    
    this.getAvailableSizes = function() {
        return this.sizes.map(function(val, idx) { return idx; });
    };
    
    this.setSize = function(size) {
        $("#canvas")[0].width = this.sizes[size].width;
        $("#canvas")[0].height = this.sizes[size].height;
        $("#canvas").style.width = this.sizes[size].width + "px";
        $("#canvas").style.height = this.sizes[size].height + "px";
        $("#canvasContainer").style.width = this.sizes[size].width + "px";
        $("#canvasContainer").style.height = this.sizes[size].height + "px";
        $(".size").text(size);
        this.activeSize = size;
    };
}
