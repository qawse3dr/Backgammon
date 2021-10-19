using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour {
  public static GameState Game;
  // Start is called before the first frame update
  void Start() {
    Game = new GameState();
  }

  // Update is called once per frame
  void Update() {}
}
