using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour {
  public static GameState Game = null;
  // Start is called before the first frame update
  public void Start() {
    if (Game == null)
      Game = new GameState();
  }

  // Update is called once per frame
  void Update() {}
}
