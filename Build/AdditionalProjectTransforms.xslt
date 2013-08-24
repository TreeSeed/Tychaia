<xsl:if test="/Input/Properties/PostProcessWithDx = 'True'">
  <Target Name="AfterBuild">
    <xsl:choose>
      <xsl:when test="/Input/Generation/Platform = 'Windows'">
        <Exec>
          <xsl:attribute name="Command">
            <xsl:value-of select="concat(
  /Input/Generation/RootPath,
  'Protogame/ThirdParty/Dx/Process4.Task/bin/Debug/Process4.Task.exe')" />
            <xsl:text> "$(TargetPath)"</xsl:text>
          </xsl:attribute>
        </Exec>
      </xsl:when>
      <xsl:otherwise>
        <Exec>
          <xsl:attribute name="Command">
            <xsl:text>mono </xsl:text>
            <xsl:value-of select="concat(
  /Input/Generation/RootPath,
  'Protogame/ThirdParty/Dx/Process4.Task/bin/Debug/Process4.Task.exe')" />
            <xsl:text> "$(TargetPath)"</xsl:text>
          </xsl:attribute>
        </Exec>
      </xsl:otherwise>
    </xsl:choose>
  </Target>
</xsl:if>

<xsl:if test="/Input/Properties/PrecompileProtobuf = 'True'">
  <Target Name="AfterBuild">
    <Exec>
      <xsl:attribute name="WorkingDirectory">
        <xsl:value-of select="/Input/Generation/RootPath" />
      </xsl:attribute>
      <xsl:attribute name="Command">
        <xsl:if test="/Input/Generation/Platform != 'Windows'">
          <xsl:text>mono </xsl:text>
        </xsl:if>
        <xsl:text>Libraries/protobuf-net/precompiler/precompile.exe </xsl:text>
        <xsl:value-of select="/Input/Properties/PrecompileProtobufInputPath" />
        <xsl:text> -o:</xsl:text>
        <xsl:value-of select="/Input/Properties/PrecompileProtobufOutputPath" />
        <xsl:text> -t:</xsl:text>
        <xsl:value-of select="/Input/Properties/PrecompileProtobufSerializerName" />
        <xsl:text> -access:Public</xsl:text>
      </xsl:attribute>
    </Exec>
  </Target>
</xsl:if>
