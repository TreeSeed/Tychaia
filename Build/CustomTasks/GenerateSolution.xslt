<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:user="urn:my-scripts"
  exclude-result-prefixes="xsl,msxsl,user"
  version="1.0">
  
  <xsl:output method="text" indent="no" />
 
  <msxsl:script language="C#" implements-prefix="user">
    <msxsl:assembly name="System.Web" />
    <msxsl:using namespace="System" />
    <msxsl:using namespace="System.Web" />
    <![CDATA[
    public string GetRelativePath(string from, string to)
    {
      try
      {
        var current = Environment.CurrentDirectory;
        Uri fromUri = new Uri(System.IO.Path.Combine(current, from));
        Uri toUri = new Uri(System.IO.Path.Combine(current, to));
        Uri relativeUri = fromUri.MakeRelativeUri(toUri);
        String relativePath = Uri.UnescapeDataString(relativeUri.ToString());
        return relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar);
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }
    ]]>
  </msxsl:script> 
 
  <xsl:template match="/">
    <xsl:text>
Microsoft Visual Studio Solution File, Format Version 11.00
# Visual Studio 2010
</xsl:text>
    <xsl:for-each select="/Input/Projects/Project">
      <xsl:text>Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = </xsl:text>
      <xsl:text>"</xsl:text>
      <xsl:value-of select="current()/@Name" />
      <xsl:text>", "</xsl:text>
      <xsl:value-of select="concat(
                        current()/@Path,
                        '\',
                        current()/@Name,
                        '.',
                        /Input/Generation/Platform,
                        '.csproj')" />
      <xsl:text>", "{</xsl:text>
      <xsl:value-of select="current()/@Guid" />
      <xsl:text>}"
EndProject
</xsl:text>
    </xsl:for-each>
    <xsl:text>Global
        GlobalSection(SolutionConfigurationPlatforms) = preSolution
                Debug|Any CPU = Debug|Any CPU
                Release|Any CPU = Release|Any CPU
        EndGlobalSection
        GlobalSection(ProjectConfigurationPlatforms) = postSolution
</xsl:text>
    <xsl:for-each select="/Input/Projects/Project">
      <xsl:text>                {</xsl:text>
      <xsl:value-of select="current()/@Guid" />
      <xsl:text>}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
</xsl:text>
      <xsl:text>                {</xsl:text>
      <xsl:value-of select="current()/@Guid" />
      <xsl:text>}.Debug|Any CPU.Build.0 = Debug|Any CPU
</xsl:text>
      <xsl:text>                {</xsl:text>
      <xsl:value-of select="current()/@Guid" />
      <xsl:text>}.Release|Any CPU.ActiveCfg = Release|Any CPU
</xsl:text>
      <xsl:text>                {</xsl:text>
      <xsl:value-of select="current()/@Guid" />
      <xsl:text>}.Release|Any CPU.Build.0 = Release|Any CPU
</xsl:text>
    </xsl:for-each>
    <xsl:text>        EndGlobalSection
EndGlobal
</xsl:text>
  </xsl:template>
  
  <xsl:template match="*">
    <xsl:element 
      name="{name()}" 
      namespace="http://schemas.microsoft.com/developer/msbuild/2003">
      <xsl:apply-templates select="@*|node()"/>
    </xsl:element>
  </xsl:template>
  
</xsl:stylesheet>