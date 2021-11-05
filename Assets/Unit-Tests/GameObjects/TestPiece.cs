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
  private Piece _piece2;

  [UnitySetUp]
  public IEnumerator SetUp() {
    Logger.Debug("IN SETUP");
    GameHandler.Game = new GameState();
    SceneManager.LoadScene("Backgammon");
    yield return new WaitForSeconds(3);
    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Piece")) {
      go.GetComponent<Piece>().Start();
      go.GetComponent<Piece>().Update();
    }
    yield return new WaitForSeconds(3);
    GameHandler.Game.AllowAnyMove = true;
    _pieceGameObject = GameHandler.Game.Pieces.WhiteBoard[0][1].gameObject;
    _player = new Player(PlayerEnum.Player1);
    _piece = _pieceGameObject.GetComponent<Piece>();
    _piece2 = GameHandler.Game.Pieces.WhiteBoard[0][0];
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
  [UnityTest]
  public IEnumerator Test_MoveToHome() {
    GameHandler.Game.ChangeWhiteHome(true);
    GameHandler.Game.ChangeState(GamePhase.MOVE);

    _piece.MoveIntoHome();
    Assert.AreEqual(_piece.GetPieceStatus().PieceLocation, PieceStatus.PieceLocationEnum.inHome);
    return null;
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
    GameHandler.Game.ChangeState(GamePhase.MOVE);
    GameHandler.Game.AllowAnyMove = true;

    Assert.True(_piece.PickUpOrDrop());
    Assert.AreEqual(_piece, GameHandler.Game.Pieces.PieceInHand);
    Assert.True(_piece.PickUpOrDrop());
    Assert.AreEqual(null, GameHandler.Game.Pieces.PieceInHand);
  }

  [Test]
  public void Test_PickUpPieceWithPieceInHand() {
    // Create gamestate object
    GameHandler.Game.AllowAnyMove = true;
    GameHandler.Game.ChangeState(GamePhase.MOVE);

    Assert.True(_piece.PickUpOrDrop());
    Assert.AreEqual(_piece, GameHandler.Game.Pieces.PieceInHand);
    Assert.False(_piece2.PickUpOrDrop());
    Assert.AreEqual(_piece, GameHandler.Game.Pieces.PieceInHand);
  }

  // Tries to pick up a piece that isn't the top piece (This should fail)
  [Test]
  public void Test_PickUpBottomPiece() {
    // Create gamestate object
    GameHandler.Game.ChangeState(GamePhase.MOVE);
    GameHandler.Game.AllowAnyMove = true;

    // Move 2 pieces to Spot 5
    GameHandler.Game.MovePiece(_piece, 5);
    GameHandler.Game.MovePiece(_piece2, 5);

    _piece.OnPointerDown(null);
    Assert.AreEqual(null, GameHandler.Game.Pieces.PieceInHand);
    _piece2.OnPointerDown(null);
    Assert.AreEqual(_piece2, GameHandler.Game.Pieces.PieceInHand);
  }

  [Test]
  public void Test_MoveToHomeYourTurnAndMovePhase() {
    GameHandler.Game.AllowAnyMove = true;
    GameHandler.Game.ChangeWhiteHome(true);
    GameHandler.Game.ChangeState(GamePhase.MOVE);

    Assert.True(GameHandler.Game.MovePiece(_piece, 25));
  }

  [UnityTest]
  public IEnumerator Test_MoveToHomeYourTurnAndRollPhase() {
    GameHandler.Game.ChangeWhiteHome(true);
    GameHandler.Game.ChangeState(GamePhase.ROLL);

    Assert.False(GameHandler.Game.MovePiece(_piece, 25));
    return null;
  }

  [Test]
  public void Test_MoveToHomeHomeFalse() {
    GameHandler.Game.ChangeWhiteHome(false);
    GameHandler.Game.ChangeState(GamePhase.MOVE);

    Assert.False(GameHandler.Game.MovePiece(_piece, 25));
  }

  [Test]
  public void Test_MoveToHomeNotYourTurn() {
    GameHandler.Game.ChangeWhiteHome(true);
    GameHandler.Game.ChangeState(GamePhase.MOVE);
    GameHandler.Game.ChangeCurrentPlayer();

    Assert.False(GameHandler.Game.MovePiece(_piece, 25));
  }

  [Test]
  public void Test_MoveToHomeNotYourHome() {
    GameHandler.Game.ChangeWhiteHome(true);
    GameHandler.Game.ChangeState(GamePhase.MOVE);

    Assert.False(GameHandler.Game.MovePiece(_piece, 26));
  }
}
