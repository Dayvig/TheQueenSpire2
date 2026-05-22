using MegaCrit.Sts2.Core.Localization;
using SmartFormat.Core.Parsing;

namespace QueenMod2.QueenMod2Code.Formatters;


using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;
using System;

public class DanceFormatter : IFormatter
{
    public string Name
    {
        get => "steps";
        set => throw new NotImplementedException();
    }

    public bool CanAutoDetect { get; set; }
    
    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (!(formattingInfo.CurrentValue is DanceVar currentValue))
            return false;
        MainFile.Logger.Info("Writing To Description");
        formattingInfo.Write(parseDanceSteps(currentValue.danceSteps, currentValue.place));
        return true;
    }
    
    private string parseDanceSteps(List<MainFile.DanceStep> steps, int place)
    {
        string returnString = " \n ";
        for (int i = 0; i < steps.Count; i++)
        {
            switch (steps[i])
            {
                case MainFile.DanceStep.ATTACK:
                    if (place == i) { returnString += "[red]Attack[/red]"; }
                    else { returnString += "Attack"; }
                    break;
                case MainFile.DanceStep.SKILL:
                    if (place == i) { returnString += "[green]Skill[/green]"; }
                    else { returnString += "Skill"; }
                    break;                
                case MainFile.DanceStep.POWER:
                    if (place == i) { returnString += "[blue]Power[/blue]"; }
                    else { returnString += "Power"; }
                    break;
            }

            returnString += i < steps.Count-1 ? " - " : "";
        }
        return returnString;
    }

    
    
    /*public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (!(formattingInfo.CurrentValue is IfUpgradedVar currentValue))
            return false;
        IList<Format> formatList = formattingInfo.Format?.Split('|');
        if (formatList == null)
            throw new LocException($"Format expression must contain at least 1 option. format={formattingInfo.Format}.");
        Format format1 = formatList.Count <= 2 ? formatList[0] : throw new LocException($"Format expression cannot contain more than 2 options. num_of_options={formatList.Count} format={formattingInfo.Format}.");
        Format format2 = formatList.Count > 1 ? formatList[1] : (Format) null;
        switch (currentValue.upgradeDisplay)
        {
            case UpgradeDisplay.Normal:
                formattingInfo.FormatAsChild(format2, formattingInfo.CurrentValue);
                break;
            case UpgradeDisplay.Upgraded:
                formattingInfo.FormatAsChild(format1, formattingInfo.CurrentValue);
                break;
            case UpgradeDisplay.UpgradePreview:
                formattingInfo.Write("[green]");
                formattingInfo.FormatAsChild(format1, formattingInfo.CurrentValue);
                formattingInfo.Write("[/green]");
                break;
            default:
                throw new ArgumentOutOfRangeException("upgradeDisplay", $"Unexpected value: {currentValue.upgradeDisplay}");
        }
        return true;
    }*/
}