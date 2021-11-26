using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Logger = LNAR.Logger;

/**
 * The database class uses the singleton design pattern so to create the db
 * you will have to use the CreateDatabase() or CreateDatabase(string dbPath) method
 * if CreateDatabase() is used the DB_PATH must be set ahead of time or else it will return
 * null due to not knowing where the db is located
 */
public class Database {
  // Database singleton object
  private static Database _database = null;

  /**
   * Path of where the DB is located this must be set before using the database.
   * This can either be set directely Database.DB_PATH = "path/to/db.db" or using passed directly
   * when creating the db CreateDatabase("path/to/db.db).
   */
  public static string DB_PATH = null;

  /**
   * This function is used to create the database to avoid creating un-needed objects
   * if the database already exists it will just resuse that object.
   * it should be noted DB_PATH must be set before using this function
   */
  public static Database CreateDatabase() {
    if (DB_PATH == null) {
      Logger.Debug("DB_PATH not set can't read");
      return null;
    }
    if (_database == null)
      _database = new Database();
    return _database;
  }

  /**
   * Creates a database with a given dbpath
   */
  public static Database CreateDatabase(string dbPath) {
    DB_PATH = dbPath;
    return CreateDatabase();
  }
  private Database() {}

  /**
   * Reads all the current players from the db.
   * if the db doesn't exist it will create it with
   * 2 guest profiles
   * @return profiles in the db
   */
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

  /**
   * Writes a single player to the database
   * on success it will return true
   */
  public bool WritePlayerToDB(Player playerToWrite) {
    return WritePlayerToDB(playerToWrite, null);
  }

  /**
   * Writes a two player to the database
   * on success it will return true
   */
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
