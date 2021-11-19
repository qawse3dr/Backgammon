using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectCharacter {
  GameObject player1;
  GameObject player2;
  GameObject errMsg;
  GameObject selectPlayerUI;
  List<Player> playerList;

  [SetUp]
  public void Setup() {
    Database.DB_PATH = null;
    GameHandler.Game = TestUtil.CreateGameState();
    player1 = new GameObject("Text", typeof(Text));
    player2 = new GameObject("Text", typeof(Text));
    errMsg = new GameObject("Text", typeof(Text));

    selectPlayerUI = new GameObject("selectPlayerUI", typeof(SelectPlayerUIHandler));
    selectPlayerUI.GetComponent<SelectPlayerUIHandler>().Player1DropDownText =
        player1.GetComponent<Text>();
    selectPlayerUI.GetComponent<SelectPlayerUIHandler>().Player2DropDownText =
        player2.GetComponent<Text>();
    selectPlayerUI.GetComponent<SelectPlayerUIHandler>().ErrorMsg = errMsg.GetComponent<Text>();

    playerList = new List<Player> {
      Player.CreateNewPlayer("Larry", PlayerEnum.NotSet),
      Player.CreateNewPlayer("Rachel", PlayerEnum.NotSet),
      Player.CreateNewPlayer("Numan", PlayerEnum.NotSet),
      Player.CreateNewPlayer("Ajit", PlayerEnum.NotSet),
    };

    selectPlayerUI.GetComponent<SelectPlayerUIHandler>().PlayerList = playerList;
  }

  // Tests adding 2 players with the same name should fail
  [Test]
  public void Test_2PlayersSameName() {
    player1.GetComponent<Text>().text = "Larry";
    player2.GetComponent<Text>().text = "Larry";
    Player p1 = null, p2 = null;

    Assert.False(selectPlayerUI.GetComponent<SelectPlayerUIHandler>().GetPlayersFromDropdown(
        ref p1, ref p2));
  }

  // Tests a player that is not in the list should fail
  [Test]
  public void Test_NameNotInList() {
    player1.GetComponent<Text>().text = "Steve";
    player2.GetComponent<Text>().text = "Larry";
    Player p1 = null, p2 = null;

    Assert.False(selectPlayerUI.GetComponent<SelectPlayerUIHandler>().GetPlayersFromDropdown(
        ref p1, ref p2));
  }

  // Tests a player that is not in the list should fail
  [Test]
  public void Test_SelectCharacterValid() {
    player1.GetComponent<Text>().text = "Rachel";
    player2.GetComponent<Text>().text = "Larry";
    Player p1 = null, p2 = null;

    Assert.True(selectPlayerUI.GetComponent<SelectPlayerUIHandler>().GetPlayersFromDropdown(
        ref p1, ref p2));
  }
}
