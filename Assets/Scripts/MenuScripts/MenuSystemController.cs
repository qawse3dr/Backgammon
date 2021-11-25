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
<<<<<<< HEAD

=======
>>>>>>> d1b4de4 (Profile creation is done)
  public GameObject CreatePlayerPopup;
  public InputField input;
  // public Database db = Database.CreateDatabase();
  void Start() {
    Database.DB_PATH = "stats.db";

    CreatePlayerPopup.SetActive(false);
<<<<<<< HEAD
    Database.DB_PATH = "stats.db";
=======
>>>>>>> d1b4de4 (Profile creation is done)
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
    // var input = gameObject.GetComponent<InputField>();
    int flag = 0;
    Logger.Info(input.text);
    if (input.text == "") {
      GameObject.Find("CreatePlayerHeader").GetComponent<Text>().text =
          "Please Do Not Leave The Text Field Empty";
    } else {
      Database db = Database.CreateDatabase();
      List<Player> players = db.ReadDB();

      foreach (var player in players) {
        if (input.text == player.Name) {
          flag = 1;
        }
      }

      if (flag == 0) {
        db.WritePlayerToDB(Player.CreateNewPlayer(input.text, PlayerEnum.NotSet));
        CreatePlayerPopup.SetActive(false);
      } else {
        GameObject.Find("CreatePlayerHeader").GetComponent<Text>().text =
            "Account Already Exists Please Try A Different Username";
      }
    }
    flag = 0;
  }
}
