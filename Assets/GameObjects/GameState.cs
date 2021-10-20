using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/*
For a given player currently playing, that player could either be in the "roll phase" or "move
phase" of their turn. ROLL = roll phase, waiting for player to roll the die or while die is being
rolled MOVE = die have been rolled and the player has not used both of their turns (one turn for
each die) or the player has not yet been forced to (and accepted) pass
*/
public enum GamePhase { ROLL, MOVE }

/*
Holds all the information needed to fully describe the state of the given "turn" of the game
    OnBar     - indicates whether or not the current player has any pieces on the bar. This is
                important becaue the rules of backgammon dictate that if a player has a piece
                on the bar, they have to use their turn(s) to get it off before they can move
                any of their pieces on the board
    Home      - indicates if the current player has all their pieces in their home section of
                the board. According to the rules of backgammon, a player can only start
                "bearing off" when all their pieces are in their home
    GameOver  - whether game is complete (a winner or loser has been determined or neither player
                can play for any roll) or still in play PlayerTurn - which player is currently
                playing i.e. which player is allowed to take actions in the game at the current
                time.
    GamePhase - roll phase vs move phase of turn for current player (specified by PlayerTurn)
*/
public struct TurnState {
  public bool GameOver;
  public bool OnBar;
  public bool Home;
  public PlayerEnum PlayerTurn;
  public GamePhase GamePhase;
  public TurnState(bool GameOver, bool OnBar, bool Home, PlayerEnum PlayerTurn,
                   GamePhase GamePhase) {
    this.GameOver = GameOver;
    this.OnBar = OnBar;
    this.Home = Home;
    this.PlayerTurn = PlayerTurn;
    this.GamePhase = GamePhase;
  }
  public string ToString(string indentOut = "") {
    string indentIn = indentOut + "\t";
    string ts = "\n" + indentIn + $"(GameOver: {GameOver}" + "\n" + indentIn + $"OnBar: {OnBar}" +
                "\n" + indentIn + $"Home: {Home}" + "\n" + indentIn +
                "PlayerTurn: " + PlayerTurn.ToString() + "\n" + indentIn +
                "GamePhase: " + GamePhase.ToString() + ")";
    return ts;
  }
};

/*
Holds all the piece objects of the game, organized by their location.
    WhiteBoard - 2-dimensional array; first dimension represents the points on the board. Second
                 dimension holds all the white pieces on the given point. Points are numbered
                 according to the rules of backgammon: point #1 is on the bottom right of the
                 board, increasing to the left, up to point #12. Point #13 is located at the top
                 left of the board and points increase along the top to the right, ending with
                 point #24 at the top left.
    BlackBoard - same as WhiteBoard except to hold the black pieces on the points.
    WhiteBar   - array of piece objects to hold all white pieces located on the bar.
    BlackBar   - array of piece objects to hold all black pieces located on the bar.
    WhiteOff   - array of piece objects to hold all white pieces that have been "beared off" i.e.
                 those that are not located on the board or the bar and are out of play.
    BlackOff   - array of piece objects to hold all black pieces that have been "beared off" i.e.
                 those that are not located on the board or the bar and are out of play.
*/
public struct PieceState {
  public List<Piece>[] BlackBoard;
  public List<Piece>[] WhiteBoard;
  public List<Piece> WhiteBar;
  public List<Piece> BlackBar;
  public List<Piece> WhiteOff;
  public List<Piece> BlackOff;

  // A reference to the current Piece in hand
  // If this is set to null it means no piece is
  // being held, and a piece can only be picked up
  // if this is set to null.
  public Piece PieceInHand;
  public PieceState(List<Piece>[] BlackBoard, List<Piece>[] WhiteBoard, List<Piece> WhiteBar,
                    List<Piece> BlackBar, List<Piece> WhiteOff, List<Piece> BlackOff) {
    this.BlackBoard = BlackBoard;
    this.WhiteBoard = WhiteBoard;
    this.WhiteBar = WhiteBar;
    this.BlackBar = BlackBar;
    this.WhiteOff = WhiteOff;
    this.BlackOff = BlackOff;
    this.PieceInHand = null;
  }
  public string ToString(string indentOut = "") {
    string indentIn = indentOut + "\t";
    string psString = "\n" + indentIn + "BlackBoard:\n" +
                      BoardPiecesToString(BlackBoard, indentIn) + indentIn + "WhiteBoard:\n" +
                      BoardPiecesToString(WhiteBoard, indentIn) + indentIn + "WhiteBar:\n" +
                      PiecesToString(WhiteBar, indentIn) + indentIn + "BlackBar:\n" +
                      PiecesToString(BlackBar, indentIn) + indentIn + "WhiteOff:\n" +
                      PiecesToString(WhiteOff, indentIn) + indentIn + "BlackOff:\n" +
                      PiecesToString(BlackOff, indentIn);
    return psString;
  }
  private string BoardPiecesToString(List<Piece>[] ps, string indentOut = "") {
    string indentIn = indentOut + "\t";
    string bp = "";
    for (int i = 1; i <= ps.Length; i++) {
      bp += indentIn + $"Point #{i}:\n" + PiecesToString(ps[i - 1], indentIn);
    }
    return bp;
  }
  private string PiecesToString(List<Piece> ps, string indentOut = "") {
    string pieces = "";
    string indentIn = indentOut + "\t";
    for (int i = 1; i <= ps.Count; i++) {
      pieces += indentIn + ps[i].ToString() + "\n";
    }
    return pieces;
  }
}

