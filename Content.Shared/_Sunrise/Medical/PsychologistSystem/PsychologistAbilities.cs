using Content.Shared.Humanoid;
using Content.Shared.DoAfter;
using Content.Shared.Nutrition;
using Robust.Shared.Prototypes;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.EntityEffects.Effects;
using Content.Shared.Popups;

namespace Content.Shared._Sunrise.Medical.PsychologistSystem;

public sealed partial class PsychologistSystem : EntitySystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;

    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HumanoidAppearanceComponent, AlcoholBlockEvent>(OnAlcoholBlockTry);
        SubscribeLocalEvent<HumanoidAppearanceComponent, DoAfterAlcoholBlockEvent>(DoAfterAlcoholBlock);

        SubscribeLocalEvent<BlockAlcoholComponent, BeforeIngestedEvent>(OnDrink);
    }

    private void DoAfterAlcoholBlock(Entity<HumanoidAppearanceComponent> ent, ref DoAfterAlcoholBlockEvent args)
    {
        if (args.Target != null)
        {
            if (CompOrNull<BlockAlcoholComponent>(args.Target) != null)
            {
                RemComp<BlockAlcoholComponent>(args.Target.Value);
            }
            else
            {
                AddComp<BlockAlcoholComponent>(args.Target.Value);
            }
        }
    }

    private void OnAlcoholBlockTry(Entity<HumanoidAppearanceComponent> ent, ref AlcoholBlockEvent args)
    {
        if (EntityManager.TryGetComponent<HumanoidAppearanceComponent>(args.Target, out var humanoidAppearanceComponent))
        {
            if (humanoidAppearanceComponent != null)
            {
                if ($"{humanoidAppearanceComponent.Species.Id}" == "Dwarf")
                {
                    _popupSystem.PopupEntity("You cant encode Dwarf", ent.Owner);
                    return;
                }
            }
        }

        if (_doAfterSystem.TryStartDoAfter(new DoAfterArgs(EntityManager, ent.Owner, args.Action.Comp.UseDelay ?? TimeSpan.FromSeconds(10),
            new DoAfterAlcoholBlockEvent(), eventTarget: args.Target, target: args.Target, used: args.Target)
        {
            BreakOnMove = true,
            BreakOnDamage = true
        }))
        {
            args.Handled = true;
        }
        else
        {
            args.Handled = false;
        }

    }
    private void OnDrink(Entity<BlockAlcoholComponent> ent, ref BeforeIngestedEvent args)
    {
        if (args.Solution != null)
        {
            foreach (var cont in args.Solution.Contents)
            {
                var reagent = _prototypeManager.Index<ReagentPrototype>($"{cont.Reagent}");

                if (reagent.Metabolisms != null)
                {
                    foreach (var metabolism in reagent.Metabolisms)
                    {
                        foreach (var effect in metabolism.Value.Effects)
                        {
                            if (effect is AdjustReagent adjust && adjust.Reagent == "Ethanol")
                            {
                                args.Cancelled = true;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            return;
        }

    }
}
