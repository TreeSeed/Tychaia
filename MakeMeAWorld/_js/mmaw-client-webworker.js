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
    /// Handler for when the web worker recieves a message.
    /// </summary>
    this.onMessageRecieved = function(data) {
        if (this[data.func] != null) {
            this[data.func].apply(this, Array.prototype.slice.call(data.arguments, 1));
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
    /// Processes an image.
    /// </summary>
    this.process = function(cell, imageData, callback) {
        var dataArray = this._decodeDataArray(cell.data);
        for (var z = 0; z < this.getRenderIncrement(); z++)
            for (var x = 0; x < this.getRenderIncrement(); x++)
                for (var y = 0; y < this.getRenderIncrement(); y++)
                {
                    var pixelPosition = this._determinePixelRenderPosition(x, y, z);
                    var value = dataArray[x + y * this.getRenderIncrement() + z * this.getRenderIncrement() * this.getRenderIncrement()];
                    if (cell.data.mappings[value] !== undefined &&
                        cell.data.mappings[value] !== null) {
                        var idx = (pixelPosition.x + pixelPosition.y * imageData.width) * 4;
                        imageData.data[idx + 0] = a;
                        imageData.data[idx + 1] = r;
                        imageData.data[idx + 2] = g;
                        imageData.data[idx + 3] = b;
                    }
                }
        if (callback == null) {
            this._webWorkerEnvironment.postMessage({imageData: imageData});
        } else {
            callback(imageData);
        }
    };
    
};

// Add the message handler.
var handler = new MMAWClientWebWorker();
handler.setWebWorkerEnvironment(this);
self.addEventListener('message', function(e) {
    handler.onMessageRecieved(e.data);
}, false);