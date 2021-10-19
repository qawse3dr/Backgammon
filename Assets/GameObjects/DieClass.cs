using UnityEngine;
using System;
using Random = System.Random;  // unity and c# both have Random classes
using System.Collections.Generic;
using System.Linq;  // for Enumerable

/*
Represents a set of dice that can all be rolled at the same time.
*/
public class Die {
  // _numDie - the number of die in the set
  private int _numDie;  // field
  public int _NumDie {  // property
    get {
      return _numDie;
    }
    private set {        // this should only be set once when object is instantiated
      if (0 <= value) {  // ensure that the number of die is non-negative
        _numDie = value;
        Debug.Log(
            $"(Die)(_NumDie set) _NumDie property used to set value of _numDie field to {value}");
      } else {
        throw new InvalidOperationException(
            "Attempting to set Die object's _numDie field to out of range value (less than 0).");
      }
    }
  }
  // _die - a list (of length _NumDie) of Dice objects included in the Die set
  private List<Dice> _die;   // field
  private List<Dice> _Die {  // property
    get {
      return _die;
    }
    set {
      _die = value;
    }
  }

  /*
  _rolls - a list of the upwards facing faces of all die in the set at a given point in time
           (integers between 1 and 6)
  */
  private List<int> _rolls;  // field
  public List<int> _Rolls    // property
      {
    get { return _rolls; }
  private
    set { _rolls = value; }
  }
  /*
  _seeds - used for seeding the random number generators in the Dice class. Useful such that all die
           in the set generate different sequences of random rolls.
  */
  private List<int> _seeds;  // field
  private List<int> _Seeds   // property
      {
    get { return _seeds; }
    set {
      if (value.Count == _NumDie) {
        _seeds = value;
      } else {
        throw new InvalidOperationException(
            "Must initiate Die object with list of seed values of same length as dieCount argument.");
      }
    }
  }
  /*
  Class Constructor
  Sets all class variables, in particular, _Die is set to a list of Dice objects, each instantiated
  with a seed value from the _Seeds field, corresponding to its index in the _Die list
  */
  public Die(int dieCount, List<int> seeds) {
    Debug.Log("(Die)Constructor called");
    _NumDie = dieCount;
    _Seeds = seeds;
    _Rolls = new List<int>();
    // set _Die
    List<Dice> d = new List<Dice>();
    foreach (var i in Enumerable.Range(0, _NumDie)) {
      d.Add(new Dice(_Seeds[i]));
    }
    _Die = d;
  }

  /*
  "Rolls" each of the die in the _Die field (calls the Roll methdd on the Dice object) to generate a
  new random integer between 1 and 6. The list of rolls is cleared and then updated with the new
  rolls. Note that a seperate "get" call is needed to retrieve thesa new values.
  */
  public void Roll() {
    Debug.Log($"(Die)Roll: {_NumDie} Dice objects of Die rolled.");
    foreach (Dice d in _Die) {
      d.Roll();
    }
    _Rolls.Clear();
    foreach (Dice d in _Die) {
      _Rolls.Add(d._Roll);
    }
  }
}