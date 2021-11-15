using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = LNAR.Logger;

/** Menu Class should deal with all the main logic included in the menu
 * including but not limited to:
 *   OnClicks
 *   Changing Scene functionality
 */
public class MenuSystemController : MonoBehaviour {
  public void StartGameOnClick() {
    Logger.Info("Starting Game Scene...");
    SceneManager.LoadScene("Backgammon");
    GameHandler.Game.InitBoardState();
  }

  public void QuitGameOnClick() {
    Logger.Info("Exiting Game...");
    Application.Quit();
  }

  public void StatsPageOnClick() {
    Logger.Info("Redirecting to stats page...");
    SceneManager.LoadScene("StatsScene");
  }
}
