using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logger = LNAR.Logger;

public class DebugMenu : MonoBehaviour {
  // If this is active the debug menu is show on the screen
  // and everyon should check this before doing any onclicks for
  // debug menu
  public bool DebugMenuActive = false;
  private CanvasGroup canvasGroup_;

  private GameObject _turnObj, _stateObj, _whiteHomeObj, _blackHomeObj, _whiteOnBarObj,
      _blackOnBarObj;

  PlayerEnum _turn;
  GamePhase _gamePhase;
  bool _whiteHome;
  bool _blackHome;
  bool _whiteOnBar;
  bool _blackOnBar;
  // Start is called before the first frame update
  void Start() {
    canvasGroup_ = GetComponentInParent<CanvasGroup>();
    canvasGroup_.alpha = 0;

#if DEBUG_MENU
    _turnObj = gameObject.transform.Find("Turn").gameObject;
    _stateObj = gameObject.transform.Find("TurnState").gameObject;
    _whiteHomeObj = gameObject.transform.Find("WhiteHome").gameObject;
    _blackHomeObj = gameObject.transform.Find("BlackHome").gameObject;
    _whiteOnBarObj = gameObject.transform.Find("WhiteOnBar").gameObject;
    _blackOnBarObj = gameObject.transform.Find("BlackOnBar").gameObject;
#endif
  }

  // Update is called once per frame
  void Update() {
#if DEBUG_MENU
    if (Input.GetKeyDown(KeyCode.Alpha1)) {
      Logger.Debug("Input debug menu instruction");
      if (DebugMenuActive) {
        canvasGroup_.alpha = 0;
      } else {
        canvasGroup_.alpha = 1;
      }
      DebugMenuActive = !DebugMenuActive;
    }
    PlayerEnum turn = GameHandler.Game.GetPlayerTurn();
    if (_turn != turn) {
      if (turn == PlayerEnum.Player1) {
        _turnObj.GetComponent<Text>().text = "Turn: Player 1";
      } else {
        _turnObj.GetComponent<Text>().text = "Turn: Player 2";
      }
      _turn = turn;
    }
    GamePhase gamePhase = GameHandler.Game.GetTurnPhase();
    if (_gamePhase == gamePhase) {
      if (gamePhase == GamePhase.MOVE) {
        _stateObj.GetComponent<Text>().text = "State: Move";
      } else {
        _stateObj.GetComponent<Text>().text = "State: Roll";
      }
      _gamePhase = gamePhase;
    }
    bool whiteHome = GameHandler.Game.GetWhiteHome();
    if (_whiteHome == whiteHome) {
      if (whiteHome == true) {
        _whiteHomeObj.GetComponent<Text>().text = "WhiteHome: 1";
      } else {
        _whiteHomeObj.GetComponent<Text>().text = "WhiteHome: 0";
      }
      _whiteHome = whiteHome;
    }
    bool blackHome = GameHandler.Game.GetBlackHome();
    if (_blackHome == blackHome) {
      if (blackHome == true) {
        _blackHomeObj.GetComponent<Text>().text = "BlackHome: 1";
      } else {
        _blackHomeObj.GetComponent<Text>().text = "BlackHome: 0";
      }
      _blackHome = blackHome;
    }

    bool whiteOnBar = GameHandler.Game.GetWhiteOnBar();
    if (_whiteOnBar == whiteOnBar) {
      if (whiteOnBar == true) {
        _whiteOnBarObj.GetComponent<Text>().text = "WhtOnBar: 1";
      } else {
        _blackHomeObj.GetComponent<Text>().text = "WhtOnBar: 0";
      }
      _blackHome = whiteOnBar;
    }

    bool blackOnBar = GameHandler.Game.GetBlackOnBar();
    if (_blackHome == blackOnBar) {
      if (blackOnBar == true) {
        _blackOnBarObj.GetComponent<Text>().text = "BlkOnBar: 1";
      } else {
        _blackOnBarObj.GetComponent<Text>().text = "BlkOnBar: 0";
      }
      _blackHome = blackOnBar;
    }
#endif
  }
}
