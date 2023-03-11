namespace BossMod;

using System;
using UnityEngine;

internal static class Extensions {
	public static Transform FirstChildOrDefault(this Transform parent, Func<Transform, bool> query) {
		if (parent.childCount == 0) {
			return null;
		}

		Transform result = null;
		for (int i = 0; i < parent.childCount; i++) {
			var child = parent.GetChild(i);
			if (query(child)) {
				return child;
			}
			result = FirstChildOrDefault(child, query);
		}

		return result;
	}
}
