"use strict";

/// <summary>
/// A background worker to render the world on a
/// seperate thread, allowing highly parallized
/// rendering of the world on the client computer.
/// </summary>
function MMAWClientWebWorker() {
    
    /// <summary>
    /// The web worker environment.
    /// </summary>
    this._webWorkerEnvironment = null;
    
    /// <summary>
    /// The image data to render onto.
    /// </summary>
    this._imageData = null;
    
    /// <summary>
    /// Handler for when the web worker recieves a message.
    /// </summary>
    this.onMessageRecieved = function(data) {
        if (this[data.func] != null) {
            this[data.func].bind(this).apply(this, data.arguments);
        }
    };
    
    /// <summary>
    /// Sets this to be running in a web worker environment.  This causes
    /// it to post a message on completion, rather than running a callback.
    /// </summary>
    this.setWebWorkerEnvironment = function(env) {
        this._webWorkerEnvironment = env;
    };
    
    /// <summary>
    /// Sets the image data we're going to render images onto.
    /// </summary>
    this.setImageData = function(imageData) {
        this._imageData = imageData;
    };
    
    /// <summary>
    /// The render increment, or the total size of each cell that
    /// is rendered.
    /// </summary>
    this.getRenderIncrement = function() {
        // Our cell position calculations assume this
        // value (so we can optimize the algorithms).
        return 64;
    };
    
    this._determinePixelRenderPosition = function(x, y, z) {
        return {
            x: 63 + (x - y),
            y: 49 + (x + y) - z - 64
        };
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
    /// Processes an image.
    /// </summary>
    this.process = function(cell, cellPosition, callback, token) {
        var dataArray = this._decodeDataArray(cell.data);
        for (var z = 0; z < this.getRenderIncrement(); z++)
            for (var x = 0; x < this.getRenderIncrement(); x++)
                for (var y = 0; y < this.getRenderIncrement(); y++)
                {
                    var pixelPosition = this._determinePixelRenderPosition(x, y, z);
                    var value = dataArray[x + y * this.getRenderIncrement() + z * this.getRenderIncrement() * this.getRenderIncrement()];
                    
                    var ax = cellPosition.x + pixelPosition.x;
                    var ay = cellPosition.y + pixelPosition.y;
                    if (cell.data.mappings[value] !== undefined &&
                        cell.data.mappings[value] !== null) {
                        var idx;
                        if (!(ax < 0 || ay < 0 || ax >= this._imageData.width || ay >= this._imageData.height)) {
                            idx = (ax + ay * this._imageData.width) * 4;
                            this._imageData.data[idx + 0] = cell.data.mappings[value][1];
                            this._imageData.data[idx + 1] = cell.data.mappings[value][2];
                            this._imageData.data[idx + 2] = cell.data.mappings[value][3];
                            this._imageData.data[idx + 3] = cell.data.mappings[value][0];
                        }
                        if (!(ax + 1 < 0 || ay < 0 || ax + 1 >= this._imageData.width || ay >= this._imageData.height)) {
                            idx = (ax + 1 + ay * this._imageData.width) * 4;
                            this._imageData.data[idx + 0] = cell.data.mappings[value][1];
                            this._imageData.data[idx + 1] = cell.data.mappings[value][2];
                            this._imageData.data[idx + 2] = cell.data.mappings[value][3];
                            this._imageData.data[idx + 3] = cell.data.mappings[value][0];
                        }
                    }
                }
        if (callback == null) {
            this._webWorkerEnvironment.postMessage({token: token, data: {imageData: this._imageData}});
        } else {
            callback(this._imageData);
        }
    };
    
};

// Add the message handler.
var handler = new MMAWClientWebWorker();
handler.setWebWorkerEnvironment(this);
self.addEventListener('message', function(e) {
    handler.onMessageRecieved(e.data);
}, false);