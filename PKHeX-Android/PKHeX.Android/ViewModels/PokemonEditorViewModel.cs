using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Android.Services;
using PKHeX.Core;

namespace PKHeX.Android.ViewModels;

[QueryProperty(nameof(BoxIndex), "BoxIndex")]
[QueryProperty(nameof(SlotIndex), "SlotIndex")]
public partial class PokemonEditorViewModel : ObservableObject
{
    private readonly SaveFileService _saveService;
    private PKM? _originalPkm;
    private PKM? _editingPkm;

    // Navigation params
    [ObservableProperty] private int _boxIndex;
    [ObservableProperty] private int _slotIndex;

    // Basic Info
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private string _otName = string.Empty;
    [ObservableProperty] private int _species;
    [ObservableProperty] private string _speciesName = string.Empty;
    [ObservableProperty] private int _level = 1;
    [ObservableProperty] private bool _isShiny;
    [ObservableProperty] private int _gender;
    [ObservableProperty] private int _form;
    [ObservableProperty] private int _nature;
    [ObservableProperty] private string _natureName = string.Empty;
    [ObservableProperty] private int _ability;
    [ObservableProperty] private string _abilityName = string.Empty;
    [ObservableProperty] private int _heldItem;
    [ObservableProperty] private string _heldItemName = string.Empty;
    [ObservableProperty] private ulong _exp;

    // Stats
    [ObservableProperty] private int _hpStat;
    [ObservableProperty] private int _atkStat;
    [ObservableProperty] private int _defStat;
    [ObservableProperty] private int _spaStat;
    [ObservableProperty] private int _spdStat;
    [ObservableProperty] private int _speStat;

    // EVs
    [ObservableProperty] private int _hpEv;
    [ObservableProperty] private int _atkEv;
    [ObservableProperty] private int _defEv;
    [ObservableProperty] private int _spaEv;
    [ObservableProperty] private int _spdEv;
    [ObservableProperty] private int _speEv;

    // IVs
    [ObservableProperty] private int _hpIv;
    [ObservableProperty] private int _atkIv;
    [ObservableProperty] private int _defIv;
    [ObservableProperty] private int _spaIv;
    [ObservableProperty] private int _spdIv;
    [ObservableProperty] private int _speIv;

    // Moves
    [ObservableProperty] private string _move1Name = string.Empty;
    [ObservableProperty] private string _move2Name = string.Empty;
    [ObservableProperty] private string _move3Name = string.Empty;
    [ObservableProperty] private string _move4Name = string.Empty;
    [ObservableProperty] private int _move1;
    [ObservableProperty] private int _move2;
    [ObservableProperty] private int _move3;
    [ObservableProperty] private int _move4;

    // Legality
    [ObservableProperty] private string _legalityReport = string.Empty;
    [ObservableProperty] private bool _isLegal;

    // Picker data
    public ObservableCollection<string> SpeciesNames { get; } = [];
    public ObservableCollection<string> MoveNames { get; } = [];
    public ObservableCollection<string> NatureNames { get; } = [];
    public ObservableCollection<string> ItemNames { get; } = [];
    public ObservableCollection<string> AbilityNames { get; } = [];

    private GameStrings? _strings;

    public PokemonEditorViewModel(SaveFileService saveService)
    {
        _saveService = saveService;
    }

    public void LoadPokemon()
    {
        var save = _saveService.CurrentSave;
        if (save == null) return;

        _strings = GameInfo.GetStrings(GameLanguage.DefaultLanguage);

        // Populate picker data
        SpeciesNames.Clear();
        foreach (var s in _strings.Species) SpeciesNames.Add(s);

        MoveNames.Clear();
        foreach (var m in _strings.Move) MoveNames.Add(m);

        NatureNames.Clear();
        foreach (var n in _strings.Natures) NatureNames.Add(n);

        ItemNames.Clear();
        foreach (var i in _strings.Item) ItemNames.Add(i);

        // Load PKM
        _originalPkm = BoxIndex >= 0
            ? save.GetBoxSlotAtIndex(BoxIndex, SlotIndex)
            : save.GetPartySlotAtIndex(SlotIndex);

        _editingPkm = _originalPkm.Clone();
        RefreshFromPkm();
    }