/* The GameState epic encompasses everything related to the flow of the game in the backend and
   holds all information needed to describe the current status of the game (i.e. reference to pieces
   of each team, current player, etc.). The GameState class holds the implementation for all
   substories related to the GameState epic. The GameState class is a code implementation of the UML
   diagrams created for issue #13.
   Attributes:
   _blackOnBar - boolean to indicate whether or not there are any black pieces on the bar (true if
                 length of BlackBar within PieceState struct for _pieces attribute has length
                 greater than 0) _whiteOnBar - boolean to indicate whether or not there are any
                 white pieces on the bar (true if length of WhiteBar within PieceState struct for
                 _pieces attribute has length greater than 0)
  _blackHome   - boolean to indicate whether or not all black pieces have reached the black home
                 quadrant of the board (or if this has been accomplished and pieces have since been
                 beared off the board)
  _whiteHome   - boolean to indicate whether or not all white pieces have reached the white home
                 quadrant of the board (or if this has been accomplished and pieces have since been
                 beared off the board)
  _pieces      - struct of type PieceState which holds arrays of Piece objects divided up by
                 location groupings that the pieces can fall into
  _die         - a Die object which holds 2 Dice objects that can be "randomly" rolled.
                 !!!!!!! When a
                 player makes a move, the corresponding value will be removed from this array.
                 Immeadiately after the die is rolled, _roll will have length 2 (for the 2 die) or 4
                 (if doubles are rolled).
  _players     - array of Player objects holding the 2 players playing the current game.
  _playerTurn  - PlayerTurn type enum that differentiates between which player is currently playing.
  _gamePhase   - GamePhase type enum that differentiates between the "roll phase" or "move phase" of
                 a given players' turn.
*/
public class GameState {
  private bool _blackOnBar;
  private bool _whiteOnBar;
  private bool _blackHome;
  private bool _whiteHome;
  private PieceState _pieces;
  private Die _die;
  public PieceState Pieces {
    get => _pieces;
  private
    set { _pieces = value; }
  }
  private List<int> _roll;
  private Player[] _players;
  private PlayerEnum _playerTurn;
  public PlayerEnum PlayerTurn => _playerTurn;
  private GamePhase _gamePhase;

  /*
  Constructor with no arguments.
  Simply calls InitBoardState. Functionality not implemented directly in constructor to allow for
  the board to be initialized multiple times or a given game i.e. if a game is restarted.
  */
  public GameState() {
    Logger.Debug("(GameState)Constructor called");
    InitBoardState();
  }

  /*
  Called by class constructor or through outside calls when game is started. Sets/resets all the
  variables of the class including _pieces which creates piece objects for all 30 pieces and
  organizes them into their correct location groupings withint the PieceState struct.
  */
  public void InitBoardState() {
    Logger.Debug($"(GameState)InitBoardState: game and board initialized");
    // no pieces start off on the bar
    _blackOnBar = false;
    _whiteOnBar = false;
    // initial layout of board has 5 white pieces in white home and 5 black pieces in black home
    _blackHome = false;
    _whiteHome = false;
    // initialize PieceState struct
    _pieces = InitPieceState();
    // no roll has been done yet so roll should be empty array
    _die = new Die(2, new List<int> { 1, 2 });
    // get players playing the game currently
    _players = InitPlayers();
    // assign one of these players (whichever is Player #1) to start the game
    _playerTurn = InitPlayerTurn();
    // init GamePhase to ROLL since ROLL comes before MOVE
    _gamePhase = GamePhase.ROLL;
    Logger.Debug($"(GameState)Object: \n" + ToString());
  }

