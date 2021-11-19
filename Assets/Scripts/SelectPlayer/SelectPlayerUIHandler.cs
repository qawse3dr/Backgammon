using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Logger = LNAR.Logger;

public class SelectPlayerUIHandler : MonoBehaviour {
  public Text ErrorMsg;
  public Text Player1DropDownText;
  public Text Player2DropDownText;

  public Dropdown Player1DropDown;
  public Dropdown Player2DropDown;

  private List<Player> _playerList;
  public List<Player> PlayerList {
    get { return _playerList; }
    set { _playerList = value; }
  }
  // Start is called before the first frame update
  void Start() {
    // TODO change once where figure out the db
    Database db = Database.CreateDatabase();
    _playerList = db.ReadDB();

    // Updates list
    Player1DropDown.ClearOptions();
    Player2DropDown.ClearOptions();

    List<string> options = new List<string>();
    foreach (Player p in _playerList) {
      options.Add(p.Name);
    }
    Player1DropDown.AddOptions(options);
    Player2DropDown.AddOptions(options);

    Player2DropDown.SetValueWithoutNotify(1);

    ErrorMsg.enabled = false;
  }

  // Update is called once per frame
  void Update() {}

  public bool GetPlayersFromDropdown(ref Player player1, ref Player player2) {
    string player1Name = Player1DropDownText.text;
    string player2Name = Player2DropDownText.text;
    if (player1Name == player2Name) {
      Logger.Info("Same Player selected printing error");
      ErrorMsg.enabled = true;
      return false;
    }

    Logger.Info("Starting Game Scene...");
    foreach (Player p in _playerList) {
      if (p.Name == player1Name) {
        player1 = p;
      } else if (p.Name == player2Name) {
        player2 = p;
      }
    }
    if (player1 == null || player2 == null) {
      Logger.Error("Player 1 or player 2 is null");
      return false;
    }
    return true;
  }
  public void StartGameOnClick() {
    Player player1 = null;
    Player player2 = null;

    if (!GetPlayersFromDropdown(ref player1, ref player2)) {
      return;
    }
    GameHandler.Game = new GameState();
    GameHandler.Game.InitBoardState(player1, player2);
    SceneManager.LoadScene("Backgammon");
  }
}
