using UnityEngine;

public static class DebugSettings {
	public static bool IsDebugEnabled {
		get {
			return Debug.unityLogger.logEnabled;
		}
		set {
			Debug.unityLogger.logEnabled = value;
		}
	}
}