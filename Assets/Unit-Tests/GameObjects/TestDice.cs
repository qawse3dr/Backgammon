using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class TestDiceDie
{
    [Test]
    public void Test_Dice()
    {
        Dice d = new Dice();
        int r;
        for(int i = 0; i < 20; i++){ // repeat test
          d.Roll();
          r = d._Roll;
          Assert.True(1 <= r);
          Assert.True(6 >= r);
        } 
    }

    [Test]
    public void Test_SetRollSuccess()
    {
        Dice d = new Dice();
        d._Roll = 3;
        Assert.AreEqual(3, d._Roll);
    }

    [Test]
    public void Test_SetRollFail()
    {
        Dice d = new Dice();
        Assert.Throws<System.InvalidOperationException>(() => d._Roll = 0);

    }    

    // // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // // `yield return null;` to skip a frame.
    // [UnityTest]
    // public IEnumerator TestDiceDieWithEnumeratorPasses()
    // {
    //     // Use the Assert class to test conditions.
    //     // Use yield to skip a frame.
    //     yield return null;
    // }
}
