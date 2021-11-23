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
  void Start()
  {
    _playerList = setUpPlayerList();
    //Fetch the Dropdown GameObject
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

    //Add listener for when the value of the Dropdown changes, to take action
    m_Dropdown.onValueChanged.AddListener(delegate {
        DropdownValueChanged(m_Dropdown);
    });

    resetUI();

  }
  //Ouput the new value of the Dropdown into Text
  void DropdownValueChanged(Dropdown change)
  {
    Logger.Info("HEREEEEEE");
    // total wins and losses
    curPlayer = _playerList[change.value];
    int wins = curPlayer.Wins;
    int losses = curPlayer.Losses;
    TotalWinNum.GetComponent<UnityEngine.UI.Text>().text = wins.ToString();
    TotalLossNum.GetComponent<UnityEngine.UI.Text>().text = losses.ToString();
    // match history
    curMatchHistory = curPlayer._matchHistory; // FIXME 
    Logger.Info("***" + curMatchHistory.Count + "***");
    int curWin = 1;
    int curLoss = 1;
    int curGame = 1;
    // reset all grey squares and Loss/WinGame#
    resetUI();
    Logger.Info(curPlayer.ToString());
    foreach (MatchRecord mr in curMatchHistory)
    {
        Logger.Info("~~~~~~~" + mr.ToString() + "\n");
        if (mr.Winner == curPlayer.Name){ // win
          // grey square off
          Logger.Info("Disabling WinSquareGrey(" + curWin + ")");
          GameObject.Find("WinSquareGrey(" + curWin + ")").GetComponent<Renderer>().enabled = false; //!GameObject.Find("WinSquareGrey(" + curWin + ")").GetComponent<Renderer>().enabled; 
          Logger.Info("Finding WinGame# (" + curWin + ")");
          Logger.Info("Setting WinGame# (" + curWin + ") to " + curGame);
          // add 'game #' label
          GameObject.Find("WinGame (" + curWin + ")").GetComponent<Text>().text = "Game # ";
          // add game number value
          curWinNum = GameObject.Find("WinGame# (" + curWin + ")").GetComponent<Text>();
          curWinNum.text = curGame.ToString();
          // add opponent
          GameObject.Find("OpponentWin (" + curWin + ")").GetComponent<Text>().text = mr.Opponent.ToString();
          curWin++;
          curGame++; 
        }
        else{ // loss
          // grey square off
          Logger.Info("Disabling GreyLossSquare(" + curLoss + ")"); 
          GameObject.Find("GreyLossSquare (" + curLoss + ")").GetComponent<Renderer>().enabled = false; //!GameObject.Find("GreyLossSquare (" + curLoss + ")").GetComponent<Renderer>().enabled;
          Logger.Info("Finding LossGame# (" + curLoss + ")");
          // add 'game #' label
          GameObject.Find("LossGame (" + curLoss + ")").GetComponent<Text>().text = "Game # ";
          // add game number value
          curLossNum = GameObject.Find("LossGame# (" + curLoss + ")").GetComponent<Text>();
          Logger.Info("Setting LossGame# (" + curLoss + ") to " + curGame);
          curLossNum.text = curGame.ToString();
          // add opponent
          GameObject.Find("OpponentLoss (" + curLoss + ")").GetComponent<Text>().text = mr.Opponent.ToString();
          curLoss++;
          curGame++; 
        }
    }
  }

  private void resetUI(){
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
  List<Player> setUpPlayerList(){
    // setup database
    Database.DB_PATH = "stats.db";
    Player pAjit = Player.CreateNewPlayerUnitTest("Ajit", PlayerEnum.NotSet, 6, 9);
    Player pLarry = Player.CreateNewPlayerUnitTest("Larry", PlayerEnum.NotSet, 5, 3);
    Player pRachel = Player.CreateNewPlayerUnitTest("Rachel", PlayerEnum.NotSet, 1, 2);
    Player pNuman = Player.CreateNewPlayerUnitTest("Numan", PlayerEnum.NotSet, 7, 5);
    // add match records
    pAjit.AddMatchHistory(new MatchRecord(pAjit, pLarry, pLarry));
    pLarry.AddMatchHistory(new MatchRecord(pLarry, pAjit, pLarry));

    pAjit.AddMatchHistory(new MatchRecord(pAjit, pRachel, pRachel));
    pRachel.AddMatchHistory(new MatchRecord(pRachel, pAjit, pRachel));

    pAjit.AddMatchHistory(new MatchRecord(pAjit, pRachel, pAjit));
    pRachel.AddMatchHistory(new MatchRecord(pRachel, pAjit, pAjit));

    pNuman.AddMatchHistory(new MatchRecord(pNuman, pRachel, pRachel));
    pRachel.AddMatchHistory(new MatchRecord(pRachel, pNuman, pRachel));

    pNuman.AddMatchHistory(new MatchRecord(pNuman, pAjit, pAjit));
    pAjit.AddMatchHistory(new MatchRecord(pAjit, pNuman, pAjit));

    pNuman.AddMatchHistory(new MatchRecord(pNuman, pAjit, pNuman));
    pAjit.AddMatchHistory(new MatchRecord(pAjit, pNuman, pNuman));

    pNuman.AddMatchHistory(new MatchRecord(pNuman, pAjit, pNuman));
    pAjit.AddMatchHistory(new MatchRecord(pAjit, pNuman, pNuman));

    pNuman.AddMatchHistory(new MatchRecord(pNuman, pLarry, pNuman));
    pLarry.AddMatchHistory(new MatchRecord(pLarry, pNuman, pNuman));

    pNuman.AddMatchHistory(new MatchRecord(pNuman, pAjit, pNuman));
    pAjit.AddMatchHistory(new MatchRecord(pAjit, pNuman, pNuman));

    pNuman.AddMatchHistory(new MatchRecord(pNuman, pRachel, pRachel));
    pRachel.AddMatchHistory(new MatchRecord(pRachel, pNuman, pRachel));

    pLarry.AddMatchHistory(new MatchRecord(pLarry, pRachel, pRachel));
    pRachel.AddMatchHistory(new MatchRecord(pRachel, pLarry, pRachel));

    pLarry.AddMatchHistory(new MatchRecord(pLarry, pAjit, pAjit));
    pAjit.AddMatchHistory(new MatchRecord(pAjit, pLarry, pAjit));

    pLarry.AddMatchHistory(new MatchRecord(pLarry, pAjit, pAjit));
    pAjit.AddMatchHistory(new MatchRecord(pAjit, pLarry, pAjit));

    pLarry.AddMatchHistory(new MatchRecord(pLarry, pAjit, pLarry));
    pAjit.AddMatchHistory(new MatchRecord(pAjit, pLarry, pLarry));

    pNuman.AddMatchHistory(new MatchRecord(pNuman, pRachel, pRachel));
    pRachel.AddMatchHistory(new MatchRecord(pRachel, pNuman, pRachel));

    pAjit.AddMatchHistory(new MatchRecord(pAjit, pRachel, pRachel));
    pRachel.AddMatchHistory(new MatchRecord(pRachel, pAjit, pRachel));

    pLarry.AddMatchHistory(new MatchRecord(pLarry, pRachel, pRachel));
    pRachel.AddMatchHistory(new MatchRecord(pRachel, pLarry, pRachel));
    Database db = Database.CreateDatabase();
    db.WritePlayerToDB(pAjit);
    db.WritePlayerToDB(pLarry);
    db.WritePlayerToDB(pRachel);
    db.WritePlayerToDB(pNuman);
    
    _playerList = db.ReadDB();
  
    return _playerList;
  }
  
}


