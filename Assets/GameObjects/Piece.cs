using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This will give infomation about where the Piece is on
 * the board.
 */
public struct PieceStatus {
  // Where
  public enum PieceLocationEnum { OnBar, OnBoard, inHome }
  public PieceLocationEnum PieceLocation;

  // This is only used if PieceLocation is onBoard.
  public int BoardIndex;
}
public class Piece : MonoBehaviour {
  private int _boardIndex;
  private bool _onBar;
  private bool _inHome;
  private bool _isPickedUp;

  // The player who's Piece this is
  public Player Owner;

  // Start is called before the first frame update
  public void Start() {
    _onBar = false;
    _inHome = false;
    _boardIndex = 0;
    _isPickedUp = false;

    SpriteRenderer renderer;
    if (TryGetComponent<SpriteRenderer>(out renderer)) {
      renderer.color = Owner.GetPlayerColour();
      Debug.Log($"Test: {ToString()}");
    } else {
      Debug.Log($"Warn: Unable to get rendered for piece color will not be updated: {ToString()}");
    }
  }

  public bool PickUpOrDrop() {
    Debug.Log($"Piece PickedUp or Dropped: {ToString()}");
    return true;
  }

  public void hoverShowMoves() {}

  public void hoverHideMoves() {}

  /**
   * Moves piece to bar, setting all other flags to false or -1 for _boardIndex
   */
  public void MoveToBar() {
    _onBar = true;
    _boardIndex = -1;
    Debug.Log($"Piece move to bar: {ToString()}");
  }

  /* Movies Piece into home. assumes that all checks have already occurred. */
  public void MoveIntoHome() {
    _onBar = false;
    _boardIndex = -1;
    _inHome = true;
    Debug.Log($"Piece move to home: {ToString()}");

    // TODO add moving the piece asset to the correct home
  }
  /**
   * Moves the piece on board index. This will assume that checks have already been done,
   * All other flags will be set to false.
   */
  public void MoveToBoardIndex(int boardIndex) {
    _boardIndex = boardIndex;
    _onBar = false;
    Debug.Log($"Piece moved: {ToString()}");
  }

  public PieceStatus GetPieceStatus() {
    PieceStatus status = new PieceStatus();
    if (_onBar) {
      status.PieceLocation = PieceStatus.PieceLocationEnum.OnBar;
    } else if (_inHome) {
      status.PieceLocation = PieceStatus.PieceLocationEnum.inHome;
    } else {
      status.PieceLocation = PieceStatus.PieceLocationEnum.OnBoard;
      status.BoardIndex = _boardIndex;
    }
    return status;
  }

  public override string ToString() {
    string ownerName = (Owner.PlayerNum == PlayerEnum.Player1) ? "Player1" : "Player2";
    return $"Piece Object: (Owner:{ownerName}, OnBar: {_onBar}, boardIndex: {_boardIndex}, InHome: {_inHome}, PickedUp: {_isPickedUp} )";
  }
}
