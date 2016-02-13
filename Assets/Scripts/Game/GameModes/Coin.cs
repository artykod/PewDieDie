using UnityEngine;
using System.Collections.Generic;

public class Coin : MonoBehaviour {
	[SerializeField]
	private int amount = 1;

	private static LinkedList<Coin> activeCoins = new LinkedList<Coin>();

	public int Amount {
		get {
			return amount;
		}
	}
	public bool IsCollected {
		get;
		private set;
	}

	public static Coin GetClosestCoinForPosition(Vector3 position) {
		var minDistance = float.MaxValue;
		Coin closestCoin = null;

		foreach (var i in activeCoins) {
			var distance = (i.transform.position - position).magnitude;
			if (distance < minDistance) {
				closestCoin = i;
				minDistance = distance;
			}
		}

		return closestCoin;
	}

	public void Collect() {
		IsCollected = true;
	}

	private void Awake() {
		activeCoins.AddLast(this);
	}

	private void OnDestroy() {
		activeCoins.Remove(this);
	}
}
