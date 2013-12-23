<?php

/**
 * Forcibly resets and cleans up the entire project.
 *
 * @group workflow
 */
final class RedpointForceResetWorkflow extends ArcanistBaseWorkflow {

  public function getWorkflowName() {
    return 'force-reset';
  }

  public function getCommandSynopses() {
    return phutil_console_format(<<<EOTEXT
      **force-reset** [--fetch]
EOTEXT
      );
  }

  public function getCommandHelp() {
    return phutil_console_format(<<<EOTEXT
          Supports: git
          Completely cleans out the project, all build files and resets all
          submodules to their correct state as well.

          This will **ERASE** any changes in the local working copy!
EOTEXT
      );
  }

  public function getArguments() {
    return array(
      'fetch' => array(
        'help' =>
          "Perform 'git fetch --all' in all of the submodules."
      ),
    );
  }

  public function requiresConduit() {
    return true;
  }

  public function requiresWorkingCopy() {
    return true;
  }

  public function shouldShellComplete() {
    return false;
  }

  private function writeOutOkay($console, $msg = null) {
    if ($msg === null) {
      $msg = "OKAY";
    }
    $console->writeOut("<bg:green> %s </bg>\n", $msg);
  }
  
  public function run() {
    $working_copy = $this->getWorkingCopy();
    $root = $working_copy->getProjectRoot();
    $console = PhutilConsole::getConsole();

    // All commands have to execute at the root of the site.
    chdir($root);

    list($out, $err) = execx("git status -s");
    $changes = phutil_split_lines($out);
    
    $console->writeOut("\n<bg:red> WARNING WARNING WARNING </bg>\n\n");
    $console->writeOut("You are about to permanently lose the ");
    $console->writeOut("changes to the following files:\n");
    foreach ($changes as $change) {
      $console->writeOut("\n * ".substr($change, 2));
    }
    
    $phrase = "Revert ".count($changes)." files";
    if (count($changes) === 1) {
      $phrase = "Revert ".count($changes)." file";
    }
    
    $answer = phutil_console_prompt('Exactly the phrase "'.$phrase.'":');
    if ($answer !== $phrase) {
      throw new ArcanistUsageException(
        "You did not type '".$phrase."'.  Refusing to run!");
    }
    $console->writeOut("\n\n");

    $console->writeOut("Resetting working directory... ");
    execx("git reset --hard HEAD");
    $this->writeOutOkay($console);

    $console->writeOut("Cleaning working directory... ");
    execx("git clean -xdf");
    $this->writeOutOkay($console);

    $console->writeOut("Updating submodules... ");
    execx("git submodule update --init --recursive");
    $this->writeOutOkay($console);

    if ($this->getArgument('fetch')) {
      $console->writeOut("Fetching submodules... ");
      execx("git submodule foreach --recursive git fetch --all");
      $this->writeOutOkay($console);
    }

    $console->writeOut("Resetting submodules... ");
    execx("git submodule foreach --recursive git reset --hard HEAD");
    $this->writeOutOkay($console);

    $console->writeOut("Cleaning submodules... ");
    execx("git submodule foreach --recursive git clean -xdf");
    $this->writeOutOkay($console);

    $console->writeOut("\n");
    $this->writeOutOkay($console, "WORKING DIRECTORY RESET");

    return 0;
  }
}
