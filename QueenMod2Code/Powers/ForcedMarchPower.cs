using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace QueenMod2.QueenMod2Code.Powers;

public class ForcedMarchPower : QueenMod2Power
{
    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            March(choiceContext);
        }
        return Task.CompletedTask;
    }

    public async Task March(PlayerChoiceContext choiceContext)
    {
        await CardPileCmd.Shuffle(choiceContext, Owner.Player);
        await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
    }
    
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override Color AmountLabelColor => _normalAmountLabelColor;
    
}