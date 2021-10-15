using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
For a given player currently playing, that player could either be in the "roll phase" or "move phase" of their turn.
ROLL = roll phase, waiting for player to roll the die or while die is being rolled
MOVE = die have been rolled and the player has not used both of their turns (one turn for each die) or the player has not yet been forced to (and accepted) pass
*/
public enum GamePhase
{
    ROLL,
    MOVE
}

/*
Holds all the information needed to fully describe the state of the given "turn" of the game
    OnBar      - indicates whether or not the current player has any pieces on the bar. This is important becaue the rules of backgammon dictate that if a player has a piece on the
                 bar, they have to use their turn(s) to get it off before they can move any of their pieces on the board
    Home       - indicates if the current player has all their pieces in their home section of the board. According to the rules of backgammon, a player can only start "bearing  
                 off" when all their pieces are in their home
    GameOver   - whether game is complete (a winner or loser has been determined or neither player can play for any roll) or still in play
    PlayerTurn - which player is currently playing i.e. which player is allowed to take actions in the game at the current time
    GamePhase  - roll phase vs move phase of turn for current player (specified by PlayerTurn)
*/
public struct TurnState {
   public bool GameOver;
   public bool OnBar;
   public bool Home;
   public Player PlayerTurn;
   public GamePhase GamePhase;
   public TurnState(bool GameOver, bool OnBar, bool Home, Player PlayerTurn, GamePhase GamePhase)
    {
        this.GameOver = GameOver;
        this.OnBar = OnBar;
        this.Home = Home;
        this.PlayerTurn = PlayerTurn;
        this.GamePhase = GamePhase;
    }
};


/*
Holds all the piece objects of the game, organized by their location. 
    WhiteBoard - 2-dimensional array; first dimension represents the points on the board. Second dimension holds all the white pieces on the given point. Points are numbered 
                 according to the rules of backgammon: point #1 is on the bottom right of the board, increasing to the left, up to point #12. Point #13 is located at the top 
                 left of the board and points increase along the top to the right, ending with point #24 at the top left.
    BlackBoard - same as WhiteBoard except to hold the black pieces on the points.
    WhiteBar   - array of piece objects to hold all white pieces located on the bar.
    BlackBar   - array of piece objects to hold all black pieces located on the bar.
    WhiteOff   - array of piece objects to hold all white pieces that have been "beared off" i.e. those that are not located on the board or the bar and are out of play.
    BlackOff   - array of piece objects to hold all black pieces that have been "beared off" i.e. those that are not located on the board or the bar and are out of play.
    Jagged arrays: https://www.geeksforgeeks.org/c-sharp-jagged-arrays/#:~:text=Prerequisite%3A%20Arrays%20in%20C%23,initialized%20to%20null%20by%20default.
*/
public struct PieceState {
    public Piece[][] BlackBoard;
    public Piece[][] WhiteBoard;
    public Piece[] WhiteBar;
    public Piece[] BlackBar;
    public Piece[] WhiteOff;
    public Piece[] BlackOff;
    
    public PieceState(Piece[][] BlackBoard, Piece[][] WhiteBoard, Piece[] WhiteBar, Piece[] BlackBar, Piece[] WhiteOff, Piece[] BlackOff)
    {
        this.BlackBoard = BlackBoard;
        this.WhiteBoard = WhiteBoard;
        this.WhiteBar = WhiteBar;
        this.BlackBar = BlackBar;
        this.WhiteOff = WhiteOff;
        this.BlackOff = BlackOff;
    }
    public override string ToString(){
        string psString = "BlackBoard:\n" + BoardPiecesToString(BlackBoard) + "\nWhiteBoard:\n" + BoardPiecesToString(WhiteBoard) + "\nWhiteBar:\n" + PiecesToString(WhiteBar) + 
                          "\nBlackBar:\n" + PiecesToString(BlackBar) + "\nWhiteOff:\n" + PiecesToString(WhiteOff) + "\nBlackOff:\n" + PiecesToString(BlackOff) + "\n";
        return psString;
    }
    private string BoardPiecesToString(Piece[][] ps){
        string bp = "";
        for(int i = 1; i <= ps.GetLength(0); i++){
            bp += $"\tPoint #{i}:\n\t" + PiecesToString(ps[i - 1]);
        }
        return bp;
    }
    private string PiecesToString(Piece[] ps){
        string pieces = "\n";
        for(int i = 1; i <= ps.GetLength(0); i++){
            pieces += "\t" + ps[i].ToString() + ",\n";
        }
        return pieces;
    }
}

