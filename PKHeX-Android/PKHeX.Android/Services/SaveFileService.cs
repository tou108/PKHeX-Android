using PKHeX.Core;

namespace PKHeX.Android.Services;

/// <summary>
/// Application-wide service that manages the currently loaded PKHeX SaveFile.
/// Wraps PKHeX.Core's SaveUtil for Android.
/// </summary>
public class SaveFileService
{
    private SaveFile? _currentSave;

    public SaveFile? CurrentSave => _currentSave;
    public bool IsLoaded => _currentSave != null;

    public event EventHandler<SaveFile?>? SaveFileChanged;

    /// <summary>
    /// Load a save file from a byte array (read from picked file).
    /// </summary>
    public bool TryLoadFromBytes(byte[] data, string fileName, out string errorMessage)
    {
        errorMessage = string.Empty;
        try
        {
            var save = SaveUtil.GetVariantSAV(data);
            if (save == null)
            {
                errorMessage = "このファイルは対応していない保存データです。";
                return false;
            }
            _currentSave = save;
            SaveFileChanged?.Invoke(this, _currentSave);
            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"読み込みエラー: {ex.Message}";
            return false;
        }
    }

    /// <summary>
    /// Load a save file from the file system path.
    /// </summary>
    public async Task<(bool Success, string Error)> LoadFromPathAsync(string filePath)
    {
        try
        {
            var data = await File.ReadAllBytesAsync(filePath);
            if (TryLoadFromBytes(data, Path.GetFileName(filePath), out var err))
                return (true, string.Empty);
            return (false, err);
        }
        catch (Exception ex)
        {
            return (false, $"ファイル読み込みエラー: {ex.Message}");
        }
    }

    /// <summary>
    /// Export the current save file to bytes.
    /// </summary>
    public byte[]? ExportBytes()
    {
        return _currentSave?.Write();
    }

    /// <summary>
    /// Get game version display string.
    /// </summary>
    public string GetGameInfo()
    {
        if (_currentSave == null) return "未読み込み";
        return $"{_currentSave.Version} - {_currentSave.OT} (TID: {_currentSave.DisplayTID})";
    }

    /// <summary>
    /// Get all Pokémon in box storage as a flat list.
    /// </summary>
    public List<PKM?> GetBoxPokemon()
    {
        if (_currentSave == null) return [];
        var result = new List<PKM?>();
        for (int box = 0; box < _currentSave.BoxCount; box++)
        {
            for (int slot = 0; slot < _currentSave.BoxSlotCount; slot++)
            {
                var pkm = _currentSave.GetBoxSlotAtIndex(box, slot);
                result.Add(pkm.Species == 0 ? null : pkm);
            }
        }
        return result;
    }

    /// <summary>
    /// Get party Pokémon (up to 6).
    /// </summary>
    public List<PKM?> GetPartyPokemon()
    {
        if (_currentSave == null) return [];
        var result = new List<PKM?>();
        for (int i = 0; i < _currentSave.PartyCount; i++)
            result.Add(_currentSave.GetPartySlotAtIndex(i));
        return result;
    }

    /// <summary>
    /// Save a modified Pokémon back to the box.
    /// </summary>
    public void SetBoxPokemon(PKM pkm, int box, int slot)
    {
        _currentSave?.SetBoxSlotAtIndex(pkm, box, slot);
    }

    /// <summary>
    /// Apply legality fixes to a Pokémon.
    /// </summary>
    public static string GetLegalityReport(PKM pkm)
    {
        var la = new LegalityAnalysis(pkm);
        return la.Valid ? "✅ 合法" : $"❌ 違法\n{string.Join("\n", la.Results.Select(r => r.Comment))}";
    }
}
