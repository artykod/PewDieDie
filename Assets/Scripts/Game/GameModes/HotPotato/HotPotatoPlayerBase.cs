using UnityEngine;
using System.Collections.Generic;

public class HotPotatoPlayerBase : MonoBehaviour {
	public Vector3 Direction {
		get;
		protected set;
	}
	public float Speed {
		get;
		protected set;
	}
	public float DirectionInRadians {
		get {
			return Mathf.Atan2(-Direction.z, Direction.x);
		}
	}
	public float DirectionInDegrees {
		get {
			return DirectionInRadians * 180f / Mathf.PI;
		}
	}
	public bool HasBomb {
		get {
			return hasBomb;
		}
		set {
			if (hasBomb != value) {
				hasBomb = value;
				OnHasBombChanged(hasBomb);
			}
		}
	}
	public virtual string PlayerName {
		get {
			return "Unknown " + PlayerGameId;
		}
	}
	public int PlayerGameId {
		get;
		private set;
	}
	new public Transform transform {
		get;
		private set;
	}
	new public Rigidbody rigidbody {
		get;
		private set;
	}
	public bool IsDead {
		get;
		set;
	}
	public bool HasProtection {
		get;
		protected set;
	}
	public float SpeedMultiplier {
		get;
		protected set;
	}

	public event System.Action<HotPotatoPlayerBase> OnDoAction = delegate { };

	[SerializeField]
	private TextMesh textName = null;

	protected static HashSet<HotPotatoPlayerBase> allPlayers = new HashSet<HotPotatoPlayerBase>();
	protected static HotPotatoPlayerBase playerWithBomb = null;

	private bool hasBomb = false;

	protected virtual void Awake() {
		transform = base.transform;
		rigidbody = GetComponent<Rigidbody>();

		allPlayers.Add(this);
		PlayerGameId = allPlayers.Count;

		textName.text = string.Format("<b>{0}</b>", PlayerName);
	}

	protected virtual void Start() {
		GameCameraAction.PlayerActivated(this);
	}

	protected virtual void OnDestroy() {
		GameCameraAction.PlayerDeactivated(this);
		allPlayers.Remove(this);
	}

	protected virtual void FixedUpdate() {
		if (IsDead) {
			return;
		}
		
		Quaternion rotation = Quaternion.Euler(0f, DirectionInDegrees, 0f);
		Vector3 position = transform.position + Direction * (HasBomb ? Speed * 1.25f : Speed) * Time.fixedDeltaTime;
		
		rigidbody.MovePosition(position);
		rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, rotation, 0.175f));
	}

	protected virtual void Update() {

	}

	protected void DoAction() {
		if (IsDead) {
			return;
		}

		OnDoAction(this);
	}

	protected virtual void OnHasBombChanged(bool hasBombNow) {
		if (hasBombNow) {
			playerWithBomb = this;
		}
	}

	protected virtual void OnCollisionEnter(Collision collision) {

	}
	protected virtual void OnCollisionStay(Collision collision) {

	}
	protected virtual void OnCollisionExit(Collision collision) {

	}
}
