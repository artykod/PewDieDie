using UnityEngine;
using CnControls;

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

	private ControlsBase controls = null;

	public override string PlayerName {
		get {
			return "Player " + PlayerGameId;
		}
	}

	protected override void Awake() {
		bool isMobile = false
			|| Application.platform == RuntimePlatform.Android
			|| Application.platform == RuntimePlatform.IPhonePlayer
			//|| Application.platform == RuntimePlatform.WindowsEditor
		;

		if (isMobile) {
			controls = new ControlsMobileJoytick();
		} else {
			controls = new ControlsKeyboard(keyboardControls);
		}

		base.Awake();
	}

	protected override void Update() {
		base.Update();

		Vector3 direction = Vector3.zero;
		if (controls.GetDirection(out direction)) {
			Direction = direction.normalized;
			Speed = 10f;
		} else {
			Speed *= 0.8f;
		}

		if (controls.IsActionClicked) {
			DoAction();
		}
	}

	private abstract class ControlsBase {
		public abstract bool GetDirection(out Vector3 directionExternal);
		public abstract bool IsActionClicked {
			get;
		}
	}

	private class ControlsKeyboard : ControlsBase {
		private KeyboardConfig controlKeysConfig = null;

		public override bool GetDirection(out Vector3 directionExtenral) {
			var direction = Vector3.zero;
			var isControlled = false;

			if (Input.GetKey(controlKeysConfig.MoveUp)) {
				direction.z += 1f;
				isControlled = true;
			}
			if (Input.GetKey(controlKeysConfig.MoveDown)) {
				direction.z -= 1f;
				isControlled = true;
			}
			if (Input.GetKey(controlKeysConfig.MoveLeft)) {
				direction.x -= 1f;
				isControlled = true;
			}
			if (Input.GetKey(controlKeysConfig.MoveRight)) {
				direction.x += 1f;
				isControlled = true;
			}

			directionExtenral = direction;

			return isControlled;
		}

		public override bool IsActionClicked {
			get {
				return Input.GetKeyDown(controlKeysConfig.DoAction);
			}
		}

		public ControlsKeyboard(KeyboardConfig keysConfig) {
			controlKeysConfig = keysConfig;
		}
	}

	private class ControlsMobileJoytick : ControlsBase {
		public override bool GetDirection(out Vector3 directionExternal) {
			directionExternal = new Vector3(CnInputManager.GetAxis("Horizontal"), 0f, CnInputManager.GetAxis("Vertical"));
			return directionExternal.sqrMagnitude > 0.001f;
		}

		public override bool IsActionClicked {
			get {
				return CnInputManager.GetButtonDown("Action");
			}
		}
	}
}
