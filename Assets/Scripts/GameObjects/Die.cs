using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;  // for Enumerable
using Logger = LNAR.Logger;

/*
Represents a set of dice that can all be rolled at the same time.
*/
public class Die {
  // NumDie - the number of die in the set
  private int _numDie;  // field
  public int NumDie {   // property
    get {
      return _numDie;
    }
    private set {        // this should only be set once when object is instantiated
      if (0 <= value) {  // ensure that the number of die is non-negative
        _numDie = value;
        Logger.Debug(
            $"(Die)(NumDie set) NumDie property used to set value of _numDie field to {value}");
      } else {
        throw new InvalidOperationException(
            "Attempting to set Die object's NumDie field to out of range value (less than 0).");
      }
    }
  }
  // _die - a list (of length NumDie) of Dice objects included in the Die set
  private List<Dice> _die;   // field
  private List<Dice> _Die {  // property
    get {
      return _die;
    }
    set {
      _die = value;
    }
  }

  // Doubles were rolled
  bool _doubles;

  /*
  _rolls - a list of the upwards facing faces of all die in the set at a given point in time
           (integers between 1 and 6)
  */
  private List<int> _rolls;  // field
  public List<int> Rolls     // property
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
  private List<int> Seeds    // property
      {
    get { return _seeds; }
    set {
      if (value.Count == NumDie) {
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
  with a seed value from the Seeds field, corresponding to its index in the _Die list
  */
  public Die(int dieCount, List<int> seeds) {
    Logger.Debug("(Die)Constructor called");
    NumDie = dieCount;
    Seeds = seeds;
    Rolls = new List<int>();
    _doubles = false;
    // set _Die
    List<Dice> d = new List<Dice>();
    foreach (var i in Enumerable.Range(0, NumDie)) {
      d.Add(new Dice(Seeds[i]));
    }
    _Die = d;
  }

  /*
  "Rolls" each of the die in the _Die field (calls the Roll methdd on the Dice object) to generate a
  new random integer between 1 and 6. The list of rolls is cleared and then updated with the new
  rolls. Note that a seperate "get" call is needed to retrieve thesa new values.
  */
  public void Roll() {
    Logger.Debug($"(Die)1.Roll: {NumDie} Dice objects of Die rolled.");
    _doubles = false;
    foreach (Dice d in _Die) {
      d.RollDice();
    }
    Rolls.Clear();
    foreach (Dice d in _Die) {
      Rolls.Add(d.Roll);
    }
    if (Rolls.All(o => o == Rolls[0])) {  // all die roll to the same values (i.e. doubles)
      Doubles(Rolls[0]);
      _doubles = true;
    }
    Logger.Debug($"(Die)2.Roll: Rolls: (" + string.Join(", ", Rolls.ToArray()) + ")");
  }

  /*
  Handles situation in which doubles are rolled (backgammon rules dictate that when doubles are
  rolled, the player gets 4 moves corresponding to the value rolled). Since this class is general
  for any number of Die, it will append NumDie of the doubles value rolled to Rolls.
  */
  private void Doubles(int roll) {
    foreach (var i in Enumerable.Range(0, NumDie)) {
      Rolls.Add(roll);
    }
  }

  /*
  Remove a single Dice's roll from the list of available rolls and calls "Grey" method of the given
  Dice object. Does not remove the Dice object from the list as the Dice will be reused between
  players by "re-rolling".
  */
  public void ClearRoll(int roll) {
    // error checking
    if (!Rolls.Contains(roll)) {
      throw new InvalidOperationException(
          "Attempting to remove a roll for Die object that does not exist (or has already been removed).");
    } else {
      int index = Rolls.IndexOf(roll);
      Logger.Debug($"(Die)ClearRoll: {roll} from available rolls removed.");
      Rolls.Remove(roll);

      // If die are used up, switch to next player's turn
      if (Rolls.Count == 0) {
        GameHandler.Game.ChangeCurrentPlayer();
      }
      // Greys out the die if it isn't a double and if it is only do it on 0 and 2
      if (!_doubles || (_doubles && (Rolls.Count() == 0 || Rolls.Count() == 2))) {
        _Die[index].IsGrey = true;
      }
    }
  }

  public string ToString(string indentOut) {
    string indentIn = indentOut + "\t";
    string str = indentIn + $"Num die: {NumDie}\n" + indentIn + $"Available rolls: (" +
                 string.Join(", ", Rolls.ToArray()) + ")";
    return str;
  }

#if DEBUG_MENU

  // NOT COMPLETE
  public void SetDieValue(int oldValue, int newValue) {
    int index = Rolls.IndexOf(oldValue);
    if (index != -1) {
      Rolls[index] = newValue;
    }
  }

  // NOT COMPLETE
  public void ChangeDieState(int dieNum, bool isActive, int value) {
    if (isActive) {
    } else {
      ClearRoll(value);
      if (_doubles) {
        if (!_die[0].IsGrey) {
        }
      }
    }
  }
#endif
}