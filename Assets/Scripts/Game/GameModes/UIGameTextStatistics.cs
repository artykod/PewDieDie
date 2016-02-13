using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIGameTextStatistics : MonoBehaviour {
	[SerializeField]
	private Text textInGame = null;
	[SerializeField]
	private GameObject rootInGameUI = null;
	[SerializeField]
	private GameObject rootInResultsUI = null;
	[SerializeField]
	private RectTransform resultLinesList = null;
	[SerializeField]
	private UIGameResultLine resultLinePrefab = null;

	public void SwitchToGameUI() {
		rootInGameUI.SetActive(true);
		rootInResultsUI.SetActive(false);
	}
	public void RefreshInGameStats(string newText) {
		textInGame.text = newText;
	}

	public void SwitchToResultUI() {
		rootInGameUI.SetActive(false);
		rootInResultsUI.SetActive(true);
	}
	public void RefreshResultsStats(GameStateModeHotPotato.PlayerStatistics[] playersStats) {
		var list = new List<GameStateModeHotPotato.PlayerStatistics>(playersStats);
		list.Sort((a, b) => a.points > b.points ? -1 : (a.points < b.points ? 1 : a.coins >= b.coins ? -1 : 1));

		for (int i = 0, l = Mathf.Min(list.Count, 4); i < l; i++) {
			var player = list.Count > i ? list[i] : null;
			var line = Instantiate(resultLinePrefab);
			line.transform.SetParent(resultLinesList, false);
			line.BuildForPlayer(player);
		}
	}
}
