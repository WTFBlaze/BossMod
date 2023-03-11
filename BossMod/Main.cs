using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using CheetoClient;
using MelonLoader;
using Photon.Bolt;
using UnityEngine;
using VRC.UI;
using Color = CheetoClient.Color;

[assembly: MelonInfo(typeof(BossMod.Main), "BossMod", "1.0.0", "WTFBlaze, Cheetos, Mora")]
[assembly: MelonGame("Playside Studios", "WorldBoss")]

namespace BossMod;

public class Main : MelonMod {
	public static bool IsInGame;
	public static GameObject Container;
	public static Gamelogic GameLogic;

	public override void OnPreSupportModule() {
		Log.Level = Log.LogLevel.INFO;
		Performance.ApplyTweaks();
		ConsoleInitializer.Initialize();
		ConsoleUtils.AppendTitle($" - BossMod {BuildInfo.Version}");
		Log.Write("Testing Console Writing", Color.Crayola.Present.PigPink);
	}

	public override void OnInitializeMelon() {
		HookManager.Initialize();
		//AssemblyDump();
	}

	public override void OnSceneWasInitialized(int buildIndex, string sceneName) {
		Log.Write($"Scene Initialized: [{buildIndex}] '{sceneName}'");
		switch (buildIndex) {
			case 3:
				MelonCoroutines.Start(OnGameInit());
				break;
		}
	}

	private IEnumerator OnGameInit() {
		while (GameUtils.Instance == null) yield return new WaitForSeconds(1f);
		while (PlayerUtils.Self == null) yield return new WaitForSeconds(1f);
		GameInitialized();
	}

	private void GameInitialized() {
		Log.Write("Game Initialized");
		MelonCoroutines.Start(WeaponModLoop());
		MelonCoroutines.Start(GodModeLoop());
		MelonCoroutines.Start(TestLoop());
	}

	private IEnumerator WeaponModLoop() {
		Log.Write("Weapon Modifier Loop Started");
		for (; ; )
		{
			if (PlayerUtils.Self == null) {
				Log.Write($"Weapon Modifier Loop Stopped");
				yield break;
			}

			foreach (var weapon in PlayerUtils.Self.SimulatedWeapons) {
				if (weapon == null) continue;
				var stats = weapon.WeaponStatsScriptable;
				if (stats == null) continue;
				stats.spreadSettings.useSpread = false;
				stats.recoilSettings.recoilApplyHoldTicks = 0;
				stats.recoilSettings.verticalRecoil = new(0, 0);
				stats.recoilSettings.recoilAddTime = 0;
				stats.recoilSettings.recoilRecoverTime = 0;

				// Not sure if this even works, but why not?
				stats.damageSettings.dropOff_1 = new Vector2Int(0, 0);
				stats.damageSettings.dropOff_2 = new Vector2Int(0, 0);
				stats.damageSettings.dropOff_3 = new Vector2Int(0, 0);
			}

			yield return new WaitForSeconds(1f); // I was lazy :)
		}
	}

	private int _maxHealth = 1000;
	private IEnumerator GodModeLoop()
	{
		Log.Write("GodMode Loop Started");

		for (; ; )
		{
			if (PlayerUtils.Self == null) {
				Log.Write($"GodMode Loop Stopped");
				yield break;
			}

			if (PlayerUtils.Self.prevHealth > _maxHealth)
			{
				_maxHealth = PlayerUtils.Self.prevHealth;
			}

			if (PlayerUtils.Self.prevHealth < _maxHealth)
			{
				try {
					var g = Resources.FindObjectsOfTypeAll<ItemPickup>().Where(e => e.cachedNameString.Contains("_HEALTH")).First();
					if (g != null) {
						g.OnPickedUp(PlayerUtils.Self);
						g.OnInteract(PlayerUtils.Self.gameObject.GetComponent<BoltEntity>(), false);
					}
				}
				catch { }
			}

			yield return new WaitForSeconds(0.01f);
		}
	}

	private Camera _camera;
	private IEnumerator TestLoop() {
		Log.Write("Test Loop Started");
		_camera = PlayerUtils.Self.weaponCamera;
		var self = PlayerUtils.Self;

		if (_camera == null)
		{
			Log.Warn($"Camera Not Found");
		}

		for (; ; )
		{
			if (PlayerUtils.Self == null) {
				Log.Write($"Test Loop Stopped");
				yield break;
			}

			try {
				int layerMask = LayerMask.NameToLayer("Character");
				RaycastHit[] HitObjects = Physics.RaycastAll(_camera.transform.position, _camera.transform.forward);
				foreach (RaycastHit hitObj in HitObjects)
				{
					if (hitObj.transform.name.Contains("Character") && hitObj.transform.name != self.name)
					{
						self.inputPoll.fire = true;
					}
				}
			}
			catch { }

			yield return new WaitForFixedUpdate();
		}
	}

	public void AssemblyDump() {
		if (!Directory.Exists("BossMod")) {
			Directory.CreateDirectory("BossMod");
		}
		var buffer = new StringBuilder();

		var assembly = System.Reflection.Assembly.GetAssembly(typeof(Gamelogic));
		buffer.AppendLine($"Calling Assembly: {assembly.FullName}");
		buffer.AppendLine($"{assembly.HostContext}");
		foreach (var module in assembly.GetModules()) {
			try {
				buffer.AppendLine($"Module: {module.FullyQualifiedName}");

				foreach (var type in module.GetTypes()) {
					buffer.AppendLine($"Type: {type.FullName}");

					foreach (var method in type.GetMethods()) {
						buffer.AppendLine($"\tMethod: {method.ReturnType} {method.Name}");
						foreach (var parameter in method.GetParameters()) {
							buffer.AppendLine($"\t\tParameter: {parameter.Name} {parameter.ParameterType}");
						}
					}
					foreach (var field in type.GetFields()) {
						buffer.AppendLine($"\tField: {field.Name}");
					}
					foreach (var prop in type.GetProperties()) {
						buffer.AppendLine($"\tProperty: {prop.Name}");
					}
				}
			}
			catch {
			}
		}

		File.WriteAllText(@"BossMod\assemblies.txt", buffer.ToString());
		//Environment.Exit(0);
	}
}