using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = LNAR.Logger;

public class GameHandler : MonoBehaviour {
  public static GameState Game = null;
  private bool _gameStarted = false;
  // Start is called before the first frame update
  public void Start() {
    _gameStarted = false;
    Logger.Debug($"GameHandler Started ########################");
    if (Game == null)
      Game = new GameState();
  }

  // Update is called once per frame
  void Update() {
    if (!_gameStarted) {
      Game.OnGameStart();
      _gameStarted = true;
    }
  }
}
