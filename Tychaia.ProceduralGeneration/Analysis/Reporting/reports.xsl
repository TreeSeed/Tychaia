<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:i="http://www.w3.org/2001/XMLSchema-instance"
                xmlns:z="http://schemas.microsoft.com/2003/10/Serialization/"
                xmlns:t="http://schemas.datacontract.org/2004/07/Tychaia.ProceduralGeneration.Analysis.Reporting">
                
    <xsl:output indent="yes" method="html" />
    
    <xsl:template match="/t:analysis">
        <html>
            <head>
                <title>Tychaia Analysis Reports</title>
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <link href="bootstrap/css/bootstrap.min.css" rel="stylesheet" />
                <link href="bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" />
                <link href="bootstrap/css/prettify.css" rel="stylesheet" />
                <link href="bootstrap/css/docs.css" rel="stylesheet" />
                <style type="text/css">
                <![CDATA[
                a.report-code-highlight, a.report-code-highlight:hover { margin-left: 2px; margin-right: 2px; text-decoration: none; cursor: default; }
                a.report-code-highlight *, .linenums div.tooltip { text-shadow: none; }   
                ]]>
                </style>
            </head>
            <body data-spy="scroll" data-target=".bs-docs-sidebar">
                <div class="container-fluid">
                    <div class="row-fluid">
                        <div class="span2 bs-docs-sidebar">
                            <ul class="nav nav-list bs-docs-sidenav">
                                <xsl:for-each select="/t:analysis/t:layers/t:layer">
                                    <li>
                                        <a>
                                            <xsl:attribute name="href">
                                                <xsl:text>#</xsl:text>
                                                <xsl:value-of select="t:hash" />
                                            </xsl:attribute>
                                            <i class="icon-chevron-right"></i>
                                            <xsl:value-of select="t:name" />
                                        </a>
                                    </li>
                                </xsl:for-each>
                            </ul>
                        </div>
                        <div class="span10">
                            <div class="page-header">
                                <h1>Tychaia Analysis Reports</h1>
                            </div>
                            <xsl:apply-templates />
                        </div>
                    </div>
                </div>
                <script src="bootstrap/js/jquery-1.9.1.min.js"></script>
                <script src="bootstrap/js/bootstrap.min.js"></script>
                <script src="bootstrap/js/prettify.js"></script>
                <script type="text/javascript">
                <![CDATA[
                    $(function () {
                        $("[data-toggle='tooltip']").tooltip();
                        $("a.report-code-highlight[data-uid!='']").mouseenter(function() {
                            $("a.report-code-highlight:not([data-uid-refs*='[" + $(this).data("uid") + "]'])", $(this).parents("pre")).stop();
                            $("a.report-code-highlight:not([data-uid-refs*='[" + $(this).data("uid") + "]'])", $(this).parents("pre")).fadeTo('fast', 0.1);
                        });
                        $("a.report-code-highlight[data-uid!='']").mouseleave(function() {
                            $("a.report-code-highlight:not([data-uid-refs*='[" + $(this).data("uid") + "]'])", $(this).parents("pre")).stop();
                            $("a.report-code-highlight:not([data-uid-refs*='[" + $(this).data("uid") + "]'])", $(this).parents("pre")).fadeTo('fast', 1);
                        });
                        var $window = $(window)
                        $(document).ready(function () {
                            $('.bs-docs-sidenav').affix();
                        });
                    });
                    prettyPrint();
                ]]>
                </script>
            </body>
        </html>
    </xsl:template>
    
    <xsl:template match="/t:analysis/t:layers/t:layer">
        <h2>
            <xsl:attribute name="id">
                <xsl:value-of select="t:hash" />
            </xsl:attribute>
            <xsl:value-of select="t:name" />
        </h2>
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="/t:analysis/t:layers/t:layer/t:name"></xsl:template>
    <xsl:template match="/t:analysis/t:layers/t:layer/t:code"></xsl:template>
    <xsl:template match="/t:analysis/t:layers/t:layer/t:hash"></xsl:template>
    
    <xsl:template match="/t:analysis/t:layers/t:layer/t:reports">
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="/t:analysis/t:layers/t:layer/t:reports/t:report">
        <h3>
            <xsl:value-of select="t:name" />
        </h3>
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="/t:analysis/t:layers/t:layer/t:reports/t:report/t:name" />
    <xsl:template match="/t:analysis/t:layers/t:layer/t:reports/t:report/t:hash" />
    
    <xsl:template match="/t:analysis/t:layers/t:layer/t:reports/t:report/t:issues">
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="/t:analysis/t:layers/t:layer/t:reports/t:report/t:issues/t:issue">
        <h4>
            <xsl:value-of select="t:id" />
            <xsl:text>: </xsl:text>
            <xsl:value-of select="t:name" />
        </h4>
        <p>
            <xsl:value-of select="t:description" />
        </p>
        <pre class="prettyprint linenums">
            <xsl:call-template name="format-locations">
                <xsl:with-param name="text" select="../../../../t:code" />
                <xsl:with-param name="locations" select="t:locations" />
            </xsl:call-template>
        </pre>
    </xsl:template>
    
    <xsl:template match="t:locations/t:location[@i:type='replace']">
        <xsl:param name="previous" />
        <xsl:param name="current" />
        <span style="background-color: red;">
            <xsl:value-of select="." />
        </span>
    </xsl:template>
    
    <xsl:template match="t:locations/t:location[@i:type='highlight']">
        <xsl:param name="previous" />
        <xsl:param name="current" />
        <a data-toggle="tooltip" class="report-code-highlight">
            <xsl:attribute name="data-uid">
                <xsl:value-of select="$current/t:uniqueid" />
            </xsl:attribute>
            <xsl:attribute name="data-uid-refs">
                <xsl:value-of select="$current/t:uniqueidrefs" />
            </xsl:attribute>
            <xsl:attribute name="title">
                <xsl:value-of select="$current/t:message" />
            </xsl:attribute>
            <xsl:attribute name="style">
                <xsl:text>background-color: rgb(255, </xsl:text>
                <xsl:value-of select="255 - round(($current/t:importance div 100) * 200)" />
                <xsl:text>, </xsl:text>
                <xsl:value-of select="200 - round(($current/t:importance div 100) * 200)" />
                <xsl:text>);</xsl:text>
            </xsl:attribute>
            <xsl:value-of select="$previous" />
        </a>
    </xsl:template>
    
    <!-- Formats the locations in the code as needed -->
    <xsl:template name="format-locations">
        <xsl:param name="text"/>
        <xsl:param name="locations"/>

        <xsl:variable name="result">
            <xsl:if test="$locations/t:*[1]/t:start = 1">
                <xsl:message terminate="no">Start points should be greater than 0 (character indexing starts at 1).</xsl:message>
            </xsl:if>
            <xsl:if test="$locations/t:*[1]/t:end &lt; $locations/t:*[1]/t:start">
                <xsl:message terminate="yes">End position of location must be after start.</xsl:message>
            </xsl:if>
            <xsl:value-of select="substring($text, 1, $locations/t:*[1]/t:start)"/>
            <xsl:apply-templates select="$locations/t:*[1]">
                <xsl:with-param name="previous" select="substring($text, $locations/t:*[1]/t:start + 1, $locations/t:*[1]/t:end - $locations/t:*[1]/t:start)" />
                <xsl:with-param name="current" select="$locations/t:*[1]" />
            </xsl:apply-templates>
            <xsl:for-each select="$locations/t:*[position() &gt; 1]">
                <xsl:if test="t:end &lt; t:start">
                    <xsl:message terminate="yes">End position of location must be after start.</xsl:message>
                </xsl:if>
                <xsl:value-of select="substring($text, preceding-sibling::t:*[1]/t:end + 1, t:start - preceding-sibling::t:*[1]/t:end)"/>
                <xsl:apply-templates select="$locations/t:*[1]">
                    <xsl:with-param name="previous" select="substring($text, t:start + 1, t:end - t:start)" />
                    <xsl:with-param name="current" select="." />
                </xsl:apply-templates>
            </xsl:for-each>
            <xsl:value-of select="substring($text, $locations/t:*[last()]/t:end + 1)"/>
        </xsl:variable>
        <xsl:copy-of select="$result"/>
    </xsl:template>
    
</xsl:stylesheet>