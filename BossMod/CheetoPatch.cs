namespace CheetoClient;

#region Usings
using System;
using System.Reflection;
using System.Text;
using HarmonyLib;
#endregion

internal class CheetoPatch
{
	private string _patchIdentifier { get; } = "CheetoPatch";
	private MethodInfo _targetMethodMethodInfo { get; set; }
	private MethodBase _targetMethodMethodBase { get; set; }
	private string _harmonyInstanceId { get; set; }
	private HarmonyMethod _prefix { get; set; }
	private HarmonyMethod _postFix { get; set; }
	private HarmonyMethod _transpiler { get; set; }
	private HarmonyMethod _finalizer { get; set; }

	private HarmonyMethod _ilManipulator { get; set; }
	private Harmony _instance { get; set; }
	private bool _hasThrownException { get; set; }
	private bool _showErrorOnConsole { get; set; } = true;
	private bool _isActivePatch { get; set; }
	private bool _isMethodInfoPatch { get; set; }

	private string _targetPathMethodInfo => $"{_targetMethodMethodInfo?.DeclaringType?.FullName}.{_targetMethodMethodInfo?.Name}";
	private string _targetPathBase => $"{_targetMethodMethodInfo?.DeclaringType?.FullName}.{_targetMethodMethodBase.Name}";

	internal string PatchType
	{
		get
		{
			StringBuilder patchType = new();
			if (_postFix != null)
			{
				string patch = $"PostFix Patch : {_postFix.method?.DeclaringType?.FullName}.{_postFix.method?.Name} ";
				if (patchType.Length != 0)
				{
					patchType.AppendLine(patch);
				}
				else
				{
					patchType.Append(patch);
				}
			}

			if (_prefix != null)
			{
				string patch = $"Prefix Patch : {_prefix.method?.DeclaringType?.FullName}.{_prefix.method?.Name} ";
				if (patchType.Length != 0)
				{
					patchType.AppendLine(patch);
				}
				else
				{
					patchType.Append(patch);
				}
			}

			if (_transpiler != null)
			{
				string patch = $"Transpiler Patch : {_transpiler.method?.DeclaringType?.FullName}.{_transpiler.method?.Name} ";
				if (patchType.Length != 0)
				{
					patchType.AppendLine(patch);
				}
				else
				{
					patchType.Append(patch);
				}
			}

			if (_finalizer != null)
			{
				string patch = $"Finalizer Patch : {_finalizer.method?.DeclaringType?.FullName}.{_finalizer.method?.Name} ";
				if (patchType.Length != 0)
				{
					patchType.AppendLine(patch);
				}
				else
				{
					patchType.Append(patch);
				}
			}

			if (_ilManipulator != null)
			{
				string patch = $"IlManipulator Patch : {_ilManipulator.method?.DeclaringType?.FullName}.{_ilManipulator.method?.Name} ";
				if (patchType.Length != 0)
				{
					patchType.AppendLine(patch);
				}
				else
				{
					patchType.Append(patch);
				}
			}

			return patchType.Length == 0 ? "Failed to Read Patch." : patchType.ToString();
		}
	}

	internal CheetoPatch(MethodInfo targetMethod, HarmonyMethod prefix = null, HarmonyMethod postFix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null, HarmonyMethod lmanipulator = null, bool showErrorOnConsole = true)
	{
		if (targetMethod == null || (prefix == null && postFix == null && transpiler == null && finalizer == null && lmanipulator == null))
		{
			StringBuilder failureReason = new();
			if (prefix == null)
			{
				const string reason = "Prefix Method is null";
				if (failureReason.Length != 0)
				{
					failureReason.AppendLine(reason);
				}
				else
				{
					failureReason.Append(reason);
				}
			}

			if (postFix == null)
			{
				const string reason = "PostFix Method is null";
				if (failureReason.Length != 0)
				{
					failureReason.AppendLine(reason);
				}
				else
				{
					failureReason.Append(reason);
				}
			}

			if (transpiler == null)
			{
				const string reason = "Transpiler Method is null";
				if (failureReason.Length != 0)
				{
					failureReason.AppendLine(reason);
				}
				else
				{
					failureReason.Append(reason);
				}
			}

			if (finalizer == null)
			{
				const string reason = "Finalizer Method is null";
				if (failureReason.Length != 0)
				{
					failureReason.AppendLine(reason);
				}
				else
				{
					failureReason.Append(reason);
				}
			}

			if (lmanipulator == null)
			{
				const string reason = "ILmanipulator Method is null";
				if (failureReason.Length != 0)
				{
					failureReason.AppendLine(reason);
				}
				else
				{
					failureReason.Append(reason);
				}
			}

			if (targetMethod != null)
			{
				Log.ErrorNoCall($"[{_patchIdentifier}] TargetMethod is NULL");
			}
			else
			{
				Log.ErrorNoCall($"[{_patchIdentifier}] Failed to Patch {targetMethod.DeclaringType?.FullName}.{targetMethod?.Name} because {failureReason}.");
			}
			return;
		}

		_targetMethodMethodInfo = targetMethod;
		_prefix = prefix;
		_postFix = postFix;
		_transpiler = transpiler;
		_finalizer = finalizer;
		_ilManipulator = lmanipulator;
		_showErrorOnConsole = showErrorOnConsole;
		_harmonyInstanceId = $"{_patchIdentifier}: {_targetPathMethodInfo}, {PatchType}";
		_isMethodInfoPatch = true;
		_instance = new Harmony(_harmonyInstanceId);
		DoPatch_info(this);
	}

