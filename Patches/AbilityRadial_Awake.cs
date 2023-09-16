using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Abyss.Api;
using Abyss.Api.Abilities;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Abyss.Patches;

[HarmonyPatch(typeof(AbilityRadial), nameof(AbilityRadial.Awake))]
internal static class AbilityRadial_Awake
{
    [HarmonyPrefix]
    private static void Prefix(AbilityRadial __instance)
    {
        foreach (var modAbilityData in ModContent.GetContent<ScriptableObjectModContent<AbilityData>>())
        {
            var abilityRadialWedge = new GameObject(modAbilityData.Id, typeof(RectTransform), typeof(Image))
            {
                transform =
                {
                    parent = __instance.transform.GetChild(0).Find("Abilities"),
                    localScale = Vector3.one,
                },
            }.AddComponent<AbilityRadialWedge>();
            abilityRadialWedge.abilityData = modAbilityData.Item;
            var image = abilityRadialWedge.gameObject.GetComponent<Image>();
            image.sprite = modAbilityData.Item.icon;
            abilityRadialWedge.image = image;
            abilityRadialWedge.index = __instance.abilityWedges.Count;
            __instance.abilityWedges.Add(abilityRadialWedge);
            abilityRadialWedge.radius = 200;
            abilityRadialWedge.lockedSprite = __instance.abilityWedges.FirstOrDefault()?.lockedSprite;
            abilityRadialWedge.buttonCenter = __instance.transform.GetChild(0).gameObject;
        }
    }

    public static int NumExtraAbilities(int existing)
    {
        return existing + ModContent.GetContent<ScriptableObjectModContent<AbilityData>>().Length;
    }

    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var code in instructions)
        {
            if (code.StoresField(AccessTools.Field(typeof(AbilityRadial), "numAbilitiesEnabled")))
            {
                yield return new CodeInstruction(OpCodes.Call,
                    typeof(AbilityRadial_Awake).GetMethod(nameof(NumExtraAbilities)));
                yield return code;
                //before any time its stored, add the number of custom abilities
            }
            else
            {
                yield return code;
            }
        }
    }
}