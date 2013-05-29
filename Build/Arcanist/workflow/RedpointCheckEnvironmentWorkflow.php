<?php

/**
 * Checks the current environment.
 *
 * @group workflow
 */
final class RedpointCheckEnvironmentWorkflow extends ArcanistBaseWorkflow {

  public function getWorkflowName() {
    return 'check-env';
  }

  public function getCommandSynopses() {
    return phutil_console_format(<<<EOTEXT
      **check-env**
EOTEXT
      );
  }

  public function getCommandHelp() {
    return phutil_console_format(<<<EOTEXT
          Supports: git
          Validates the current environment to make sure known issues aren't
          existant.
EOTEXT
      );
  }

  public function getArguments() {
    return array(
    );
  }

  public function requiresConduit() {
    return true;
  }

  public function requiresWorkingCopy() {
    return true;
  }

  public function shouldShellComplete() {
    return true;
  }
  
  private function troubleshootingLink($name) {
    return "http://code.redpointsoftware.com.au/w/troubleshooting/mono/$name/";
  }
  
  private function checkMonoBinaryIsRunnable($binary) {
    if (!Filesystem::binaryExists($binary)) {
      throw new Exception(
        "Unable to locate '$binary' in your PATH.  If using Mono, ensure ".
        "that you have created missing batch files in the Mono binary ".
        "directory.  Read {$this->troubleshootingLink('dmcs_gmcs_missing')} ".
        "for a solution to this issue.");
    }
    try {
      execx($binary." --help");
    } catch (CommandException $ex) {
      throw new Exception(
        "Unable to run '$binary'.  If using Mono, ensure ".
        "that you have created missing batch files in the Mono binary ".
        "directory.  Read {$this->troubleshootingLink('v10_dir_missing')} ".
        "for a solution to this issue.");
    }
  }

  private function checkMonoWebSiteWorkaroundIsActive() {
    $nine = "v9.0";
    $ten = "v10.0";
    $dirSeperator = null;
    $bases = null;
    if (phutil_is_windows()) {
      $dirSeperator = "\\";
      $bases = array(
        "C:\\Program Files\\Mono-3.0.10\\lib\\mono\\xbuild\\Microsoft\\VisualStudio",
        "C:\\Program Files (x86)\\Mono-3.0.10\\lib\\mono\\xbuild\\Microsoft\\VisualStudio");
    } else {
      $dirSeperator = "/";
      $bases = array(
        "/usr/lib/mono/xbuild/Microsoft/VisualStudio/");
    }
    foreach ($bases as $base) {
      if (is_dir($base) && is_dir($base.$dirSeperator.$nine)) {
        // We have found the base directory for the VisualStudio rules in Mono.
        if (!is_dir($base.$dirSeperator.$ten)) {
          throw new Exception(
            "Your version of Mono is missing the '$ten' directory, which ".
            "is required for .NET 4 websites to build correctly.  This ".
            "can be corrected by copying or linking the '$nine' directory ".
            "that is located under '$base' as '$ten'.  Read ".
            "{$this->troubleshootingLink('dmcs_gmcs_missing')} ".
            "for a solution to this issue.");
        }
      }
    }
  }
  
  public function run() {
    $console = PhutilConsole::getConsole();
    
    try {
      $this->checkMonoBinaryIsRunnable("dmcs");
      $this->checkMonoBinaryIsRunnable("gmcs");
      $this->checkMonoWebSiteWorkaroundIsActive();
    } catch (Exception $ex) {
      $console->writeOut("<bg:green> FAIL </bg> %s.\n", $ex->getMessage());
      return 1;
    }

    $console->writeOut("<bg:green> PASS </bg> All environment checks complete.\n");
    return 0;
  }
}
