using UnityEngine;
using System.Collections.Generic;

public class HotPotatoPlayerBot : HotPotatoPlayerBase {
	private Vector3 targetPosition = Vector3.zero;
	private float doActionCooldown = 0f;
	private float throwBombCooldown = 0f;
	private float beatEnemyCooldown = 0f;
	private float targetOffsetCooldown = 0f;
	private float changePositionCooldown = 0f;
	private Vector3 targetOffset = Vector3.zero;
	private Material overrideMaterial = null;
	private Transform targetObject = null; // coin, etc.

	private static Color[] randomColors = new Color[] {
		//Color.black,
		//Color.blue,
		//Color.gray,
		Color.white,
		//Color.green,
		//Color.yellow,
	};
	private static int colorCounter = 0;

	public override string PlayerName {
		get {
			return "Bot " + PlayerGameId;
		}
	}

	protected override void Start() {
		base.Start();

		ApplyRandomTarget();

		var rnds = GetComponentsInChildren<MeshRenderer>();
		var onlyMeshes = new List<MeshRenderer>();
		foreach (var i in rnds) {
			if (i.GetComponent<TextMesh>() == null) {
				onlyMeshes.Add(i);
			}
		}
		rnds = onlyMeshes.ToArray();

		if (rnds.Length > 0) {
			overrideMaterial = Instantiate(rnds[0].sharedMaterial);
			overrideMaterial.color = randomColors[(colorCounter = (colorCounter + 1) % randomColors.Length)];
			foreach (var i in rnds) {
				i.sharedMaterial = overrideMaterial;
			}
		}
	}

	protected override void OnDestroy() {
		base.OnDestroy();

		if (overrideMaterial != null) {
			Destroy(overrideMaterial);
		}
	}

	private void ApplyRandomTarget() {
		if (targetObject == null) {
			var closestCoin = Coin.GetClosestCoinForPosition(transform.position);
			if (closestCoin != null) {
				targetObject = closestCoin.transform;
			}
		}

		if (targetObject == null) {
			targetPosition = new Vector3(Random.Range(-10f, 32f), 0f, Random.Range(0f, 20f));
		} else {
			targetPosition = targetObject.position;
			targetPosition.y = 0f;
		}
	}

	protected override void Update() {
		base.Update();

		if (IsDead) {
			return;
		}

		HotPotatoPlayerBase closestPlayer = null;
		float minDistance = float.MaxValue;

		foreach (var i in allPlayers) {
			if (i != this && !i.IsDead) {
				float distance = (i.transform.position - transform.position).magnitude;
				if (distance < minDistance) {
					minDistance = distance;
					closestPlayer = i;
				}
			}
		}

		if (!HasBomb || doActionCooldown > 0f) {
			if (playerWithBomb != null) {
				if ((transform.position - playerWithBomb.transform.position).magnitude < 10f || changePositionCooldown < 0f) {
					if ((transform.position - targetPosition).magnitude < 2f) {
						ApplyRandomTarget();
						changePositionCooldown = Random.Range(3f, 7f);
					} else {
						Speed = 0f;
					}
					Direction = Vector3.Lerp(Direction, new Vector3(targetPosition.x - transform.position.x, 0f, targetPosition.z - transform.position.z).normalized, 0.1f);
				} else {
					changePositionCooldown -= Time.deltaTime;
					Speed = 0f;
				}

				if (closestPlayer != null && minDistance < 2f && beatEnemyCooldown <= 0f) {
					DoAction();
					beatEnemyCooldown = Random.Range(0.5f, 2f);
				}
			}
		} else {
			if (closestPlayer != null) {
				targetPosition = closestPlayer.transform.position;

				if (doActionCooldown < 0f) {
					if (throwBombCooldown < 0f) {
						DoAction();
						throwBombCooldown = Random.Range(0.5f, 1f);
					} else {
						throwBombCooldown -= Time.deltaTime;
					}
				}
			}

			if (targetOffsetCooldown < 0f) {
				targetOffset = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
				targetOffsetCooldown = Random.Range(0.05f, 0.15f);
				var targetWithOffset = targetPosition + targetOffset;
				Direction = Vector3.Lerp(Direction, new Vector3(targetWithOffset.x - transform.position.x, 0f, targetWithOffset.z - transform.position.z).normalized, 0.5f).normalized;
			} else {
				targetOffsetCooldown -= Time.deltaTime;
			}
		}

		if (doActionCooldown > 0f) {
			doActionCooldown -= Time.deltaTime;
		}
		if (beatEnemyCooldown > 0f) {
			beatEnemyCooldown -= Time.deltaTime;
		}
		
		Speed = 10f;
	}

	protected override void OnHasBombChanged(bool hasBombNow) {
		base.OnHasBombChanged(hasBombNow);

		doActionCooldown = Random.Range(0.1f, 0.5f);
	}

	protected override void OnCollisionEnter(Collision collision) {
		base.OnCollisionEnter(collision);

		if (collision.gameObject.tag == "Obstacle") {
			ApplyRandomTarget();
			changePositionCooldown = -1f;
		}
	}
	protected override void OnCollisionStay(Collision collision) {
		base.OnCollisionStay(collision);
		OnCollisionEnter(collision);
	}
}
