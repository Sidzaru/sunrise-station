using Robust.Shared.GameStates;

namespace Content.Shared.Weapons;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class EquipDelayComponent : Component
{
    [DataField("delay"), AutoNetworkedField]
    public float EquipDelayTime = 0.3f;
}