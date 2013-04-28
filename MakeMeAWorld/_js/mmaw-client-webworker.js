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
    /// The render increment we're using.
    /// </summary>
    this._renderIncrement = 64;
    
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
    /// Sets the render increment we are using.
    /// </summary>
    this.setRenderIncrement = function(renderIncrement) {
        this._renderIncrement = renderIncrement;
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
        var end = this._renderIncrement *
                  this._renderIncrement *
                  this._renderIncrement;
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
    /// Draws a pixel to the image data, taking into
    /// account the existing pixel data.
    /// </summary>
    this.drawPixel = function(imageData, idx, color) {
        if (color[0] == 0) {
            return;
        }
        imageData.data[idx + 0] = color[1];
        imageData.data[idx + 1] = color[2];
        imageData.data[idx + 2] = color[3];
        imageData.data[idx + 3] = color[0];
    };
    
    /// <summary>
    /// Processes an image.
    /// </summary>
    this.process = function(cell, cellPosition, callback, token) {
        if (typeof cell.data === "string") {
            cell.data = JSON.parse(cell.data);
        }
        var dataArray = this._decodeDataArray(cell.data);
        var max = this._renderIncrement;
        if (dataArray === undefined) {
            var err = "dataArray still undefined even after attempting fallback JSON parse";
            if (callback == null) {
                this._webWorkerEnvironment.postMessage({token: token, data: {message: err}});
            } else {
                callback({message: err});
            }
            return;
        }
        for (var z = 0; z < max; z++)
            for (var x = 0; x < max; x++)
                for (var y = 0; y < max; y++)
                {
                    var pixelPosition = this._determinePixelRenderPosition(x, y, z);
                    var value = dataArray[x + y * this._renderIncrement + z * this._renderIncrement * this._renderIncrement];
                    
                    var ax = cellPosition.x + pixelPosition.x;
                    var ay = cellPosition.y + pixelPosition.y;
                    if (cell.data.mappings[value] !== undefined &&
                        cell.data.mappings[value] !== null) {
                        if (!(ax < 0 || ay < 0 || ax >= this._imageData.width || ay >= this._imageData.height)) {
                            this.drawPixel(this._imageData, (ax + ay * this._imageData.width) * 4, cell.data.mappings[value]);
                        }
                        if (!(ax + 1 < 0 || ay < 0 || ax + 1 >= this._imageData.width || ay >= this._imageData.height)) {
                            this.drawPixel(this._imageData, (ax + 1 + ay * this._imageData.width) * 4, cell.data.mappings[value]);
                        }
                    }
                }
        if (callback == null) {
            this._webWorkerEnvironment.postMessage({token: token, data: {imageData: this._imageData}});
        } else {
            callback({imageData: this._imageData});
        }
    };
    
};

// Add the message handler.
var handler = new MMAWClientWebWorker();
handler.setWebWorkerEnvironment(this);
self.addEventListener('message', function(e) {
    handler.onMessageRecieved(e.data);
}, false);