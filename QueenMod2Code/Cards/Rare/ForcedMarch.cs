using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class ForcedMarch() : QueenMod2Card(1,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ForcedMarch march = this;
        await PowerCmd.Apply<ForcedMarchPower>(choiceContext, march.Owner.Creature,
            1M,
            march.Owner.Creature, (CardModel)this);
    }
    
    
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}