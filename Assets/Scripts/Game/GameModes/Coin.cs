using UnityEngine;

public class Coin : MonoBehaviour {
	[SerializeField]
	private int amount = 1;

	public int Amount {
		get {
			return amount;
		}
	}
}
