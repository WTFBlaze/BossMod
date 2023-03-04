namespace BossMod;

using System;
using MelonLoader;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;
using UnityEngine;

public class ESP : MonoBehaviour {
	static ESP() => ClassInjector.RegisterTypeInIl2Cpp<ESP>();
	public ESP(IntPtr ptr) : base(ptr) { }

	private void OnGUI() {
		if (!Main.IsInGame)
			return;

		if (Utils.GetAllPlayers() is null) {
			MelonLogger.Msg("1");
			return;
		}

		if (Utils.GetAllPlayers().Count == 0)
			return;

		foreach (var player in Utils.GetAllPlayers()) {
			if (player is null) {
				MelonLogger.Msg("2");
				continue;
			}

			if (player.IsControllingUser)
				continue;

			var pivotPos = player.transform.position;
			var footPos = new Vector3(pivotPos.x, pivotPos.y - 2f, pivotPos.z);
			var headPos = new Vector3(pivotPos.x, pivotPos.y + 2f, pivotPos.z);

			var w2s_footpos = Camera.main.WorldToScreenPoint(footPos);
			var w2s_headpos = Camera.main.WorldToScreenPoint(headPos);

			if (w2s_footpos.z > 0f) {
				DrawBox(w2s_footpos, w2s_headpos, Color.red);
			}
		}
	}

	[HideFromIl2Cpp]
	public void DrawBox(Vector3 footpos, Vector3 headpos, Color color) {
		var height = headpos.y - footpos.y;
		var widthOffset = 2f;
		var width = height / widthOffset;
		RenderManager.DrawBox(footpos.x - (width / 2), Screen.height - footpos.y - height, width, height, color, 2f);
	}
}