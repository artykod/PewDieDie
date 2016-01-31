using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {
	public class Target {
		public Transform targetObject = null;
		public Vector3 targetOffest = Vector3.zero;
	}

	[SerializeField]
	private ParticleSystem boomParticles = null;

	private Target currentTarget = null;
	private IEnumerator currentFlyRoutine = null;
	private float lifeTime = 0f;

	public event System.Action<Target> OnBoom = delegate { };
	public event System.Action<Target> OnTargetReached = delegate { };

	public Target CurrentTarget {
		get {
			return currentTarget;
		}
	}

	public bool SetNewTarget(Target target) {
		if (currentFlyRoutine != null) {
			return false;
		}

		OnTargetReached(null);
		StartCoroutine(currentFlyRoutine = FlyToTarget(target));

		return true;
	}

	private IEnumerator FlyToTarget(Target target) {
		currentTarget = target;

		Vector3 startPosition = transform.position;
		float flyTime = 0.5f;
		float time = 0f;
		while (time < flyTime) {
			time += Time.deltaTime;
			float t = time / flyTime;
			if (t > 1f) {
				t = 1f;
			}

			Vector3 targetPosition = currentTarget.targetObject.position + currentTarget.targetOffest;
			transform.position = BezierTool.CalculateBezier(startPosition, startPosition + new Vector3(0f, 5f, 0f), targetPosition + new Vector3(0f, 5f, 0f), targetPosition, t);

			yield return null;
		}

		currentFlyRoutine = null;

		OnTargetReached(currentTarget);
	}

	private void Awake() {
		transform.position = Camera.main.transform.position;
		lifeTime = Random.Range(10f, 20f);

		//lifeTime = float.MaxValue;
	}

	private void Update() {
		if (currentFlyRoutine == null && currentTarget != null && currentTarget.targetObject != null) {
			transform.position = currentTarget.targetObject.position + currentTarget.targetOffest;

			lifeTime -= Time.deltaTime;
			if (lifeTime <= 0f) {
				lifeTime = float.MaxValue;
				Boom();
			}
		}
	}

	private void Boom() {
		boomParticles.Play();
		OnBoom(currentTarget);
	}
}
