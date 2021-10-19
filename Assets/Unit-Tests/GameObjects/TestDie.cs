using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestDie {
  [Test]
  public void Test_DieInit() {
    List<int> seeds = new List<int> { 11, 517 };
    Die d = new Die(2, seeds);  // init 2 die (the number of die that will be used in the game)
    Assert.AreEqual(2, d._NumDie);
    List<int> seeds2 = new List<int> {};
    Die d2 = new Die(0, seeds2);
    Assert.AreEqual(0, d2._NumDie);
  }

  [Test]
  public void Test_DieFail() {
    Die d;
    List<int> seeds = new List<int> { 1, 2 };
    Assert.Throws<System.InvalidOperationException>(() => d = new Die(-1, seeds));
  }

  [Test]
  public void Test_DieRoll() {
    List<int> seeds = new List<int> { 1, 2 };
    Die d = new Die(2, seeds);
    for (int i = 0; i < 20; i++) {
      d.Roll();
      List<int> rolls = d._Rolls;
      foreach (int r in rolls) {
        Assert.True(1 <= r);
        Assert.True(6 >= r);
      }
    }
  }

  // // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
  // // `yield return null;` to skip a frame.
  // [UnityTest]
  // public IEnumerator TestDieWithEnumeratorPasses()
  // {
  //     // Use the Assert class to test conditions.
  //     // Use yield to skip a frame.
  //     yield return null;
  // }
}
