using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Logger = LNAR.Logger;
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
public class Piece : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
  private int _boardIndex;
  private bool _onBar;
  private bool _inHome;
  private bool _isPickedUp;

  // Used to reset the piece object if moving it fails
  private Vector2 _previousPostion;
  // The player who's Piece this is
  // TODO remove once owner is set
  private Player _owner = new Player(PlayerEnum.Player1);
  public Player Owner {
    get { return _owner; }
    set {
      _owner = value;
      SetColour();
    }
  }

  public void OnPointerDown(PointerEventData data) {
    if (GameHandler.Game.PlayerTurn == Owner.GetPlayerNum()) {
      // Make sure that its the top piece
      // Due to pieces not being inited yet assume if its not in the
      // list assume that its the top one
      List<Piece>[] piecesList;
      if (Owner.GetPlayerNum() == PlayerEnum.Player1) {
        piecesList = GameHandler.Game.Pieces.WhiteBoard;
      } else {
        piecesList = GameHandler.Game.Pieces.BlackBoard;
      }

      // Checks thats its on the top.
      if (piecesList[_boardIndex - 1].Contains(this)) {
        if (piecesList[_boardIndex - 1].IndexOf(this) != piecesList[_boardIndex - 1].Count - 1) {
          Logger.Warn("Please select the top pieces");
          return;
        }
      }
      Logger.Debug($"Piece Picked Up: {ToString()}", "PIECE");
      PickUpOrDrop();
    } else {
      Logger.Warn("Can't pick up piece you are not its owner");
    }
  }

  public void OnPointerUp(PointerEventData data) {
    if (_isPickedUp) {
      Logger.Debug($"Piece Picked Up: {ToString()}", "PIECE");
      PickUpOrDrop();
    }
  }

  public void SetColour() {
    SpriteRenderer renderer;
    if (TryGetComponent<SpriteRenderer>(out renderer)) {
      renderer.color = Owner.GetPlayerColour();
    } else {
      Logger.Warn($"Unable to get rendered for piece color will not be updated: {ToString()}");
    }
  }

  // Start is called before the first frame update
  public void Start() {
    _onBar = false;
    _inHome = false;
    // Set to one due to having a error when programming drag story
    // As piece Init is not fully done yet
    _boardIndex = 1;
    _isPickedUp = false;
  }

  public void Update() {
    if (_isPickedUp) {
      // There seems to be a bug where the mousePos.z can't be the same as the mouse or else
      // OnClicks Won't work. So a work around is to just set it to 3.
      Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      mousePos.z = -3;
      transform.position = mousePos;
    }
  }

  public bool PickUpOrDrop() {
    Logger.Debug($"Piece PickedUp or Dropped: {ToString()}");
    if (_isPickedUp) {
      // Todo place code
      GameHandler.Game.SetPieceInHand(null);
      _isPickedUp = false;

      // Check for collisions
      foreach (GameObject hitBox in GameObject.FindGameObjectsWithTag("BoardHitBox")) {
        if (hitBox.GetComponent<Collider2D>().OverlapPoint(transform.position)) {  // Hit Found
          // Placing piece on hitbox it came from just assume it failed so it gets reset.
          if (hitBox.GetComponent<BoardHitBox>().BoardIndex == _boardIndex)
            break;

          // Try to move piece return if it failed or not
          if (GameHandler.Game.MovePiece(this, hitBox.GetComponent<BoardHitBox>().BoardIndex)) {
            return true;
          }
          break;
        }
      }

      // Failed to place pieces start move back animation
      // TODO Cool animation ??
      transform.position = _previousPostion;
      return true;

    } else {
      if (GameHandler.Game.SetPieceInHand(this)) {
        _isPickedUp = true;
        _previousPostion = transform.position;
        return true;
      }
    }
    // Failed to pickup or wasn't moved
    return false;
  }

  public void hoverShowMoves() {}

  public void hoverHideMoves() {}

  /**
   * Moves piece to bar, setting all other flags to false or -1 for _boardIndex
   */
  public void MoveToBar() {
    _onBar = true;
    _boardIndex = -1;
    Logger.Info($"Piece move to bar: {ToString()}");
  }

  /* Movies Piece into home. assumes that all checks have already occurred. */
  public void MoveIntoHome() {
    _onBar = false;
    _boardIndex = -1;
    _inHome = true;
    Logger.Info($"Piece move to home: {ToString()}");

    // TODO add moving the piece asset to the correct home
  }
  /**
   * Moves the piece on board index. This will assume that checks have already been done,
   * All other flags will be set to false.
   */
  public void MoveToBoardIndex(int boardIndex) {
    _boardIndex = boardIndex;
    _onBar = false;
    Logger.Info($"Piece moved: {ToString()}");
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
    string ownerName = "Not Set";
    if (Owner != null)
      ownerName = (Owner.PlayerNum == PlayerEnum.Player1) ? "Player1" : "Player2";

    return $"Piece Object: (Owner:{ownerName}, OnBar: {_onBar}, boardIndex: {_boardIndex}, InHome: {_inHome}, PickedUp: {_isPickedUp} )";
  }
}
