"use strict";

/// <summary>
/// Deals with reading and writing setting information.
/// </summary>
function MMAWSettings()
{
    /// <summary>
    /// Loads the settings from the URL hash fragment.
    /// </summary>
    this.loadSeedFromHash = function() {
        var split = window.location.hash.substring(1).split("&", 2);
        if (split.length == 1) {
            $("#seedSet").val(decodeURIComponent(split[0]));
            $("#outputLayer").val($("#defaultLayer").text());
        } else {
            $("#seedSet").val(decodeURIComponent(split[1]));
            $("#outputLayer").val(decodeURIComponent(split[0]));
        }
    };
    
    /// <summary>
    /// Saves the current settings into the URL hash fragment.
    /// </summary>
    this.setHashFromSettings = function() {
        if ($("#outputLayer").val() == $("#defaultLayer").text()) {
            window.location.hash = encodeURIComponent($("#seed").text());
        } else {
            window.location.hash = encodeURIComponent($("#outputLayer").val()) + "&" + encodeURIComponent($("#seed").text());
        }
    }
}
