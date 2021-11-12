using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Logger = LNAR.Logger;

public class TestPlayer {
  private Player _unInitPlayer;
  private Player _playerLarry;
  private Queue<MatchRecord> _larryMatchHistory;
  private Player _playerNuman;
  private Queue<MatchRecord> _numanMatchHistory;

  private Player _playerAjit;
  private Queue<MatchRecord> _ajitMatchHistory;

  private Player _playerRachel;
  private Queue<MatchRecord> _rachelMatchHistory;

  [SetUp]
  public void Setup() {
    _unInitPlayer = new Player(PlayerEnum.NotSet);
    _playerLarry = Player.CreateNewPlayerUnitTest("Larry", PlayerEnum.NotSet, 5, 3);
    _playerAjit = Player.CreateNewPlayerUnitTest("Ajit", PlayerEnum.NotSet, 6, 9);
    _playerRachel = Player.CreateNewPlayerUnitTest("Rachel", PlayerEnum.NotSet, 1, 2);
    _playerNuman = Player.CreateNewPlayerUnitTest("Numan", PlayerEnum.NotSet, 4, 20);

    // Setup match history
    _larryMatchHistory = new Queue<MatchRecord>();
    _larryMatchHistory.Enqueue(new MatchRecord(_playerLarry, _playerNuman, _playerNuman));
    _larryMatchHistory.Enqueue(new MatchRecord(_playerLarry, _playerRachel, _playerRachel));
    _larryMatchHistory.Enqueue(new MatchRecord(_playerLarry, _playerAjit, _playerLarry));
    _larryMatchHistory.Enqueue(new MatchRecord(_playerLarry, _playerRachel, _playerLarry));
    _larryMatchHistory.Enqueue(new MatchRecord(_playerLarry, _playerNuman, _playerLarry));
    _playerLarry.SetMatchHistoryUnitTest(_larryMatchHistory);

    _numanMatchHistory = new Queue<MatchRecord>();
    _numanMatchHistory.Enqueue(new MatchRecord(_playerNuman, _playerLarry, _playerNuman));
    _numanMatchHistory.Enqueue(new MatchRecord(_playerNuman, _playerRachel, _playerNuman));
    _numanMatchHistory.Enqueue(new MatchRecord(_playerNuman, _playerAjit, _playerNuman));
    _numanMatchHistory.Enqueue(new MatchRecord(_playerNuman, _playerRachel, _playerNuman));
    _numanMatchHistory.Enqueue(new MatchRecord(_playerNuman, _playerAjit, _playerAjit));
    _playerNuman.SetMatchHistoryUnitTest(_numanMatchHistory);

    _ajitMatchHistory = new Queue<MatchRecord>();
    _ajitMatchHistory.Enqueue(new MatchRecord(_playerAjit, _playerLarry, _playerAjit));
    _ajitMatchHistory.Enqueue(new MatchRecord(_playerAjit, _playerRachel, _playerAjit));
    _ajitMatchHistory.Enqueue(new MatchRecord(_playerAjit, _playerRachel, _playerAjit));
    _ajitMatchHistory.Enqueue(new MatchRecord(_playerAjit, _playerNuman, _playerNuman));
    _ajitMatchHistory.Enqueue(new MatchRecord(_playerAjit, _playerNuman, _playerNuman));
    _playerAjit.SetMatchHistoryUnitTest(_ajitMatchHistory);

    _rachelMatchHistory = new Queue<MatchRecord>();
    _rachelMatchHistory.Enqueue(new MatchRecord(_playerRachel, _playerLarry, _playerLarry));
    _rachelMatchHistory.Enqueue(new MatchRecord(_playerRachel, _playerLarry, _playerRachel));
    _rachelMatchHistory.Enqueue(new MatchRecord(_playerRachel, _playerNuman, _playerNuman));
    _playerRachel.SetMatchHistoryUnitTest(_rachelMatchHistory);
  }

  [Test]
  public void Test_MatchRecordToString() {
    Assert.AreEqual(_rachelMatchHistory.Dequeue().ToString(), "(Rachel,Larry,Larry)");
    Assert.AreEqual(_rachelMatchHistory.Dequeue().ToString(), "(Rachel,Larry,Rachel)");
    Assert.AreEqual(_rachelMatchHistory.Dequeue().ToString(), "(Rachel,Numan,Numan)");

    Assert.AreEqual(_larryMatchHistory.Dequeue().ToString(), "(Larry,Numan,Numan)");
    Assert.AreEqual(_larryMatchHistory.Dequeue().ToString(), "(Larry,Rachel,Rachel)");
    Assert.AreEqual(_larryMatchHistory.Dequeue().ToString(), "(Larry,Ajit,Larry)");
    Assert.AreEqual(_larryMatchHistory.Dequeue().ToString(), "(Larry,Rachel,Larry)");
    Assert.AreEqual(_larryMatchHistory.Dequeue().ToString(), "(Larry,Numan,Larry)");

    Assert.AreEqual(_numanMatchHistory.Dequeue().ToString(), "(Numan,Larry,Numan)");
    Assert.AreEqual(_numanMatchHistory.Dequeue().ToString(), "(Numan,Rachel,Numan)");
    Assert.AreEqual(_numanMatchHistory.Dequeue().ToString(), "(Numan,Ajit,Numan)");
    Assert.AreEqual(_numanMatchHistory.Dequeue().ToString(), "(Numan,Rachel,Numan)");
    Assert.AreEqual(_numanMatchHistory.Dequeue().ToString(), "(Numan,Ajit,Ajit)");

    Assert.AreEqual(_ajitMatchHistory.Dequeue().ToString(), "(Ajit,Larry,Ajit)");
    Assert.AreEqual(_ajitMatchHistory.Dequeue().ToString(), "(Ajit,Rachel,Ajit)");
    Assert.AreEqual(_ajitMatchHistory.Dequeue().ToString(), "(Ajit,Rachel,Ajit)");
    Assert.AreEqual(_ajitMatchHistory.Dequeue().ToString(), "(Ajit,Numan,Numan)");
    Assert.AreEqual(_ajitMatchHistory.Dequeue().ToString(), "(Ajit,Numan,Numan)");
  }

