using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace NoMoreTimer
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public void Start()
        {
            var instance3 = new Harmony("JobMissionTimerPatch");
            instance3.PatchAll(typeof(PatchJobMissionTimer));

            Logger.LogInfo("Patched JobMissionTimer");
        }
    }

    class PatchJobMissionTimer
    {
        [HarmonyPatch(typeof(JobMissionTimer), "OnServerUpdate")]
        [HarmonyPrefix]
        static bool OnServerUpdatePrefix(JobMissionTimer __instance)
        {
            JobMission jobMission = (JobMission)AccessTools.Field(typeof(JobMissionTimer), "jobMission").GetValue(__instance);
            PlayerController[] players = GameObject.FindObjectsOfType<PlayerController>();

            for (int i = 0; i < players.Length; i++)
            {
                if (jobMission && __instance.onTimerFinishedShouldJobSuccess != null)
                {
                    jobMission.ServerJobCompleted(players[i]);
                }
            }

            return false;
        }
    }
}
