"use strict";

/// <summary>
/// The results screen for Make Me a World.
/// </summary>
function MMAWUIResults(controller)
{
    /// <summary>
    /// The UI controller.
    /// </summary>
    this.controller = controller;

    /// <summary>
    /// Initializes the UI after the initial page load.
    /// </summary>
    this.initialize = function() {
        // Register UI handlers.
        $("#downloadResult").click(this._onDownloadResultClicked.bind(this));
        $("#renderAnother").click(this._onRenderAnotherClicked.bind(this));
        $("#shareOnTwitter").click(this._onShareOnTwitterClicked.bind(this));
    };
    
    /// <summary>
    /// Called when the stage is made the current stage.
    /// </summary>
    this.activate = function() {
        $("#welcomeMessage")[0].style.top = "100px";
        $("#header").show();
        $("#welcomeMessage").hide();
        $("#progress").hide();
        $("#remainingHolder").hide();
        $("#info").hide();
        $("#end").show();
        $("#watermark").hide();
        $("#renderAnother").show();
    };
    
    /// <summary>
    /// Event handler for when the user clicks the "Download Result" button.
    /// </summary>
    this._onDownloadResultClicked = function () {
        try
        {
            if ($.browser.msie)
                throw "msie";
            $("#canvas")[0].toBlob(function(blob) {
                saveAs(blob, "makemeaworld-" + $("#seed").text() + ".png");
            });
        }
        catch (ex)
        {
            $("#ieImgDownload").src = $("#canvas")[0].toDataURL("image/png");
            $("#ieDownload").modal();
        }
    };
    
    /// <summary>
    /// Event handler for when the user clicks the "Render Another" button.
    /// </summary>
    this._onRenderAnotherClicked = function () {
        this.controller.gotoStage("main");
        $("#seedSet").focus();
        $("#seedSet").select();
    };
    
    /// <summary>
    /// Event handler for when the user clicks the "Share on Twitter" button.
    /// </summary>
    this._onShareOnTwitterClicked = function () {
        var popupwindow = function (url, title, w, h) {
            var left = (screen.width / 2) - (w / 2);
            var top = (screen.height / 2) - (h / 2);
            return window.open(url, title, 'toolbar=no, location=no, directories=no, ' +
                               'status=no, menubar=no, scrollbars=no, resizable=no, ' +
                               'copyhistory=no, width=' + w + ', height=' + h + ', ' +
                               'top=' + top + ', left=' + left);
        }
        this.controller.settings.setHashFromSettings();
        popupwindow("https://twitter.com/intent/tweet?hashtags=Tychaia&related=hachque" +
                    "&text=I%20just%20made%20a%20world%20with%20Tychaia!&tw_p=tweetbutton" +
                    "&url=" + escape("http://makemeaworld.com/" + window.location.hash),
                    "tweet", 500, 350);
    };
}