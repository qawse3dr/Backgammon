using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Logger = LNAR.Logger;

public class TestGameState {
  [UnitySetUp]
  public IEnumerator Setup() {
    SceneManager.LoadScene("Backgammon");
    GameHandler.Game = new GameState();
    yield return new WaitForSeconds(1);
  }

  [Test]
  public void Test_GamePhaseToString1() {
    GamePhase gp = GamePhase.ROLL;
    Assert.AreEqual(gp.ToString(), "ROLL");
    Debug.Log(gp.ToString());
  }

  [Test]
  public void Test_GamePhaseToString2() {
    GamePhase gp = GamePhase.MOVE;
    Assert.AreEqual(gp.ToString(), "MOVE");
    Logger.Debug(gp.ToString());
  }

  // as long as this doesnt crash, its considered a pass
  [Test]
  public void Test_PieceStateToString() {
    List<Piece>[] bb = new List<Piece>[24];
    List<Piece>[] wb = new List<Piece>[24];
    for (int i = 0; i < wb.Length; i++) {
      bb[i] = new List<Piece>();
      wb[i] = new List<Piece>();
    }
    List<Piece> bbar = new List<Piece>();
    List<Piece> wbar = new List<Piece>();
    List<Piece> wo = new List<Piece>();
    List<Piece> bo = new List<Piece>();
    PieceState ps = new PieceState(bb, wb, bbar, wbar, wo, bo);
    Logger.Debug(ps.ToString());
  }

  [Test]
  public void Test_GameStateInit() {
    GameState gs = new GameState();
    TurnState ts = gs.GetTurnState();
    gs.ChangeState(GamePhase.MOVE);
    Assert.AreEqual(ts, new TurnState(false, false, false, PlayerEnum.Player1, GamePhase.ROLL));
  }

  [Test]
  public void Test_ChangePlayer() {
    GameState gs = new GameState();
    gs.ChangeState(GamePhase.MOVE);
    gs.ChangeCurrentPlayer();
    TurnState ts = gs.GetTurnState();
    Logger.Debug($"(GameState)Object: \n" + ToString());

    Assert.AreEqual(ts, new TurnState(false, false, false, PlayerEnum.Player2, GamePhase.ROLL));
  }

  [Test]
  public void Test_SetPieceInHand() {
    GameState gs = new GameState();

    gs.ChangeState(GamePhase.MOVE);
    Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    pc.Owner = new Player(PlayerEnum.Player1);

    Assert.True(gs.SetPieceInHand(pc));
  }

  [Test]
  public void Test_SetPieceInHandWhenPieceInHand() {
    GameHandler.Game.ChangeState(GamePhase.MOVE);
    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Piece")) {
      go.GetComponent<Piece>().Start();
      go.GetComponent<Piece>().Update();
    }
    Logger.Info(GameHandler.Game.ToString());
    Piece pc_1 = GameHandler.Game.Pieces.WhiteBoard[0][0];
    Piece pc_2 = GameHandler.Game.Pieces.WhiteBoard[0][1];
    Assert.True(GameHandler.Game.SetPieceInHand(pc_1));
    Assert.False(GameHandler.Game.SetPieceInHand(pc_2));
  }

  [Test]
  public void Test_MovePiece() {

    GameHandler.Game.AllowAnyMove = true;
    GameHandler.Game.RollDice(GameObject.FindObjectOfType<BackgammonUIController>());
    GameHandler.Game.ChangeState(GamePhase.MOVE);
    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Piece")) {
      go.GetComponent<Piece>().Start();
      go.GetComponent<Piece>().Update();
    }
    Assert.True(GameHandler.Game.MovePiece(GameObject.FindGameObjectWithTag("Piece").GetComponent<Piece>(), 2));
  }

  public void Test_MovePieceSameSpot() {
    GameState gs = new GameState();
    gs.ChangeState(GamePhase.MOVE);
    gs.AllowAnyMove = true;
    Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();

    pc.Owner = new Player(PlayerEnum.Player1);
    pc.MoveToBoardIndexNoCheck(2);
    Assert.False(gs.MovePiece(pc, 2));
  }

  [Test]
  public void Test_MovePieceToBoardIndexWithOpponentsPieces() {
    GameHandler.Game.ChangeCurrentPlayer();
    GameHandler.Game.AllowAnyMove = true;
    GameHandler.Game.RollDice(GameObject.FindObjectOfType<BackgammonUIController>());
    GameHandler.Game.ChangeState(GamePhase.MOVE);
    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Piece")) {
      go.GetComponent<Piece>().Start();
      go.GetComponent<Piece>().Update();
    }
    Piece pc = GameHandler.Game.Pieces.BlackBoard[5][4];

    Assert.False(GameHandler.Game.MovePiece(pc, 1));
  }

  [Test]
  public void Test_PossibleMoves() {
    // GameState gs = new GameState();
    PieceState pieces = GameHandler.Game.Pieces;
    // Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    Piece pc = pieces.WhiteBoard[5][0];  // white has piece on points 6, 8, 13 and 24 upon init
    // (subract 1 for index)
    List<(int roll, int point)> rollsPlusPoints = GameHandler.Game.PossibleMoves(pc);
  }

  [Test]
  public void Test_AddPieceInit() {
    PieceState pieces = GameHandler.Game.Pieces;

    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Piece")) {
      go.GetComponent<Piece>().Start();
      go.GetComponent<Piece>().Update();
    }

    List<Piece>[] bBoard = pieces.BlackBoard;
    List<Piece>[] wBoard = pieces.BlackBoard;
    List<Piece> wBar = pieces.WhiteBar;
    List<Piece> bBar = pieces.BlackBar;
    List<Piece> wOff = pieces.WhiteOff;
    List<Piece> bOff = pieces.BlackOff;

    // check white off and black off are empty
    Assert.AreEqual(bOff.Count, 0);
    Assert.AreEqual(wOff.Count, 0);
    // check there are no pieces on the bar
    Assert.AreEqual(wBar.Count, 0);
    Assert.AreEqual(bBar.Count, 0);
    // check black board
    Assert.AreEqual(bBoard[5].Count, 5);   // 5 black pieces on point 6 (index 5)
    Assert.AreEqual(bBoard[7].Count, 3);   // 3 black pieces on point 8 (index 7)
    Assert.AreEqual(bBoard[12].Count, 5);  // 5 black pieces on point 13 (index 12)
    Assert.AreEqual(bBoard[23].Count, 2);  // 2 black pieces on point 24 (index 23)

    // check white board
    Assert.AreEqual(wBoard[0].Count, 2);   // 2 white pieces on point 1 (index 0)
    Assert.AreEqual(wBoard[11].Count, 5);  // 5 white pieces on point 12 (index 11)
    Assert.AreEqual(wBoard[16].Count, 3);  // 3 white pieces on point 17 (index 16)
    Assert.AreEqual(wBoard[18].Count, 5);  // 5 white pieces on point 19 (index 18)
  }
}
