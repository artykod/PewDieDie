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
	public bool HasDebuff {
		get {
			return beatedTime > 0f;
		}
	}

	public event System.Action<HotPotatoPlayerBase> OnDoAction = delegate { };
	public event System.Action<HotPotatoPlayerBase, Coin> OnCoinCollected = delegate { };

	[SerializeField]
	private TextMesh textName = null;

	protected static HashSet<HotPotatoPlayerBase> allPlayers = new HashSet<HotPotatoPlayerBase>();
	protected static HotPotatoPlayerBase playerWithBomb = null;

	private bool hasBomb = false;
	private float beatedTime = 0f;

	public void BeatedByOtherPlayer(HotPotatoPlayerBase otherPlayer) {
		beatedTime = 0.5f;
	}

	protected virtual void Awake() {
		transform = base.transform;
		rigidbody = GetComponent<Rigidbody>();

		allPlayers.Add(this);
		PlayerGameId = allPlayers.Count;

		textName.text = string.Format("<b>{0}</b>", PlayerName);

		SpeedMultiplier = 1f;
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
		Vector3 position = transform.position + Direction * (HasBomb ? Speed * 1.25f : Speed) * SpeedMultiplier * Time.fixedDeltaTime;
		
		rigidbody.MovePosition(position);
		rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, rotation, 0.175f));
	}

	protected virtual void Update() {
		SpeedMultiplier = 1f;

		// players is beated
		if (beatedTime > 0f) {
			if (!hasBomb) {
				beatedTime -= Time.deltaTime;
				SpeedMultiplier *= 0.25f;
			} else {
				beatedTime = 0f;
			}
		}
	}

	protected virtual bool DoAction() {
		if (IsDead || HasDebuff) {
			return false;
		}

		OnDoAction(this);

		return true;
	}

	protected virtual void OnHasBombChanged(bool hasBombNow) {
		if (hasBombNow) {
			playerWithBomb = this;

			// reset debuff
			beatedTime = 0f;
		}
	}

	protected virtual void OnCollisionEnter(Collision collision) {
		CheckCoinCollision(collision.gameObject);
	}
	protected virtual void OnCollisionStay(Collision collision) {
		CheckCoinCollision(collision.gameObject);
	}
	protected virtual void OnCollisionExit(Collision collision) {

	}

	private void CheckCoinCollision(GameObject obj) {
		if (obj != null) {
			var coin = obj.GetComponent<Coin>();
			if (coin != null) {
				OnCoinCollected(this, coin);
			}
		}
	}
}
