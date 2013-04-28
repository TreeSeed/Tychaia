"use strict";

/// <summary>
/// A renderer that draws directly onto the local canvas using
/// just the raw data provided by the server.
/// </summary>
function MMAWClientRenderer() {
    
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
    /// The web worker if available.
    /// </summary>
    this._webWorker = null;
    
    /// <summary>
    /// Whether we are actually using web workers.
    /// </summary>
    this._webWorkerIsWebWorker = false;
    
    /// <summary>
    /// The web worker callback handlers.
    /// </summary>
    this._webWorkerHandlers = [];
    
    /// <summary>
    /// The web worker token for callbacks.
    /// </summary>
    this._webWorkerToken = 0;
    
    /// <summary>
    /// If the image data has been passed to
    /// the web worker instance.
    /// </summary>
    this._webWorkerHasImageData = false;
    
    /// <summary>
    /// The queue for non-web worker mode (since web
    /// workers implicitly give us a queue through
    /// the messaging system).
    /// </summary>
    this._nonWebWorkerQueue = [];
    
    /// <summary>
    /// The render increment, or the total size of each cell that
    /// is rendered.
    /// </summary>
    this.getRenderIncrement = function(layerName) {
        // Our cell position calculations assume this
        // value (so we can optimize the algorithms).
        return 64;
    };
    
    /// <summary>
    /// Whether to show the info panel message referencing the
    /// server-side rendering.
    /// </summary>
    this.showInfoPanel = function() {
        return false;
    };
    
    /// <summary>
    /// Determines the 2D position at which a cell will be rendered
    /// based on it's location in 3D space.
    /// </summary>
    this._determineCellRenderPosition = function(x, y, z) {
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
        var position = this._determineCellRenderPosition(x, y, z);
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
        if (cell.data.empty) {
            this.processor.cellsSkipped += 1;
            if (this.processor.onProgress != null)
                this.processor.onProgress();
            if (this.processor.onFinish != null &&
                this.processor.cells.length == this.processor.cellsRendered + this.processor.cellsSkipped)
                this.processor.onFinish();
            return; // Don't need to render empty data.
        }
        
        // Set up the image data and finish handler.
        var cellPosition = this._determineCellRenderPosition(cell.x, cell.y, cell.z);
        var finished = function(imageData) {
            this._context.putImageData(imageData, 0, 0);
            this.processor.cellsRendered += 1;
            if (this.processor.onProgress != null)
                this.processor.onProgress();
            if (this.processor.onFinish != null &&
                this.processor.cells.length == this.processor.cellsRendered + this.processor.cellsSkipped)
                this.processor.onFinish();
        }.bind(this);
        
        // Define our callback.
        var handler = function(data) {
            if (data.message != null) {
                console.log(data.message);
            }
            if (data.imageData != null) {
                finished(data.imageData);
            }
        };
        
        // If we have web worker support, use that.
        if (this._webWorkerIsWebWorker) {
            // Clear onRetrieved handler so we can pass it
            // to the web worker.
            var oldRetrieved = cell.onRetrieved;
            cell.onRetrieved = null;
            
            // Set the up the token and callback.
            var token = this._webWorkerToken++;
            this._webWorkerHandlers[token] = handler;
            if (!this._webWorkerHasImageData) {
                this._webWorker.postMessage({func: "setImageData", arguments: [this._context.getImageData(0, 0, this.processor.canvas.width, this.processor.canvas.height)]});
                this._webWorker.postMessage({func: "setRenderIncrement", arguments: [this.getRenderIncrement($("#outputLayer").val())]});
                this._webWorkerHasImageData = true;
            }
            this._webWorker.postMessage({func: "process", arguments: [cell, cellPosition, null, token]});
            
            // Set the onRetrieved handler back.
            cell.onRetrieved = oldRetrieved;
        } else {
            if (!this._webWorkerHasImageData) {
                this._webWorker.setImageData(this._context.getImageData(0, 0, this.processor.canvas.width, this.processor.canvas.height));
                this._webWorker.setRenderIncrement(this.getRenderIncrement($("#outputLayer").val()));
            }
            
            this._nonWebWorkerQueue.push(function() {
                this._webWorker.process(cell, cellPosition, handler, null);
            }.bind(this));
        }
    };
    
    this._processNonWebWorkerQueue = function() {
        if (this._nonWebWorkerQueue.length > 0) {
            this._nonWebWorkerQueue.shift()();
        }
        if (this._rendering) {
            window.setTimeout(this._processNonWebWorkerQueue.bind(this), 100);
        }
    };
    
    /// <summary>
    /// The web worker handler.
    /// </summary>
    this._onWebWorkerMessage = function(event) {
        this._webWorkerHandlers[event.data.token](event.data.data);
    };
    
    /// <summary>
    /// Starts rendering.
    /// </summary>
    this.start = function() {
        this._rendering = true;
        this._context = this.processor.canvas.getContext("2d");
        
        // Try to use web workers.
        try {
            this._webWorker = new Worker("/_js/mmaw-client-webworker.js");
            this._webWorker.onmessage = this._onWebWorkerMessage.bind(this);
            this._webWorkerIsWebWorker = true;
        } catch (ex) {
            this._webWorker = new MMAWClientWebWorker();
            this._webWorkerIsWebWorker = false;
            window.setTimeout(this._processNonWebWorkerQueue.bind(this), 100);
            if (console && console.log) {
                console.log("Unable to use Web Workers for rendering.");
                console.log(ex);
            }
        }
    };
    
    /// <summary>
    /// Stops rendering.
    /// </summary>
    this.stop = function() {
        this._rendering = false;
        if (this._webWorkerIsWebWorker) {
            this._webWorker.terminate();
        }
    };
};

