using Godot;
using GodotPlugins.Game;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using QueenMod2.QueenMod2Code.Extensions;
using QueenUI = QueenMod2.QueenMod2Code.Nodes.QueenUI;

namespace QueenMod2.QueenMod2Code.Cards.Uncommon;

public class StingerLaunchers() : QueenMod2Card(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3M, ValueProp.Move),
        new RepeatVar(4)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        QueenMod2Keywords.Hivebound
    ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        StingerLaunchers stingers = this;
        AttackCommand attackCommand = await DamageCmd.Attack(stingers.DynamicVars.Damage.IntValue).FromCard(stingers, play).WithHitCount(stingers.DynamicVars.Repeat.IntValue).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
    }
    
    public override Task AfterAutoPrePlayPhaseEnteredLate(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (CardPile.GetCards(this.Owner, PileType.Draw).Contains(this))
        {
            return triggerHiveBoundEffect(choiceContext);
        }
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1M);
    }

    public async Task triggerHiveBoundEffect(PlayerChoiceContext choiceContext)
    {
        StingerLaunchers stingers = this;
        //CardCmd.Preview(stingers, 0.4f, CardPreviewStyle.GridLayout);
        Creature target = Owner.RunState.Rng.CombatTargets.NextItem<Creature>((IEnumerable<Creature>) Owner.Creature.CombatState.HittableEnemies);
        while (target == null)
        {
            target = Owner.RunState.Rng.CombatTargets.NextItem<Creature>((IEnumerable<Creature>) Owner.Creature.CombatState.HittableEnemies);
            if (CombatState.HittableEnemies.Count <= 0 || CombatManager.Instance.IsOverOrEnding)
            {
                return;
            }
        }
        if (LocalContext.IsMine(this))
        {
            Control previewContainer = QueenUI.Instance.HiveBoundPreviewContainer;
            NCard cardNode = NCard.Create(this);
            if (previewContainer != null)
                previewContainer.AddChildSafely((Node) NCardUpgradeVfx.Create(this));
        }
        await CreatureCmd.Damage((PlayerChoiceContext)new ThrowingPlayerChoiceContext(), target,
            (Decimal)DynamicVars.Damage.IntValue, ValueProp.Unpowered, null,
            null);
    }
}