using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Android.Pages;
using PKHeX.Android.Services;
using PKHeX.Core;

namespace PKHeX.Android.ViewModels;

public partial class BoxViewModel : ObservableObject
{
    private readonly SaveFileService _saveService;

    [ObservableProperty]
    private string _boxName = "Box 1";

    [ObservableProperty]
    private int _currentBox = 0;

    [ObservableProperty]
    private int _totalBoxes = 0;

    [ObservableProperty]
    private ObservableCollection<PokemonSlotViewModel> _slots = [];

    [ObservableProperty]
    private bool _showParty = false;

    public BoxViewModel(SaveFileService saveService)
    {
        _saveService = saveService;
    }

    public void Initialize()
    {
        var save = _saveService.CurrentSave;
        if (save == null) return;
        TotalBoxes = save.BoxCount;
        LoadBox(0);
    }

    private void LoadBox(int boxIndex)
    {
        var save = _saveService.CurrentSave;
        if (save == null) return;

        CurrentBox = boxIndex;
        BoxName = save.GetBoxName(boxIndex);
        Slots.Clear();

        for (int slot = 0; slot < save.BoxSlotCount; slot++)
        {
            var pkm = save.GetBoxSlotAtIndex(boxIndex, slot);
            Slots.Add(new PokemonSlotViewModel(pkm, boxIndex, slot));
        }
    }

    [RelayCommand]
    private void PreviousBox()
    {
        if (CurrentBox > 0)
            LoadBox(CurrentBox - 1);
    }

    [RelayCommand]
    private void NextBox()
    {
        if (CurrentBox < TotalBoxes - 1)
            LoadBox(CurrentBox + 1);
    }

    [RelayCommand]
    private async Task SelectSlotAsync(PokemonSlotViewModel? slot)
    {
        if (slot == null || slot.IsEmpty) return;

        var nav = new Dictionary<string, object>
        {
            { "BoxIndex", slot.BoxIndex },
            { "SlotIndex", slot.SlotIndex },
        };
        await Shell.Current.GoToAsync(nameof(PokemonEditorPage), nav);
    }

    [RelayCommand]
    private void TogglePartyView()
    {
        ShowParty = !ShowParty;
        if (ShowParty) LoadParty();
        else LoadBox(CurrentBox);
    }

    private void LoadParty()
    {
        var save = _saveService.CurrentSave;
        if (save == null) return;

        Slots.Clear();
        for (int i = 0; i < 6; i++)
        {
            if (i < save.PartyCount)
            {
                var pkm = save.GetPartySlotAtIndex(i);
                Slots.Add(new PokemonSlotViewModel(pkm, -1, i) { IsParty = true });
            }
            else
            {
                Slots.Add(new PokemonSlotViewModel(null, -1, i) { IsParty = true });
            }
        }
    }
}

/// <summary>
/// Represents a single slot in a box or party.
/// </summary>
public partial class PokemonSlotViewModel : ObservableObject
{
    public PKM? Pokemon { get; }
    public int BoxIndex { get; }
    public int SlotIndex { get; }
    public bool IsParty { get; set; }
    public bool IsEmpty => Pokemon == null || Pokemon.Species == 0;

    public string DisplayName => IsEmpty
        ? "---"
        : GameInfo.GetStrings("en").Species[Pokemon!.Species];

    public string DisplayLevel => IsEmpty ? "" : $"Lv.{Pokemon!.CurrentLevel}";

    public string DisplayGender => IsEmpty ? "" : Pokemon!.Gender switch
    {
        0 => "♂",
        1 => "♀",
        _ => "⚲"
    };

    public string ShinyIndicator => (!IsEmpty && Pokemon!.IsShiny) ? "✨" : "";

    public Color SlotBackground => IsEmpty
        ? Color.FromArgb("#2A2A2A")
        : Color.FromArgb("#1E3A5F");

    public PokemonSlotViewModel(PKM? pkm, int boxIndex, int slotIndex)
    {
        Pokemon = pkm?.Species == 0 ? null : pkm;
        BoxIndex = boxIndex;
        SlotIndex = slotIndex;
    }
}
