using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logger = LNAR.Logger;

public class DebugMenu : MonoBehaviour {
  // If this is active the debug menu is show on the screen
  // and everyon should check this before doing any onclicks for
  // debug menu
  public bool DebugMenuActive = false;
  private CanvasGroup canvasGroup_;
  // Start is called before the first frame update
  void Start() {
    canvasGroup_ = GetComponentInParent<CanvasGroup>();
    canvasGroup_.alpha = 0;
  }

  // Update is called once per frame
  void Update() {
    if (Input.GetKeyDown(KeyCode.Alpha1)) {
      Logger.Debug("Input debug menu instruction");
      if (DebugMenuActive) {
        canvasGroup_.alpha = 0;
      } else {
        canvasGroup_.alpha = 1;
      }
      DebugMenuActive = !DebugMenuActive;
    }
#if DEBUG_MENU
#endif
  }
}
