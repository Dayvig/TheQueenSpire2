using MegaCrit.Sts2.Core.Localization;
using SmartFormat.Core.Parsing;

namespace QueenMod2.QueenMod2Code.Formatters;


using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;
using System;

public class StrategizeFormatter : IFormatter
{
    public string Name
    {
        get => "typeDisplay";
        set => throw new NotImplementedException();
    }

    public bool CanAutoDetect { get; set; }
    
    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (!(formattingInfo.CurrentValue is StrategizeVar currentValue))
            return false;
        MainFile.Logger.Info("Writing To Description");
        formattingInfo.Write(parseStrategize(currentValue, currentValue.place, currentValue.plural));
        return true;
    }
    
    private string parseStrategize(StrategizeVar var, int place, bool plural)
    {
        string returnString = " \n ";
        switch (var.TypeList[place])
        {
            case MainFile.StrategizeType.ATTACK:
                returnString += "[red]Attack";
                returnString += plural ? "s[/red]" : "[/red]";
                break;
            case MainFile.StrategizeType.BLOCKSKILL:
                returnString += "[blue]Block Skill";
                returnString += plural ? "s[/blue]" : "[/blue]";
                break;
            case MainFile.StrategizeType.UTILITYSKILL:
                returnString += "[blue]Utility Skill";
                returnString += plural ? "s[/blue]" : "[/blue]";
                break;
            case MainFile.StrategizeType.POWER:
                returnString += "[cyan]Power";
                returnString += plural ? "s[/cyan]" : "[/cyan]";
                break;
        }
        return returnString;
    }
}