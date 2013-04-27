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
            $("#seedSet").val(Math.random().toString() * 0xFFFFFFFFFFFFFF);
        }
        $("#seed").text($("#seedSet").val());
        this.controller.seed = this.controller.misc.returnNumberOrHash($("#seedSet").val());
        
        // Register UI handlers.
        $("#seedSet").change(this._onSeedSetChanged.bind(this));
        $("#randomize").click(this._onRandomizeClicked.bind(this));
        $("#outputLayer").change(this._onOutputLayerChanged.bind(this));
        $("#showAdvanced").click(this._onShowAdvancedClicked.bind(this));
        $("#loadButton").click(this._onGenerationStarted.bind(this));
        $("#stopEarly").click(this._onStopEarlyClicked.bind(this));
        
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
        $("#seed").text($("#seedSet").val());
        this.controller.seed = this.controller.misc.returnNumberOrHash($("#seedSet").val());
        this.controller.settings.setHashFromSettings();
    };
    
    /// <summary>
    /// Event handler for when the "Randomize" button is clicked.
    /// </summary>
    this._onRandomizeClicked = function() {
        $("#seedSet").val(Math.random().toString() * 0xFFFFFFFFFFFFFF);
        $("#seed").text($("#seedSet").val());
        this.controller.seed = this.controller.misc.returnNumberOrHash($("#seedSet").val());
        this.controller.settings.setHashFromSettings();
    };
    
    /// <summary>
    /// Event handler for when user changes the output layer.
    /// </summary>
    this._onOutputLayerChanged = function() {
        this.controller.settings.setHashFromSettings();
    };
    
    /// <summary>
    /// Event handler for when the user shows the advanced options.
    /// </summary>
    this._onShowAdvancedClicked = function () {
        $("#advancedOptionsTabs").toggle();
        $("#advancedOptions").toggle();
        return false;
    };
    
    /// <summary>
    /// Event handler for when the user clicks the "Make me a world" button.
    /// </summary>
    this._stopEarly = false;
    this._stopFailure = false;
    this._stopCallback = null;
    this._onGenerationStarted = function () {
        this.controller.gotoStage("processing");
        $("#progress").html("0% complete (0 tiles remaining)<br />0 retrieved, 0 rendered, 0 skipped");

        var retriever, renderer;
        if ($("#serverSideRenderingForced").length > 0 ||
            $("#serverSideRendering").is(':checked')) {
            retriever = new MMAWServerRetriever();
            renderer = new MMAWServerRenderer();
        } else {
            retriever = new MMAWClientRetriever();
            renderer = new MMAWClientRenderer();
        }
        $("#info").data("never-show", !renderer.showInfoPanel());
        if (!renderer.showInfoPanel()) {
            $("#info").hide();
        }
        var processor = new MMAWProcessor(
                retriever,
                renderer,
                $("#canvas")[0],
                this.controller.seed);
        if ($("#outputLayer").val().substring(0, 2) == "2D") {
            processor.minDepth = 0;
            processor.maxDepth = 1;
        }
        processor.onProgress = function() {
            processor.calculateProgressInformation(this.controller.misc.zeroFill);
        }.bind(this);
        processor.onFinish = function() {
            if (this.controller.currentStage == this.controller.stages["mainAndProcessing"])
                this.controller.gotoStage("mainAndResults");
            else
                this.controller.gotoStage("results");
            $("#endMessage").text("Rendering completed successfully.");
            this.controller.rendering.renderWatermark($("#canvas")[0], $("#seed").text());
        }.bind(this);
        processor.onFailure = function(ex) {
            if (this.controller.currentStage == this.controller.stages["mainAndProcessing"])
                this.controller.gotoStage("mainAndResults");
            else
                this.controller.gotoStage("results");
            $("#endMessage").text("Rendering failed (" + ex + ").");
            this.controller.rendering.renderWatermark($("#canvas")[0], $("#seed").text());
        }.bind(this);
        this._stopCallback = function() {
            processor.stopProcessing();
            if (this.controller.currentStage == this.controller.stages["mainAndProcessing"])
                this.controller.gotoStage("mainAndResults");
            else
                this.controller.gotoStage("results");
            $("#endMessage").text("Rendering stopped.");
            this.controller.rendering.renderWatermark($("#canvas")[0], $("#seed").text());
        };
        processor.startProcessing();
        
        // Since this is a submit button, we don't want
        // the page to actually reload.
        return false;
    };
    
    /// <summary>
    /// Event handler for when the user clicks the "Stop Early" button.
    /// </summary>
    this._onStopEarlyClicked = function () {
        this._stopEarly = true;
        this._stopFailure = false;
        this._stopCallback();
    };
};
