"use strict";

/// <summary>
/// A render that uses the server to pre-render the content.
/// </summary>
function MMAWServerRenderer() {
    
    /// <summary>
    /// The main processing object.
    /// </summary>
    this.processor = null;
    
    /// <summary>
    /// Whether we are actively rendering.
    /// </summary>
    this._rendering = false;
    
    /// <summary>
    /// The canvas rendering context.
    /// </summary>
    this._context = null;
    
    /// <summary>
    /// The render increment, or the total size of each cell that
    /// is rendered.
    /// </summary>
    this.getRenderIncrement = function(layerName) {
        return 64;
    };
    
    /// <summary>
    /// Whether to show the info panel message referencing the
    /// server-side rendering.
    /// </summary>
    this.showInfoPanel = function() {
        return true;
    };
    
    /// <summary>
    /// Determines the 2D position at which a cell will be rendered
    /// based on it's location in 3D space.
    /// </summary>
    this._determineRenderPosition = function(x, y, z) {
        var rcx = 32 / 2 + this.processor.canvas.width / 2 - this.getRenderIncrement($("#outputLayer").val());
        var rcy = 32 / 2;
        var rx = rcx + ((x / 32 - y / 32) / 2.0 * 64);
        var ry = rcy + (x / 32 + y / 32) * 32 - (z / 32 - 0) * 32;
        return {
            x: rx,
            y: ry
        };
    };
    
    /// <summary>
    /// Whether a cell at the specified position even needs to be rendered.
    /// </summary>
    this.canSkip = function(x, y, z) {
        var position = this._determineRenderPosition(x, y, z);
        if (position.x < (-this.getRenderIncrement($("#outputLayer").val()) * 3) || position.x > this.processor.canvas.width ||
            position.y < (-this.getRenderIncrement($("#outputLayer").val()) * 3) || position.y > this.processor.canvas.height) {
            return true;
        }
        return false;
    };
    
    /// <summary>
    /// Associates a cell to be rendered by this renderer.
    /// </summary>
    this.associateWithCell = function(cell) {
        cell.onRetrieved = this._onCellDataRetrieved.bind(this);
    };
    
    /// <summary>
    /// Event handler for when cell data has been retrieved
    /// from the server.
    /// </summary>
    this._onCellDataRetrieved = function(cell) {
        if (!this._rendering)
            return;
        var position = this._determineRenderPosition(cell.x, cell.y, cell.z);
        this._context.drawImage(cell.data, position.x, position.y);
        this.processor.cellsRendered += 1;
        if (this.processor.onProgress != null)
            this.processor.onProgress();
        if (this.processor.onFinish != null &&
            this.processor.cells.length == this.processor.cellsRendered)
            this.processor.onFinish();
    };
    
    /// <summary>
    /// Starts rendering.
    /// </summary>
    this.start = function() {
        this._rendering = true;
        this._context = this.processor.canvas.getContext("2d");
    };
    
    /// <summary>
    /// Stops rendering.
    /// </summary>
    this.stop = function() {
        this._rendering = false;
    };
};

