
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

  public Sprite Die1;
  public Sprite Die2;
  public Sprite Die3;
  public Sprite Die4;
  public Sprite Die5;
  public Sprite Die6;

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

  public void GameOver(Player player) {
    winner.text = $"Player {player.Name} won";

    image.enabled = true;
    text.enabled = true;
    mainMenu.enabled = true;
    winner.enabled = true;
    buttonBorder.enabled = true;
    panelBorder.enabled = true;
  }
  public void RollDice() {
    Logger.Debug("Rolling dice");
    Logger.Debug("Setting roll UI to false");
    GameObject.Find("Roll").GetComponent<Text>().enabled = false;
    Logger.Debug("Rolling dice");
    GameHandler.Game.RollDice(this);
  }

}
