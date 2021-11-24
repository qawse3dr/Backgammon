using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = LNAR.Logger;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour {
  public void Exit() {
    Logger.Info("Redirecting to main menu...");
    SceneManager.LoadScene("MainMenu");
  }
}
