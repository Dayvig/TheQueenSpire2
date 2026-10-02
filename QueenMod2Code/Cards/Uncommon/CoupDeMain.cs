using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.ValueProps;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class CoupDeMain() : QueenMod2Card(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10M, ValueProp.Move),
        new RepeatVar(2)
    ];

    protected override bool ShouldGlowGoldInternal => (CardPile.Get(PileType.Hand, Owner)!.Cards.Count >= 8);
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Godot.Color color = new Godot.Color("FFFFFF80");
        double num2 = SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast ? 0.2 : 0.3;
        NCombatRoom instance1 = NCombatRoom.Instance;
        ArgumentNullException.ThrowIfNull((object) Owner, "play.Target");
        for (int i = 0; i < DynamicVars.Repeat.IntValue; i++)
        {
            if (instance1 != null)
                instance1.CombatVfxContainer.AddChildSafely((Godot.Node) NHorizontalLinesVfx.Create(color, 0.8 + (double)(2 * num2)));
            AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.IntValue).FromCard(this, play).TargetingAllOpponents(this.CombatState)
                .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        }
        if (CardPile.Get(PileType.Hand, Owner) != null && CardPile.Get(PileType.Hand, Owner)!.Cards.Count >= 7)
        {  
            await PlayerCmd.GainEnergy(2, Owner);
        }
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}