using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using QueenMod2.QueenMod2Code.Cards.Generated;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Rare;

public class SwarmTactics() : QueenMod2Card(2,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new("Swarm", 1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower(ModelDb.Power<Swarm>())
    ];  
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        SwarmTactics armorers = this;

        await PowerCmd.Apply<SwarmTacticsPower>(choiceContext, armorers.Owner.Creature,
            DynamicVars["Swarm"].IntValue,
            armorers.Owner.Creature, (CardModel)this);
    }
    
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}