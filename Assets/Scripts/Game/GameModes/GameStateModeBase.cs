using UnityEngine;

public abstract class GameStateModeBase : GameStateBase {
	[SerializeField]
	private Bomb bombPrefab = null;

	protected Bomb bomb = null;

	public bool IsRoundEnd {
		get;
		protected set;
	}

	public override void Activate() {
		bomb = Instantiate(bombPrefab);
		bomb.transform.parent = transform;
		bomb.OnBoom += OnBombBoom;
		bomb.OnTargetReached += OnBombReachedTarget;
	}

	public override void Deactivate() {
		
	}

	protected virtual void Update() {

	}

	protected virtual void OnBombReachedTarget(Bomb.Target target) {
		
	}

	protected virtual void OnBombBoom(Bomb.Target target) {

	}
}