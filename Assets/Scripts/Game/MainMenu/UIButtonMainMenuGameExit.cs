using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonMainMenuGameExit : UIButton {
	protected override void OnClick(PointerEventData eventData) {
		base.OnClick(eventData);
		Application.Quit();
	}
}
