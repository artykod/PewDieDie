using UnityEngine;

public class GameCore : AbstractSingletonBehaviour<GameCore, GameCore> {
	private void Awake() {
		Application.targetFrameRate = 60;

		var allStatesInScene = FindObjectsOfType<GameStateBase>();
		foreach (var i in allStatesInScene) {
			if (i.transform.parent == null) {
				Destroy(i.gameObject);
			}
		}

		GameStateMachine.Instance.Initialize();
	}

	private void Start() {
		GameStateMachine.Instance.GoToState(StateTypes.MainMenu);
		//GameStateMachine.Instance.GoToState(StateTypes.GameModeHotPotato);
	}
}