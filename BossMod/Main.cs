using CheetoClient;
using MelonLoader;

[assembly: MelonInfo(typeof(BossMod.Main), "BossMod", "1.0.0", "WTFBlaze")]
[assembly: MelonGame("Playside Studios", "WorldBoss")]

namespace BossMod
{
    public class Main : MelonMod
    {
		public override void OnPreSupportModule()
        {
			Log.Level = Log.LogLevel.INFO;
			Performance.ApplyTweaks();
            ConsoleInitializer.Initialize();
			ConsoleUtils.AppendTitle($" - BoddMod {BuildInfo.Version}");
			Log.Write("Testing Console Writing", Color.Crayola.Present.PigPink);
		}

		public override void OnInitializeMelon()
        {
            
        }
    }
}