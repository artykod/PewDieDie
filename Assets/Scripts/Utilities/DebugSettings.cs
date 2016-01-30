using UnityEngine;

public static class DebugSettings {
	public static bool IsDebugEnabled {
		get {
			return Debug.logger.logEnabled;
		}
		set {
			Debug.logger.logEnabled = value;
		}
	}
}