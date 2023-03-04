using System.Linq;
using CheetoClient;
using UnityEngine;

namespace BossMod;

public static class GameUtils
{
	private static Gamelogic _instance;
	public static Gamelogic Instance {
		get {
			if (_instance == null) {
				var found = Resources.FindObjectsOfTypeAll<Gamelogic>();
				if (found != null && found.Any()) _instance = found.First();
				if (_instance != null) {
					Log.Write($"Found: Gamelogic -> {_instance.name}");
				}
			}
			return _instance;
		}
	}
}