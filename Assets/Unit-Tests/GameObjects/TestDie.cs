using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Logger = LNAR.Logger;

public class TestDie {
  [UnitySetUp]
  public IEnumerator Setup() {
    SceneManager.LoadScene("Backgammon");
    GameHandler.Game = TestUtil.CreateGameState();
    yield return new WaitForSeconds(1);
  }

  [Test]
  public void Test_DieInit() {
    List<int> seeds = new List<int> { 11, 517 };
    Die d = new Die(2, seeds);  // init 2 die (the number of die that will be used in the game)
    Assert.AreEqual(2, d.NumDie);
    List<int> seeds2 = new List<int> {};
    Die d2 = new Die(0, seeds2);
    Assert.AreEqual(0, d2.NumDie);
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
      List<int> rolls = d.Rolls;
      foreach (int r in rolls) {
        Assert.True(1 <= r);
        Assert.True(6 >= r);
      }
    }
  }

  [Test]
  public void Test_ClearRollOrder1() {
    List<int> seeds = new List<int> { 1, 2 };
    Die d = new Die(2, seeds);
    d.Roll();
    List<int> rolls = d.Rolls;
    int numRolls = rolls.Count;
    // remove first roll
    d.ClearRoll(rolls[0]);
    Assert.AreEqual(rolls.Count, numRolls - 1);
    Assert.AreEqual(d.NumDie, 2);
    // remove second roll
    d.ClearRoll(rolls[0]);
    Assert.AreEqual(rolls.Count, numRolls - 2);
    Assert.AreEqual(d.NumDie, 2);
  }

  [Test]
  public void Test_ClearRollOrder2() {
    List<int> seeds = new List<int> { 1, 2 };
    Die d = new Die(2, seeds);
    d.Roll();
    List<int> rolls = d.Rolls;
    int numRolls = rolls.Count;
    // remove second roll
    d.ClearRoll(rolls[1]);
    Assert.AreEqual(rolls.Count, numRolls - 1);
    Assert.AreEqual(d.NumDie, 2);
    // remove first roll
    d.ClearRoll(rolls[0]);
    Assert.AreEqual(rolls.Count, numRolls - 2);
    Assert.AreEqual(d.NumDie, 2);
  }

  [Test]
  public void Test_ClearRollFail() {
    List<int> seeds = new List<int> { 1, 2 };
    Die d = new Die(2, seeds);
    d.Roll();
    List<int> rolls = d.Rolls;
    int numRolls = rolls.Count;
    // (5, 2) is always rolled first with the given seeds so try removing something other than these
    // values
    Assert.Throws<System.InvalidOperationException>(() => d.ClearRoll(1));
  }

  // this test passes as long as no errors are thrown
  [Test]
  public void Test_ToString() {
    List<int> seeds = new List<int> { 1, 2 };
    Die d = new Die(2, seeds);
    string str1 = d.ToString("");
    Logger.Debug(str1);
    d.Roll();
    str1 = d.ToString("");
    Logger.Debug(str1);
  }
}
