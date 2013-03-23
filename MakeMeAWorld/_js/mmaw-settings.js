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
        var split = window.location.hash.substring(1).split("#", 2);
        if (split.length == 1) {
            $("#seedSet")[0].value = decodeURIComponent(split[0]);
            $("#outputFormat")[0].value = "3D-Game World";
        } else {
            $("#seedSet")[0].value = decodeURIComponent(split[1]);
            $("#outputFormat")[0].value = decodeURIComponent(split[0]);
        }
    };
    
    /// <summary>
    /// Saves the current settings into the URL hash fragment.
    /// </summary>
    this.setHashFromSettings = function() {
        if ($("#outputFormat")[0].value == "3D-Game World") {
            window.location.hash = encodeURIComponent($("#seed").text());
        } else {
            window.location.hash = encodeURIComponent($("#outputFormat")[0].value) + "#" + encodeURIComponent($("#seed").text());
        }
    }
}
