using System.Collections;
using System.Linq;
using CheetoClient;
using MelonLoader;
using UnityEngine;
using Color = CheetoClient.Color;

[assembly: MelonInfo(typeof(BossMod.Main), "BossMod", "1.0.0", "WTFBlaze, Cheetos, Mora")]
[assembly: MelonGame("Playside Studios", "WorldBoss")]

namespace BossMod;

public class Main : MelonMod
{
	public static bool IsInGame;
	public static GameObject Container;
	public static Gamelogic GameLogic;

	public override void OnPreSupportModule()
	{
		Log.Level = Log.LogLevel.INFO;
		Performance.ApplyTweaks();
		ConsoleInitializer.Initialize();
		ConsoleUtils.AppendTitle($" - BossMod {BuildInfo.Version}");
		Log.Write("Testing Console Writing", Color.Crayola.Present.PigPink);
	}

	public override void OnInitializeMelon()
	{
		HookManager.Initialize();
	}

	public override void OnSceneWasInitialized(int buildIndex, string sceneName)
	{
		Log.Write($"Scene Initialized: [{buildIndex}] '{sceneName}'");
		switch (buildIndex) {
			case 3:
				MelonCoroutines.Start(OnGameInit());
				break;
		}
	}

	private IEnumerator OnGameInit()
	{
		while (GameUtils.Instance == null) yield return new WaitForSeconds(1f);
		while (PlayerUtils.Self == null) yield return new WaitForSeconds(1f);
		GameInitialized();
	}

	private void GameInitialized()
	{
		Log.Write("Game Initialized");
		MelonCoroutines.Start(WeaponModLoop());
	}

	private IEnumerator WeaponModLoop()
	{
		Log.Write("Weapon Modifier Loop Started");
		for (; ; )
		{
			if (PlayerUtils.Self == null)
			{
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
				stats.handlingSettings.reloadFrames = 0;
				stats.handlingSettings.autoReloadDelayTime = 0;

				// Not sure if this even works, but why not?
				stats.damageSettings.dropOff_1 = new Vector2Int(0, 0);
				stats.damageSettings.dropOff_2 = new Vector2Int(0, 0);
				stats.damageSettings.dropOff_3 = new Vector2Int(0, 0);
			}

			yield return new WaitForSeconds(1f); // I was lazy :)
		}
	}
}