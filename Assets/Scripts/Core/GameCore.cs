public class GameCore : AbstractSingletonBehaviour<GameCore, GameCore> {
	private void Awake() {
		GameStateMachine.Instance.Initialize();
	}
}