/* The GameState epic encompasses everything related to the flow of the game in the backend and holds all information needed to describe the 
   current status of the game (i.e. reference to pieces of each team, current player, etc.). The GameState class holds the implementation for
   all substories related to the GameState epic. 
   The GameState class is a code implementation of the UML diagrams created for issue #13.
   Attributes:
   _blackOnBar - boolean to indicate whether or not there are any black pieces on the bar (true if length of BlackBar within PieceState struct for _pieces attribute has length 
                 greater than 0)
   _whiteOnBar - boolean to indicate whether or not there are any white pieces on the bar (true if length of WhiteBar within PieceState struct for _pieces attribute has length 
                 greater than 0)
   _blackHome  - boolean to indicate whether or not all black pieces have reached the black home quadrant of the board (or if this has been accomplished and pieces have since 
                 been beared off the board) 
   _whiteHome  - boolean to indicate whether or not all white pieces have reached the white home quadrant of the board (or if this has been accomplished and pieces have since 
                 been beared off the board) 
   _pieces     - struct of type PieceState which holds arrays of Piece objects divided up by location groupings that the pieces can fall into 
   _roll       - array of the available die rolls that the current player can use for moving their pieces. When a player makes a move, the 
                 corresponding value will be removed from this array. Immeadiately after the die is rolled, _roll will have length 2 (for the 2 die) 
                 or 4 (if doubles are rolled).
   _players    - array of Player objects holding the 2 players playing the current game.
   _playerTurn - PlayerTurn type enum that differentiates between which player is currently playing
   _gamePhase  - GamePhase type enum that differentiates between the "roll phase" or "move phase" of a given players' turn
*/
public class GameState : MonoBehaviour 
{
    public bool _blackOnBar;
    public bool _whiteOnBar;
    public bool _blackHome;
    public bool _whiteHome;
    public PieceState _pieces; 
    public int[] _roll;
    public Player[] _players;
    public Player _playerTurn;
    public GamePhase _gamePhase;


    /* 
    Constructor with no arguments.
    Simply calls InitBoardState. Functionality not implemented directly in constructor to allow for the board to be initialized multiple times 
    or a given game i.e. if a game is restarted.
    */
    public GameState()
    {   Debug.Log("GameState object initialized");
        InitBoardState();
    }
    
    /*
    Called by class constructor or through outside calls when game is started. Sets/resets all the variables of the class including _pieces 
    which creates piece objects for all 30 pieces and organizes them into their correct location groupings withint the PieceState struct.
    */
    public void InitBoardState()
    {
        Debug.Log($"(GameState)InitBoardState: game and board initialized");
        // no pieces start off on the bar
        _blackOnBar = false;
        _whiteOnBar = false;
        // initial layout of board has 5 white pieces in white home and 5 black pieces in black home
        _blackHome = true;
        _whiteHome = true;
        // initialize PieceState struct
        _pieces = InitPieceState();
        // no roll has been done yet so roll should be empty array
        _roll = new int[] {};
        // get players playing the game currently 
        _players = InitPlayers();
        // assign one of these players (whichever is Player #1) to start the game
        _playerTurn = InitPlayerTurn();
        // init GamePhase to ROLL since ROLL comes before MOVE
        _gamePhase = GamePhase.ROLL;
        Debug.Log($"GameState: " + ToString() + "\n");
    }

