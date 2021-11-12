using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Logger = LNAR.Logger;
public class Database {
  private static Database _database = null;
  public static string DB_PATH = null;

  public static Database CreateDatabase() {
    if (DB_PATH == null) {
      Logger.Debug("DB_PATH not set can't read");
      return null;
    }
    if (_database == null)
      _database = new Database();
    return _database;
  }

  public static Database CreateDatabase(string dbPath) {
    DB_PATH = dbPath;
    return CreateDatabase();
  }
  private Database() {}

  public List<Player> ReadDB() {
    if (DB_PATH == null) {
      Logger.Debug("DB_PATH not set can't read");
      return null;
    }
    var players = new List<Player>();
    if (File.Exists(DB_PATH)) {
      Logger.Debug("File exists reading data");
      StreamReader file = new StreamReader(DB_PATH);

      string line;
      while ((line = file.ReadLine()) != null) {
        players.Add((Player)line);
      }
      file.Close();
    } else {
      Logger.Debug("File DNE creating file and writing guests files");
      // Create text file and write default profiles to db
      File.CreateText(DB_PATH).Close();
      players.Add(Player.CreateNewPlayer("Guest 1", PlayerEnum.NotSet));
      players.Add(Player.CreateNewPlayer("Guest 2", PlayerEnum.NotSet));
      WritePlayerToDB(players[0], players[1]);
    }
    return players;
  }

  public bool WritePlayerToDB(Player playerToWrite) {
    return WritePlayerToDB(playerToWrite, null);
  }
  public bool WritePlayerToDB(Player playerToWrite1, Player playerToWrite2) {
    if (DB_PATH == null) {
      Logger.Debug("DB_PATH not set can't read");
      return false;
    }
    // Reads the Players from Current DB
    var players = ReadDB();

    StreamWriter file = new StreamWriter(DB_PATH);
    foreach (Player player in players) {
      if (player == playerToWrite1) {
        file.WriteLine(playerToWrite1.ToString());
        playerToWrite1 = null;
      } else if (player == playerToWrite2) {
        file.WriteLine(playerToWrite2.ToString());
        playerToWrite2 = null;
      } else {
        file.WriteLine(player.ToString());
      }
    }
    if (playerToWrite1 != null)
      file.WriteLine(playerToWrite1.ToString());
    if (playerToWrite2 != null)
      file.WriteLine(playerToWrite2.ToString());

    file.Close();
    return true;
  }
}
