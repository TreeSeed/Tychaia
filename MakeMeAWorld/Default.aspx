<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MakeMeAWorld.Default" %>
<!DOCTYPE html> 
<html lang="en">
    <head>
        <title>Make me a world with Tychaia!</title>
        <link rel="stylesheet" href="_css/bootstrap.min.css" />
        <link rel="stylesheet" href="_css/browser.css" />
        <link rel="stylesheet" href="_css/platformer.css" />
        <meta http-equiv="X-UA-Compatible" content="IE=edge" />
        <script src="_js/jquery-1.8.3.min.js" type="text/javascript"></script>
        <script src="_js/bootstrap.min.js" type="text/javascript"></script>
        <script src="_js/BlobBuilder.js" type="text/javascript"></script>
        <script src="_js/canvas-toBlob.js" type="text/javascript"></script>
        <script src="_js/FileSaver.js" type="text/javascript"></script>
        <script src="_js/logic.js" type="text/javascript"></script>
        <link rel="stylesheet" href="_css/main.css" />
    </head>
    <body style="margin: 0px; padding: 0px;">
        <canvas id="canvas" width="1920" height="1080"></canvas>

        <div id="container">
            <h1 id="header">Make me a world with Tychaia!</h1>
            <div id="progress" style="display: none; margin-top: 30px;">0% complete (0 tiles remaining)<br />0 rendered, 0 skipped</div>
            <div id="remainingHolder" style="display: none; font-weight: bold;">
                <input id="stopEarly" type="button" class="btn btn-mini btn-danger" value="Stop" />
                <div id="timeRemaining" style="font-weight: bold;">Completion in -:-- minutes.</div>
            </div>
            <div id="info" style="display: none;">
                The map is now being rendered.  Only cells which contain content
                are rendered by the server, and thus as the underground is rendered,
                it may appear as though chunks of the map are missing.
            </div>
            <div id="end" style="display: none;">
                <div id="endMessage">No result information.</div>
                <input type="button" class="btn btn-success" id="downloadResult" value="Download 1080p PNG" /><br />
                <input type="button" class="btn" id="shareOnTwitter" value="Share on Twitter" /><br />
                <input type="button" class="btn" id="renderAnother" value="Render Another" />
            </div>
            <div id="messageContainer">
                <div id="welcomeMessage">
                    <div id="welcomeText">
                        <p>
                            <a href="http://tychaia.com">Tychaia</a> is an infinite, procedurally generated RPG set in a medieval world.  It's
                        set with an isometric view and a play style similar to Diablo, but with a twist: when you die, the entire world is
                        regenerated from scratch and you get to play an entirely new game.
                        </p>
                        <p>
                            A large portion of the development of Tychaia is focused on the procedural generation of a world in an almost
                            infinite space.  To give an idea of the potential size of a world, the maximum size of a world is 1.8&times;10<sup>18</sup>
                            times larger than what is rendered here.  Each pixel on the render translates to around about 1cm on your screen when
                            playing in-game.  For all intents and purposes, the game provides limitless exploration.
                        </p>
                        <p>
                            On this site you can generate a 1920x1080 wallpaper render using the current Tychaia world generator.  You can enter
                            a seed below and our server will generate the cell renders, which are then formed into a complete image by the
                            web browser.
                        </p>
                    </div>
                    <form id="newRender" class="form-inline" style="margin-bottom: 0px; margin-top: 13px;">
                        <div class="input-append">
                            <input type="text" id="seedSet" value="" style="width: 375px;" placeholder="Enter seed..." />
                            <div class="btn-group">
                                <input type="button" class="btn" id="randomize" value="Randomize" />
                            </div>
                        </div>
                        <div class="btn-group">
                            <button class="btn btn-primary" id="loadButton">Make me a world!</button>
                            <button class="btn btn-primary dropdown-toggle" data-toggle="dropdown" id="showAdvanced">
                                <span class="caret"></span>
                            </button>
                        </div>
                    </form>
                    <div id="advancedOptions" style="display: none;" class="well">
                        <form class="form-horizontal">
                            <div class="control-group">
                                <label class="control-label" for="outputFormat">Output Layer</label>
                                <div class="controls">
                                    <select id="outputFormat">
                                        <%=m_HtmlLayerOptions%>
                                    </select>
                                </div>
                            </div>
                            <div class="control-group">
                                <div class="controls">
                                    <label class="checkbox">
                                        <input type="checkbox" id="enableRenderDebugging" /> Enable render debugging
                                    </label>
                                </div>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
        <div id="watermark">Powered by <a href="http://tychaia.com/">Tychaia</a>.  Image is 1920x1080 when saved.  Seed is <span id="seed">0</span>.</div>
        <div id="ieDownload" class="modal hide fade">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                <h3>Right-click and save the image</h3>
            </div>
            <div class="modal-body">
                <img id="ieImgDownload" src="" alt="Image Download" />
            </div>
        </div>
    </body>
</html>
