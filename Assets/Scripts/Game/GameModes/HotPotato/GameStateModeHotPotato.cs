using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameStateModeHotPotato : GameStateModeBase {
	public class PlayerStatistics {
		public HotPotatoPlayerBase player = null;
		public int points = 0;
		public int coins = 0;
	}

	[SerializeField]
	private HotPotatoPlayerLocal playerLocalPrefab = null;
	[SerializeField]
	private HotPotatoPlayerBot playerBotPrefab = null;
	[SerializeField]
	private Coin coinPrefab = null;
	[SerializeField]
	private UIGameTextStatistics uiStatText = null;

	private HotPotatoPlayerBase[] players = null;
	private float dropCoinCooldown = 1f;

	private static int playersCount = 3;
	private static int roundsCount = 0;

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
			i.OnCoinCollected += OnCoinCollectedByPlayer;
			i.transform.position = new Vector3(Random.Range(0f, 25f), 1.25f, Random.Range(0f, 20f));

			if (!playersStatistics.ContainsKey(i.PlayerGameId)) {
				playersStatistics[i.PlayerGameId] = new PlayerStatistics();
			}
			playersStatistics[i.PlayerGameId].player = i;
		}

		ThrowBombToRandomPlayer();

		uiStatText.SwitchToGameUI();
		RefreshInGameStats();
	}

	private void OnCoinCollectedByPlayer(HotPotatoPlayerBase player, Coin coin) {
		if (player == null || coin == null || coin.IsCollected) {
			return;
		}
		playersStatistics[player.PlayerGameId].coins += coin.Amount;
		coin.Collect();
		Destroy(coin.gameObject);
	}

	public override void Deactivate() {
		base.Deactivate();

	}

	public static void ClearStatistics() {
		roundsCount = 0;
		playersStatistics.Clear();
	}

	protected override void Update() {
		base.Update();

		if (dropCoinCooldown > 0f) {
			dropCoinCooldown -= Time.deltaTime;
		}

		if (dropCoinCooldown < 0f) {
			dropCoinCooldown = 3f;

			StartCoroutine(DropCoinAtFreePosition());
		}
	}

	private IEnumerator DropCoinAtFreePosition() {
		var coin = Instantiate(coinPrefab);
		var noObstaclesAtBottom = false;
		var maxTries = 3;

		coin.transform.SetParent(transform);

		do {
			coin.transform.position = new Vector3(Random.Range(0f, 25f), 15f, Random.Range(0f, 20f));

			var hits = Physics.RaycastAll(new Ray(coin.transform.position, Vector3.down));
			noObstaclesAtBottom = true;
			foreach (var i in hits) {
				if (i.collider.gameObject.tag == "Obstacle") {
					noObstaclesAtBottom = false;
					break;
				}
			}

			maxTries--;
			if (maxTries < 0) {
				maxTries = 3;
				yield return null;
			}

		} while (!noObstaclesAtBottom);
	}

	protected void RefreshInGameStats() {
		string text = "ROUND #" + (roundsCount + 1) + "\nDEATHS:  ";
		foreach (var i in playersStatistics) {
			text += string.Format("{0}-{1}  ", i.Value.player.PlayerName, i.Value.points);
		}
		uiStatText.RefreshInGameStats(text);
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
			foreach (var p in playersStatistics) {
				if (playerWithBomb.PlayerGameId != p.Key) {
					p.Value.points++;
				}
			}

			roundsCount++;
			if (roundsCount < 5) {
				StartCoroutine(StartNewRound());
			} else {
				StartCoroutine(AllRoundsDone());
			}
		} else {
			Debug.Log("On boom bomb has no player target!");
		}

		RefreshInGameStats();

		IsRoundEnd = true;
	}

	private void OnPlayerDoAction(HotPotatoPlayerBase player) {
		HotPotatoPlayerBase targetPlayer = null;
		float minDistance = float.MaxValue;

		foreach (var anotherPlayer in players) {
			if (player != anotherPlayer && !anotherPlayer.HasProtection) {
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

		if (targetPlayer != null) {
			//
			// throw bomb to enemy
			//
			if (player == FindPlayerWithBomb()) {
				if (minDistance < 8f) {
					bomb.SetNewTarget(new Bomb.Target {
						targetObject = targetPlayer.transform,
						targetOffest = new Vector3(0f, 1f, 0f),
					});
				}
			} else {
				//
				// beat enemy
				//
				if (minDistance < 2f) {
					targetPlayer.BeatedByOtherPlayer(player);
				}
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

	private IEnumerator StartNewRound() {
		yield return new WaitForSeconds(1f);

		GameStateMachine.Instance.GoToState(Type);
	}

	private IEnumerator AllRoundsDone() {
		yield return new WaitForSeconds(1f);

		uiStatText.SwitchToResultUI();
		uiStatText.RefreshResultsStats(playersStatistics.Values.ToArray());
	}
}