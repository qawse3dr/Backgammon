
using UnityEngine;

/**
    This Enum will be used to keep track of which player
    the Piece belongs to as well has whose turn it is.
*/
public enum PlayerEnum { Player1, Player2, NotSet }

/**
 * Holds all useful information about a given player
 * Players can either be player1 or player2
 * based on PlayerEnum, but can also have unique
 * attributes for example Piece colour, name, stats
 */
public class Player {
  public PlayerEnum PlayerNum = PlayerEnum.NotSet;

  public PlayerEnum GetPlayerNum(){
    return PlayerNum;
  }
  public Player(PlayerEnum pNum) {
    this.PlayerNum = pNum;
  }

  /** Gets the Piece Color in the future this will be
   *  based of profiles colour but for now it is just hard coded
   */
  public Color GetPlayerColour() {
    if (PlayerNum == PlayerEnum.Player1) {
      return Color.white;
    } else if (PlayerNum == PlayerEnum.Player2) {
      return Color.black;
    } else {
      return Color.green;
    }
  }

  public override string ToString(){
    return "insert player info here";
  }
}