    private void RefreshFromPkm()
    {
        var pkm = _editingPkm;
        if (pkm == null || _strings == null) return;

        Species = pkm.Species;
        SpeciesName = _strings.Species.SafeGet(pkm.Species);
        Nickname = pkm.Nickname;
        OtName = pkm.OriginalTrainerName;  // FIX: OT_Name -> OriginalTrainerName
        Level = pkm.CurrentLevel;
        IsShiny = pkm.IsShiny;
        Gender = pkm.Gender;
        Form = pkm.Form;
        Nature = (int)pkm.StatNature;  // FIX: StatNature returns Nature enum, cast to int
        NatureName = _strings.Natures.SafeGet((int)pkm.StatNature);  // FIX: same
        Ability = pkm.Ability;
        AbilityName = _strings.Ability.SafeGet(pkm.Ability);
        HeldItem = pkm.HeldItem;
        HeldItemName = _strings.Item.SafeGet(pkm.HeldItem);
        Exp = pkm.EXP;

        // Stats - FIX: Stat_HP -> Stat_HPCurrent
        HpStat = pkm.Stat_HPCurrent;
        AtkStat = pkm.Stat_ATK;
        DefStat = pkm.Stat_DEF;
        SpaStat = pkm.Stat_SPA;
        SpdStat = pkm.Stat_SPD;
        SpeStat = pkm.Stat_SPE;

        // EVs
        HpEv = pkm.EV_HP;
        AtkEv = pkm.EV_ATK;
        DefEv = pkm.EV_DEF;
        SpaEv = pkm.EV_SPA;
        SpdEv = pkm.EV_SPD;
        SpeEv = pkm.EV_SPE;

        // IVs
        HpIv = pkm.IV_HP;
        AtkIv = pkm.IV_ATK;
        DefIv = pkm.IV_DEF;
        SpaIv = pkm.IV_SPA;
        SpdIv = pkm.IV_SPD;
        SpeIv = pkm.IV_SPE;

        // Moves
        Move1 = pkm.Move1; Move1Name = _strings.Move.SafeGet(pkm.Move1);
        Move2 = pkm.Move2; Move2Name = _strings.Move.SafeGet(pkm.Move2);
        Move3 = pkm.Move3; Move3Name = _strings.Move.SafeGet(pkm.Move3);
        Move4 = pkm.Move4; Move4Name = _strings.Move.SafeGet(pkm.Move4);

        CheckLegality();
    }

    private void CheckLegality()
    {
        if (_editingPkm == null) return;
        LegalityReport = SaveFileService.GetLegalityReport(_editingPkm);
        IsLegal = LegalityReport.StartsWith("✅");
    }

    [RelayCommand]
    private void ApplyMaxStats()
    {
        var pkm = _editingPkm;
        if (pkm == null) return;

        // Max IVs
        pkm.IV_HP = pkm.IV_ATK = pkm.IV_DEF = pkm.IV_SPA = pkm.IV_SPD = pkm.IV_SPE = 31;
        // Max EVs (252/252/4 spread)
        pkm.EV_HP = 4; pkm.EV_ATK = pkm.EV_SPA = 0;
        pkm.EV_DEF = pkm.EV_SPD = 0; pkm.EV_SPE = 252;
        pkm.EV_ATK = 252;

        pkm.CurrentLevel = 100;
        pkm.EXP = Experience.GetEXP(100, pkm.PersonalInfo.EXPGrowth);
        pkm.HealPP();
        pkm.SetMoveset();  // FIX: SetSuggestedMoves() -> SetMoveset()
        pkm.RefreshAbility(pkm.AbilityNumber);

        RefreshFromPkm();
    }

    [RelayCommand]
    private void MakeShiny()
    {
        var pkm = _editingPkm;
        if (pkm == null) return;
        CommonEdits.SetShiny(pkm);
        IsShiny = pkm.IsShiny;
    }

    [RelayCommand]
    private async Task FixLegalityAsync()
    {
        // FIX: pkm.Legalize() is no longer available in this version of PKHeX.Core.
        // Applying suggested moveset and refreshing as a best-effort improvement.
        var pkm = _editingPkm;
        if (pkm == null) return;

        try
        {
            pkm.SetMoveset();
            pkm.HealPP();
            pkm.RefreshChecksum();
            RefreshFromPkm();
        }
        catch (Exception)
        {
            // Legalization may not always be possible
        }

        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SavePokemonAsync()
    {
        var pkm = _editingPkm;
        var save = _saveService.CurrentSave;
        if (pkm == null || save == null) return;

        // Write back all edited values
        pkm.Nickname = Nickname;
        pkm.OriginalTrainerName = OtName;  // FIX: OT_Name -> OriginalTrainerName
        pkm.CurrentLevel = Math.Clamp(Level, 1, 100);
        pkm.EV_HP = Math.Clamp(HpEv, 0, 252);
        pkm.EV_ATK = Math.Clamp(AtkEv, 0, 252);
        pkm.EV_DEF = Math.Clamp(DefEv, 0, 252);
        pkm.EV_SPA = Math.Clamp(SpaEv, 0, 252);
        pkm.EV_SPD = Math.Clamp(SpdEv, 0, 252);
        pkm.EV_SPE = Math.Clamp(SpeEv, 0, 252);
        pkm.IV_HP = Math.Clamp(HpIv, 0, 31);
        pkm.IV_ATK = Math.Clamp(AtkIv, 0, 31);
        pkm.IV_DEF = Math.Clamp(DefIv, 0, 31);
        pkm.IV_SPA = Math.Clamp(SpaIv, 0, 31);
        pkm.IV_SPD = Math.Clamp(SpdIv, 0, 31);
        pkm.IV_SPE = Math.Clamp(SpeIv, 0, 31);
        // FIX: Move1/2/3/4 are ushort, explicit cast required from int
        pkm.Move1 = (ushort)Move1; pkm.Move2 = (ushort)Move2;
        pkm.Move3 = (ushort)Move3; pkm.Move4 = (ushort)Move4;
        pkm.SetNature((Nature)Nature);
        pkm.RefreshChecksum();

        // Write to save
        if (BoxIndex >= 0)
            _saveService.SetBoxPokemon(pkm, BoxIndex, SlotIndex);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}

internal static class StringArrayExtensions
{
    // FIX: Changed parameter type from string[] to IReadOnlyList<string>
    // because GameStrings.Species, .Move, .Ability, etc. are IReadOnlyList<string>
    public static string SafeGet(this IReadOnlyList<string> list, int index)
        => (uint)index < (uint)list.Count ? list[index] : "???";
}
