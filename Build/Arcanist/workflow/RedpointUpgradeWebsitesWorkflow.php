<?php

/**
 * Upgrades the current repository for running the websites.
 *
 * @group workflow
 */
final class RedpointUpgradeWebsitesWorkflow extends ArcanistBaseWorkflow {

  public function getWorkflowName() {
    return 'upgrade-websites';
  }

  public function getCommandSynopses() {
    return phutil_console_format(<<<EOTEXT
      **upgrade-websites**
EOTEXT
      );
  }

  public function getCommandHelp() {
    return phutil_console_format(<<<EOTEXT
          Supports: git
          Configures the current directory to run the Tychaia websites.  This
          will **ERASE** any changes in the local working copy!
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

    $console->writeOut("Stopping service...\n");
    phutil_passthru("sudo systemctl stop mono-makemeaworld.com.service");

    $console->writeOut("Clearing out temporary directories... ");
    execx("rm -Rf /tmp/*aspnet*");
    $this->writeOutOkay($console);

    $console->writeOut("Resetting changes... ");
    execx("git reset --hard HEAD");
    $this->writeOutOkay($console);

    $console->writeOut("Cleaning changes... ");
    execx("git clean -xdf");
    $this->writeOutOkay($console);

    $console->writeOut("Force checking out master... ");
    execx("git checkout master -f");
    $this->writeOutOkay($console);

    $console->writeOut("Pulling latest version... ");
    execx("git pull");
    $this->writeOutOkay($console);

    $console->writeOut("Linking back Local.config... ");
    execx("ln -s ../../Local.config $root/MakeMeAWorld/Local.config");
    $this->writeOutOkay($console);

    $console->writeOut("Starting service again... ");
    phutil_passthru("sudo systemctl start mono-makemeaworld.com.service");
    $console->writeOut("\n");
    $this->writeOutOkay($console, "ALL OPERATIONS COMPLETE");

    return 0;
  }
}
