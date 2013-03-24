"use strict";

/// <summary>
/// Controller for Make Me a World.
/// </summary>
function MMAWController()
{
    /// <summary>
    /// The current seed.
    /// </summary>
    this.seed = 0;
    
    /// <summary>
    /// Utility object for rendering functionality.
    /// </summary>
    this.rendering = null;
    
    /// <summary>
    /// Utility object for settings management.
    /// </summary>
    this.settings = null;
    
    /// <summary>
    /// Utility object for miscellaneous functionality.
    /// </summary>
    this.misc = null;

    /// <summary>
    /// Utility object for handling rendering sizes.
    /// </summary>
    this.sizes = null;
            
    /// <summary>
    /// An array of stages that can be active in the UI.
    /// </summary>
    this.stages = null;
    
    /// <summary>
    /// The current active UI stage.
    /// </summary>
    this.currentStage = null;
    
    /// <summary>
    /// Registers this UI to handle client interactions.
    /// </summary>
    this.register = function() {
        // Register the on page load handler.
        $(document).ready(this.initialize.bind(this));
        
        // Initalize some variables (that need other JS files
        // loaded first).
        this.rendering = new MMAWRendering();
        this.settings = new MMAWSettings();
        this.misc = new MMAWMisc();
        this.sizes = new MMAWSizes();
        
        // Create the stage handlers.
        var main = new MMAWUIMain(this);
        var processing = new MMAWUIProcessing(this);
        var results = new MMAWUIResults(this);
        var mainAndProcessing = new MMAWUIMainAndProcessing(this, main, processing);
        var resultsAndProcessing = new MMAWUIResultsAndProcessing(this, results, processing);
        
        // Assign the stage handlers.
        this.stages = {
            main: main,
            processing: processing,
            results: results,
            mainAndProcessing: mainAndProcessing,
            mainAndResults: resultsAndProcessing
        };
    };
    
    /// <summary>
    /// Initializes the UI after the initial page load.
    /// </summary>
    this.initialize = function() {
        // Initialize all stages.
        for (var i in this.stages)
        {
            this.stages[i].initialize();
        }
        
        // Initialize the sizes option.
        this.sizes.initializeOutputSizeOption();
        
        // Goto the main stage.
        this.gotoStage("main");
    };
    
    /// <summary>
    /// Sets a stage as the active one.
    /// </summary>
    this.gotoStage = function(stage) {
        this.currentStage = this.stages[stage];
        this.currentStage.activate();
    };
}
