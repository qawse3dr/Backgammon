using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Logger = LNAR.Logger;
using System.IO;
using System;

public class TestDatabase {
  [SetUp]
  public void Setup() {
    Database.DB_PATH = null;
  }

  [Test]
  public void Test_CreateDatabaseNoPath() {
    Assert.IsNull(Database.CreateDatabase());
  }

  [Test]
  public void Test_CreateDatabase() {
    Database.DB_PATH = "UnitTest.db";
    Assert.IsNotNull(Database.CreateDatabase());
  }

  [Test]
  public void Test_CreateReadDatabaseFileDoesntExist() {
    Database.DB_PATH = "UnitTestDNE.db";
    var db = Database.CreateDatabase();
    Assert.IsNotNull(db);
    var players = db.ReadDB();
    Assert.AreEqual(2, players.Count);
    Assert.True(File.Exists(Database.DB_PATH));
  }

  [Test]
  public void Test_CreateReadDatabaseFile() {
    Database.DB_PATH = "Assets/Unit-Tests/Database/unitTest.db";
    var db = Database.CreateDatabase();
    Assert.IsNotNull(db);
    var players = db.ReadDB();
    Assert.AreEqual(2, players.Count);
    Assert.True(File.Exists(Database.DB_PATH));

    Assert.AreEqual("Larry,1,1,(Larry,Rachel,Larry),(Larry,Rachel,Rachel)", players[0].ToString());
    Assert.AreEqual("Rachel,1,1,(Larry,Rachel,Larry),(Larry,Rachel,Rachel)", players[1].ToString());
  }

  [Test]
  public void Test_CreateReadDatabaseWrite1Player() {
    Database.DB_PATH = "UnitTestDNE.db";
    var db = Database.CreateDatabase();
    Assert.IsNotNull(db);
    var players = db.ReadDB();
    Assert.AreEqual(2, players.Count);
    Assert.True(File.Exists(Database.DB_PATH));
    db.WritePlayerToDB(Player.CreateNewPlayer("Numan", PlayerEnum.NotSet));
    players = db.ReadDB();
    Assert.AreEqual(3, players.Count);
    Assert.AreEqual("Guest 1,0,0", players[0].ToString());
    Assert.AreEqual("Guest 2,0,0", players[1].ToString());
    Assert.AreEqual("Numan,0,0", players[2].ToString());

    Assert.True(File.Exists(Database.DB_PATH));
  }

  [Test]
  public void Test_CreateReadDatabaseWrite2Players() {
    Database.DB_PATH = "UnitTestDNE.db";
    var db = Database.CreateDatabase();
    Assert.IsNotNull(db);
    var players = db.ReadDB();
    Assert.AreEqual(2, players.Count);
    Assert.True(File.Exists(Database.DB_PATH));
    db.WritePlayerToDB(Player.CreateNewPlayer("Numan", PlayerEnum.NotSet),
                       Player.CreateNewPlayer("Ajit", PlayerEnum.NotSet));
    players = db.ReadDB();
    Assert.AreEqual(4, players.Count);
    Assert.AreEqual("Guest 1,0,0", players[0].ToString());
    Assert.AreEqual("Guest 2,0,0", players[1].ToString());
    Assert.AreEqual("Numan,0,0", players[2].ToString());
    Assert.AreEqual("Ajit,0,0", players[3].ToString());

    Assert.True(File.Exists(Database.DB_PATH));
  }

  [Test]
  public void Test_CreateUpdatePlayer() {
    Database.DB_PATH = "UnitTestAddUser.db";
    var db = Database.CreateDatabase();
    Assert.IsNotNull(db);
    var players = db.ReadDB();
    Assert.AreEqual(2, players.Count);
    Assert.True(File.Exists(Database.DB_PATH));
    players[0].AddMatchHistory(new MatchRecord(players[0], players[1], players[0]));
    db.WritePlayerToDB(players[0]);
    players = db.ReadDB();
    Assert.AreEqual(2, players.Count);
    Assert.AreEqual("Guest 1,1,0,(Guest 1,Guest 2,Guest 1)", players[0].ToString());
    Assert.AreEqual("Guest 2,0,0", players[1].ToString());

    Assert.True(File.Exists(Database.DB_PATH));
  }

  [TearDown]
  public void CleanUp() {
    if (Database.DB_PATH != null && !Database.DB_PATH.StartsWith("Assets")) {
      try {
        File.Delete(Database.DB_PATH);
      } catch (Exception) {
      }
    }
  }
}
