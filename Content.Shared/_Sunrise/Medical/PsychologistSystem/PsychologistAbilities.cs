using Content.Shared.Humanoid;
using Content.Shared.DoAfter;
using Content.Shared.Nutrition;
namespace Content.Shared._Sunrise.Medical.PsychologistSystem;

public sealed partial class PsychologistSystem : EntitySystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HumanoidAppearanceComponent, AlcoholBlockEvent>(onAlcoholBlockTry);
        SubscribeLocalEvent<HumanoidAppearanceComponent, DoAfterAlcoholBlockEvent>(DoAfterAlcoholBlock);

        SubscribeLocalEvent<AlcoholBlockComponent, BeforeIngestedEvent>(OnDrink);
    }

    private void DoAfterAlcoholBlock(Entity<HumanoidAppearanceComponent> ent, ref DoAfterAlcoholBlockEvent args)
    {
        if (args.Target != null)
        {
            if (CompOrNull<AlcoholBlockComponent>(args.Target) != null)
            {
                RemComp<AlcoholBlockComponent>(args.Target.Value);
                args.Handled = true;
            }
            else
            {
                AddComp<AlcoholBlockComponent>(args.Target.Value);
                args.Handled = true;
            }
        }
        else
        {
            return;
        }
    }

    private void onAlcoholBlockTry(Entity<HumanoidAppearanceComponent> ent, ref AlcoholBlockEvent args)
    {
        var tryAlcoholBlock = _doAfterSystem.TryStartDoAfter(new DoAfterArgs(EntityManager, ent.Owner, args.Action.Comp.UseDelay ?? TimeSpan.FromSeconds(10),
            new DoAfterAlcoholBlockEvent(), eventTarget: args.Target, target: args.Target, used: args.Target)
        {
            BreakOnMove = true,
            BreakOnDamage = true
        });
        if (!tryAlcoholBlock)
        {
            args.Handled = true;
        }
    }
    private void OnDrink(Entity<AlcoholBlockComponent> ent, ref BeforeIngestedEvent args)
    {
    }
}
