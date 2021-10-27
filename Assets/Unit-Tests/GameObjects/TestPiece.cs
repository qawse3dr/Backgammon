using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Logger = LNAR.Logger;

public class TestPiece {
  private Player _player;
  private GameObject _pieceGameObject;
  private Piece _piece;
  [SetUp]
  public void SetUp() {
    Logger.Debug("test");
    _player = new Player(PlayerEnum.Player1);
    _pieceGameObject = new GameObject("Piece", typeof(Piece), typeof(SpriteRenderer));
    _piece = _pieceGameObject.GetComponent<Piece>();
    _piece.Owner = _player;
    _piece.Start();
  }
  // A Test behaves as an ordinary method
  [Test]
  public void Test_CheckPieceColor() {
    Assert.AreEqual(_player.GetPlayerColour(),
                    _pieceGameObject.GetComponent<SpriteRenderer>().color);

    Player player2 = new Player(PlayerEnum.Player2);
    _piece.Owner = player2;
    Assert.AreEqual(player2.GetPlayerColour(),
                    _pieceGameObject.GetComponent<SpriteRenderer>().color);
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
    _piece.MoveIntoHome();
    Assert.AreEqual(_piece.GetPieceStatus().PieceLocation, PieceStatus.PieceLocationEnum.inHome);
  }

  // Tests moving a piece to home
  [Test]
  public void Test_MoveToBoardIndex() {
    _piece.MoveToBoardIndex(0);
    Assert.AreEqual(_piece.GetPieceStatus().PieceLocation, PieceStatus.PieceLocationEnum.OnBoard);
    Assert.AreEqual(_piece.GetPieceStatus().BoardIndex, 0);

    _piece.MoveToBoardIndex(10);
    Assert.AreEqual(_piece.GetPieceStatus().PieceLocation, PieceStatus.PieceLocationEnum.OnBoard);
    Assert.AreEqual(_piece.GetPieceStatus().BoardIndex, 10);
  }

  [Test]
  public void Test_PickUpPiece() {
    // Create gamestate object
    GameHandler.Game = new GameState();
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
}
