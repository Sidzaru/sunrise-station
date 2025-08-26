using Content.Client.UserInterface.Controls;
using Content.Shared.SmartFridge;
using Robust.Client.UserInterface;
using Robust.Shared.Input;

namespace Content.Client.SmartFridge;

public sealed class SmartFridgeBoundUserInterface : BoundUserInterface
{
    private SmartFridgeMenu? _menu;

    public SmartFridgeBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<SmartFridgeMenu>();
        _menu.OnItemSelected += OnItemSelected;
        Refresh();
    }

    public void Refresh()
    {
        if (_menu is not { } menu || !EntMan.TryGetComponent(Owner, out SmartFridgeComponent? fridge))
            return;

        menu.SetFlavorText(Loc.GetString(fridge.FlavorText));
        menu.Populate((Owner, fridge));
    }

    private void OnItemSelected(GUIBoundKeyEventArgs args, ListData data)
    {
        // Sunrise Edit
        if (data is not SmartFridgeListData entry)
            return;
        if (args.Function == EngineKeyFunctions.UIRightClick)
            SendPredictedMessage(new SmartFridgeDeleteItemMessage(entry.Entry));
        // Sunrise Edit End
        if (args.Function != EngineKeyFunctions.UIClick)
            return;
        SendPredictedMessage(new SmartFridgeDispenseItemMessage(entry.Entry));
    }
}
