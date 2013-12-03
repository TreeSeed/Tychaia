<?php

/**
 * Tychaia license linter.
 *
 * @group linter
 */
final class ArcanistTychaiaLicenseLinter extends ArcanistLinter {

  const LINT_NO_LICENSE_HEADER = 1;

  public function willLintPaths(array $paths) {
    return;
  }

  public function getLinterName() {
    return 'TYCHAIALICENSE';
  }

  public function getLinterConfigurationName() {
    return 'tychaialicense';
  }

  public function getLintSeverityMap() {
    return array();
  }

  public function getLintNameMap() {
    return array(
      self::LINT_NO_LICENSE_HEADER   => 'No License Header',
    );
  }

  public function setHintPath($cslint) {
    $this->cslintHintPath = $cslint;
    return $this;
  }

  public function lintPath($path) {
    $data = $this->getData($path);
    if (ord($data[0]) == 0xEF &&
      ord($data[1]) == 0xBB &&
      ord($data[2]) == 0xBF) {
      $data = substr($data, 3);
    }
    $license = <<<EOLICENSE
// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
EOLICENSE;
    if (strncmp($data, $license, strlen($license)) !== 0) {
      $this->raiseLintAtOffset(
        0,
        self::LINT_NO_LICENSE_HEADER,
        'This file has a missing or out of date license header.');
    }
  }
}
