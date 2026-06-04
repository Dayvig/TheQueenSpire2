using GodotPlugins.Game;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Powers;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class Slap() : QueenMod2Card(0,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3M, ValueProp.Move),
        new("TemporaryStrength", 1M)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        QueenMod2Keywords.Hivebound
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        Slap slap = this;
        AttackCommand attackCommand = await DamageCmd.Attack(slap.DynamicVars.Damage.IntValue).FromCard(slap).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        SlapPower setupStrikePower = await PowerCmd.Apply<SlapPower>(choiceContext, slap.Owner.Creature, slap.DynamicVars["TemporaryStrength"].BaseValue, slap.Owner.Creature, (CardModel) slap);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
        DynamicVars["TemporaryStrength"].UpgradeValueBy(1M);
    }

}