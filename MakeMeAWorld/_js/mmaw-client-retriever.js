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
    
    this._cellsRetrieved = [];
    this._cellsRetrievedCount = 0;
    
    /// <summary>
    /// Starts retrieving.
    /// </summary>
    this.start = function(processor) {
        this._retrieving = true;
        this._cellsRetrieved = [];
        for (var i = 0; i < this._cellsToRetrieve.length; i++) {
            this._cellsRetrieved[i] = null;
        }
        
        for (var i = 0; i < this._cellsToRetrieve.length; i++) {
            (function (cell, ii) {
                $.get("raw/map.json?x=" + cell.x + "&y=" + cell.y + "&z=" + cell.z +
                      "&size=" + this.processor.renderer.getRenderIncrement($("#outputLayer").val()) +
                      "&seed=" + this.processor.seed + "&layer=" +
                      $("#outputLayer").val().substring(3) +
                      "&packed=" + (($("#transmitPackedData") && $("#transmitPackedData").is(':checked')) ? "true" : "false"), function(data) {
                        if (!this._retrieving) {
                            return;
                        }
                        cell.data = data;
                        this.processor.cellsRetrieved += 1;
                        if (this.processor.onProgress != null)
                            this.processor.onProgress();
                        this._cellsRetrieved[ii] = function() {
                            if (cell.onRetrieved != null)
                                cell.onRetrieved(cell);
                        };
                        if (ii == this._cellsRetrievedCount) {
                            var ix = ii;
                            while (this._cellsRetrieved[ix] != null) {
                                if (!this._retrieving) {
                                    return;
                                }
                                this._cellsRetrieved[ix]();
                                this._cellsRetrievedCount++;
                                ix++;
                            }
                        }
                }.bind(this)).fail(function() {
                    processor.failProcessing("Can't reach server");
                }.bind(this));
            }.bind(this))(this._cellsToRetrieve[i], i);
        }
    };
    
    /// <summary>
    /// Stops retrieving.
    /// </summary>
    this.stop = function() {
        this._retrieving = false;
    };
};

