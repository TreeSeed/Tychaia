<?php

/**
 * C# linter for Arcanist.
 *
 * @group linter
 */
final class ArcanistCSharpLinter extends ArcanistFutureLinter {

  private $runtimeEngine;
  private $cslintEngine;
  private $cslintHintPath;
  private $loaded;

  public function getLinterName() {
    return 'C#';
  }

  public function getLintSeverityMap() {
    return array();
  }

  public function getLintNameMap() {
    return array();
  }

  public function setHintPath($cslint) {
    $this->cslintHintPath = $cslint;
    return $this;
  }

  /**
   * Determines what executables and test paths to use.  Between platforms
   * this also changes whether the test engine is run under .NET or Mono.  It
   * also ensures that all of the required binaries are available for the tests
   * to run successfully.
   *
   * @return void
   */
  private function loadEnvironment() {
    if ($this->loaded) {
      return;
    }

    // Determine runtime engine (.NET or Mono).
    if (phutil_is_windows()) {
      $this->runtimeEngine = "";
    } else if (Filesystem::binaryExists("mono")) {
      $this->runtimeEngine = "mono ";
    } else {
      throw new Exception("Unable to find Mono and you are not on Windows!");
    }

    // Determine cslint path.
    $cslint = $this->cslintHintPath;
    if ($cslint !== null && file_exists($cslint)) {
      $this->cslintEngine = Filesystem::resolvePath($cslint);
    } else if (Filesystem::binaryExists("cslint.exe")) {
      $this->cslintEngine = "cslint.exe";
    } else {
      throw new Exception("Unable to locate cslint.");
    }

    $this->loaded = true;
  }

  protected function buildFutures(array $paths) {
    $this->loadEnvironment();

    $futures = array();

    foreach ($paths as $path) {
      $futures[$path] = new ExecFuture(
        "%C %s",
        $this->runtimeEngine.$this->cslintEngine,
        $this->getEngine()->getFilePathOnDisk($path));
    }

    return $futures;
  }

  protected function resolveFuture($path, Future $future) {
    list($rc, $stdout) = $future->resolve();
    $results = json_decode($stdout);
    foreach ($results->Issues as $issue) {
      $message = new ArcanistLintMessage();
      $message->setPath($path);
      $message->setLine($issue->LineNumber);
      $message->setCode("CS".$issue->Index->Code);
      $message->setName($issue->Index->Name);
      $message->setDescription(
        vsprintf($issue->Index->Message, $issue->Parameters));
      $severity = ArcanistLintSeverity::SEVERITY_ADVICE;
      switch ($issue->Index->Severity) {
        case 0:
          $severity = ArcanistLintSeverity::SEVERITY_ADVICE;
          break;
        case 1:
          $severity = ArcanistLintSeverity::SEVERITY_AUTOFIX;
          break;
        case 2:
          $severity = ArcanistLintSeverity::SEVERITY_WARNING;
          break;
        case 3:
          $severity = ArcanistLintSeverity::SEVERITY_ERROR;
          break;
        case 4:
          $severity = ArcanistLintSeverity::SEVERITY_DISABLED;
          break;
      }
      $message->setSeverity($severity);
      $this->addLintMessage($message);

    }
  }

}
