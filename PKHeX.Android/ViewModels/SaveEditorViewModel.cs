using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Android.Services;
using PKHeX.Core;

namespace PKHeX.Android.ViewModels;

public partial class SaveEditorViewModel : ObservableObject
{
    private readonly SaveFileService _saveService;

    [ObservableProperty]
    private string _trainerName = string.Empty;

    [ObservableProperty]
    private string _gameVersion = string.Empty;

    [ObservableProperty]
    private string _tid = string.Empty;

    [ObservableProperty]
    private string _sid = string.Empty;

    [ObservableProperty]
    private string _money = string.Empty;

    [ObservableProperty]
    private int _badgeCount = 0;

    [ObservableProperty]
    private string _playTime = string.Empty;

    [ObservableProperty]
    private bool _isModified = false;

    public SaveEditorViewModel(SaveFileService saveService)
    {
        _saveService = saveService;
    }

    public void LoadFromSave()
    {
        var save = _saveService.CurrentSave;
        if (save == null) return;

        TrainerName = save.OT;
        GameVersion = save.Version.ToString();
        Tid = save.DisplayTID;
        Sid = save.DisplaySID;
        Money = save is IEventFlag ef ? save.ToString() ?? "" : "";
        PlayTime = $"{save.PlayedHours}:{save.PlayedMinutes:D2}:{save.PlayedSeconds:D2}";

        // Money
        if (save is SAV2 sav2) Money = sav2.Money.ToString();
        else if (save is SAV3 sav3) Money = sav3.Money.ToString();
        else if (save is SAV4 sav4) Money = sav4.Money.ToString();
        else if (save is SAV5 sav5) Money = sav5.Money.ToString();
        else if (save is SAV6 sav6) Money = sav6.Money.ToString();
        else if (save is SAV7 sav7) Money = sav7.Money.ToString();
        else if (save is SAV8SWSH swsh) Money = swsh.Money.ToString();
        else if (save is SAV9SV sv) Money = sv.Money.ToString();

        IsModified = false;
    }

    [RelayCommand]
    private async Task SaveChangesAsync()
    {
        var save = _saveService.CurrentSave;
        if (save == null) return;

        try
        {
            save.OT = TrainerName;

            // Apply money if parseable
            if (ulong.TryParse(Money, out var money))
            {
                if (save is SAV2 sav2) sav2.Money = (uint)Math.Min(money, 999999);
                else if (save is SAV3 sav3) sav3.Money = (uint)Math.Min(money, 999999);
                else if (save is SAV4 sav4) sav4.Money = (uint)Math.Min(money, 9999999);
                else if (save is SAV5 sav5) sav5.Money = (uint)Math.Min(money, 9999999);
                else if (save is SAV6 sav6) sav6.Money = (uint)Math.Min(money, 9999999);
                else if (save is SAV7 sav7) sav7.Money = (uint)Math.Min(money, 9999999);
                else if (save is SAV8SWSH swsh) swsh.Money = (uint)Math.Min(money, 9999999);
                else if (save is SAV9SV sv) sv.Money = (uint)Math.Min(money, 9999999);
            }

            IsModified = false;
            await Shell.Current.DisplayAlert("保存完了", "変更をメモリに適用しました。\nエクスポートでファイルに書き出してください。", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("エラー", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task ExportSaveAsync()
    {
        try
        {
            var data = _saveService.ExportBytes();
            if (data == null) return;

            var fileName = $"PKHeX_{GameVersion}_{TrainerName}_{DateTime.Now:yyyyMMdd_HHmmss}.sav";
            var path = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllBytesAsync(path, data);

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "セーブデータをエクスポート",
                File = new ShareFile(path)
            });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("エクスポートエラー", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task GiveAllItemsAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "全アイテム入手",
            "全てのアイテムを最大数入手しますか？",
            "はい", "いいえ");

        if (!confirm) return;

        var save = _saveService.CurrentSave;
        if (save == null) return;

        try
        {
            // Use PKHeX.Core's bulk item editor
            var items = save.Inventory;
            foreach (var pouch in items)
            {
                for (int i = 0; i < pouch.Items.Length; i++)
                {
                    if (pouch.Items[i].Index == 0) continue;
                    pouch.Items[i] = pouch.Items[i] with { Count = pouch.MaxCount };
                }
                save.SetInventory(pouch);
            }
            IsModified = true;
            await Shell.Current.DisplayAlert("完了", "全アイテムを最大数に設定しました。", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("エラー", ex.Message, "OK");
        }
    }
}
