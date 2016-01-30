using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCameraAction : MonoBehaviour {
	private static HashSet<PlayerBase> allActivePlayers = new HashSet<PlayerBase>();

	private Vector3 initialPosition = Vector3.zero;

	public static void PlayerActivated(PlayerBase player) {
		allActivePlayers.Add(player);
	}
	public static void PlayerDeactivated(PlayerBase player) {
		allActivePlayers.Remove(player);
	}

	private void Awake() {
		initialPosition = transform.position;
	}

	private void Update() {
		Vector3 targetPoint = Vector3.zero;
		float maxDistance = 0f;

		if (allActivePlayers.Count > 0) {
			HashSet<PlayerBase> toRemove = new HashSet<PlayerBase>();
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

		var direction = (targetPoint - initialPosition).normalized;
		var targetRotation = Quaternion.LookRotation(direction);
		var targetPosition = initialPosition - direction * Mathf.Clamp(maxDistance, 0f, 25f);

		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.15f);
		transform.position = Vector3.Lerp(transform.position, targetPosition, 0.1f);
	}
}
