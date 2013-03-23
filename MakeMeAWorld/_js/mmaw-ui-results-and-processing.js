"use strict";

/// <summary>
/// The results and processing screens for Make Me a World.
/// </summary>
function MMAWUIResultsAndProcessing(controller)
{
    /// <summary>
    /// The UI controller.
    /// </summary>
    this.controller = controller;

    /// <summary>
    /// Initializes the UI after the initial page load.
    /// </summary>
    this.initialize = function() {
    };
    
    /// <summary>
    /// Called when the stage is made the current stage.
    /// </summary>
    this.activate = function() {
        $("#header").show();
        $("#welcomeMessage").show();
        $("#progress").hide();
        $("#remainingHolder").hide();
        $("#info").hide();
        $("#end").show();
        $("#watermark").hide();
        $("#renderAnother").hide();
        $("#welcomeMessage")[0].style.top = "250px";
        $("#newRender").show();
    };
};