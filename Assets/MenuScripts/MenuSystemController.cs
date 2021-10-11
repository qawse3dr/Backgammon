using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/** Menu Class should deal with all the main logic included in the menu
 * including but not limited to:
 *   OnClicks
 *   Changing Scene functionality
 */
public class MenuSystemController : MonoBehaviour {
  public void StartGameOnClick() {
    Debug.Log("Starting Game Scene...");
    SceneManager.LoadScene("Backgammon");
  }

  public void QuitGameOnClick() {
    Debug.Log("Exiting Game...");
    Application.Quit();
  }
}
