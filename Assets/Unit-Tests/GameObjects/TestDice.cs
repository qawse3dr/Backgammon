using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestDiceDie {
  [Test]
  public void Test_Dice() {
    Dice d = new Dice();
    int r;
    for (int i = 0; i < 20; i++) {  // repeat test
      d.RollDice();
      r = d.Roll;
      Assert.True(1 <= r);
      Assert.True(6 >= r);
    }
  }

  [Test]
  public void Test_SetRollSuccess() {
    Dice d = new Dice();
    d.Roll = 3;
    Assert.AreEqual(3, d.Roll);
  }

  [Test]
  public void Test_SetRollFail() {
    Dice d = new Dice();
    Assert.Throws<System.InvalidOperationException>(() => d.Roll = 0);
  }
}
