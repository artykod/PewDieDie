using UnityEngine;
using UnityEngine.UI;

public class UIGameResultLine : MonoBehaviour {
	[SerializeField]
	private Text playerNameText = null;
	[SerializeField]
	private Text pointsText = null;
	[SerializeField]
	private Text coinsText = null;

	public bool IsLocalPlayer {
		get;
		private set;
	}

	public void BuildForPlayer(GameStateModeHotPotato.PlayerStatistics playerInfo) {
		if (playerInfo == null || playerInfo.player == null) {
			playerNameText.text = "----";
			pointsText.text = "--";
			coinsText.text = "--";
			IsLocalPlayer = false;
		} else {
			playerNameText.text = playerInfo.player.PlayerName;
			pointsText.text = string.Format("{0} pts", Mathf.Max(0, playerInfo.points));
			coinsText.text = string.Format("{0}", Mathf.Max(0, playerInfo.coins));
			IsLocalPlayer = playerInfo.player is HotPotatoPlayerLocal;
		}

		RefreshTextsColor(IsLocalPlayer ? Color.green : Color.white);
	}

	private void RefreshTextsColor(Color color) {
		playerNameText.color = color;
		pointsText.color = color;
		coinsText.color = color;
	}
}
