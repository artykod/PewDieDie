using UnityEngine;
using UnityEngine.UI;

public class UIGameTextStatistics : MonoBehaviour {
	[SerializeField]
	private Text textInGame = null;
	[SerializeField]
	private Text textResults = null;
	[SerializeField]
	private GameObject rootInGameUI = null;
	[SerializeField]
	private GameObject rootInResultsUI = null;

	public void ShowInGameUI() {
		rootInGameUI.SetActive(true);
		rootInResultsUI.SetActive(false);
	}
	public void ShowResultsUI() {
		rootInGameUI.SetActive(false);
		rootInResultsUI.SetActive(true);
	}

	public void UpdateStatisticsText(string newText) {
		textInGame.text = newText;
	}

	public void UpdateResultsText(string newText) {
		textResults.text = newText;
	}
}
