using UnityEngine;

public class GameStateMainMenu : GameStateBase {
	public override StateTypes Type {
		get {
			return StateTypes.MainMenu;
		}
	}

	public override void Activate() {
		GameStateModeHotPotato.ClearStatistics();
	}

	public override void Deactivate() {
		
	}
}