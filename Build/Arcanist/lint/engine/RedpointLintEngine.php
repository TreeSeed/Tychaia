<?php

/**
 * Redpoint lint engine.
 *
 * @group linter
 */
final class RedpointLintEngine extends ArcanistLintEngine {

  public function buildLinters() {
    $linters = array();

    $paths = $this->getPaths();

    foreach ($paths as $key => $path) {
      if (!$this->pathExists($path)) {
        unset($paths[$key]);
      }
      if (preg_match('@^Libraries/@', $path)) {
        // Third-party stuff lives in /externals/; don't run lint engines
        // against it.
        unset($paths[$key]);
      }
      if (preg_match('@\.Designer\.cs$@', $path)) {
        // Designer files are automatically generated.
        unset($paths[$key]);
      }

    }

    // Determine the list of Tychaia licensed files (we exclude Protogame, etc.)
    $licensed = preg_grep('/\.cs$/', $paths);
    foreach ($licensed as $key => $path) {
      if (preg_match('@^Protogame@', $path)) {
        unset($licensed[$key]);
      }
    }

    $text_paths = preg_grep('/\.(php|cs|css|hpp|cpp|l|y|py|pl)$/', $paths);
    $linters[] = id(new ArcanistGeneratedLinter())->setPaths($text_paths);
    $linters[] = id(new ArcanistNoLintLinter())->setPaths($text_paths);
    $linters[] = id(new ArcanistTextLinter())
      ->setPaths($text_paths)
      ->setMaxLineLength(120);

    $linters[] = id(new ArcanistFilenameLinter())->setPaths($paths);
    $linters[] = id(new ArcanistTychaiaLicenseLinter())
      ->setPaths(preg_grep('/\.cs$/', $licensed));

    $linters[] = id(new ArcanistXHPASTLinter())
      ->setPaths(preg_grep('/\.php$/', $paths));

    $linters[] = id(new ArcanistJSHintLinter())
      ->setPaths(preg_grep('/\.js$/', $paths));

    $linters[] = id(new ArcanistCSharpLinter())
      ->setPaths(preg_grep('/\.cs$/', $paths))
      ->setHintPath(
        $this->getWorkingCopy()->getProjectRoot().
        "/Build/cslint/bin/Debug/cslint.exe");

    return $linters;
  }

}