	internal CheetoPatch(MethodBase targetMethod, HarmonyMethod prefix = null, HarmonyMethod postFix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null, HarmonyMethod lmanipulator = null, bool showErrorOnConsole = true)
	{
		if (targetMethod == null || (prefix == null && postFix == null && transpiler == null && finalizer == null && lmanipulator == null))
		{
			StringBuilder failureReason = new();
			if (prefix == null)
			{
				string reason = "Prefix Method is null";
				if (failureReason.Length != 0)
				{
					failureReason.AppendLine(reason);
				}
				else
				{
					failureReason.Append(reason);
				}
			}

			if (postFix == null)
			{
				string reason = "PostFix Method is null";
				if (failureReason.Length != 0)
				{
					failureReason.AppendLine(reason);
				}
				else
				{
					failureReason.Append(reason);
				}
			}

			if (transpiler == null)
			{
				string reason = "Transpiler Method is null";
				if (failureReason.Length != 0)
				{
					failureReason.AppendLine(reason);
				}
				else
				{
					failureReason.Append(reason);
				}
			}

			if (finalizer == null)
			{
				string reason = "Finalizer Method is null";
				if (failureReason.Length != 0)
				{
					failureReason.AppendLine(reason);
				}
				else
				{
					failureReason.Append(reason);
				}
			}

			if (lmanipulator == null)
			{
				string reason = "ILmanipulator Method is null";
				if (failureReason.Length != 0)
				{
					failureReason.AppendLine(reason);
				}
				else
				{
					failureReason.Append(reason);
				}
			}

			if (targetMethod != null)
			{
				Log.Error($"[{_patchIdentifier}] TargetMethod is NULL");
			}
			else
			{
				Log.Error($"[{_patchIdentifier}] Failed to Patch {targetMethod.DeclaringType?.FullName}.{targetMethod?.Name} because {failureReason}.");
			}
			return;
		}

		_targetMethodMethodBase = targetMethod;
		_prefix = prefix;
		_postFix = postFix;
		_transpiler = transpiler;
		_finalizer = finalizer;
		_ilManipulator = lmanipulator;
		_showErrorOnConsole = showErrorOnConsole;
		_harmonyInstanceId = $"{_patchIdentifier}: {_targetPathMethodInfo}, {PatchType}";
		_isMethodInfoPatch = false;
		_instance = new Harmony(_harmonyInstanceId);
		DoPatch_base(this);
	}

	private void DoPatch_info(CheetoPatch patch)
	{
		try
		{
			patch._instance.Patch(patch._targetMethodMethodInfo, patch._prefix, patch._postFix, patch._transpiler, patch._finalizer, patch._ilManipulator);
		}
		catch (Exception ex)
		{
			_hasThrownException = true;
			if (_showErrorOnConsole)
			{
				Log.Exception(ex, "0xAP01");
			}
		}
		finally
		{
			if (!_hasThrownException)
			{
				_isActivePatch = true;
				Log.Fine($"[{patch._patchIdentifier}] Patched {patch._targetPathMethodInfo} | with {patch.PatchType}");
			}
			else
			{
				_isActivePatch = false;
				Log.ErrorNoCall($"[{patch._patchIdentifier}] Failed At {patch._targetPathMethodInfo} | with {patch.PatchType}");
			}
		}
	}
	private void DoPatch_base(CheetoPatch patch)
	{
		try
		{
			patch._instance.Patch(patch._targetMethodMethodBase, patch._prefix, patch._postFix, patch._transpiler, patch._finalizer, patch._ilManipulator);
		}
		catch (Exception ex)
		{
			_hasThrownException = true;
			if (_showErrorOnConsole)
			{
				Log.Exception(ex, "0xAP02");
			}
		}
		finally
		{
			if (!_hasThrownException)
			{
				_isActivePatch = true;
				Log.Fine($"[{patch._patchIdentifier}] Patched {patch._targetPathBase} | with {patch.PatchType}");
			}
			else
			{
				_isActivePatch = false;
				Log.ErrorNoCall($"[{patch._patchIdentifier}] Failed At {patch._targetPathBase} | with {patch.PatchType}");
			}
		}
	}

	internal void Unpatch()
	{
		if (_isActivePatch)
		{
			_instance.UnpatchSelf();
			if (!_isMethodInfoPatch)
			{
				Log.Fine($"[{_patchIdentifier}] Removed Patch from {_targetPathBase} , Unlinked Method : {PatchType}");
			}
			else
			{
				Log.Fine($"[{_patchIdentifier}] Removed Patch from {_targetPathMethodInfo} , Unlinked Method : {PatchType}");
			}
			_isActivePatch = false;
		}
	}

	internal void Patch()
	{
		if (!_isActivePatch)
		{
			if (_isMethodInfoPatch)
			{
				DoPatch_info(this);
			}
			else
			{
				DoPatch_base(this);
			}
		}
	}
}