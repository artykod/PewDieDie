using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonMainMenuPlay : UIButton {
	[SerializeField]
	private StateTypes gameMode = StateTypes.GameModeHotPotato;

	protected override void OnClick(PointerEventData eventData) {
		base.OnClick(eventData);
		GameStateMachine.Instance.GoToState(gameMode);
	}
}
