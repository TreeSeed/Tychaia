"use strict";

/// <summary>
/// The processing screen for Make Me a World.
/// </summary>
function MMAWUIProcessing(controller)
{
    /// <summary>
    /// The UI controller.
    /// </summary>
    this.controller = controller;

    /// <summary>
    /// Initializes the UI after the initial page load.
    /// </summary>
    this.initialize = function() {
        // Register UI handlers.
        $("#stopEarly").click(this._onStopEarlyClicked.bind(this));
    };
    
    /// <summary>
    /// Called when the stage is made the current stage.
    /// </summary>
    this.activate = function() {
        $("#welcomeMessage")[0].style.top = "100px";
        $("#header").show();
        $("#welcomeMessage").hide();
        $("#progress").show();
        $("#remainingHolder").show();
        $("#info").show();
        $("#end").hide();
        $("#watermark").show();
    };
    
    /// <summary>
    /// Event handler for when the user clicks the "Stop Early" button.
    /// </summary>
    this._onStopEarlyClicked = function () {
        this.stopEarly = true;
        this.stopFailure = false;
    };
};