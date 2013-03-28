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
    /// The render increment, or the total size of each cell that
    /// is rendered.
    /// </summary>
    this.getRenderIncrement = function() {
        // Our cell position calculations assume this
        // value (so we can optimize the algorithms).
        return 64;
    };
    
    /// <summary>
    /// Determines the 2D position at which a cell will be rendered
    /// based on it's location in 3D space.
    /// </summary>
    this._determineCellRenderPosition = function(x, y, z) {
        var rcx = 32 / 2 + this.processor.canvas.width / 2 - this.getRenderIncrement();
        var rcy = 32 / 2;
        var rx = rcx + ((x / 32 - y / 32) / 2.0 * 64);
        var ry = rcy + (x / 32 + y / 32) * 32 - (z / 32 - 0) * 32;
        return {
            x: rx,
            y: ry
        };
    };
    
    this._determinePixelRenderPosition = function(x, y, z) {
        return {
            x: 63 + (x - y),
            y: 49 + (x + y) - z - 64
        };
    };
    
    /// <summary>
    /// Whether a cell at the specified position even needs to be rendered.
    /// </summary>
    this.canSkip = function(x, y, z) {
        var position = this._determineCellRenderPosition(x, y, z);
        if (position.x < (-this.getRenderIncrement() * 3) || position.x > this.processor.canvas.width ||
            position.y < (-this.getRenderIncrement() * 3) || position.y > this.processor.canvas.height) {
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
    
    this._decodeDataArray = function(data) {
        if (!data.packed)
        {
            // Unpacked data doesn't need decoding.
            return data.data;
        }
        var start = 0;
        var end = this.getRenderIncrement() *
                  this.getRenderIncrement() *
                  this.getRenderIncrement();
        var extractCount = 0;
        var dataIndex = 0;
        var dataArray = [];
        for (var i = start; i < end; i++) {
            var value = data.data[dataIndex];
            if (value instanceof Array) {
                if (extractCount < value[0]) {
                    dataArray.push(value[1]);
                    extractCount++;
                } else {
                    extractCount = 0;
                    dataIndex++;
                }
            } else {
                dataArray.push(value);
                extractCount = 0;
                dataIndex++;
            }
        }
        return dataArray;
    };
    
    /// <summary>
    /// Event handler for when cell data has been retrieved
    /// from the server.
    /// </summary>
    this._onCellDataRetrieved = function(cell) {
        if (!this._rendering)
            return;
        if (cell.data.empty)
            return; // Don't need to render empty data.
        var dataArray = this._decodeDataArray(cell.data);
        var cellPosition = this._determineCellRenderPosition(cell.x, cell.y, cell.z);
        for (var z = 0; z < this.getRenderIncrement(); z++)
            for (var x = 0; x < this.getRenderIncrement(); x++)
                for (var y = 0; y < this.getRenderIncrement(); y++)
                {
                    var pixelPosition = this._determinePixelRenderPosition(x, y, z);
                    var value = dataArray[x + y * this.getRenderIncrement() + z * this.getRenderIncrement() * this.getRenderIncrement()];
                    if (cell.data.mappings[value] !== undefined &&
                        cell.data.mappings[value] !== null) {
                        var a = cell.data.mappings[value][0];
                        if (a > 0)
                        {
                            var r = cell.data.mappings[value][1];
                            var g = cell.data.mappings[value][2];
                            var b = cell.data.mappings[value][3];
                            this._context.fillStyle = "rgba("+r+","+g+","+b+","+(a/255)+")";
                            this._context.fillRect(cellPosition.x + pixelPosition.x, cellPosition.y + pixelPosition.y, 2, 1);
                        }
                    }
                }
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