  // Tests player to string this will be used for the database so formating is very important
  [Test]
  public void Test_PlayerToString() {
    Assert.AreEqual(
        _playerLarry.ToString(),
        "Larry,5,3,(Larry,Numan,Numan),(Larry,Rachel,Rachel),(Larry,Ajit,Larry),(Larry,Rachel,Larry),(Larry,Numan,Larry)");
    Assert.AreEqual(
        _playerNuman.ToString(),
        "Numan,4,20,(Numan,Larry,Numan),(Numan,Rachel,Numan),(Numan,Ajit,Numan),(Numan,Rachel,Numan),(Numan,Ajit,Ajit)");
    Assert.AreEqual(
        _playerAjit.ToString(),
        "Ajit,6,9,(Ajit,Larry,Ajit),(Ajit,Rachel,Ajit),(Ajit,Rachel,Ajit),(Ajit,Numan,Numan),(Ajit,Numan,Numan)");
    Assert.AreEqual(_playerRachel.ToString(),
                    "Rachel,1,2,(Rachel,Larry,Larry),(Rachel,Larry,Rachel),(Rachel,Numan,Numan)");
  }

  [Test]
  public void Test_SimpleAddMatchWin() {
    Assert.AreEqual(_playerRachel.ToString(),
                    "Rachel,1,2,(Rachel,Larry,Larry),(Rachel,Larry,Rachel),(Rachel,Numan,Numan)");
    _playerRachel.AddMatchHistory(new MatchRecord(_playerRachel, _playerLarry, _playerRachel));
    Assert.AreEqual(
        _playerRachel.ToString(),
        "Rachel,2,2,(Rachel,Larry,Larry),(Rachel,Larry,Rachel),(Rachel,Numan,Numan),(Rachel,Larry,Rachel)");
  }

  [Test]
  public void Test_SimpleAddMatchLose() {
    Assert.AreEqual(_playerRachel.ToString(),
                    "Rachel,1,2,(Rachel,Larry,Larry),(Rachel,Larry,Rachel),(Rachel,Numan,Numan)");
    _playerRachel.AddMatchHistory(new MatchRecord(_playerRachel, _playerLarry, _playerLarry));
    Assert.AreEqual(
        _playerRachel.ToString(),
        "Rachel,1,3,(Rachel,Larry,Larry),(Rachel,Larry,Rachel),(Rachel,Numan,Numan),(Rachel,Larry,Larry)");
  }
  [Test]
  public void Test_AddMatchWithFullQueueWin() {
    Assert.AreEqual(
        _playerLarry.ToString(),
        "Larry,5,3,(Larry,Numan,Numan),(Larry,Rachel,Rachel),(Larry,Ajit,Larry),(Larry,Rachel,Larry),(Larry,Numan,Larry)");

    _playerLarry.AddMatchHistory(new MatchRecord(_playerRachel, _playerLarry, _playerLarry));
    Assert.AreEqual(
        _playerLarry.ToString(),
        "Larry,6,3,(Larry,Rachel,Rachel),(Larry,Ajit,Larry),(Larry,Rachel,Larry),(Larry,Numan,Larry),(Rachel,Larry,Larry)");
  }
  [Test]
  public void Test_AddMatchWithFullQueueLose() {
    Assert.AreEqual(
        _playerLarry.ToString(),
        "Larry,5,3,(Larry,Numan,Numan),(Larry,Rachel,Rachel),(Larry,Ajit,Larry),(Larry,Rachel,Larry),(Larry,Numan,Larry)");
    _playerLarry.AddMatchHistory(new MatchRecord(_playerRachel, _playerLarry, _playerRachel));
    Assert.AreEqual(
        _playerLarry.ToString(),
        "Larry,5,4,(Larry,Rachel,Rachel),(Larry,Ajit,Larry),(Larry,Rachel,Larry),(Larry,Numan,Larry),(Rachel,Larry,Rachel)");
  }

  [Test]
  public void Test_AddMatchBadRecord() {
    Assert.AreEqual(
        _playerLarry.ToString(),
        "Larry,5,3,(Larry,Numan,Numan),(Larry,Rachel,Rachel),(Larry,Ajit,Larry),(Larry,Rachel,Larry),(Larry,Numan,Larry)");
    Assert.False(_playerLarry.AddMatchHistory(new MatchRecord(_playerRachel, _playerLarry, null)));
  }
  [Test]
  public void Test_AddMatchBadPlayerInRecord() {
    Assert.AreEqual(
        _playerLarry.ToString(),
        "Larry,5,3,(Larry,Numan,Numan),(Larry,Rachel,Rachel),(Larry,Ajit,Larry),(Larry,Rachel,Larry),(Larry,Numan,Larry)");
    Assert.False(
        _playerLarry.AddMatchHistory(new MatchRecord(_unInitPlayer, _playerLarry, _playerLarry)));
  }
}
