using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestGameState {
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
    Debug.Log(gp.ToString());
  }

  // as long as this doesnt crash, its considered a pass
  [Test]
  public void Test_PieceStateToString() {
    Piece[][] wb = new Piece[24][];
    Piece[][] bb = new Piece[24][];
    for (int i = 0; i < wb.Length; i++) {
      wb[i] = new Piece[] {};
      bb[i] = new Piece[] {};
    }
    Piece[] bbar = new Piece[] {};
    Piece[] wbar = new Piece[] {};
    Piece[] wo = new Piece[] {};
    Piece[] bo = new Piece[] {};
    PieceState ps = new PieceState(bb, wb, bbar, wbar, wo, bo);
    Debug.Log(ps.ToString());
  }

  [Test]
  public void Test_GameStateInit() {
    GameState gs = new GameState();
    TurnState ts = gs.GetTurnState();

    Assert.AreEqual(ts, new TurnState(false, false, false, PlayerEnum.Player1, GamePhase.ROLL));
  }

  [Test]
  public void Test_ChangePlayer() {
    GameState gs = new GameState();
    gs.ChangeCurrentPlayer();
    TurnState ts = gs.GetTurnState();
    Debug.Log($"(GameState)Object: \n" + ToString());

    Assert.AreEqual(ts, new TurnState(false, false, false, PlayerEnum.Player2, GamePhase.ROLL));
  }
}
