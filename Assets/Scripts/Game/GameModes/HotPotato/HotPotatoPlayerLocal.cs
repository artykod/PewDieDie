using UnityEngine;

public class HotPotatoPlayerLocal : HotPotatoPlayerBase {
	[System.Serializable]
	public class KeyboardConfig {
		public KeyCode MoveUp = KeyCode.UpArrow;
		public KeyCode MoveDown = KeyCode.DownArrow;
		public KeyCode MoveLeft = KeyCode.LeftArrow;
		public KeyCode MoveRight = KeyCode.RightArrow;
		public KeyCode DoAction = KeyCode.RightControl;
	}

	[SerializeField]
	private KeyboardConfig keyboardControls = null;

	public override string PlayerName {
		get {
			return "Player #" + PlayerGameId;
		}
	}

	protected override void Update() {
		base.Update();

		bool isControlled = false;
		Vector3 direction = Vector3.zero;

		if (Input.GetKey(keyboardControls.MoveUp)) {
			direction.z += 1f;
			isControlled = true;
		}
		if (Input.GetKey(keyboardControls.MoveDown)) {
			direction.z -= 1f;
			isControlled = true;
		}
		if (Input.GetKey(keyboardControls.MoveLeft)) {
			direction.x -= 1f;
			isControlled = true;
		}
		if (Input.GetKey(keyboardControls.MoveRight)) {
			direction.x += 1f;
			isControlled = true;
		}

		if (Input.GetKeyDown(keyboardControls.DoAction)) {
			DoAction();
		}

		if (isControlled) {
			direction.Normalize();

			Speed = 10f;
			Direction = direction;
		} else {
			Speed *= 0.8f;
		}
	}
}
