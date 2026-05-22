using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace QueenMod2.QueenMod2Code.Formatters;

public class StrategizeVar : DynamicVar
{
    public const string defaultName = "Strategize";
    public bool plural = false;

    public List<MainFile.StrategizeType> TypeList = new List<MainFile.StrategizeType>()
    {
        MainFile.StrategizeType.ATTACK,
        MainFile.StrategizeType.BLOCKSKILL,
        MainFile.StrategizeType.UTILITYSKILL,
        MainFile.StrategizeType.POWER
    };
    public int place = 0;
    
    public StrategizeVar(int place, bool plural)
        : base("Strategize", (Decimal) (int) 0M)
    {
        this.place = place;
        this.plural = plural;
    }
    
    public StrategizeVar(string name, Decimal amount)
        : base(name, amount)
    {
    }
}