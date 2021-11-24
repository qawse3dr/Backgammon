using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = LNAR.Logger;

public class RollSequence : MonoBehaviour {
  private IEnumerator coroutine;
  private System.Random r;
  // Start is called before the first frame update
  void Start() {
    r = new System.Random(System.Environment.TickCount ^
                          this.name[this.name.Length - 1]);  // use dice number as part of seed so
                                                             // that each die rolls differently
  }

  public void SequenceStart(BackgammonUIController uiController) {
    List<UnityEngine.Sprite> allDice =
        new List<UnityEngine.Sprite>() { uiController.Die1, uiController.Die2, uiController.Die3,
                                         uiController.Die4, uiController.Die5, uiController.Die6 };
    // show some random dies to simulate rolling
    coroutine = RandomRollCoroutine(uiController, allDice);
    StartCoroutine(coroutine);
  }

  // reference: https://docs.unity3d.com/ScriptReference/MonoBehaviour.StartCoroutine.html
  IEnumerator RandomRollCoroutine(BackgammonUIController uiController,
                                  List<UnityEngine.Sprite> allDice) {
    Logger.Info("Started Coroutine at timestamp : " + Time.time);
    int dIndex;
    for (int i = 0; i < 10; i++) {
      dIndex = r.Next(0, 6);  // for ints
      gameObject.GetComponent<SpriteRenderer>().sprite = allDice[dIndex];
      // yield on a new YieldInstruction that waits for 5 seconds.
      yield return new WaitForSeconds(0.1f);
    }
    GameHandler.Game.RollDice(
        uiController);  // call next step from inside coroutine so it waits for random rolls to
                        // finish before allowing player to move
  }
}