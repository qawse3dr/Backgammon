using UnityEngine;
using System;
using Random = System.Random; // unity and c# both have Random classes
using System.Collections.Generic;
using System.Linq; // for Enumerable

public class Die {
  private int _numDie; // field

  public int _NumDie {   // property
    get { return _numDie; } 
    private set {  // this should only be set once when object is instantiated
        if (0 <= value){
          _numDie = value;
          Debug.Log($"(Die)(_NumDie set) _NumDie property used to set value of _numDie field to {value}");
        }
        else{
            throw new InvalidOperationException("Attempting to set Die object's _numDie field to out of range value (less than 0).");
        }        
    }
  }
  private List<Dice> _die; // field
  private List<Dice> _Die   // property
  {
    get { return _die; } 
    set { _die = value; }
  }

  private List<int> _rolls; // field
  public List<int> _Rolls   // property
  {
    get { return _rolls; } 
    private set { _rolls = value; }
  }  

  private List<int> _seeds; // field
  private List<int> _Seeds   // property
  {
    get { return _seeds; } 
    set { 
        if (value.Count == _NumDie){
            _seeds = value;
        }
        else{
            throw new InvalidOperationException("Must initiate Die object with list of seed values of same length as dieCount argument.");
        }
    }
  }
  public Die(int dieCount, List<int> seeds) {
    Debug.Log("(Die)Constructor called");
    _NumDie = dieCount;
    _Seeds = seeds;
    _Rolls = new List<int> ();
    // set _Die
    List<Dice> d = new List<Dice> ();
    foreach (var i in Enumerable.Range(0, _NumDie)) { d.Add(new Dice(_Seeds[i])); }
    _Die = d;    
  }

  public void Roll(){
    Debug.Log($"(Die)Roll: {_NumDie} Dice objects of Die rolled.");
    foreach (Dice d in _Die) { d.Roll(); }
    _Rolls.Clear();
    foreach (Dice d in _Die) { _Rolls.Add(d._Roll); }
  }

}