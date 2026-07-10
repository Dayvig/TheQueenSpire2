using Godot;
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
        MainFile.StrategizeType.POWER
    };

    public List<Color> StrategizeColors = new List<Color>()
    {
        new Color(1f, 0.0f, 0f, 0.98f),
        new Color(0f, 0.85f, 0f, 0.98f),
        new Color(0.125f, 0.9f, 1f, 0.98f)
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