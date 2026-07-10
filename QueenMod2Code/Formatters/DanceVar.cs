using Godot;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace QueenMod2.QueenMod2Code.Formatters;

public class DanceVar : DynamicVar
{
    public const string defaultName = "Dance";
    public List<MainFile.DanceStep> danceSteps = new List<MainFile.DanceStep>();
    public int place = 0;
    
    public DanceVar(List<MainFile.DanceStep> steps, int place)
        : base("Dance", (Decimal) (int) 0M)
    {
        danceSteps = steps;
        this.place = place;
        MainFile.Logger.Info("Setting Dance default Constructor");
    }
    
    public List<Color> DanceColors = new List<Color>()
    {
        new Color(1f, 0.0f, 0f, 0.98f),
        new Color(0f, 0.85f, 0f, 0.98f),
        new Color(0.125f, 0.9f, 1f, 0.98f)
    };

    public int stepToColor(MainFile.DanceStep step)
    {
        switch (step)
        {
            case MainFile.DanceStep.ATTACK:
                return 0;
            case MainFile.DanceStep.SKILL:
                return 1;
            case MainFile.DanceStep.POWER:
                return 2;
        }
        return 0;
    }
    
    public DanceVar(string name, Decimal amount)
        : base(name, amount)
    {
    }
}