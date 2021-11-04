using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

using Logger = LNAR.Logger;

public class TestPiece {
  private Player _player;
  private GameObject _pieceGameObject;
  private Piece _piece;
  [UnitySetUp]
  public IEnumerator SetUp() {
    SceneManager.LoadScene("Backgammon");
    GameHandler.Game = new GameState();
    yield return new WaitForSeconds(1);
    _player = new Player(PlayerEnum.Player1);
    GameHandler.Game = new GameState();
    _pieceGameObject = CreateMockPieceObject();
    _piece = _pieceGameObject.GetComponent<Piece>();
    _piece.Owner = _player;
    _piece.Start();
  }

  private GameObject CreateMockPieceObject() {
    GameObject pieceObj = new GameObject("Piece", typeof(Piece));
    GameObject pieceFlat = new GameObject("PieceFlat", typeof(SpriteRenderer));
    GameObject pieceFlatBorder = new GameObject("PieceFlat", typeof(SpriteRenderer));
    GameObject pieceOnSideAsset = new GameObject("PieceOnSideAsset 1");
    GameObject pieceOnSide = new GameObject("PieceOnSide", typeof(SpriteRenderer));
    GameObject pieceOnSideBorder = new GameObject("PieceOnSideBorder", typeof(SpriteRenderer));

    // Top layer
    pieceFlat.transform.parent = pieceObj.transform;
    pieceFlatBorder.transform.parent = pieceObj.transform;
    pieceOnSideAsset.transform.parent = pieceObj.transform;

    // second layer for on side
    pieceOnSide.transform.parent = pieceOnSideAsset.transform;
    pieceOnSideBorder.transform.parent = pieceOnSideAsset.transform;

    return pieceObj;
  }
  private Color GetPieceColour(GameObject gameObject) {
    foreach (Transform t in gameObject.transform) {
      if (t.gameObject.name == "PieceFlat") {
        return t.gameObject.GetComponent<SpriteRenderer>().color;
      }
    }
    return Color.clear;
  }
  // A Test behaves as an ordinary method
  [Test]
  public void Test_CheckPieceColor() {
    Assert.AreEqual(_player.GetPlayerColour(), GetPieceColour(_pieceGameObject));

    Player player2 = new Player(PlayerEnum.Player2);
    _piece.Owner = player2;
    Assert.AreEqual(player2.GetPlayerColour(), GetPieceColour(_pieceGameObject));
  }

  // Tests moving a piece to bar
  [Test]
  public void Test_MoveToBar() {
    _piece.MoveToBar();
    Assert.AreEqual(_piece.GetPieceStatus().PieceLocation, PieceStatus.PieceLocationEnum.OnBar);
  }

  // Tests moving a piece to home
  [Test]
  public void Test_MoveToHome() {
    GameHandler.Game.ChangeWhiteHome(true);
    GameHandler.Game.ChangeState(GamePhase.MOVE);

    _piece.MoveIntoHome();
    Assert.AreEqual(_piece.GetPieceStatus().PieceLocation, PieceStatus.PieceLocationEnum.inHome);
  }

  // Tests moving a piece to home
  [Test]
  public void Test_MoveToBoardIndex() {
    _piece.MoveToBoardIndex(1);
    Assert.AreEqual(_piece.GetPieceStatus().PieceLocation, PieceStatus.PieceLocationEnum.OnBoard);
    Assert.AreEqual(_piece.GetPieceStatus().BoardIndex, 1);

    _piece.MoveToBoardIndex(10);
    Assert.AreEqual(_piece.GetPieceStatus().PieceLocation, PieceStatus.PieceLocationEnum.OnBoard);
    Assert.AreEqual(_piece.GetPieceStatus().BoardIndex, 10);
  }

  [Test]
  public void Test_PickUpPiece() {
    // Create gamestate object
    GameHandler.Game = new GameState();
    GameHandler.Game.ChangeState(GamePhase.MOVE);
    GameHandler.Game.AllowAnyMove = true;
    Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    pc.Owner = new Player(PlayerEnum.Player1);
    pc.Start();

    Assert.True(pc.PickUpOrDrop());
    Assert.AreEqual(pc, GameHandler.Game.Pieces.PieceInHand);
    Assert.True(pc.PickUpOrDrop());
    Assert.AreEqual(null, GameHandler.Game.Pieces.PieceInHand);
  }

