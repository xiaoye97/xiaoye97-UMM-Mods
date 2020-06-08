//#define Chinese
using Harmony12;
using UnityEngine;
using Coatsink.Common;
using System.Reflection;
using UnityModManagerNet;
using System.Collections.Generic;

namespace me.xiaoye97.UMMMod.KingdomTwoCrowns.CanDestroyWall
{
    public static class Main
    {
        public static UnityModManager.ModEntry mod;
        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;
            mod.OnGUI = OnGUI;
            HarmonyInstance.Create(mod.Info.Id).PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        public static void OnGUI(UnityModManager.ModEntry modEntry)
        {
#if Chinese
            GUILayout.BeginVertical("可以拆除墙", GUI.skin.window, new GUILayoutOption[0]);
            GUILayout.Label("站在墙的中间，按C键，可以消耗1金币拆除当前墙壁。", new GUILayoutOption[0]);
            GUILayout.Label("作者:xiaoye97 QQ:1066666683", new GUILayoutOption[0]);
#else
            GUILayout.BeginVertical("CanDestroyWall", GUI.skin.window, new GUILayoutOption[0]);
            GUILayout.Label("Standing in the middle of the wall, press the C key, you can use 1 coin to destroy the current wall.", new GUILayoutOption[0]);
            GUILayout.Label("Author:xiaoye97 QQ:1066666683 Discord:xiaoye#3171", new GUILayoutOption[0]);
#endif
            GUILayout.EndVertical();
        }

        public static void RemoveWall()
        {
            Kingdom kingdom = SingletonMonoBehaviour<Managers>.Inst.kingdom;
            if (kingdom.playerOne != null)
            {
                if (kingdom.playerOne.coins > 0)
                {
                    var walls = Traverse.Create(kingdom).Field("_walls").GetValue<HashSet<Wall>>();
                    Wall wall = null;
                    float min = float.MaxValue;
                    walls.Do((w) =>
                    {
                        float dis = Vector3.Distance(w.transform.position, kingdom.playerOne.transform.position);
                        if (dis < 1.4f && dis < min)
                        {
                            wall = w;
                            min = dis;
                        }
                    });
                    if (wall != null)
                    {
                        Traverse.Create(wall).Field("_damageable").GetValue<Damageable>().ReceiveDamage(999, kingdom.playerOne.gameObject, DamageSource.Knight);
                        kingdom.playerOne._wallet.coins--;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Kingdom), "Update")]
        class InputPatch
        {
            public static void Postfix()
            {
                if (mod.Enabled)
                {
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        RemoveWall();
                    }
                }
            }
        }
    }
}