  /*
  Helper method for InitBoardState. Initializes all piece objects and organizes them into the
  location groupings of the PieceState struct.
  */
  private PieceState InitPieceState() {
    Logger.Debug($"(GameState)InitPieceState: pieces initialized");
    List<Piece>[] bb = new List<Piece>[24];
    List<Piece>[] wb = new List<Piece>[24];
    for (int i = 0; i < wb.Length; i++) {
      bb[i] = new List<Piece>();
      wb[i] = new List<Piece>();
    }
    PieceState ps = new PieceState(wb, bb, new List<Piece>(), new List<Piece>(), new List<Piece>(),
                                   new List<Piece>());
    return ps;
  }

  /*
  Helper method for InitBoardState. Initializes _players attribute, the 2 players of the game to
  their respective player objects and sets one to be "Player #1" and another to be "Player #2". Also
  initializes _playersTurn to the Player object of these 2 that is labeled "Player #1".
  */
  private Player[] InitPlayers() {
    Logger.Debug($"(GameState)InitPlayers: players initialized");
    Player[] p = new Player[2];
    p[0] = new Player(PlayerEnum.Player1);
    p[1] = new Player(PlayerEnum.Player2);
    return p;
  }

  /*
  Helper method for InitBoardState. Initializes _playerTurn attribute
  */
  private PlayerEnum InitPlayerTurn() {
    Logger.Info($"(GameState)InitPlayerTurn: current player initialized");
    return _players[0].GetPlayerNum();
  }

  /*
  Returns a TurnState struct holding information on current state of game
  Recall, player1 = white, player2 = black
  */
  public TurnState GetTurnState() {
    Logger.Debug(
        "(GameState)GetTurnState: Creating TurnState struct to describe GameState instance");
    bool onbar = false;
    int numHome;
    bool home = false;
    if (_playerTurn == PlayerEnum.Player1) {  // current player is white
      onbar = _pieces.WhiteBar.Count == 0 ? false : true;
      if (_whiteHome !=
          true) {  // will not change from true once the player has gotten all pieces into home
        numHome = _pieces.WhiteBoard[0].Count + _pieces.WhiteBoard[1].Count +
                  _pieces.WhiteBoard[2].Count + _pieces.WhiteBoard[3].Count +
                  _pieces.WhiteBoard[4].Count + _pieces.WhiteBoard[5].Count;
        home = (numHome == 15) ? true : false;
      }
    } else {  // current player is black
      onbar = _pieces.BlackBar.Count == 0 ? false : true;
      if (_blackHome !=
          true) {  // will not change from true once the player has gotten all pieces into home
        numHome = _pieces.BlackBoard[0].Count + _pieces.BlackBoard[1].Count +
                  _pieces.BlackBoard[2].Count + _pieces.BlackBoard[3].Count +
                  _pieces.BlackBoard[4].Count + _pieces.BlackBoard[5].Count;
        home = (numHome == 15) ? true : false;
      }
    }
    TurnState ts = new TurnState(this.IsGameOver(), onbar, home, _playerTurn, _gamePhase);
    Logger.Debug("(GameState)TurnState Object: " + ts.ToString());
    return ts;
  }

  /*
  Updates attributes of piece object being moved. Moves piece object within PieceState stuct arrays.
  Moves any pieces that are moved as a consequence of this piece being moved i.e. overtaking.
  Parameters:
      piece - Piece object to be moved
      boardIndex - Point number that the piece is located on. -1 for on bar, -2 for beared off/ off
                   board
  */
  public bool MovePiece(Piece piece, int boardIndex) {
    Logger.Info($"(GameState)MovePiece: piece moved to index {boardIndex}.\n\tPiece moved: " +
                piece.ToString() + "\n");
    if (PossibleMoves(piece).Contains(boardIndex)) {
      Logger.Info(
          $"Moving {piece.ToString()}: from {piece.GetPieceStatus().BoardIndex} to {boardIndex}");
      if (_playerTurn == PlayerEnum.Player1) {
        if (_pieces.WhiteBoard[piece.GetPieceStatus().BoardIndex - 1].Contains(piece)) {
          _pieces.WhiteBoard[piece.GetPieceStatus().BoardIndex - 1].Remove(piece);
        }
        _pieces.WhiteBoard[boardIndex - 1].Add(piece);

      } else {
        if (_pieces.BlackBoard[piece.GetPieceStatus().BoardIndex - 1].Contains(piece)) {
          _pieces.BlackBoard[piece.GetPieceStatus().BoardIndex - 1].Remove(piece);
        }
        _pieces.BlackBoard[boardIndex - 1].Add(piece);
      }

      // Place The piece asset on the board
      if (boardIndex <= 24) {
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

        deltaY = (_pieces.WhiteBoard[boardIndex - 1].Count +
                  _pieces.BlackBoard[boardIndex - 1].Count - 1) *
                 0.5460075f;
        // Delta y (top board or bottom board)
        if (boardIndex <= 12) {
          deltaY = -3.87f + deltaY;
        } else if (boardIndex <= 24) {
          deltaY = 1.257f - deltaY;
        }

        piece.transform.position = new Vector2(deltaX, deltaY);
        piece.MoveToBoardIndex(boardIndex);
      }

      return true;
    } else {
      Logger.Warn($"MovePiece: Invalid Move to {boardIndex}");
      return false;
    }
  }

