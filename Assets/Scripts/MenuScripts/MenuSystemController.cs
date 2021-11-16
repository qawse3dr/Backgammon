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

  public void EditProfilesOnClick() {
    Logger.Info("Opening Edit Profiles Page...");
  }

  public void StatsOnClick() {
    Logger.Info("Opening Stats Page...");
  }
}
