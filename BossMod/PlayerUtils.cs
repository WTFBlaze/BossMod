using System.Linq;
using CheetoClient;
using UnityEngine;

namespace BossMod;

public static class PlayerUtils
{
	private static PlayerController _self;
	public static PlayerController Self {
		get {
			if (_self == null) {
				var found = GameUtils.Instance.ObservedController;
				if (found != null) _self = found;
				if (_self != null) {
					Log.Write($"Found: PlayerController -> {_self.name}");
				}
			}
			return _self;
		}
	}
}