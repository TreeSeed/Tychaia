"use strict";

/// <summary>
/// The main and processing screens for Make Me a World.
/// </summary>
function MMAWUIMainAndProcessing(controller)
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
        $("#welcomeMessage")[0].style.top = "350px";
        $("#header").show();
        $("#welcomeMessage").show();
        $("#progress").show();
        $("#remainingHolder").show();
        if (!$("#info").data("never-show")) {
            $("#info").show();
        }
        $("#end").hide();
        $("#watermark").show();
        $("#newRender").hide();
    };
};