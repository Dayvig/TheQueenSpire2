using BaseLib.Abstracts;
using QueenMod2.QueenMod2Code.Extensions;
using Godot;

namespace QueenMod2.QueenMod2Code.Character;

public class QueenMod2RelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => QueenMod2.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}