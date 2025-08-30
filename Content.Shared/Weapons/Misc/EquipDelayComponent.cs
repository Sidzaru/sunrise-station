using Robust.Shared.GameStates;

namespace Content.Shared.Weapons;

[RegisterComponent, NetworkedComponent]
public sealed partial class EquipDelayComponent : Component
{
    [DataField("delay")]
    public float EquipDelayTime = 0.3f;
}