using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UI;
using UnityEngine;
using Logger = LNAR.Logger;
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

// holds information about the board state with reference to a Player turn
public struct BoardState {
  // Board info
  public List<Piece>[] MyBoard;
  public List<Piece>[] OtherBoard;

  // Bar info
  public List<Piece> MyBar;
  public List<Piece> OtherBar;

  // home info
  public List<Piece> MyHome;
  public int MyHomeIndex;
  public int OtherHomeIndex;
}

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
    for (int i = 0; i < ps.Count; i++) {
      if (ps[i])
        pieces += indentIn + ps[i].ToString() + "\n";
      else
        pieces += indentIn + "null" + "\n";
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
  private Player[] _players;
  private PlayerEnum _playerTurn;
  public PlayerEnum PlayerTurn => _playerTurn;
  private GamePhase _gamePhase;

  public bool AllowAnyMove = false;

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
    _die = new Die(
        2, new List<int> { DateTime.Now.Millisecond, DateTime.Now.Millisecond + 1 });  // 2 Dice
    // get players playing the game currently
    _players = InitPlayers();
    // assign one of these players (whichever is Player #1) to start the game
    _playerTurn = InitPlayerTurn();
    // init GamePhase to ROLL since ROLL comes before MOVE
    _gamePhase = GamePhase.ROLL;
    // Logger.Debug($"(GameState)Object: \n" + ToString());
  }

  /*
  Helper method for InitBoardState. Initializes all piece objects and organizes them into the
  location groupings of the PieceState struct.
  */
  private PieceState InitPieceState() {
    Logger.Debug($"(GameState)InitPieceState: pieces initialized");
    List<Piece>[] wb = new List<Piece>[24];
    List<Piece>[] bb = new List<Piece>[24];
    List<int> toBeFillBlack = new List<int> { 5, 7, 12, 23 };
    List<int> toBeFillWhite = new List<int> { 0, 11, 16, 18 };
    List<int> fillNumBlack = new List<int> { 2, 5, 3, 5 };
    List<int> fillNumWhite = new List<int> { 5, 3, 5, 2 };

    for (int i = 0; i < wb.Length; i++) {
      wb[i] = new List<Piece>();
      bb[i] = new List<Piece>();
      bb = FillBlackBoard(i, bb);
      wb = FillWhiteBoard(i, wb);
    }

    PieceState ps = new PieceState(bb, wb, new List<Piece>(), new List<Piece>(), new List<Piece>(),
                                   new List<Piece>());
    return ps;
  }

  /*
  Helper function for InitPieceState. Adds nulls to points lists that will have black pieces in them
  when the game is initialized.
  */
  private List<Piece>[] FillBlackBoard(int i, List<Piece>[] board) {
    List<int> toBeFill = new List<int> { 5, 7, 12, 23 };
    List<int> fillNum = new List<int> { 5, 3, 5, 2 };

    if (toBeFill.Contains(i)) {
      int index = toBeFill.IndexOf(i);
      for (int j = 0; j < fillNum[index]; j++) {
        board[i].Add(null);
      }
    }

    return board;
  }

  /*
 Helper function for InitPieceState. Adds nulls to points lists that will have white pieces in them
 when the game is initialized.
 */
  private List<Piece>[] FillWhiteBoard(int i, List<Piece>[] board) {
    List<int> toBeFill = new List<int> { 0, 11, 16, 18 };
    List<int> fillNum = new List<int> { 2, 5, 3, 5 };

    if (toBeFill.Contains(i)) {
      int index = toBeFill.IndexOf(i);
      for (int j = 0; j < fillNum[index]; j++) {
        board[i].Add(null);
      }
    }

    return board;
  }

  /*
  Called before first frame is loaded from Piece class' start method. Each piece on the board will
  have this method called on it.
  Parameters: piece       - the piece object (corresponding to a piece object in the backend and a
                            piece on the unity board in the frontend)
              verticality - the pieces are 'stacked' on top of each other on each point. Verticality
                            defines how high in the stack the given piece is on the point it starts
                            on.
  */
  public void AddPieceInit(Piece piece, int verticality) {
    Logger.Debug(
        $"Initializing piece at verticality {verticality} on point {piece.StartPointIndex}");

    if (piece.StartPointIndex < 1 || verticality < 1) {
      Logger.Error("PIECE INDEX IS TO SMALL");
      return;
    }
    if (piece.Owner.PlayerNum == PlayerEnum.Player1) {
      List<Piece> point = _pieces.WhiteBoard[piece.StartPointIndex - 1];
      point[verticality - 1] = piece;
    } else {
      List<Piece> point = _pieces.BlackBoard[piece.StartPointIndex - 1];
      point[verticality - 1] = piece;
    }
  }

  /*
  Helper method for InitBoardState. Initializes _players attribute, the 2 players of the game to
  their respective player objects and sets one to be "Player #1" and another to be "Player #2". Also
  initializes _playersTurn to the Player object of these 2 that is labeled "Player #1".
  */
  private Player[] InitPlayers() {
    Logger.Debug($"(GameState)InitPlayers: players initialized");
    Player[] p = new Player[2];
    p[0] = Player.CreateNewPlayer("Larry and Numan", PlayerEnum.Player1);
    p[1] = Player.CreateNewPlayer("Rachel and Ajit", PlayerEnum.Player2);

    return p;
  }

  // This should be called once when the game starts
  public void OnGameStart() {
    GameObject.Find("PlayerName1")
        .transform.Find("Name")
        .GetComponent<Text>()
        .GetComponent<Text>()
        .text = _players[0].Name;
    GameObject.Find("PlayerName2")
        .transform.Find("Name")
        .GetComponent<Text>()
        .GetComponent<Text>()
        .text = _players[1].Name;
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
      } else {
        home = _whiteHome;
      }
    } else {  // current player is black
      onbar = _pieces.BlackBar.Count == 0 ? false : true;
      if (_blackHome !=
          true) {  // will not change from true once the player has gotten all pieces into home
        numHome = _pieces.BlackBoard[0].Count + _pieces.BlackBoard[1].Count +
                  _pieces.BlackBoard[2].Count + _pieces.BlackBoard[3].Count +
                  _pieces.BlackBoard[4].Count + _pieces.BlackBoard[5].Count;
        home = (numHome == 15) ? true : false;
      } else {
        home = _blackHome;
      }
    }
    TurnState ts = new TurnState(this.IsGameOver(), onbar, home, _playerTurn, _gamePhase);
    Logger.Debug("(GameState)TurnState Object: " + ts.ToString());
    return ts;
  }

  public BoardState GetBoardState() {
    BoardState boardState = new BoardState();
    if (PlayerTurn == PlayerEnum.Player1) {
      boardState.MyBoard = _pieces.WhiteBoard;
      boardState.OtherBoard = _pieces.BlackBoard;
      boardState.MyHomeIndex = 25;
      boardState.OtherHomeIndex = 26;
      boardState.MyHome = _pieces.WhiteOff;
      boardState.MyBar = _pieces.WhiteBar;
      boardState.OtherBar = _pieces.BlackBar;

    } else {
      boardState.MyBoard = _pieces.BlackBoard;
      boardState.OtherBoard = _pieces.WhiteBoard;
      boardState.MyHomeIndex = 26;
      boardState.OtherHomeIndex = 25;
      boardState.MyHome = _pieces.BlackOff;
      boardState.MyBar = _pieces.BlackBar;
      boardState.OtherBar = _pieces.WhiteBar;
    }
    return boardState;
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
    bool result = false;
    if (_gamePhase == GamePhase.ROLL) {
      Logger.Warn("It's Roll phase you cant move pieces");
      return false;
    } else if (GetTurnState().GameOver) {
      Logger.Warn("Game is over you can't move pieces");
      return false;
    }

    List<(int roll, int point)> posMoves = null;
    int indexOfPoint;

    if (AllowAnyMove) {
      indexOfPoint = boardIndex;
    } else {
      posMoves = PossibleMoves(piece);
      // BUG FIX #57 to account for the board index locations
      int tempIndex = (boardIndex != 25 && boardIndex != 26) ? boardIndex - 1 : boardIndex;
      indexOfPoint = posMoves.FindIndex(rp => rp.point == tempIndex);
    }

    if (indexOfPoint != -1) {
      Logger.Info(
          $"Moving {piece.ToString()}: from {piece.GetPieceStatus().BoardIndex} to {boardIndex}");

      if (boardIndex == 25 || boardIndex == 26) {  // move to home
        if (boardIndex != GetBoardState().MyHomeIndex) {
          Logger.Info("Not your home get out");
          return false;
        }
        result = piece.MoveIntoHome();
      } else {  // move to board
        result = piece.MoveToBoardIndex(boardIndex);
      }
    } else {
      Logger.Warn($"MovePiece: Invalid Move to {boardIndex}");
      return false;
    }

    // Remove the roll if move passed
    if (result && !AllowAnyMove) {
      Logger.Debug(
          $"(GameState)MovePiece: Successful move using roll - {posMoves[indexOfPoint].roll}");

      // Update Game state
      UpdateGameState();

      _die.ClearRoll(posMoves[indexOfPoint].roll);
      if (_die.Rolls.Count > 0 && MustPass()) {
        Logger.Debug("Changing turn after one dice roll");
        ChangeCurrentPlayer();
      }
    }

    Logger.Debug($"(GameState)Object: \n" + ToString());
    return result;
  }

  /*
  Returns a boolean value indicating if the current player cannot move any of their pieces with the
  rolls they have.
  */
  public bool MustPass() {
    foreach (List<Piece> point in GetBoardState().MyBoard) {
      foreach (Piece piece in point) {
        if (PossibleMoves(piece).Count > 0) {
          Logger.Info($"(GameState)MustPass: the current player can play their turn.\n");
          return false;
        }
      }
    }
    foreach (Piece piece in GetBoardState().MyBar) {
      if (PossibleMoves(piece).Count > 0) {
        Logger.Info($"(GameState)MustPass: the current player can play their turn.\n");
        return false;
      }
    }
    Logger.Info(
        $"(GameState)MustPass: it is false that the current player must pass their turn.\n");
    return true;  // default value will not force the player to pass
  }

  /*
  Checks if the game is over. The game will be over if either of the players have all of their
  pieces off the board or if the game has reached a stalemate. Checking for a stalemate may or may
  not be implemented.
  */
  public bool IsGameOver() {
    bool igo = false;
    BackgammonUIController controller = GameObject.FindObjectOfType<BackgammonUIController>();

    if (_pieces.WhiteOff.Count == 15) {
      controller.GameOver(_players[0]);
      igo = true;
    } else if (_pieces.BlackOff.Count == 15) {
      controller.GameOver(_players[1]);
      igo = true;
    }

    Logger.Info($"(GameState)IsGameOver: It is {igo} that the game is over.");
    return igo;  // default value allows game to be played infinitely
  }

  /*
  Returns an array of point indices of points in which a given piece is eligible to move based on
  the rules of backgammon.
  */
  public List<(int roll, int point)> PossibleMoves(Piece piece) {
    // for unit testing allow any moves
    if (AllowAnyMove) {
      Logger.Debug("ALlowAnyMove");
      return new List<(int roll, int point)> {
        (_die.Rolls[0], 1),  (_die.Rolls[0], 2),  (_die.Rolls[0], 3),  (_die.Rolls[0], 4),
        (_die.Rolls[0], 5),  (_die.Rolls[0], 6),  (_die.Rolls[0], 6),  (_die.Rolls[0], 8),
        (_die.Rolls[0], 9),  (_die.Rolls[0], 10), (_die.Rolls[0], 11), (_die.Rolls[0], 12),
        (_die.Rolls[0], 13), (_die.Rolls[0], 14), (_die.Rolls[0], 15), (_die.Rolls[0], 16),
        (_die.Rolls[0], 17), (_die.Rolls[0], 18), (_die.Rolls[0], 19), (_die.Rolls[0], 20),
        (_die.Rolls[0], 21), (_die.Rolls[0], 22), (_die.Rolls[0], 23), (_die.Rolls[0], 24),
        (_die.Rolls[0], 25), (_die.Rolls[0], 26)
      };
    }

    List<(int roll, int point)> rollPlusPoint = new List<(
        int roll, int point)> {};  // return roll as well as point so that when move is made,
                                   // ClearRoll(roll) can be used to remove the corresponding roll
    List<(int roll, int point)> toAdd =
        new List<(int roll, int point)> {};  // used to overwrite values in rollPlusPoint when
                                             // checking for out of range points below
    // Points to remove
    List<(int roll, int point)> toRemove = new List<(int roll, int point)>();
    int dir = piece.Owner.PlayerNum == PlayerEnum.Player2
                  ? -1
                  : 1;  // white pieces move around board in one direction and black pieces in the
                        // other direction
    int off = piece.Owner.PlayerNum == PlayerEnum.Player1
                  ? 25
                  : 26;  // values to indicate a piece is being moved from home to off board (near
                         // end of game)
    bool onBar = piece.Owner.PlayerNum == PlayerEnum.Player1
                     ? _whiteOnBar
                     : _blackOnBar;  // is the given piece on the bar?
    int curPoint = piece.GetPieceStatus().BoardIndex - 1;
    // these are indices and not point #'s. So when you have a roll of one for for white, you end up
    // at index 0 and for red, a roll of 1 (-1) will get you to index 23.
    int startPoint = piece.Owner.PlayerNum == PlayerEnum.Player2 ? 24 : -1;
    List<Piece>[] otherPlayer = piece.Owner.PlayerNum == PlayerEnum.Player2
                                    ? _pieces.WhiteBoard
                                    : _pieces.BlackBoard;  // other players' pieces
    bool home = piece.Owner.PlayerNum == PlayerEnum.Player1 ? _whiteHome : _blackHome;
    // if the user has a piece on the bar
    if (onBar) {
      // if the given piece is on the bar
      if (piece.GetPieceStatus().PieceLocation == PieceStatus.PieceLocationEnum.OnBar) {
        foreach (int r in _die.Rolls) {
          rollPlusPoint.Add(
              (r, startPoint + dir * r));  // newPoint = startPoint + (direction) * roll
        }
      }
    } else {
      foreach (int r in _die.Rolls) {
        rollPlusPoint.Add((r, curPoint + dir * r));  // newPoint = curPoint + (direction) * roll
      }
    }

    Logger.Info(
        $"(GameState)PossibleMoves: before removal, the following (roll, point) pairs:\n\t" +
        PossibleMovesToString(rollPlusPoint));
    foreach ((int roll, int point)rollPoint in rollPlusPoint) {
      // check for any target points outside (0, 23)
      if (rollPoint.point <= -1 || rollPoint.point >= 24) {
        // overwriting given roll+point tuple
        Logger.Debug(
            $"Removing (roll, point) - ({rollPoint.roll}, {rollPoint.point}) from possible moves as it is out of range. Replacing with (roll, point) - ({rollPoint.roll}, {off})");
        toRemove.Add(rollPoint);
        if (home) {  // if the current player is eligible to start moving off the board
          toAdd.Add((rollPoint.roll, off));  // off board: 25 (white) or 26 (red)
        }
        continue;
      }

      // remove any moves which have 1+ of the other players' pieces on the target point
      if (otherPlayer[rollPoint.point].Count > 1) {
        Logger.Debug(
            $"Removing (roll, point) - ({rollPoint.roll}, {rollPoint.point}) from possible moves as there are too many opponent pieces on point");
        toRemove.Add(rollPoint);
      }
    }
    // Remove points marked for remove (too many opponent pieces on point).
    foreach (var rp in toRemove) {
      rollPlusPoint.Remove(rp);
    }
    rollPlusPoint.AddRange(toAdd);  // replacements for out of range points

    Logger.Info(
        $"(GameState)PossibleMoves: the following (roll, point) pairs are rolls that the current player can use to move the current piece to target points:\n\t" +
        PossibleMovesToString(rollPlusPoint));
    return rollPlusPoint;  // default value for now
  }

  /*
  Helper function for log message for PossibleMoves function
  */
  public string PossibleMovesToString(List<(int roll, int point)> rollsPlusPoints) {
    string str = "";
    foreach ((int roll, int point)rollPoint in rollsPlusPoints) {
      str += "(" + rollPoint.roll + ", " + rollPoint.point + "), ";
    }
    str.TrimEnd(' ', ',');
    return str;
  }

  public void UpdateGameState() {
    Logger.Info(
        $"Updating game state before (_blackHome={_blackHome}, _whiteHome={_whiteHome}, _whiteOnBar={_whiteOnBar}, _blackOnBar={_blackOnBar}, _GamePhase={_gamePhase}, _playerTurn={_playerTurn})");
    // Check if the player is all in home
    _blackHome = GetIsHome(_pieces.BlackBoard);
    _whiteHome = GetIsHome(_pieces.WhiteBoard);

    _blackOnBar = (_pieces.BlackBar.Count > 0);
    _whiteOnBar = (_pieces.WhiteBar.Count > 0);

    Logger.Info(
        $"Updating game state after (_blackHome={_blackHome}, _whiteHome={_whiteHome}, _whiteOnBar={_whiteOnBar}, _blackOnBar={_blackOnBar}, _GamePhase={_gamePhase}, _playerTurn={_playerTurn})");
  }

  private bool GetIsHome(List<Piece>[] board) {
    foreach (List<Piece> pieceList in board) {
      foreach (Piece piece in pieceList) {
        if (piece.Owner.PlayerNum == PlayerEnum.Player1) {
          if (piece.GetPieceStatus().BoardIndex < 19)
            return false;
        } else {
          if (piece.GetPieceStatus().BoardIndex > 6)
            return false;
        }
      }
    }
    return true;
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
    _die = new Die(
        2, new List<int> { DateTime.Now.Millisecond, DateTime.Now.Millisecond + 1 });  // 2 Dice
    _gamePhase = GamePhase.ROLL;
    Logger.Debug("Enableing roll text");
    GameObject.Find("Roll").GetComponent<Text>().enabled = true;

    // Change PlayerIcon prefab colours according to current player
    GameObject playerIcon1 = GameObject.Find("PlayerIcon1");
    GameObject playerIcon2 = GameObject.Find("PlayerIcon2");
    GameObject playerName1 = GameObject.Find("PlayerName1");
    GameObject playerName2 = GameObject.Find("PlayerName2");
    if (_playerTurn == _players[0].PlayerNum) {
      // Highlight current player icon and name
      playerIcon1.transform.Find("Background").GetComponent<SpriteRenderer>().color =
          new Color(1, 0.7358f, 0, 1);  // gold-ish colour
      playerName1.transform.Find("Name").GetComponent<Text>().fontStyle = FontStyle.Bold;
      // Un-highlight other player icon and name
      playerIcon2.transform.Find("Background").GetComponent<SpriteRenderer>().color = Color.black;
      playerName2.transform.Find("Name").GetComponent<Text>().fontStyle = FontStyle.Normal;
    }
    if (_playerTurn == _players[1].PlayerNum) {
      // Highlight current player icon and name
      playerIcon2.transform.Find("Background").GetComponent<SpriteRenderer>().color =
          new Color(1, 0.7358f, 0, 1);
      playerName2.transform.Find("Name").GetComponent<Text>().fontStyle = FontStyle.Bold;
      // Un-highlight other player icon and name
      playerIcon1.transform.Find("Background").GetComponent<SpriteRenderer>().color = Color.black;
      playerName1.transform.Find("Name").GetComponent<Text>().fontStyle = FontStyle.Normal;
    }
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
      Logger.Warn("Piece is set to null");
      return true;
    } else if (_pieces.PieceInHand != null) {  // Failed
      Logger.Info("Piece already held", "PIECE");
      return false;
    } else {  // Success
      if (this._gamePhase != GamePhase.MOVE) {
        Logger.Warn("You are not in the Move phase Piece can't be moved");
        return false;
      } else if (piece.GetPieceStatus().PieceLocation ==
                 PieceStatus.PieceLocationEnum
                     .inHome) {  // Shouldn't be able to move pieces in home
        Logger.Warn("Piece already in home can't be moved");
        return false;
      } else if (!(piece.GetPieceStatus().PieceLocation ==
                   PieceStatus.PieceLocationEnum.OnBar)) {  // if its on the bar its an auto pass
                                                            // (owner has already been checked)
        if (this._playerTurn == PlayerEnum.Player1) {
          if (this._whiteOnBar) {  // Can't move piece if there is one on the bar
            Logger.Warn("White Player has a piece on the bar");
            return false;
          }
        } else {
          if (this._blackOnBar) {  // Can't move piece if there is one on the bar
            Logger.Warn("Black Player has a piece on the bar");
            return false;
          }
        }
      }
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

  // TODO REMOVE once piece init is complete
  public Player GetPlayer(PlayerEnum playerNum) {
    if (playerNum == PlayerEnum.Player1) {
      return _players[0];
    } else {
      return _players[1];
    }
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

  // Passes the controller so it has access to the sprites
  public void RollDice(BackgammonUIController uiController) {
    GameObject die1 = GameObject.Find("Die1");
    GameObject die2 = GameObject.Find("Die2");
    _die.Roll();
    _gamePhase = GamePhase.MOVE;

    foreach ((GameObject go, int index)go in new List<(GameObject, int)> { (die1, 0), (die2, 1) }) {
      switch (_die.Rolls[go.index]) {
        case 1:
          go.go.GetComponent<SpriteRenderer>().sprite = uiController.Die1;
          break;
        case 2:
          go.go.GetComponent<SpriteRenderer>().sprite = uiController.Die2;
          break;
        case 3:
          go.go.GetComponent<SpriteRenderer>().sprite = uiController.Die3;
          break;
        case 4:
          go.go.GetComponent<SpriteRenderer>().sprite = uiController.Die4;
          break;
        case 5:
          go.go.GetComponent<SpriteRenderer>().sprite = uiController.Die5;
          break;
        case 6:
          go.go.GetComponent<SpriteRenderer>().sprite = uiController.Die6;
          break;
      }
    }

    if (MustPass()) {
      Logger.Info("Player cannot move passing turn");
      ChangeCurrentPlayer();
    }
  }

  /** These functions will not be in the final release and shouldn't be
   * used anywhere execpt the debuging menu and maybe unit tests
   */
#if DEBUG_MENU

  public void ChangeState(GamePhase phase) {
    Logger.Debug($"Change Game Phase from {_gamePhase} to {phase}");
    _gamePhase = phase;
  }

  public void ChangeWhiteHome(bool isHome) {
    Logger.Debug($"Change White Home from {_whiteHome} to {isHome}");
    _whiteHome = isHome;
  }

  public void ChangeBlackHome(bool isHome) {
    Logger.Debug($"Change Black Home from {_blackHome} to {isHome}");
    _blackHome = isHome;
  }

  public void ChangeWhiteOnBar(bool onBar) {
    Logger.Debug($"Change White On Bar from {_whiteHome} to {onBar}");
    _whiteOnBar = onBar;
  }

  public void ChangeBlackOnBar(bool onBar) {
    Logger.Debug($"Change Black On Bar from {_blackHome} to {onBar}");
    _blackOnBar = onBar;
  }

  public void ChangeDieActive(int dieNum, bool isActive) {
    Logger.Debug($"Set Die {dieNum} to {isActive}");
  }

  public void SetDieValue(int dieNum, int value) {
    Logger.Debug($"");
  }

  // Gets for debug menu this is not using properties because this will not be present in release
  // build
  public PlayerEnum GetPlayerTurn() {
    return _playerTurn;
  }
  public GamePhase GetTurnPhase() {
    return _gamePhase;
  }
  public bool GetWhiteHome() {
    return _whiteHome;
  }
  public bool GetBlackHome() {
    return _blackHome;
  }
  public bool GetWhiteOnBar() {
    return _whiteOnBar;
  }
  public bool GetBlackOnBar() {
    return _blackOnBar;
  }
#endif
}