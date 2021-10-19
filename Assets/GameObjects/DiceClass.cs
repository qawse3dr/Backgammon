using UnityEngine;
using System;
using Random = System.Random; // unity and c# both have Random classes

public class Dice {
  private Random randGen;
  private int _roll; // field

  public int _Roll   // property
  {
    get { return _roll; } 
    set { 
        if ((1 <= value) && (value <= 6)){
          _roll = value;
          Debug.Log($"(Dice)(_Roll set) _Roll property used to set value of _roll field to {value}");
        }
        else{
            throw new InvalidOperationException("Attempting to set Dice object's _roll field to out of range value (less than 1 or greater than 6).");
        }
    }
  }

  /*
  Constructor with no arguments.
  */
  public Dice(int seed = 11) {
    Debug.Log("(Dice)Constructor called");
    randGen = new Random(seed); // initialize random number generator
  }

  public void Roll(){
    Debug.Log("(Dice)Dice rolled.");
    _Roll  = randGen.Next(1, 7); // int between 1 and 6
  }

}

