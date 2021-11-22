using UnityEngine;
using System;
using Random = System.Random;  // unity and c# both have Random classes
using Logger = LNAR.Logger;
/*
Class to represent a single dice that can be rolled "randomly"
*/
public class Dice {
  // randGen - c# Random object used to generate random values for the rolls of the dice
  private Random randGen;
  // _roll - the upwards facing face of the dice at a given point in time (integer between 1 and 6)
  private int _roll;  // field
  private bool _isGrey;
  public bool IsGrey {
    get { return _isGrey; }
    set {
      _isGrey = value;
      Grey();
      // Do un-grey code here
    }
  }

  public string RollAssetString;
  public int Roll  // property
      {
    get { return _roll; }
    set {
      if ((1 <= value) &&
          (value <= 6)) {  // ensure that the roll is being set to a valid integer between 1 and 6
        _roll = value;
        Logger.Debug($"(Dice)(Roll set) Roll property used to set value of _roll field to {value}");
      } else {
        throw new InvalidOperationException(
            "Attempting to set Dice object's _roll field to out of range value (less than 1 or greater than 6).");
      }
    }
  }

  /*
  Class Constructor
    seed - used for seeding the random number generator. Default value supplied. Useful such that
           multiple die objects can have different seeds and thus different sequences of random
           rolls associated with them.
  */
  public Dice(int seed = 11) {
    Logger.Debug("(Dice)Constructor called");
    randGen = new Random(seed);  // initialize random number generator
  }

  /*
  Changes the _roll field to a new random number between 1 and 6. Note that a seperate "get" call is
  needed to retrieve this new value.
  */
  public void RollDice() {
    Logger.Debug("(Dice)Dice rolled.");
    IsGrey = false;
    Roll = randGen.Next(1, 7);  // int between 1 and 6
  }

  /*
  Will be used once the Dice object is connected to an asset through Monobehaviour. Changes the
  die's color to gray to indicate that a move corresponding to that die has been made.
  */
  public void Grey() {
    var die = GameObject.Find(RollAssetString);
    if (die != null) {
      die.GetComponent<SpriteRenderer>().enabled = true;
    }
    Logger.Debug(
        $"(Dice)Grey {RollAssetString}: Dice background color changed to Grey to indicate that it's rolled has been used.");
  }
}
