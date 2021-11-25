using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Logger = LNAR.Logger;

public class SelectPlayerStatsHandler : MonoBehaviour {
  Dropdown m_Dropdown;
  public Text TotalWinNum;
  public Text TotalLossNum;
  public Text curWinNum;
  public Text curLossNum;
  public Player curPlayer;
  private List<Player> _playerList;
  public List<Player> PlayerList {
    get { return _playerList; }
    set { _playerList = value; }
  }
  public Queue<MatchRecord> curMatchHistory;
  void Start() {
    Database.DB_PATH = "stats.db";
    Database db = Database.CreateDatabase();
    _playerList = db.ReadDB();
    // Fetch the Dropdown GameObject
    m_Dropdown = GetComponent<Dropdown>();
    TotalWinNum = GameObject.Find("TotalWinNum").GetComponent<Text>();
    TotalLossNum = GameObject.Find("TotalLossNum").GetComponent<Text>();
    // set value in dropdown list
    m_Dropdown.ClearOptions();
    List<string> options = new List<string>();
    foreach (Player p in _playerList) {
      options.Add(p.Name);
    }
    m_Dropdown.AddOptions(options);

    // Add listener for when the value of the Dropdown changes, to take action
    m_Dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(m_Dropdown); });

    resetUI();
  }

  void DropdownValueChanged(Dropdown change) {
    // total wins and losses
    curPlayer = _playerList[change.value];
    int wins = curPlayer.Wins;
    int losses = curPlayer.Losses;
    TotalWinNum.GetComponent<UnityEngine.UI.Text>().text = wins.ToString();
    TotalLossNum.GetComponent<UnityEngine.UI.Text>().text = losses.ToString();
    // match history
    curMatchHistory = curPlayer.GetMatchHistory();
    int curWin = 1;
    int curLoss = 1;
    int curGame = 1;
    // reset all grey squares and Loss/WinGame#
    resetUI();
    Logger.Info(curPlayer.ToString());
    foreach (MatchRecord mr in curMatchHistory) {
      Logger.Info("~~~~~~~" + mr.ToString() + "\n");
      if (mr.Winner == curPlayer.Name) {  // win
        // grey square off
        GameObject.Find("WinSquareGrey(" + curWin + ")").GetComponent<Renderer>().enabled =
            false;  //! GameObject.Find("WinSquareGrey(" + curWin +
                    //! ")").GetComponent<Renderer>().enabled;
        // add 'game #' label
        GameObject.Find("WinGame (" + curWin + ")").GetComponent<Text>().text = "Game # ";
        // add game number value
        curWinNum = GameObject.Find("WinGame# (" + curWin + ")").GetComponent<Text>();
        curWinNum.text = curGame.ToString();
        // add opponent
        GameObject.Find("OpponentWin (" + curWin + ")").GetComponent<Text>().text =
            mr.Opponent.ToString();
        curWin++;
        curGame++;
      } else {  // loss
        // grey square off
        Logger.Info("Disabling GreyLossSquare(" + curLoss + ")");
        GameObject.Find("GreyLossSquare (" + curLoss + ")").GetComponent<Renderer>().enabled =
            false;  //! GameObject.Find("GreyLossSquare (" + curLoss +
                    //! ")").GetComponent<Renderer>().enabled;
        Logger.Info("Finding LossGame# (" + curLoss + ")");
        // add 'game #' label
        GameObject.Find("LossGame (" + curLoss + ")").GetComponent<Text>().text = "Game # ";
        // add game number value
        curLossNum = GameObject.Find("LossGame# (" + curLoss + ")").GetComponent<Text>();
        Logger.Info("Setting LossGame# (" + curLoss + ") to " + curGame);
        curLossNum.text = curGame.ToString();
        // add opponent
        GameObject.Find("OpponentLoss (" + curLoss + ")").GetComponent<Text>().text =
            mr.Opponent.ToString();
        curLoss++;
        curGame++;
      }
    }
  }

  private void resetUI() {
    // enable all grey square
    GameObject.Find("WinSquareGrey(1)").GetComponent<Renderer>().enabled = true;
    GameObject.Find("WinSquareGrey(2)").GetComponent<Renderer>().enabled = true;
    GameObject.Find("WinSquareGrey(3)").GetComponent<Renderer>().enabled = true;
    GameObject.Find("WinSquareGrey(4)").GetComponent<Renderer>().enabled = true;
    GameObject.Find("WinSquareGrey(5)").GetComponent<Renderer>().enabled = true;
    GameObject.Find("GreyLossSquare (1)").GetComponent<Renderer>().enabled = true;
    GameObject.Find("GreyLossSquare (2)").GetComponent<Renderer>().enabled = true;
    GameObject.Find("GreyLossSquare (3)").GetComponent<Renderer>().enabled = true;
    GameObject.Find("GreyLossSquare (4)").GetComponent<Renderer>().enabled = true;
    GameObject.Find("GreyLossSquare (5)").GetComponent<Renderer>().enabled = true;
    // set all game # values to empty strings
    GameObject.Find("WinGame# (1)").GetComponent<Text>().text = "";
    GameObject.Find("WinGame# (2)").GetComponent<Text>().text = "";
    GameObject.Find("WinGame# (3)").GetComponent<Text>().text = "";
    GameObject.Find("WinGame# (4)").GetComponent<Text>().text = "";
    GameObject.Find("WinGame# (5)").GetComponent<Text>().text = "";
    GameObject.Find("LossGame# (1)").GetComponent<Text>().text = "";
    GameObject.Find("LossGame# (2)").GetComponent<Text>().text = "";
    GameObject.Find("LossGame# (3)").GetComponent<Text>().text = "";
    GameObject.Find("LossGame# (4)").GetComponent<Text>().text = "";
    GameObject.Find("LossGame# (5)").GetComponent<Text>().text = "";
    // disable all game # labels
    GameObject.Find("WinGame (1)").GetComponent<Text>().text = "";
    GameObject.Find("WinGame (2)").GetComponent<Text>().text = "";
    GameObject.Find("WinGame (3)").GetComponent<Text>().text = "";
    GameObject.Find("WinGame (4)").GetComponent<Text>().text = "";
    GameObject.Find("WinGame (5)").GetComponent<Text>().text = "";
    GameObject.Find("LossGame (1)").GetComponent<Text>().text = "";
    GameObject.Find("LossGame (2)").GetComponent<Text>().text = "";
    GameObject.Find("LossGame (3)").GetComponent<Text>().text = "";
    GameObject.Find("LossGame (4)").GetComponent<Text>().text = "";
    GameObject.Find("LossGame (5)").GetComponent<Text>().text = "";
    // remove opponents
    GameObject.Find("OpponentWin (1)").GetComponent<Text>().text = "";
    GameObject.Find("OpponentWin (2)").GetComponent<Text>().text = "";
    GameObject.Find("OpponentWin (3)").GetComponent<Text>().text = "";
    GameObject.Find("OpponentWin (4)").GetComponent<Text>().text = "";
    GameObject.Find("OpponentWin (5)").GetComponent<Text>().text = "";
    GameObject.Find("OpponentLoss (1)").GetComponent<Text>().text = "";
    GameObject.Find("OpponentLoss (2)").GetComponent<Text>().text = "";
    GameObject.Find("OpponentLoss (3)").GetComponent<Text>().text = "";
    GameObject.Find("OpponentLoss (4)").GetComponent<Text>().text = "";
    GameObject.Find("OpponentLoss (5)").GetComponent<Text>().text = "";
  }
}
