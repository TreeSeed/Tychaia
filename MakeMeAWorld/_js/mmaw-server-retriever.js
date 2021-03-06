"use strict";

/// <summary>
/// A retriever that uses the server to pre-render the content.
/// </summary>
function MMAWServerRetriever() {
    
    /// <summary>
    /// The main processing object.
    /// </summary>
    this.processor = null;
    
    /// <summary>
    /// Whether we are actively retrieving.
    /// </summary>
    this._retrieving = false;
    
    /// <summary>
    /// An array of cells that we need to retrieve.
    /// </summary>
    this._cellsToRetrieve = [];
    
    /// <summary>
    /// Associates a cell to be rendered by this renderer.
    /// </summary>
    this.associateWithCell = function(cell) {
        this._cellsToRetrieve.push(cell);
    };
    
    /// <summary>
    /// Starts retrieving.
    /// </summary>
    this.start = function(processor) {
        this._retrieving = true;
        
        var retrieve = function(i)
        {
            if (i >= this._cellsToRetrieve.length)
                return;
        
            var cell = this._cellsToRetrieve[i];
            var img = new Image();
            cell.data = img;
            img.onload = function () {
                if (img.width == 1 && img.height == 1) {
                    this.processor.cellsSkipped += 1;
                    if (this.processor.onProgress != null)
                        this.processor.onProgress();
                } else {
                    this.processor.cellsRetrieved += 1;
                    if (this.processor.onProgress != null)
                        this.processor.onProgress();
                    if (cell.onRetrieved != null)
                        window.setTimeout(
                            function() { cell.onRetrieved(cell); },
                            1);
                }
                if (this._retrieving)
                    retrieve(i + 1);
            }.bind(this);
            img.onerror = function () {
                processor.failProcessing("Can't reach server");
            }.bind(this);
            img.src = "/api-v1/" + $("#outputLayer").val().substring(3) +
                      "/" + this.processor.seed +
                      "/" + cell.x +
                      "/" + cell.y +
                      "/" + cell.z +
                      "/" + this.processor.renderer.getRenderIncrement($("#outputLayer").val()) +
                      "/" + "get.png";
        }.bind(this);
        retrieve(0);
    };
    
    /// <summary>
    /// Stops retrieving.
    /// </summary>
    this.stop = function() {
        if (window.stop) {
            window.stop();
        } else if (document.execCommand) {
            document.execCommand("Stop", false);
        }
        this._retrieving = false;
    };
};

