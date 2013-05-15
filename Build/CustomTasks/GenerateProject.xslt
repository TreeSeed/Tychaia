<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:user="urn:my-scripts"
  exclude-result-prefixes="xsl,msxsl,user"
  version="1.0">
  
  <xsl:output method="xml" indent="no" />
 
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
  
    <xsl:variable
      name="project"
      select="/Input/Projects/Project[@Name=/Input/Generation/ProjectName]" />
  
    <Project 
      DefaultTargets="Build"
      ToolsVersion="4.0"
      xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
      
      <PropertyGroup>
        <Configuration>Debug</Configuration>
        <Platform>AnyCPU</Platform>
        <ProductVersion>10.0.0</ProductVersion>
        <SchemaVersion>2.0</SchemaVersion>
        <ProjectGuid>{<xsl:value-of select="$project/@Guid" />}</ProjectGuid>
        <OutputType>
          <xsl:choose>
            <xsl:when test="$project/@Type = 'XNA'">
              <xsl:text>Exe</xsl:text>
            </xsl:when>
            <xsl:when test="$project/@Type = 'WindowsForms'">
              <xsl:text>Exe</xsl:text>
            </xsl:when>
            <xsl:otherwise>
              <xsl:text>Library</xsl:text>
            </xsl:otherwise>
          </xsl:choose>
        </OutputType>
        <xsl:choose>
          <xsl:when test="$project/@Type = 'Website'">            
            <ProjectTypeGuids>
              <xsl:text>{349C5851-65DF-11DA-9384-00065B846F21};</xsl:text>
              <xsl:text>{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}</xsl:text>
            </ProjectTypeGuids>
          </xsl:when>
          <xsl:otherwise>
          </xsl:otherwise>
        </xsl:choose>
        <RootNamespace>
          <xsl:value-of select="$project/@Name" />
        </RootNamespace>
        <AssemblyName>
          <xsl:value-of select="$project/@Name" />
        </AssemblyName>
        
        <!-- Always have debugging enabled. -->
        <DebugSymbols>True</DebugSymbols>
        <DebugType>full</DebugType>
        <Optimize>False</Optimize>
        <OutputPath>bin\Debug</OutputPath>
        <DefineConstants>
          <xsl:text>DEBUG;</xsl:text>
          <xsl:choose>
            <xsl:when test="/Input/Generation/Platform = 'Linux'">
              <xsl:text>PLATFORM_LINUX</xsl:text>
            </xsl:when>
            <xsl:when test="/Input/Generation/Platform = 'Windows'">
              <xsl:text>PLATFORM_WINDOWS</xsl:text>
            </xsl:when>
            <xsl:otherwise>
              <xsl:text>PLATFORM_UNKNOWN</xsl:text>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:text>;</xsl:text>
        </DefineConstants>
        <ErrorReport>prompt</ErrorReport>
        <WarningLevel>4</WarningLevel>
        <ConsolePause>False</ConsolePause>
      </PropertyGroup>
      
      <PropertyGroup
        Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
        
      </PropertyGroup>
      
      <Import Project="$(MSBuildBinPath)\Microsoft.CSharp.targets" />      
      <xsl:choose>
        <xsl:when test="$project/@Type = 'Website'">    
          <Import>
            <xsl:attribute name="Project">
              <xsl:text>$(MSBuildExtensionsPath)\Microsoft\</xsl:text>
              <xsl:text>VisualStudio\v10.0\WebApplications\</xsl:text>
              <xsl:text>\Microsoft.WebApplication.targets</xsl:text>
            </xsl:attribute>
          </Import>
        </xsl:when>
      </xsl:choose>
      
      <ItemGroup>
        <xsl:for-each select="$project/References/Reference">
          <xsl:variable name="include-path" select="./@Include" />
          <xsl:choose>
            <xsl:when test="
              count(/Input/Projects/Project[@Name=$include-path]) > 0">
              
              <ProjectReference>
                <xsl:attribute name="Include">
                  <xsl:value-of
                    select="user:GetRelativePath(
                      concat(
                        $project/@Path,
                        '\',
                        $project/@Name,
                        '.',
                        /Input/Generation/Platform,
                        '.csproj'),
                      concat(
                        $include-path,
                        '\',
                        @Include,
                        '.',
                        /Input/Generation/Platform,
                        '.csproj'))" />
                </xsl:attribute>
                <Project>{<xsl:value-of 
select="/Input/Projects/Project[@Name=$include-path]/@Guid" />}</Project>
                <Name>
                  <xsl:value-of select="@Include" />
                </Name>
              </ProjectReference>
            </xsl:when>
            <xsl:otherwise></xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
      </ItemGroup>
      
      <ItemGroup>
        <xsl:for-each select="$project/References/Reference">
          <xsl:variable name="include-path" select="./@Include" />
          <xsl:choose>
            <xsl:when test="
              count(/Input/Projects/Project[@Name=$include-path]) > 0">
            </xsl:when>
            <xsl:otherwise>
              <Reference>
                <xsl:attribute name="Include">
                  <xsl:value-of select="@Include" />
                </xsl:attribute>
                <xsl:text />
              </Reference>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
      </ItemGroup>
      
      <ItemGroup>
        <xsl:for-each select="$project/Files/Compile">
          <xsl:element 
            name="{name()}" 
            namespace="http://schemas.microsoft.com/developer/msbuild/2003">
            <xsl:attribute name="Include">
              <xsl:value-of select="@Include" />
            </xsl:attribute>
            <xsl:apply-templates select="node()"/>
          </xsl:element>
        </xsl:for-each>
      </ItemGroup>
      
      <ItemGroup>
        <xsl:for-each select="$project/Files/None">
          <xsl:element 
            name="{name()}" 
            namespace="http://schemas.microsoft.com/developer/msbuild/2003">
            <xsl:attribute name="Include">
              <xsl:value-of select="@Include" />
            </xsl:attribute>
            <xsl:apply-templates select="node()"/>
          </xsl:element>
        </xsl:for-each>
      </ItemGroup>
      
      <ItemGroup>
        <xsl:for-each select="$project/Files/Content">
          <xsl:element 
            name="{name()}" 
            namespace="http://schemas.microsoft.com/developer/msbuild/2003">
            <xsl:attribute name="Include">
              <xsl:value-of select="@Include" />
            </xsl:attribute>
            <xsl:apply-templates select="node()"/>
          </xsl:element>
        </xsl:for-each>
      </ItemGroup>
      
    </Project>
    
  </xsl:template>
  
  <xsl:template match="*">
    <xsl:element 
      name="{name()}" 
      namespace="http://schemas.microsoft.com/developer/msbuild/2003">
      <xsl:apply-templates select="@*|node()"/>
    </xsl:element>
  </xsl:template>
  
</xsl:stylesheet>