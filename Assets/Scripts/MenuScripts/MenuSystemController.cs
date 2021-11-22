using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = LNAR.Logger;

/** Menu Class should deal with all the main logic included in the menu
 * including but not limited to:
 *   OnClicks
 *   Changing Scene functionality
 */
public class MenuSystemController : MonoBehaviour {

  public GameObject CreatePlayerPopup;
  public InputField input;

  void Start() {
    CreatePlayerPopup.SetActive(false);
    Database.DB_PATH = "stats.db";
  }
  public void StartGameOnClick() {
    Logger.Info("Starting Game Scene...");
    SceneManager.LoadScene("SelectPlayer");
  }

  public void QuitGameOnClick() {
    Logger.Info("Exiting Game...");
    Application.Quit();
  }

  public void EditProfilesOnClick() {
    Logger.Info("Opening Edit Profiles Page...");
    Logger.Info("Creating Profile...");
    CreatePlayerPopup.SetActive(true);
  }

  public void StatsPageOnClick() {
    Logger.Info("Redirecting to stats page...");
    SceneManager.LoadScene("StatsScene");
  }

  public void MainMenuOnClick() {
    Logger.Info("Going to Main Menu...");
    CreatePlayerPopup.SetActive(false);
  }

  public void PlayerCreation() {
    //var input = gameObject.GetComponent<InputField>();
    Logger.Info(input.text);

    if(input.text == ""){
      GameObject.Find("CreatePlayerHeader").GetComponent<Text>().text = "Please Do Not Leave The Text Field Empty";
    } else {
      CreatePlayerPopup.SetActive(false);
    }
  }
}
