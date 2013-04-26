"use strict";

/// <summary>
/// Defines a cell to be retrieved and rendered.
/// </summary>
function MMAWCell(x, y, z)
{
    this.x = x;
    this.y = y;
    this.z = z;
    this.retriever = null;
    this.renderer = null;
    this.onRetrieved = null;
    this.onRenderer = null;
    this.data = null;
}

/// <summary>
/// Handles processing and rendering the world.
/// </summary>
/// <param name="retriever">The retriever object.</param>
/// <param name="renderer">The renderer object.</param>
/// <param name="canvas">The canvas to render to.</param>
/// <param name="seed">The seed of the generation.</param>
function MMAWProcessor(retriever, renderer, canvas, seed)
{
    /// <summary>
    /// The object responsible for retrieving data (whether it's
    /// images or raw content) from the web server.
    /// </summary>
    this.retriever = retriever;
    this.retriever.processor = this;
    
    /// <summary>
    /// The object which handles rendering of content onto the
    /// final image.
    /// </summary>
    this.renderer = renderer;
    this.renderer.processor = this;
    
    /// <summary>
    /// The canvas that we will be rendering to.
    /// </summary>
    this.canvas = canvas;

    /// <summary>
    /// A list of cells that need to be retrieved and rendered.
    /// </summary>
    this.cells = [];

    this.cellsRetrieved = 0;
    this.cellsRendered = 0;
    this.cellsSkipped = 0;
    
    /// <summary>
    /// The minimum cell depth to render at (lower is further underground).
    /// </summary>
    this.minDepth = -1;
    
    /// <summary>
    /// The maximum cell depth to render at (higher is further above ground).
    /// </summary>
    this.maxDepth = 4;
    
    /// <summary>
    /// The seed of the generation.
    /// </summary>
    this.seed = seed;

    /// <summary>
    /// Callback when progress is made.
    /// </summary>
    this.onProgress = null;
    
    /// <summary>
    /// Callback when processing is finished.
    /// </summary>
    this.onFinish = null;
    
    /// <summary>
    /// The time when processing was started.
    /// </summary>
    this._start = null;
    
    /// <summary>
    /// Whether we are currently stopping.
    /// </summary>
    this._stopping = false;

    /// <summary>
    /// Starts the processing operation.
    /// </summary>
    this.startProcessing = function() {
        // Calculate a full list of cells and their
        // positions that we need to retrieve.
        this.cells = this._createCellArray(
            this.canvas.width,
            this.canvas.height,
            this.minDepth * this.renderer.getRenderIncrement(),
            this.maxDepth * this.renderer.getRenderIncrement());
    
        // Set up the canvas as required.
        var ctx = this.canvas.getContext('2d');
        if ($("#enableRenderDebugging")[0].checked)
            ctx.fillStyle = "#F00";
        else
            ctx.fillStyle = "#000";
        ctx.fillRect(0, 0, $("#canvas")[0].width, $("#canvas")[0].height);
    
        // Get the retriever and renderer to start processing.
        this._start = new Date().getTime();
        this.retriever.start(this);
        this.renderer.start(this);
    };
    
    this.calculateProgressInformation = function(zeroFill) {
        var penalty = 0;
        var millisCurrent = new Date().getTime();
        var millisDifference = millisCurrent - this._start;
        var estimatedMillisTotal = millisDifference * (this.cells.length / this.cellsRendered);
        var estimatedMillisRemaining = estimatedMillisTotal - millisDifference;
        if (estimatedMillisRemaining < 0) estimatedMillisRemaining = 0;
        var estimatedTimeRemaining = new Date(estimatedMillisRemaining);
        var estimatedTimeTotal = new Date(estimatedMillisTotal);
        var stringCurrent = new Date(millisDifference).getMinutes() + ":" + zeroFill(new Date(millisDifference).getSeconds(), 2);
        var stringRemaining = estimatedTimeRemaining.getMinutes() + ":" + zeroFill(estimatedTimeRemaining.getSeconds(), 2);
        var stringTotal = estimatedTimeTotal.getMinutes() + ":" + zeroFill(estimatedTimeTotal.getSeconds(), 2);
        $("#progress").html(((this.cellsRendered / this.cells.length) * 100).toFixed(4) + "% complete (" + (this.cells.length - this.cellsRendered) +
                            " tiles remaining)<br />" + this.cellsRetrieved + " retrieved, " +
                            this.cellsRendered + " rendered, " + this.cellsSkipped + " skipped");
        if (this._stopping) {
            $("#timeRemaining").text("Stopping...");
        } else if (isNaN(estimatedTimeRemaining.getMinutes())) {
            $("#timeRemaining").text("Estimating...");
        } else {
            $("#timeRemaining").text("Completion in " + stringRemaining + " minutes.");
        }
    };
    
    /// <summary>
    /// Stops the processing operation.
    /// </summary>
    this.stopProcessing = function() {
        // Stop the retriever and renderer.
        this.retriever.stop(this);
        this.renderer.stop(this);
        this._stopping = true;
    };
    
    /// <summary>
    /// Creates the cell array with empty cell objects.
    /// </summary>
    this._createCellArray = function(imageWidth, imageHeight, imageZ, imageDepth) {
        var cells = [];
        for (var z = imageZ; z < imageDepth; z += this.renderer.getRenderIncrement()) {
            for (var y = -imageHeight; y < imageHeight; y += this.renderer.getRenderIncrement()) {
                for (var x = -imageWidth; x < imageWidth; x += this.renderer.getRenderIncrement()) {
                    if (this.renderer.canSkip(x, y, z)) {
                        continue;
                    }
                    var cell = new MMAWCell(x, y, z);
                    this.retriever.associateWithCell(cell);
                    this.renderer.associateWithCell(cell);
                    cells.push(cell);
                }
            }
        }
        return cells;
    };
};
