using UnityEngine;

public class BillboardObject : MonoBehaviour {
	private void Update() {
		transform.rotation = Camera.main.transform.rotation;
	}
}
