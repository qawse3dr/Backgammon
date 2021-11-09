using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Logger = LNAR.Logger;

/** This script should be attached to a UI>Text object and will control the behaviours
 * of it when the mouse hovers over it as well as adds OnClick support
 * when text is hovered over it will change colour to HoverColour and change back to the
 * default after the mouse hovers away
 */
public class MouseClickAndHoverText : MonoBehaviour,
                                      IPointerClickHandler,
                                      IPointerEnterHandler,
                                      IPointerExitHandler {
  // OnClick's that will be clicked when the text is clicked
  public UnityEvent OnClick;

  // Colour for hover (This should be set in the Unity GUI).
  public Color HoverColour;
  // Text this script gets attached too
  private Text _text;

  // The Default color that will be reverted to when the mouse is no longer hovering
  private Color _defaultColour;

  void Start() {
    // Try to get text component, if it doesn't exist we should exit with error as this script needs
    // a text component
    if (!TryGetComponent<Text>(out _text)) {
      Logger.Error("Required Text Object is not found");
      Application.Quit(1);
    }

    _defaultColour = _text.color;
  }

  /** Inherited from IPointerEnterHandler
   *  This method will be called when the mouse hovers over the text (This should not be called
   * except for by unity, or unit tests)
   *  @param pointerEventData
   */
  public void OnPointerEnter(PointerEventData pointerEventData) {
    _text.color = HoverColour;
  }

  /** Inherited from IPointerExitHandler
   *  This method will be called when the mouse stops hovering over the text (This should not be
   * called except for by unity, or unit tests)
   *  @param pointerEventData
   */
  public void OnPointerExit(PointerEventData pointerEventData) {
    _text.color = _defaultColour;
  }

  /** Inherited from IPointerClickHandler
   *  This method will be called when the mouse clicks on the text (This should not be called except
   * for by unity, or unit tests) Once the click occurs the functions OnClicks will be Invoked.
   *  @param pointerEventData
   */
  public void OnPointerClick(PointerEventData pointerEventData) {
    OnClick.Invoke();
  }
}
