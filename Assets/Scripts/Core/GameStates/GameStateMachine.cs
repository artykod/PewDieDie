using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameStateMachine : AbstractSingletonBehaviour<GameStateMachine, GameStateMachine> {
	public StateTypes CurrentState {
		get;
		private set;
	}

	public GameStateBase CurrentStateObject {
		get;
		private set;
	}

	public delegate void OnStateChangedDelegate(StateTypes newState, StateTypes oldState);
	public event OnStateChangedDelegate OnStateChanged = delegate { };

	private Dictionary<StateTypes, GameStateBase> statesPrefabs = new Dictionary<StateTypes, GameStateBase>();

	private void Awake() {
		var allStatesInResources = Resources.LoadAll<GameStateBase>("GameStates");
		foreach (var state in allStatesInResources) {
			statesPrefabs[state.Type] = state;
		}
	}

	private void Start() {
		GoToState(StateTypes.MainMenu);
	}

	public void GoToState(StateTypes state) {
		StartCoroutine(ChangeState(state));
	}

	private IEnumerator ChangeState(StateTypes state) {
		if (CurrentStateObject != null) {
			CurrentStateObject.Deactivate();

			yield return null;

			Destroy(CurrentStateObject.gameObject);
			CurrentStateObject = null;
		}

		yield return null;

		var unloadOp = Resources.UnloadUnusedAssets();
		while (!unloadOp.isDone) {
			yield return null;
		}

		var previousState = CurrentState;
		CurrentState = StateTypes.Unknown;

		GameStateBase statePrefab = null;
		if (statesPrefabs.TryGetValue(state, out statePrefab)) {

			yield return null;

			CurrentStateObject = Instantiate(statePrefab);
			CurrentStateObject.Activate();
			CurrentState = state;

			Debug.LogFormat("Loaded new state {0}", CurrentStateObject != null ? CurrentStateObject.Type : StateTypes.Unknown);

			OnStateChanged(state, previousState);
		} else {
			throw new NotSupportedException(string.Format("Not found prefab for state {0}", state));
		}
	}
}