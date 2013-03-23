"use strict";

/// <summary>
/// The main screen for Make Me a World.
/// </summary>
function MMAWUIMain(controller)
{
    /// <summary>
    /// The UI controller.
    /// </summary>
    this.controller = controller;

    /// <summary>
    /// Initializes the UI after the initial page load.
    /// </summary>
    this.initialize = function() {
        // Perform some initial data loading.
        if (window.location.hash != "") {
            this.controller.settings.loadSeedFromHash();
        } else {
            $("#seedSet")[0].value = Math.random().toString() * 0xFFFFFFFFFFFFFF;
        }
        $("#seed").text($("#seedSet")[0].value);
        this.controller.seed = this.controller.misc.returnNumberOrHash($("#seedSet")[0].value);
        
        // Register UI handlers.
        $("#seedSet").change(this._onSeedSetChanged.bind(this));
        $("#randomize").click(this._onRandomizeClicked.bind(this));
        $("#outputLayer").change(this._onOutputLayerChanged.bind(this));
        $("#showAdvanced").click(this._onShowAdvancedClicked.bind(this));
        $("#loadButton").click(this._onGenerationStarted.bind(this));
        
        // Focus the UI.
        $("#seedSet").focus();
        $("#seedSet").select();
        
        // If the window hash is not empty, simulate pressing the Generate World.
        if (window.location.hash != "") {
            $("#loadButton").click();
            this.controller.gotoStage("mainAndProcessing");
        }
    };
    
    /// <summary>
    /// Called when the stage is made the current stage.
    /// </summary>
    this.activate = function() {
        $("#welcomeMessage")[0].style.top = "100px";
        $("#header").show();
        $("#welcomeMessage").show();
        $("#progress").hide();
        $("#remainingHolder").hide();
        $("#info").hide();
        $("#end").hide();
        $("#watermark").show();
        $("#newRender").show();
    };
    
    /// <summary>
    /// Event handler for when user changes the seed value.
    /// </summary>
    this._onSeedSetChanged = function() {
        $("#seed").text($("#seedSet")[0].value);
        this.seed = this.controller.misc.returnNumberOrHash($("#seedSet")[0].value);
        this.controller.settings.setHashFromSettings();
    };
    
    /// <summary>
    /// Event handler for when the "Randomize" button is clicked.
    /// </summary>
    this._onRandomizeClicked = function() {
        $("#seedSet")[0].value = Math.random().toString() * 0xFFFFFFFFFFFFFF;
        $("#seed").text($("#seedSet")[0].value);
        this.seed = this.controller.misc.returnNumberOrHash($("#seedSet")[0].value);
        this.controller.settings.setHashFromSettings();
    };
    
    /// <summary>
    /// Event handler for when user changes the output layer.
    /// </summary>
    this._onOutputLayerChanged = function() {
        this.settings.setHashFromSettings();
    };
    
    /// <summary>
    /// Event handler for when the user shows the advanced options.
    /// </summary>
    this._onShowAdvancedClicked = function () {
        $("#advancedOptions").show();
        return false;
    };
    
    /// <summary>
    /// Event handler for when the user clicks the "Make me a world" button.
    /// </summary>
    this.stopEarly = false;
    this.stopFailure = false;
    this._onGenerationStarted = function () {
        this.controller.gotoStage("processing");

        var canvas = $("#canvas")[0];
        var ctx = canvas.getContext('2d');
        var SIZE = 64;
        var start = new Date().getTime();
        if ($("#enableRenderDebugging")[0].checked)
            ctx.fillStyle = "#F00";
        else
            ctx.fillStyle = "#000";
        ctx.fillRect(0, 0, $("#canvas")[0].width, $("#canvas")[0].height);

        var x = -canvas.width;
        var y = -canvas.height;
        var z, total;
        //if ($("#outputFormat")[0].value.substring(0, 2) == "3D")
        //{
            z = -SIZE;
            total = ((canvas.width * 2) / SIZE) * ((canvas.height * 2) / SIZE) * 5;
        /*}
        else
        {
            z = 0;
            total = ((canvas.width * 2) / SIZE) * ((canvas.height * 2) / SIZE);
        }*/
        var current = 0;
        var rendered = 0;
        var skipped = 0;

        var getCallback = function (canvas, ctx, x, y, z, call) {
            return function () {
                var rcx = 32 / 2 + canvas.width / 2 - SIZE;
                var rcy = 32 / 2; // -15 - 32 + canvas.height / 2;
                var rx = rcx + ((x / 32 - y / 32) / 2.0 * 64);
                var ry = rcy + (x / 32 + y / 32) * 32 - (z / 32 - 0) * 32;
                if (rx < (-SIZE * 3) || rx > canvas.width || ry < (-SIZE * 3) || ry > canvas.height) {
                    skipped += 1;
                    call();
                    return;
                }
                var img = new Image();
                try
                {
                    img.onload = function () {
                        if (img.width == 1 && img.height == 1) {
                            skipped += 1;
                            call();
                        } else {
                            rendered += 1;
                            ctx.drawImage(img, rx, ry);
                            call();
                        }
                    };
                    img.onerror = function () {
                        this.stopEarly = true;
                        this.stopFailure = true;
                    };
                    // FIXME: This locks up Google Chrome!?!?
                    img.src = "images/map.png?x=" + x + "&y=" + y + "&z=" + z + "&size=" + SIZE + "&seed=" + seed + "&layer=" + $("#outputFormat")[0].value;
                }
                catch (ex)
                {
                    this.stopEarly = true;
                    this.stopFailure = true;
                }
            }.bind(this);
        }.bind(this);

        var zeroFill = function (number, width) {
            width -= number.toString().length;
            if (width > 0) {
                return new Array(width + (/\./.test(number) ? 2 : 1)).join('0') + number;
            }
            return number + ""; // always return a string
        }

        var finalize = function () {
            renderWatermark(canvas, $("#seed").text());
        }

        var run = function () {
            if (this.stopEarly) {
                if (this.stopFailure)
                    $("#endMessage").text("Rendering failed due to a server error!");
                else
                    $("#endMessage").text("Rendering was stopped manually.");
                if (this.controller.currentStage == "mainAndProcessing")
                    this.controller.gotoStage("mainAndResults");
                else
                    this.controller.gotoStage("results");
                this.stopEarly = false;
                this.stopFailure = false;
                finalize();
                return;
            }
            x += SIZE;
            if (x > canvas.width) {
                x = -canvas.height;
                y += SIZE;
            }
            if (y > canvas.height) {
                x = -canvas.width;
                y = -canvas.height;
                if ($("#outputFormat")[0].value.substring(0, 2) == "3D")
                    z += SIZE;
                else
                    z = SIZE * 4 + 1; // Hack so next if statement triggers.
            }
            if (z > SIZE * 4) {
                if (currentStage == "mainAndProcessing")
                    gotoStage("mainAndResults");
                else
                    gotoStage("results");
                $("#endMessage").text("Rendering completed successfully.");
                finalize();
                return;
            }
            var penalty = 0;
            var millisCurrent = new Date().getTime();
            var millisDifference = millisCurrent - start;
            var estimatedMillisTotal = millisDifference * (total / current);
            var estimatedMillisRemaining = estimatedMillisTotal - millisDifference;
            if (estimatedMillisRemaining < 0) estimatedMillisRemaining = 0;
            var estimatedTimeRemaining = new Date(estimatedMillisRemaining);
            var estimatedTimeTotal = new Date(estimatedMillisTotal);
            var stringCurrent = new Date(millisDifference).getMinutes() + ":" + zeroFill(new Date(millisDifference).getSeconds(), 2);
            var stringRemaining = estimatedTimeRemaining.getMinutes() + ":" + zeroFill(estimatedTimeRemaining.getSeconds(), 2);
            var stringTotal = estimatedTimeTotal.getMinutes() + ":" + zeroFill(estimatedTimeTotal.getSeconds(), 2);
            $("#progress").html(((current / total) * 100).toFixed(4) + "% complete (" + (total - current) + " tiles remaining)<br />" + rendered + " rendered, " + skipped + " skipped");
            $("#timeRemaining").text("Completion in " + stringRemaining + " minutes.");
            current += 1;
            getCallback(canvas, ctx, x, y, z, run)();
        }.bind(this);
        run();
        
        return false;
    };
};
