using UnityEngine;

public class PlayerBase : MonoBehaviour {
	public Vector3 Direction {
		get;
		protected set;
	}
	public float Speed {
		get;
		protected set;
	}
	
	new public Transform transform {
		get;
		private set;
	}

	protected virtual void Awake() {
		transform = base.transform;
	}

	protected virtual void Start() {
		GameCameraAction.PlayerActivated(this);
	}

	protected virtual void OnDestroy() {
		GameCameraAction.PlayerDeactivated(this);
	}

	protected virtual void FixedUpdate() {
		//transform.position += Direction * Speed * Time.fixedDeltaTime;

		float angle = Mathf.Atan2(-Direction.z, Direction.x);
		Quaternion rotation = Quaternion.Euler(0f, angle * 180f / Mathf.PI, 0f);
		//transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.25f);

		GetComponent<Rigidbody>().MovePosition(transform.position + Direction * Speed * Time.fixedDeltaTime);
		GetComponent<Rigidbody>().MoveRotation(Quaternion.Lerp(transform.rotation, rotation, 0.25f));
	}

	protected virtual void Update() {
		/*transform.position += Direction * Speed * Time.fixedDeltaTime;

		float angle = Mathf.Atan2(-Direction.z, Direction.x);
		Quaternion rotation = Quaternion.Euler(0f, angle * 180f / Mathf.PI, 0f);
		transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.25f);*/
	}
}
