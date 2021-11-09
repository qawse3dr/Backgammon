using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class TestMouseClickAndHoverText {
  private Text _textElement;
  private GameObject _object;
  private MouseClickAndHoverText _controller;

  [UnitySetUp]
  public IEnumerator Setup() {
    SceneManager.LoadScene("MainMenu");
    yield return new WaitForSeconds(1);

    _object = new GameObject("Text", typeof(Text));
    _object.AddComponent<MouseClickAndHoverText>();
    _textElement = _object.GetComponent<Text>();
    _controller = _object.GetComponent<MouseClickAndHoverText>();

    // Set colours for hover testing.
    _textElement.color = Color.green;
    _controller.HoverColour = Color.gray;
  }

  // This class is used in Test_MouseOnClick as a onClick wrapper.
  private class TestCallbackMouseClickObj : MonoBehaviour {
    public bool Clicked = false;

    public void OnClick() {
      Clicked = true;
    }
  }

  // A simple test check if the OnClick Get invoked.
  // If TestCallbackMouseClickObj.Clicked = true, the test passed.
  [UnityTest]
  public IEnumerator Test_MouseOnClick() {
    // Create class that will be a wrapper for the OnClick
    TestCallbackMouseClickObj onClick =
        new GameObject("TestCallbackMouseClickObj", typeof(TestCallbackMouseClickObj))
            .GetComponent<TestCallbackMouseClickObj>();

    // Create OnClick
    _controller.OnClick = new UnityEvent();
    UnityAction unityAction = onClick.OnClick;
    _controller.OnClick.AddListener(unityAction);

    // Simulate mouse click
    _controller.OnPointerClick(null);

    // Use the Assert class to test conditions
    yield return new WaitForSeconds(1);

    Assert.AreEqual(true, onClick.Clicked, "OnClick was never triggered for mouse click.");
  }

  [UnityTest]
  public IEnumerator Test_MouseEnterHover() {
    Assert.AreEqual(Color.green, _textElement.color,
                    "Default colour was changed. Please change it back to green for testing.");

    // Simulate hover
    _controller.OnPointerEnter(null);
    yield return new WaitForSeconds(1);
    Assert.AreEqual(Color.gray, _textElement.color, "Colour was not changed on Hover");
  }

  [UnityTest]
  public IEnumerator Test_MouseExitHover() {
    // Set default value
    _controller.OnPointerEnter(null);
    yield return new WaitForSeconds(1);
    Assert.AreEqual(Color.gray, _textElement.color,
                    "Default colour was changed. Please change it back to green for testing.");

    // Simulate hover leaving
    _controller.OnPointerExit(null);
    yield return new WaitForSeconds(1);

    Assert.AreEqual(Color.green, _textElement.color, "Colour was not changed back on Hover exit");
  }
}
