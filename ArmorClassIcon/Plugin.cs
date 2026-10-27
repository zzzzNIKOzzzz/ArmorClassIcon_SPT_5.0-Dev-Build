using ArmorClassIcon.Patches;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace ArmorClassIcon
{
    [BepInPlugin("zzzNIKOzzz.armorclassicon", "ArmorClassIcon", PluginVersion)]
    public class Plugin : BasePlugin
    {
        public const string PluginVersion = "1.0.0";
        internal static ManualLogSource Logger;

        public override void Load()
        {
            Logger = Log;

            Logger.LogInfo("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
            Logger.LogInfo("___ArmorClassIcon от zzzNIKOzzz загружается___");
            Settings.Init(Config);

            new ItemViewStatsPatch().Enable();
            new GridItemViewPatch().Enable();

            ClassInjector.RegisterTypeInIl2Cpp<IconUpdaterBehaviour>();
            var go = new GameObject("ArmorClassIconBehaviour");
            GameObject.DontDestroyOnLoad(go);
            go.AddComponent<IconUpdaterBehaviour>();
            Logger.LogInfo("zzzNIKOzzz: Теперь всё понятно, блет! Спасибо!");
            Logger.LogInfo("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
        }
    }

    public class IconUpdaterBehaviour : MonoBehaviour
    {
        private int _frameCounter;
        private const int CacheCleanupInterval = 3600;

        private void LateUpdate()
        {
            ItemViewStatsPatch.RefreshIfRequested();

            if (++_frameCounter >= CacheCleanupInterval)
            {
                _frameCounter = 0;
                IconUpdater.ClearCache();
            }
        }
    }
}