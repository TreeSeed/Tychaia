<?php

/**
 * Cleans up all arc patch branches.
 *
 * @group workflow
 */
final class RedpointCleanupWorkflow extends ArcanistBaseWorkflow {

  public function getWorkflowName() {
    return 'cleanup';
  }

  public function getCommandSynopses() {
    return phutil_console_format(<<<EOTEXT
      **cleanup**
EOTEXT
      );
  }

  public function getCommandHelp() {
    return phutil_console_format(<<<EOTEXT
          Supports: git
          Force deletes any branches starting with 'arcpatch-'.
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

  public function requiresRepositoryAPI() {
    return true;
  }

  public function shouldShellComplete() {
    return false;
  }

  public function run() {
    $repositoryAPI = $this->getRepositoryAPI();
    $console = PhutilConsole::getConsole();

    if ($repositoryAPI->getSourceControlSystemName() !== "git") {
      $console->writeLine("This command can only be used on Git repositories.");
      return 1;
    }

    $futures = array();
    foreach ($repositoryAPI->getAllBranches() as $branch) {
      if (substr($branch["name"], 0, strlen("arcpatch-")) !== "arcpatch-") {
        continue;
      }
      if ($branch["current"]) {
        continue;
      }
      $futures[$branch["name"]] = new ExecFuture(
        "git branch -D %s",
        $branch["name"]);
    }
    foreach (Futures($futures) as $name => $future) {
      list($err) = $future->resolvex();
      $console->writeOut("Deleted ".$name.".\n");
    }

    return 0;
  }
}
