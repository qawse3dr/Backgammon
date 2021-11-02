
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logger = LNAR.Logger;

public class DebugMenuOnClickHandler : MonoBehaviour {
  // If this is active the debug menu is show on the screen
  // and everyon should check this before doing any onclicks for
  // debug men
  private DebugMenu debugMenu_;
  // Start is called before the first frame update
  void Start() {
    debugMenu_ = GetComponentInParent<DebugMenu>();
  }

  private void Update() {}

  /**
   * GameObject obj will be the text object that is clicked
   * so the text on the debug menu can be updated*/
  public void OnClickChangeTurn(GameObject obj) {
    if (!debugMenu_.DebugMenuActive)
      return;
    Text text = obj.GetComponent<Text>();
    if (text.text == "Turn: Player 1") {
      text.text = "Turn: Player 2";
    } else {
      text.text = "Turn: Player 1";
    }
    GameObject turnState = GameObject.Find("TurnState");
    if (turnState.GetComponent<Text>().text == "State: Move") {
      OnClickChangeTurnState(turnState);
    }
    GameHandler.Game.ChangeCurrentPlayer();
  }

  /**
   * GameObject obj will be the text object that is clicked
   * so the text on the debug menu can be updated
   */
  public void OnClickChangeTurnState(GameObject obj) {
    if (!debugMenu_.DebugMenuActive)
      return;
    GamePhase phase;
    Text text = obj.GetComponent<Text>();
    if (text.text == "State: Roll") {
      text.text = "State: Move";
      phase = GamePhase.MOVE;
    } else {
      text.text = "State: Roll";
      phase = GamePhase.ROLL;
    }
    GameHandler.Game.ChangeState(phase);
  }

  /**
   * GameObject obj will be the text object that is clicked
   * so the text on the debug menu can be updated
   */
  public void OnClickChangeWhiteHome(GameObject obj) {
    if (!debugMenu_.DebugMenuActive)
      return;
    bool isHome;
    Text text = obj.GetComponent<Text>();
    if (text.text == "WhiteHome: 0") {
      text.text = "WhiteHome: 1";
      isHome = true;
    } else {
      text.text = "WhiteHome: 0";
      isHome = false;
    }
    GameHandler.Game.ChangeWhiteHome(isHome);
  }

  /**
   * GameObject obj will be the text object that is clicked
   * so the text on the debug menu can be updated
   */
  public void OnClickChangeBlackHome(GameObject obj) {
    if (!debugMenu_.DebugMenuActive)
      return;
    bool isHome;
    Text text = obj.GetComponent<Text>();
    if (text.text == "BlackHome: 0") {
      text.text = "BlackHome: 1";
      isHome = true;
    } else {
      text.text = "BlackHome: 0";
      isHome = false;
    }
    GameHandler.Game.ChangeBlackHome(isHome);
  }

  /**
   * GameObject obj will be the text object that is clicked
   * so the text on the debug menu can be updated
   */
  public void OnClickChangeWhiteOnBar(GameObject obj) {
    if (!debugMenu_.DebugMenuActive)
      return;
    bool onBar;
    Text text = obj.GetComponent<Text>();
    if (text.text == "WhtOnBar: 0") {
      text.text = "WhtOnBar: 1";
      onBar = true;
    } else {
      text.text = "WhtOnBar: 0";
      onBar = false;
    }
    GameHandler.Game.ChangeWhiteOnBar(onBar);
  }

  /**
   * GameObject obj will be the text object that is clicked
   * so the text on the debug menu can be updated
   */
  public void OnClickChangeBlackOnBar(GameObject obj) {
    if (!debugMenu_.DebugMenuActive)
      return;
    bool onBar;
    Text text = obj.GetComponent<Text>();
    if (text.text == "BlkOnBar: 0") {
      text.text = "BlkOnBar: 1";
      onBar = true;
    } else {
      text.text = "BlkOnBar: 0";
      onBar = false;
    }
    GameHandler.Game.ChangeBlackOnBar(onBar);
  }
}
