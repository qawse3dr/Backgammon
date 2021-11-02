using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework.Internal;

// using MenuSystemController;
public class TestMainMenu {
  private MenuSystemController _menuController;

  [UnitySetUp]
  public IEnumerator Setup() {
    SceneManager.LoadScene("MainMenu");
    GameHandler.Game = new GameState();
    yield return new WaitForSeconds(1);

    _menuController = new GameObject("MenuSystemController", typeof(MenuSystemController))
                          .GetComponent<MenuSystemController>();
  }

  // A Test behaves as an ordinary method
  [UnityTest]
  public IEnumerator Test_MainMenuStartGame() {
    // Invoke click
    _menuController.StartGameOnClick();

    // For some reason it only works with 2 Waits but this
    // Wait for the scene to become active
    yield return new WaitForSeconds(1);

    // Test to see if Scene was changed
    Assert.AreEqual(SceneManager.GetActiveScene().name,
                    SceneManager.GetSceneByName("Backgammon").name);
  }

  /** Application can't actually quit so just make sure this doesn't crash */
  [Test]
  public void Test_MainMenuQuitGame() {
    _menuController.QuitGameOnClick();
  }
}
