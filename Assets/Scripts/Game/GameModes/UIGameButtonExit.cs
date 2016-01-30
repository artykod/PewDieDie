using UnityEngine.EventSystems;

public class UIGameButtonExit : UIButton {
	protected override void OnClick(PointerEventData eventData) {
		base.OnClick(eventData);
		GameStateMachine.Instance.GoToState(StateTypes.MainMenu);
	}
}