using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateModeHotPotato : GameStateModeBase {
	public class PlayerStatistics {
		public HotPotatoPlayerBase player = null;
		public int deaths = 0;
	}

	[SerializeField]
	private HotPotatoPlayerLocal playerLocalPrefab = null;
	[SerializeField]
	private HotPotatoPlayerBot playerBotPrefab = null;
	[SerializeField]
	private UIGameTextStatistics uiStatText = null;

	private HotPotatoPlayerBase[] players = null;

	private static int playersCount = 3;
	private static int gamesCount = 0;

	private static Dictionary<int, PlayerStatistics> playersStatistics = new Dictionary<int, PlayerStatistics>();


	public override StateTypes Type {
		get {
			return StateTypes.GameModeHotPotato;
		}
	}

	public static int PlayersCount {
		get {
			return playersCount;
		}
		set {
			playersCount = value;
		}
	}

	public override void Activate() {
		base.Activate();

		Instantiate(playerLocalPrefab).transform.parent = transform;
		for (int i = 0; i < PlayersCount; i++) {
			Instantiate(playerBotPrefab).transform.parent = transform;
		}

		players = GetComponentsInChildren<HotPotatoPlayerBase>();

		foreach (var i in players) {
			i.OnDoAction += OnPlayerDoAction;
			i.transform.position = new Vector3(Random.Range(0f, 25f), 1.25f, Random.Range(0f, 20f));

			if (!playersStatistics.ContainsKey(i.PlayerGameId)) {
				playersStatistics[i.PlayerGameId] = new PlayerStatistics();
			}
			playersStatistics[i.PlayerGameId].player = i;
		}

		ThrowBombToRandomPlayer();

		uiStatText.ShowInGameUI();
		RefreshStatisticsUI();
	}

	public override void Deactivate() {
		base.Deactivate();

	}

	public static void ClearStatistics() {
		gamesCount = 0;
		playersStatistics.Clear();
	}

	protected void RefreshStatisticsUI() {
		string text = "ROUND #" + (gamesCount + 1) + "\nDEATHS:  ";
		foreach (var i in playersStatistics) {
			text += string.Format("{0}-{1}  ", i.Value.player.PlayerName, i.Value.deaths);
		}
		uiStatText.UpdateStatisticsText(text);
	}

	protected override void OnBombReachedTarget(Bomb.Target target) {
		base.OnBombReachedTarget(target);

		var playerWithBomb = FindPlayerWithBomb();
		foreach (var i in players) {
			i.HasBomb = playerWithBomb == i;
		}
	}

	protected override void OnBombBoom(Bomb.Target target) {
		base.OnBombBoom(target);

		var playerWithBomb = FindPlayerWithBomb();
		if (playerWithBomb != null) {
			playerWithBomb.IsDead = true;
			playersStatistics[playerWithBomb.PlayerGameId].deaths++;

			gamesCount++;
			if (gamesCount < 5) {
				StartCoroutine(RestartAfter(1f));
			} else {
				uiStatText.ShowResultsUI();
				string results = "RESULTS:\n\n";
				foreach (var i in playersStatistics) {
					results += string.Format("{0} = {1}\n", i.Value.player.PlayerName, i.Value.deaths);
				}
				uiStatText.UpdateResultsText(results);
			}
		} else {
			Debug.Log("On boom bomb has no player target!");
		}

		RefreshStatisticsUI();

		IsGameEnd = true;
	}

	private void OnPlayerDoAction(HotPotatoPlayerBase player) {
		if (player == FindPlayerWithBomb()) {
			HotPotatoPlayerBase targetPlayer = null;
			float minDistance = float.MaxValue;

			foreach (var anotherPlayer in players) {
				if (player != anotherPlayer) {
					Vector3 direction = anotherPlayer.transform.position - player.transform.position;
					Vector2 toAnother = new Vector2(direction.x, direction.z).normalized;
					Vector2 fromSelf = new Vector2(player.Direction.x, player.Direction.z).normalized;

					if (Mathf.Abs(Vector2.Angle(toAnother, fromSelf)) < 60f) {
						float distance = direction.magnitude;
						if (minDistance > distance) {
							minDistance = distance;
							targetPlayer = anotherPlayer;
						}
					}
				}
			}

			if (targetPlayer != null && minDistance < 8f) {
				bomb.SetNewTarget(new Bomb.Target {
					targetObject = targetPlayer.transform,
					targetOffest = new Vector3(0f, 1f, 0f),
				});
			}
		}
	}

	private void ThrowBombToRandomPlayer() {
		bomb.SetNewTarget(new Bomb.Target {
			targetObject = players[Random.Range(0, players.Length)].transform,
			targetOffest = new Vector3(0f, 1f, 0f),
		});
	}

	private HotPotatoPlayerBase FindPlayerWithBomb() {
		if (bomb.CurrentTarget != null && bomb.CurrentTarget.targetObject != null) {
			return bomb.CurrentTarget.targetObject.GetComponent<HotPotatoPlayerBase>();
		}
		return null;
	}

	private IEnumerator RestartAfter(float delay) {
		yield return new WaitForSeconds(delay);
		GameStateMachine.Instance.GoToState(Type);
	}
}