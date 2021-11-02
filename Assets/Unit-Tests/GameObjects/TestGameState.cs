using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Logger = LNAR.Logger;

public class TestGameState {
  [SetUp]
  public void Setup() {
    GameHandler.Game = new GameState();
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
    GameState gs = new GameState();
    gs.ChangeState(GamePhase.MOVE);
    Piece pc_1 = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    Piece pc_2 = new GameObject("Piece2", typeof(Piece)).GetComponent<Piece>();

    Assert.True(gs.SetPieceInHand(pc_1));
    Assert.False(gs.SetPieceInHand(pc_2));
  }

  [Test]
  public void Test_MovePiece() {
    GameState gs = new GameState();
    gs.ChangeState(GamePhase.MOVE);
    Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    pc.Owner = new Player(PlayerEnum.Player1);
    pc.MoveToBoardIndexNoCheck(3);
    Assert.True(gs.MovePiece(pc, 2));
  }

  public void Test_MovePieceSameSpot() {
    GameState gs = new GameState();
    gs.ChangeState(GamePhase.MOVE);
    Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();

    pc.Owner = new Player(PlayerEnum.Player1);
    pc.MoveToBoardIndexNoCheck(2);
    Assert.False(gs.MovePiece(pc, 2));
  }

  [Test]
  public void Test_MovePieceToBoardIndexWithOpponentsPieces() {
    GameState gs = new GameState();
    gs.ChangeState(GamePhase.MOVE);
    Piece pc_1 = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    Piece pc_2 = new GameObject("Piece2", typeof(Piece)).GetComponent<Piece>();
    Piece pc_3 = new GameObject("Piece3", typeof(Piece)).GetComponent<Piece>();
    // Init Pieces
    pc_1.Start();
    pc_1.Owner = new Player(PlayerEnum.Player1);
    pc_1.MoveToBoardIndex(3);
    pc_2.Start();
    pc_2.Owner = new Player(PlayerEnum.Player1);
    pc_2.MoveToBoardIndex(3);
    pc_3.Start();
    pc_3.Owner = new Player(PlayerEnum.Player2);
    pc_3.MoveToBoardIndex(4);

    Assert.True(gs.MovePiece(pc_1, 5));
    Assert.True(gs.MovePiece(pc_2, 5));
    Assert.False(gs.MovePiece(pc_3, 5));
  }
}
