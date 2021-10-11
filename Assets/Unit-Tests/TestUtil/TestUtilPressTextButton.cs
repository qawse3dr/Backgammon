using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class TestUtilPressTextButton {
  public static void ClickText(string rayCasterName, string eventSystemName,
                               string textButtonName) {
    // Get Objects from scene
    GameObject playButton = GameObject.Find("PlayButton");
    GameObject UI = GameObject.Find("UI");
    GameObject eventSystem = GameObject.Find("EventSystem");

    Vector3 pos = playButton.transform.localPosition;

    pos.x += playButton.transform.localScale.x / 2;
    pos.y += playButton.transform.localScale.y / 2;
    pos.z += playButton.transform.localScale.z / 2;
    // Create click Event
    PointerEventData point = new PointerEventData(eventSystem.GetComponent<EventSystem>());
    point.pointerClick = playButton;
    point.selectedObject = playButton;
    point.pressPosition = pos;

    Debug.Log("Hit ");

    List<RaycastResult> results = new List<RaycastResult>();
    UI.GetComponent<GraphicRaycaster>().Raycast(point, results);
    foreach (RaycastResult result in results) {
      Debug.Log("Hit " + result.gameObject.name);
    }

    // playButton.GetComponent<MouseClickAndHoverText>().OnPointerClick(null);
  }
}