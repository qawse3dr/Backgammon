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
  private Player _owner = null;
  public Player Owner {
    get { return _owner; }
    set {
      _owner = value;
      SetColour();
    }
  }

  // TODO remove once init is done
  public PlayerEnum StartingOwner;

  public void OnPointerDown(PointerEventData data) {
    if (GameHandler.Game.PlayerTurn == Owner.GetPlayerNum() && !_inHome) {
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
      if(_onBar){
        BoardState boardState = GameHandler.Game.GetBoardState();
        if(boardState.OtherBar.IndexOf(this) != boardState.OtherBar.Count - 1){
          Logger.Warn("Please select the top pieces");
          return;
        }
      } else if (piecesList[_boardIndex - 1].Contains(this)) {
        if (piecesList[_boardIndex - 1].IndexOf(this) != piecesList[_boardIndex - 1].Count - 1) {
          Logger.Warn("Please select the top pieces");
          return;
        }
      }
      Logger.Debug($"Piece Picked Up: {ToString()}", "PIECE");
      PickUpOrDrop();
    } else {
      Logger.Warn("Can't pick up piece you are not its owner or its in home");
    }
  }

  public void OnPointerUp(PointerEventData data) {
    if (_isPickedUp) {
      Logger.Debug($"Piece Picked Up: {ToString()}", "PIECE");
      PickUpOrDrop();
    }
  }

  public void SetColour() {
    Color pieceFlat, pieceFlatBorder, pieceOnSide, pieceOnSideBorder;
    if (_inHome) {
      pieceFlat = new Color(0, 0, 0, 0);
      pieceFlatBorder = new Color(0, 0, 0, 0);
      pieceOnSide = Owner.GetPlayerColour();
      pieceOnSideBorder = Color.black;
      transform.localScale = new Vector2(1f, 1f);

    } else {
      pieceOnSide = Owner.GetPlayerColour();
      pieceOnSideBorder = new Color(0, 0, 0, 0);
      pieceFlat = Owner.GetPlayerColour();
      pieceFlatBorder = Color.black;
      transform.localScale = new Vector2(0.55f, 0.55f);
    }
    foreach (Transform t in gameObject.transform) {
      if (t.gameObject.name == "PieceFlat") {
        t.gameObject.GetComponent<SpriteRenderer>().color = pieceFlat;
      } else if (t.gameObject.name == "PieceFlatBorder") {
        t.gameObject.GetComponent<SpriteRenderer>().color = pieceFlatBorder;
      } else if (t.gameObject.name == "PieceOnSideAsset 1") {
        foreach (Transform sideAsset in t.gameObject.transform) {
          if (sideAsset.gameObject.name == "PieceOnSide") {
            sideAsset.gameObject.GetComponent<SpriteRenderer>().color = pieceOnSide;
          } else if (sideAsset.gameObject.name == "PieceOnSideBorder") {
            sideAsset.gameObject.GetComponent<SpriteRenderer>().color = pieceOnSideBorder;
          }
        }
      }
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
    if (Owner == null)
      Owner = GameHandler.Game.GetPlayer(StartingOwner);
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
    BoardState boardState = GameHandler.Game.GetBoardState();
    float deltaX = 0.530822f, deltaY = 0f;
    _onBar = true;
    _boardIndex = -1;
    Logger.Info($"Piece move to bar: {ToString()}");
    boardState.MyBar.Add(this);
    

    if(Owner.PlayerNum == PlayerEnum.Player1){
      deltaY = 1.287123f - (0.55f)*(boardState.MyBar.Count - 1);

    } else if(Owner.PlayerNum == PlayerEnum.Player2) {
      deltaY = -3.885724f + (0.55f)*(boardState.MyBar.Count - 1);
    }
    transform.position = new Vector2(deltaX, deltaY);
  }

  /* Movies Piece into home. assumes that all checks have already occurred. */
  public bool MoveIntoHome() {
    BoardState boardState = GameHandler.Game.GetBoardState();
    TurnState turnState = GameHandler.Game.GetTurnState();

    ///////// error checking ////////////////////

    // Player turn
    if (Owner.GetPlayerNum() != turnState.PlayerTurn) {
      Logger.Warn("Not your turn");
      return false;
    }

    // inHome
    if (!turnState.Home) {
      Logger.Warn("Can't move to home not all your pieces are in home");
      return false;
    }

    ////////// Worked update board /////////////////
    if (boardState.MyBoard[_boardIndex - 1].Contains(this))
      boardState.MyBoard[_boardIndex - 1].Remove(this);

    boardState.MyHome.Add(this);

    /////////// Update sprite pos //////////////
    float deltaX = 0, deltaY = 0;

    if (boardState.MyHome.Count < 8) {
      deltaX = 6.394f;
      deltaY = (boardState.MyHome.Count - 1) * (0.3f);
    } else {
      deltaX = -5.406f;
      deltaY = (boardState.MyHome.Count - 8) * (0.3f);
    }

    if (Owner.PlayerNum == PlayerEnum.Player1) {
      deltaY = 1.398f - deltaY;
    } else {
      deltaY += -3.976f;
    }
    transform.position = new Vector2(deltaX, deltaY);

    //////////// Update Internals /////////////
    _onBar = false;
    _boardIndex = -1;
    _inHome = true;
    SetColour();
    Logger.Info($"Piece move to home: {ToString()}");
    return true;
  }

  /**
   * Moves the piece on board index. This will assume that checks have already been done,
   * All other flags will be set to false.
   */
  public bool MoveToBoardIndex(int boardIndex) {
    BoardState boardState = GameHandler.Game.GetBoardState();
    TurnState turnState = GameHandler.Game.GetTurnState();

    ///////// error checking ////////////////////

    // Player turn
    if (Owner.GetPlayerNum() != turnState.PlayerTurn) {
      Logger.Warn("Not your turn");
      return false;
    }

    if (boardState.OtherBoard[boardIndex - 1].Count > 1) {
      Logger.Info("Invalid Move to many opponent pieces");
      return false;
    }

    ////////// Worked update board /////////////////
    if (_onBar) {
      // TODO remove from onbar
      boardState.OtherBar.Remove(this);

    } else if (boardState.MyBoard[_boardIndex - 1].Contains(this))
      boardState.MyBoard[_boardIndex - 1].Remove(this);

    boardState.MyBoard[boardIndex - 1].Add(this);

    if (boardState.OtherBoard[boardIndex - 1].Count == 1) {
      Logger.Info("Overtaking not implemented yet");
      Piece otherPiece = boardState.OtherBoard[boardIndex - 1][0];
      // TODO remove other piece from board state and implement MoveToBar
      boardState.OtherBoard[boardIndex - 1].Remove(otherPiece);
      otherPiece.MoveToBar();
    }

    /////////// Update sprite pos //////////////
    float deltaX = 0, deltaY = 0;

    if (boardIndex <= 6) {
      deltaX = 5.263f - ((boardIndex - 1) * 0.811925f);
    } else if (boardIndex <= 12) {
      deltaX = -0.49f - ((boardIndex - 7) * 0.811925f);
    } else if (boardIndex <= 18) {
      deltaX = -4.534999f + ((boardIndex - 13) * 0.811925f);
    } else if (boardIndex <= 24) {
      deltaX = 1.234f + ((boardIndex - 19) * 0.811925f);
    }

    deltaY = (boardState.MyBoard[boardIndex - 1].Count - 1) * 0.5460075f;
    // Delta y (top board or bottom board)
    if (boardIndex <= 12) {
      deltaY = -3.87f + deltaY;
    } else if (boardIndex <= 24) {
      deltaY = 1.257f - deltaY;
    }

    transform.position = new Vector2(deltaX, deltaY);

    //////////// Update Internals /////////////

    MoveToBoardIndexNoCheck(boardIndex);
    Logger.Info($"Piece moved: {ToString()}");
    return true;
  }

  public void MoveToBoardIndexNoCheck(int boardIndex) {
    _boardIndex = boardIndex;
    _onBar = false;
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
