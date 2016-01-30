using UnityEngine;

public enum StateTypes {
	Unknown,
	Loading,
	MainMenu,
	GameModeHotPotato,
	GameResult,
}

public abstract class GameStateBase : MonoBehaviour {
	public abstract StateTypes Type {
		get;
	}

	public abstract void Activate();
	public abstract void Deactivate();
}