

public class TestUtil {
  public static GameState CreateGameState() {
    GameState game = new GameState();
    Player p1 = Player.CreateNewPlayer("Larry", PlayerEnum.Player1);
    Player p2 = Player.CreateNewPlayer("Larry2ElectricBogaloo", PlayerEnum.Player1);
    game.InitBoardState(p1, p2);
    return game;
  }
}