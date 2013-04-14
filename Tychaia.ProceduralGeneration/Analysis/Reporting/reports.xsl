<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output indent="yes" method="html" />
    
    <xsl:template match="/analysis">
        <html>
            <head>
                <title>Tychaia Analysis Reports</title>
                <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                <link href="bootstrap/css/bootstrap.min.css" rel="stylesheet" />
                <link href="bootstrap/css/bootstrap-responsive.min.css" rel="stylesheet" />
                <link href="bootstrap/css/prettify.css" rel="stylesheet" />
                <style type="text/css">
                <![CDATA[
                a.report-code-highlight, a.report-code-highlight:hover { text-decoration: none; cursor: default; }
                a.report-code-highlight *, .linenums div.tooltip { text-shadow: none; }
                ]]>
                </style>
            </head>
            <body>
                <div class="container">
                    <xsl:apply-templates />
                </div>
                <script src="bootstrap/js/jquery-1.9.1.min.js"></script>
                <script src="bootstrap/js/bootstrap.min.js"></script>
                <script src="bootstrap/js/prettify.js"></script>
                <script type="text/javascript">
                <![CDATA[
                    $(function () {
                        $("[data-toggle='tooltip']").tooltip();
                    });
                    prettyPrint();
                ]]>
                </script>
            </body>
        </html>
    </xsl:template>
    
    <xsl:template match="/analysis/layers">
        <!-- Do nothing with layers as we reference them from the reports -->
    </xsl:template>
    
    <xsl:template match="/analysis/report">
        <h3>
            <xsl:value-of select="@name" />
        </h3>
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="/analysis/report/issues">
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="/analysis/report/issues/issue">
        <h4>
            <span style="text-decoration: underline;"><xsl:value-of select="id" />: <xsl:value-of select="name" /></span>
        </h4>
        <p>
            <xsl:value-of select="description" />
        </p>
        <pre class="prettyprint linenums">
            <xsl:call-template name="format-locations">
                <xsl:with-param name="text" select="/analysis/layers/layer[@id=current()/layer/@id]" />
                <xsl:with-param name="locations" select="locations" />
            </xsl:call-template>
        </pre>
    </xsl:template>
    
    <xsl:template match="locations/replace">
        <xsl:param name="previous" />
        <xsl:param name="current" />
        <span style="background-color: red;">
            <xsl:value-of select="." />
        </span>
    </xsl:template>
    
    <xsl:template match="locations/highlight">
        <xsl:param name="previous" />
        <xsl:param name="current" />
        <a data-toggle="tooltip" class="report-code-highlight">
            <xsl:attribute name="title">
                <xsl:value-of select="$current/message" />
            </xsl:attribute>
            <xsl:attribute name="style">
                <xsl:text>background-color: rgb(255, </xsl:text>
                <xsl:value-of select="255 - round(($current/importance div 100) * 255)" />
                <xsl:text>, 0);</xsl:text>
            </xsl:attribute>
            <xsl:value-of select="$previous" />
        </a>
    </xsl:template>
    
    <!-- Formats the locations in the code as needed -->
    <xsl:template name="format-locations">
        <xsl:param name="text"/>
        <xsl:param name="locations"/>

        <xsl:variable name="result">
            <xsl:if test="$locations/*[1]/@start = 1">
                <xsl:message terminate="no">Start points should be greater than 0 (character indexing starts at 1).</xsl:message>
            </xsl:if>
            <xsl:if test="$locations/*[1]/@end &lt; $locations/*[1]/@start">
                <xsl:message terminate="yes">End position of location must be after start.</xsl:message>
            </xsl:if>
            <xsl:value-of select="substring($text, 1, $locations/*[1]/@start)"/>
            <xsl:apply-templates select="$locations/*[1]">
                <xsl:with-param name="previous" select="substring($text, $locations/*[1]/@start + 1, $locations/*[1]/@end - $locations/*[1]/@start)" />
                <xsl:with-param name="current" select="$locations/*[1]" />
            </xsl:apply-templates>
            <xsl:for-each select="$locations/*[position() &gt; 1]">
                <xsl:if test="@end &lt; @start">
                    <xsl:message terminate="yes">End position of location must be after start.</xsl:message>
                </xsl:if>
                <xsl:value-of select="substring($text, preceding-sibling::*[1]/@end + 1, @start - preceding-sibling::*[1]/@end)"/>
                <xsl:apply-templates select="$locations/*[1]">
                    <xsl:with-param name="previous" select="substring($text, @start + 1, @end - @start)" />
                    <xsl:with-param name="current" select="." />
                </xsl:apply-templates>
            </xsl:for-each>
            <xsl:value-of select="substring($text, $locations/*[last()]/@end + 1)"/>
        </xsl:variable>
        <xsl:copy-of select="$result"/>
    </xsl:template>
    
</xsl:stylesheet>