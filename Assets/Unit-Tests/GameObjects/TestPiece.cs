using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestPiece {
  private Player _player;
  private GameObject _pieceGameObject;
  private Piece _piece;
  [SetUp]
  public void SetUp() {
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
}
