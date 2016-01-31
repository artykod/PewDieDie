using UnityEngine;
using System.Collections.Generic;

public class GameCameraAction : MonoBehaviour {
	private static HashSet<HotPotatoPlayerBase> allActivePlayers = new HashSet<HotPotatoPlayerBase>();

	private Vector3 initialPosition = Vector3.zero;

	public static void PlayerActivated(HotPotatoPlayerBase player) {
		allActivePlayers.Add(player);
	}
	public static void PlayerDeactivated(HotPotatoPlayerBase player) {
		allActivePlayers.Remove(player);
	}

	private void Awake() {
		initialPosition = transform.position;
	}

	private void Update() {
		Vector3 targetPoint = Vector3.zero;
		float maxDistance = 0f;

		if (allActivePlayers.Count > 0) {
			HashSet<HotPotatoPlayerBase> toRemove = new HashSet<HotPotatoPlayerBase>();
			foreach (var player in allActivePlayers) {
				if (player != null) {
					targetPoint += player.transform.position;
				} else {
					toRemove.Add(player);
				}
			}
			targetPoint /= allActivePlayers.Count;

			foreach (var player in toRemove) {
				allActivePlayers.Remove(player);
			}

			foreach (var player in allActivePlayers) {
				foreach (var playerAnother in allActivePlayers) {
					if (player != playerAnother) {
						maxDistance = Mathf.Max((player.transform.position - playerAnother.transform.position).magnitude, maxDistance);
					}
				}
			}
		}

		maxDistance = maxDistance - 10f;

		var direction = (targetPoint - initialPosition).normalized;
		var targetRotation = Quaternion.LookRotation(direction);
		var targetPosition = initialPosition - direction * Mathf.Clamp(maxDistance, -5f, 16f);

		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.015f);
		transform.position = Vector3.Lerp(transform.position, targetPosition, 0.015f);
	}
}