  /*
  Returns a boolean value indicating if the current player cannot move any of their pieces with the
  rolls they have.
  */
  public bool MustPass() {
    bool mp = false;
    Logger.Info($"(GameState)MustPass: it is {mp} that the current player must pass their turn.\n");
    return mp;  // default value will not force the player to pass
  }

  /*
  Checks if the game is over. The game will be over if either of the players have all of their
  pieces off the board or if the game has reached a stalemate. Checking for a stalemate may or may
  not be implemented.
  */
  public bool IsGameOver() {
    bool igo = false;
    Logger.Info($"(GameState)IsGameOver: It is {igo} that the game is over.");
    return igo;  // default value allows game to be played infinitely
  }

  /*
  Returns an array of point indices of points in which a given piece is eligible to move based on
  the rules of backgammon.
  */
  public List<int> PossibleMoves(Piece piece) {
    List<int> moves = new List<int> { 1,  2,  3,  4,  5,  6,  7,  8,  9,  10, 11, 12,
                                      13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
    string move = "\n" + string.Join(",", moves);
    Logger.Info(
        $"(GameState)PossibleMoves: it is {moves} that the current player must pass their turn.\n");
    return moves;  // default value for now
  }

  /*
  Updates GameState variables when the play switches from one player to another.
  Set to public for unit testing purposes.
  */
  public void ChangeCurrentPlayer() {
    Logger.Info(
        "(GameState)ChangePlayer: Changing the currently playing player to the other player.");
    if (_playerTurn == _players[0].PlayerNum) {  // current player is first player in _players array
      _playerTurn = _players[1].GetPlayerNum();
      Logger.Debug("\t\t" + _players[0].GetPlayerNum().ToString() + " -> " +
                   _players[1].GetPlayerNum().ToString());
    } else {  // current player is first player in _players array
      _playerTurn = _players[0].GetPlayerNum();
      Logger.Debug("\t\t" + _players[1].GetPlayerNum().ToString() + " -> " +
                   _players[0].GetPlayerNum().ToString());
    }
    _die = new Die(2, new List<int> { 3, 4 });  // 2 Dice
    _gamePhase = GamePhase.ROLL;
  }

  /**
   * Attempts to pick up a piece. if a piece is already
   * held return false
   * @param piece - piece to pick up, if null It means drop the current piece
   **/
  public bool SetPieceInHand(Piece piece) {
    if (piece == null) {  // Drop piece
      _pieces.PieceInHand = null;
      // Todo check if piece can be dropped.
      return true;
    } else if (_pieces.PieceInHand != null) {  // Failed
      Logger.Info("Piece already held", "PIECE");
      return false;
    } else {  // Success
      Logger.Info($"Picking up Piece: {piece.ToString()}");
      _pieces.PieceInHand = piece;
      return true;
    }
  }

  /*
  String description of the entire GameState instance.
  */
  public override string ToString() {
    string indent = "\t";
    string die = _die.ToString(indent);
    string players = PlayersToString(indent);
    string playerTurn = _playerTurn.ToString();
    return indent + $"(BlackOnBar: {_blackOnBar}\n" + indent + $"WhiteOnBar: {_whiteOnBar}\n" +
           indent + $"BlackHome: {_blackHome}\n" + indent + $"Whitehome: {_whiteHome}\n" + indent +
           $"Pieces: {_pieces.ToString(indent)}" + indent + $"Die: " + die + "\n" + indent +
           $"Players: " + players + indent + $"PlayerTurn: " + playerTurn + "\n" + indent +
           $"GamePhase: {_gamePhase})";
  }

  /*
  Helper method for ToString method. Creates a string to describe the _players attribute of the
  GameState instance.
  */
  private string PlayersToString(string indentOut = "") {
    string indentIn = indentOut + "\t";
    string players = "\n";
    foreach (Player player in _players) {
      players += indentIn + player.ToString() + ",\n";
    }
    return players;
  }
}