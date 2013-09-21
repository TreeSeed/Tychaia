<?php

final class CSharpToolsTestEngine extends XUnitTestEngine {

  private $coverEngine;

  protected function loadEnvironment() {
    parent::loadEnvironment();

    if (!$this->getEnableCoverage()) {
      return;
    }

    // Determine coverage path.
    $cscover = $this->projectRoot.
               "/cstools/cscover/bin/Debug/cscover.exe";
    if (file_exists($cscover)) {
      $this->coverEngine = Filesystem::resolvePath($cscover);
    } else {
      throw new Exception(
        "Unable to locate cscover coverage runner ".
        "(have you built yet?");
    }
  }

  protected function buildTestFuture($test_assembly) {
    if (!$this->getEnableCoverage()) {
      return parent::buildTestFuture($test_assembly);
    }

    // FIXME: Can't use TempFile here as xUnit doesn't like
    // UNIX-style full paths.  It sees the leading / as the
    // start of an option flag, even when quoted.
    $xunit_temp = $test_assembly.".results.xml";
    if (file_exists($xunit_temp)) {
      unlink($xunit_temp);
    }
    $cover_temp = new TempFile();
    $cover_temp->setPreserveFile(true);
    $xunit_cmd = $this->runtimeEngine;
    $xunit_args = null;
    if ($xunit_cmd === "") {
      $xunit_cmd = $this->testEngine;
      $xunit_args = csprintf(
        "%s /xml %s /silent",
        str_replace('/', '_', $test_assembly).".dll",
        $xunit_temp);
    } else {
      $xunit_args = csprintf(
        "%s %s /xml %s /silent",
        $this->testEngine,
        str_replace('/', '_', $test_assembly).".dll",
        $xunit_temp);
    }
    $assembly_dir =
      str_replace('/', '_', $test_assembly).
      "/bin/Debug/";
    $assemblies_to_instrument = array();
    foreach (Filesystem::listDirectory($assembly_dir) as $file) {
      if (substr($file, -4) == ".dll") {
        $assemblies_to_instrument[] = $assembly_dir.$file;
      }
    }
    $future = new ExecFuture(
      "%C -o %s -c %s -a %s -w %s --copy-to=%s %Ls",
      trim($this->runtimeEngine." ".$this->coverEngine),
      $cover_temp,
      $xunit_cmd,
      $xunit_args,
      $assembly_dir,
      $assembly_dir,
      $assemblies_to_instrument);
    $future->setCWD(Filesystem::resolvePath($this->projectRoot));
    return array(
      $future,
      $this->projectRoot.$assembly_dir.$xunit_temp,
      $cover_temp);
  }

  protected function parseCoverageResult($cover_file) {
    if (!$this->getEnableCoverage()) {
      return parent::parseCoverageResult($cover_file);
    }
    return $this->readCoverage($cover_file);
  }

  public function readCoverage($cover_file) {
    $coverage_dom = new DOMDocument();
    $coverage_dom->loadXML(Filesystem::readFile($cover_file));

    $files = array();
    $reports = array();
    $instrumented = $coverage_dom->getElementsByTagName("instrumented");

    foreach ($instrumented as $instrument) {
      $absolute_file = $instrument->getAttribute("file");
      $relative_file = substr($absolute_file, strlen($this->projectRoot) + 1);
      $files[] = $relative_file;
    }

    foreach ($files as $file) {
      $absolute_file = $this->projectRoot."/".$file;

      // get total line count in file
      $line_count = count(file($absolute_file));

      $coverage = array();
      for ($i = 0; $i < $line_count; $i++) {
        $coverage[$i] = 'N';
      }

      foreach ($instrumented as $instrument) {
        if ($instrument->getAttribute("file") !== $absolute_file) {
          continue;
        }
        for (
          $i = $instrument->getAttribute("start");
          $i <= $instrument->getAttribute("end");
          $i++) {
          $coverage[$i - 1] = 'U';
        }
      }

      foreach ($coverage_dom->getElementsByTagName("executed") as $execute) {
        if ($execute->getAttribute("file") !== $absolute_file) {
          continue;
        }
        for (
          $i = $execute->getAttribute("start");
          $i <= $execute->getAttribute("end");
          $i++) {
          $coverage[$i - 1] = 'C';
        }
      }

      $reports[$file] = implode($coverage);
    }

    return $reports;
  }
}


