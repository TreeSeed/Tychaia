"use strict";

/// <summary>
/// A retriever that uses the server to get the raw data.
/// </summary>
function MMAWClientRetriever() {
    
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
    this.start = function() {
        this._retrieving = true;
        
        var retrieve = function(i)
        {
            if (i >= this._cellsToRetrieve.length)
                return;
        
            var cell = this._cellsToRetrieve[i];
            $.get("raw/map.json?x=" + cell.x + "&y=" + cell.y + "&z=" + cell.z +
                  "&size=" + this.processor.renderer.getRenderIncrement() +
                  "&seed=" + this.processor.seed + "&layer=" +
                  $("#outputLayer")[0].value.substring(3) +
                  "&packed=" + ($("#transmitPackedData")[0].checked ? "true" : "false"), function(data) {
                    cell.data = data;
                    this.processor.cellsRetrieved += 1;
                    if (this.processor.onProgress != null)
                        this.processor.onProgress();
                    if (cell.onRetrieved != null)
                        window.setTimeout(
                            function() { cell.onRetrieved(cell); },
                            1);
//                    if (this._retrieving)
//                        retrieve(i + 1);
            }.bind(this));
        }.bind(this);
//        retrieve(0);

        for (var i = 0; i < this._cellsToRetrieve.length; i++)
            retrieve(i);
    };
    
    /// <summary>
    /// Stops retrieving.
    /// </summary>
    this.stop = function() {
        this._retrieving = false;
    };
};

