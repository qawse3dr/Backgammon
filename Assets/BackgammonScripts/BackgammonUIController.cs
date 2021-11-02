using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = LNAR.Logger;
/** This calls is responsible for all UI elements on the screen
 * This includes text popups, and so on but not the gameboard itself
 * that should be handled somewhere else
 */
public class BackgammonUIController : MonoBehaviour {
  public Image image;
  public Image buttonBorder;
  public Image panelBorder;
  public Text text;
  public Text mainMenu;
  public Text winner;

  void Start() {
    image.enabled = false;
    text.enabled = false;
    mainMenu.enabled = false;
    winner.enabled = false;
    buttonBorder.enabled = false;
    panelBorder.enabled = false;
  }

  public void MainMenuOnClick() {
    Logger.Info("Exiting Game Going back to MainMenu.");
    /* A warning should probably be added explain it will
     * not save the current state of the game and all progress will
     * be lost. Maybe it should also count as a LOSS for both of them
     * or maybe a draw.
     */

    SceneManager.LoadScene("MainMenu");
  }

  public void GameOverOnClick() {
    Logger.Info("Simulating a game over");

    image.enabled = true;
    text.enabled = true;
    mainMenu.enabled = true;
    winner.enabled = true;
    buttonBorder.enabled = true;
    panelBorder.enabled = true;
  }

  public void GameOver(int winPlayer){

    if(winPlayer == 1){
      winner.text = "Player 1 won";
    } else if(winPlayer == 2){
      winner.text = "Player 2 won";
    }
    image.enabled = true;
    text.enabled = true;
    mainMenu.enabled = true;
    winner.enabled = true;
    buttonBorder.enabled = true;
    panelBorder.enabled = true;
  }
}
