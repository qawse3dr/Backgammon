using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = LNAR.Logger;

public class GameHandler : MonoBehaviour {
  public static GameState Game = null;
  // Start is called before the first frame update
  public void Start() {
    Logger.Debug($"GameHandler Started ########################");
    if (Game == null)
      Game = new GameState();
  }

  // Update is called once per frame
  void Update() {}
}
