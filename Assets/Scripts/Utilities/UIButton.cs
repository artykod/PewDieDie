using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerClickHandler {
	private Button button = null;

	private void Awake() {
		button = GetComponent<Button>();
	}

	protected virtual void OnClick(PointerEventData eventData) {
		// override this
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
		if (button != null && !button.interactable) {
			return;
		}

		OnClick(eventData);
	}
}
