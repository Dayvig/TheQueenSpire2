using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace QueenMod2.QueenMod2Code;

public static class QueenMod2Keywords
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Swarm;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Hive;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Honeycomb;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Multiply;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword HiveTip;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Hivebound;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Dance;
    [CustomEnum, KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Strategize;

}
