using UnityEngine;
using System.Collections.Generic;

public class PlatformDependedAllow : MonoBehaviour {
	[SerializeField]
	private List<RuntimePlatform> allowOnPlatforms = null;

	private void Awake() {
		if (!allowOnPlatforms.Contains(Application.platform)) {
			Destroy(gameObject);
		}
	}
}
