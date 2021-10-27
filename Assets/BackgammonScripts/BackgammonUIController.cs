using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = LNAR.Logger;
/** This calls is responsible for all UI elements on the screen
 * This includes text popups, and so on but not the gameboard itself
 * that should be handled somewhere else
 */
public class BackgammonUIController : MonoBehaviour {
  public void MainMenuOnClick() {
    Logger.Info("Exiting Game Going back to MainMenu.");

    /* A warning should probably be added explain it will
     * not save the current state of the game and all progress will
     * be lost. Maybe it should also count as a LOSS for both of them
     * or maybe a draw.
     */

    SceneManager.LoadScene("MainMenu");
  }
}
