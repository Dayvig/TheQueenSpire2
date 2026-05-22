using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using QueenMod2.QueenMod2Code.Formatters;
using SmartFormat;

[HarmonyPatch(typeof(LocManager), ("LoadLocFormatters"))]
public static class LocManagerPatch
{
    [HarmonyPostfix]
    private static void AddCustomFormatters()
    {
        Smart.Default.AddExtensions(
            new DanceFormatter(),
            new StrategizeFormatter()
        );
    }
}