  [Test]
  public void Test_PickUpPieceWithPieceInHand() {
    // Create gamestate object
    GameHandler.Game = new GameState();
    GameHandler.Game.AllowAnyMove = true;
    GameHandler.Game.ChangeState(GamePhase.MOVE);
    Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    pc.Owner = new Player(PlayerEnum.Player1);
    pc.Start();

    Piece pc_2 = new GameObject("Piece2", typeof(Piece)).GetComponent<Piece>();
    pc_2.Owner = new Player(PlayerEnum.Player1);
    pc_2.Start();

    Assert.True(pc.PickUpOrDrop());
    Assert.AreEqual(pc, GameHandler.Game.Pieces.PieceInHand);
    Assert.False(pc_2.PickUpOrDrop());
    Assert.AreEqual(pc, GameHandler.Game.Pieces.PieceInHand);
  }

  // Tries to pick up a piece that isn't the top piece (This should fail)
  [Test]
  public void Test_PickUpBottomPiece() {
    // Create gamestate object
    GameHandler.Game = new GameState();
    GameHandler.Game.ChangeState(GamePhase.MOVE);
    GameHandler.Game.AllowAnyMove = true;
    Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    pc.Owner = new Player(PlayerEnum.Player1);
    pc.Start();

    Piece pc_2 = new GameObject("Piece2", typeof(Piece)).GetComponent<Piece>();
    pc_2.Owner = new Player(PlayerEnum.Player1);
    pc_2.Start();

    // Move 2 pieces to Spot 5
    GameHandler.Game.MovePiece(pc, 5);
    GameHandler.Game.MovePiece(pc_2, 5);

    pc.OnPointerDown(null);
    Assert.AreEqual(null, GameHandler.Game.Pieces.PieceInHand);
    pc_2.OnPointerDown(null);
    Assert.AreEqual(pc_2, GameHandler.Game.Pieces.PieceInHand);
  }

  [Test]
  public void Test_MoveToHomeYourTurnAndMovePhase() {
    GameHandler.Game = new GameState();
    GameHandler.Game.AllowAnyMove = true;
    GameHandler.Game.ChangeWhiteHome(true);
    GameHandler.Game.ChangeState(GamePhase.MOVE);

    Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    pc.Owner = new Player(PlayerEnum.Player1);
    pc.Start();

    Assert.True(GameHandler.Game.MovePiece(pc, 25));
  }

  [Test]
  public void Test_MoveToHomeYourTurnAndRollPhase() {
    GameHandler.Game = new GameState();
    GameHandler.Game.ChangeWhiteHome(true);
    GameHandler.Game.ChangeState(GamePhase.ROLL);

    Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    pc.Owner = new Player(PlayerEnum.Player1);
    pc.Start();

    Assert.False(GameHandler.Game.MovePiece(pc, 25));
  }

  [Test]
  public void Test_MoveToHomeHomeFalse() {
    GameHandler.Game = new GameState();
    GameHandler.Game.ChangeWhiteHome(false);
    GameHandler.Game.ChangeState(GamePhase.MOVE);

    Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    pc.Owner = new Player(PlayerEnum.Player1);
    pc.Start();

    Assert.False(GameHandler.Game.MovePiece(pc, 25));
  }

  [Test]
  public void Test_MoveToHomeNotYourTurn() {
    GameHandler.Game = new GameState();
    GameHandler.Game.ChangeWhiteHome(true);
    GameHandler.Game.ChangeState(GamePhase.MOVE);
    GameHandler.Game.ChangeCurrentPlayer();
    Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    pc.Owner = new Player(PlayerEnum.Player1);
    pc.Start();

    Assert.False(GameHandler.Game.MovePiece(pc, 25));
  }

  [Test]
  public void Test_MoveToHomeNotYourHome() {
    GameHandler.Game = new GameState();
    GameHandler.Game.ChangeWhiteHome(true);
    GameHandler.Game.ChangeState(GamePhase.MOVE);

    Piece pc = new GameObject("Piece1", typeof(Piece)).GetComponent<Piece>();
    pc.Owner = new Player(PlayerEnum.Player1);
    pc.Start();

    Assert.False(GameHandler.Game.MovePiece(pc, 26));
  }
}