    /*
    Helper method for InitBoardState. Initializes all piece objects and organizes them into the location groupings of the PieceState struct.
    */
    private PieceState InitPieceState(){
        Debug.Log($"(GameState)InitPieceState: pieces initialized");
        PieceState ps = new PieceState(null, null, null, null, null, null);
        return ps;
    }

    /*
    Helper method for InitBoardState. Initializes _players attribute, the 2 players of the game to their respective player objects and sets one 
    to be "Player #1" and another to be "Player #2". Also initializes _playersTurn to the Player object of these 2 that is labeled "Player #1".
    */
    private Player[] InitPlayers(){
        Debug.Log($"(GameState)InitPlayers: players initialized");
        Player[] p = new Player[2];
        p[0] = new Player(PlayerEnum.Player1);
        p[1] = new Player(PlayerEnum.Player2);
        return p;
    }

    /*
    Helper method for InitBoardState. Initializes _playerTurn attribute
    */
    private Player InitPlayerTurn(){
        Debug.Log($"(GameState)InitPlayerTurn: current player initialized");
        return _players[0];
    }

    /*
    Returns a TurnState struct holding information on current state of game
    */
    public TurnState GetTurnState(){ 
        Debug.Log($"(GameState)InitPieceState: pieces initialized");
        // will need to figure out if current player (_playerTurn) is playing black or white and return values for OnBar and Home corresponding
        // to that color
        TurnState ts = new TurnState(this.IsGameOver(), false, false, _playerTurn, _gamePhase);
        return ts;
    }

    /*
    Updates attributes of piece object being moved. Moves piece object within PieceState stuct arrays. Moves any pieces that are moved as a 
    consequence of this piece being moved i.e. overtaking.
    Parameters:
        piece - Piece object to be moved
        boardIndex - Point number that the piece is located on. -1 for on bar, -2 for beared off/ off board
    */
    public void MovePiece(Piece piece, int boardIndex){
        Debug.Log($"(GameState)MovePiece: piece moved to index {boardIndex}.\n\tPiece moved: " + piece.ToString() + "\n");

    }

    /*
    Returns a boolean value indicating if the current player cannot move any of their pieces with the rolls they have.
    */
    public bool MustPass(){
        bool mp = false;
        Debug.Log($"(GameState)MustPass: it is {mp} that the current player must pass their turn.\n");
        return mp; // default value will not force the player to pass
    }

    /*
    Checks if the game is over. The game will be over if either of the players have all of their pieces off the board or if the game has 
    reached a stalemate. Checking for a stalemate may or may not be implemented.
    */
    public bool IsGameOver(){
        bool igo = false;
        Debug.Log($"(GameState)IsGameOver: it is {igo} that the game is over.\n");
        return igo; // default value allows game to be played infinitely
    }

    /*
    Returns an array of point indices of points in which a given piece is eligible to move based on the rules of backgammon. 
    */
    public int[] PossibleMoves(Piece piece){
        int[] moves = new int[] {};
        string move = "\n" + string.Join(",", moves);
        Debug.Log($"(GameState)PossibleMoves: it is {moves} that the current player must pass their turn.\n");
        return moves; // default value for now
    }

    /*
    String description of the entire GameState instance.
    */
    public override string ToString(){
        string roll = "\n" + string.Join(",", _roll);
        string players = PlayersToString();
        string playerTurn = _playerTurn.GetPlayerNum().ToString();
        return @$"GameState Object: (BlackOnBar: {_blackOnBar},\n
                                     WhiteOnBar: {_whiteOnBar},\n 
                                     BlackHome: {_blackHome},\n 
                                     Whitehome: {_whiteHome},\n
                                     Pieces: {_pieces.ToString()},\n
                                     Roll: {roll},\n 
                                     Players: {players},\n
                                     PlayerTurn: {playerTurn},\n
                                     GamePhase: {_gamePhase})\n"; 
    }

    /*
    Helper method for ToString method. Creates a string to describe the _players attribute of the GameState instance.
    */
    private string PlayersToString(){
        string players = "\n";
        foreach (Player player in _players) {
            players += "\t" + player.ToString() + ",\n";
        }
        return players;

    }

}