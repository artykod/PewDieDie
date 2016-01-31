using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuCountChanger : MonoBehaviour {
	[SerializeField]
	private Text sliderLabel = null;
	private Slider slider = null;
	private string labelFormat = null;

	private void Awake() {
		if (sliderLabel != null) {
			labelFormat = sliderLabel.text;
		}

		slider = GetComponent<Slider>();
		slider.onValueChanged.AddListener(OnValueChanged);
		slider.value = GameStateModeHotPotato.PlayersCount;
		OnValueChanged(slider.value);
	}

	private void OnValueChanged(float value) {
		if (sliderLabel != null) {
			sliderLabel.text = string.Format(labelFormat, (int)value);
		}
		GameStateModeHotPotato.PlayersCount = (int)value;
	}
}
