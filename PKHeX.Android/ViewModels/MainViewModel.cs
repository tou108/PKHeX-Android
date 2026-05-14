using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PKHeX.Android.Pages;
using PKHeX.Android.Services;

namespace PKHeX.Android.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SaveFileService _saveService;

    [ObservableProperty]
    private string _statusText = "セーブデータを開いてください";

    [ObservableProperty]
    private string _gameInfo = string.Empty;

    [ObservableProperty]
    private bool _isSaveLoaded = false;

    [ObservableProperty]
    private bool _isLoading = false;

    public MainViewModel(SaveFileService saveService)
    {
        _saveService = saveService;
        _saveService.SaveFileChanged += OnSaveFileChanged;
    }

    private void OnSaveFileChanged(object? sender, PKHeX.Core.SaveFile? save)
    {
        IsSaveLoaded = save != null;
        if (save != null)
        {
            GameInfo = _saveService.GetGameInfo();
            StatusText = $"読み込み完了: {save.Version}";
        }
    }

    [RelayCommand]
    private async Task OpenSaveFileAsync()
    {
        try
        {
            IsLoading = true;
            StatusText = "ファイルを選択中...";

            var options = new PickOptions
            {
                PickerTitle = "セーブデータを選択",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, ["application/octet-stream", "*/*"] },
                })
            };

            var result = await FilePicker.Default.PickAsync(options);
            if (result == null)
            {
                StatusText = "キャンセルされました";
                return;
            }

            StatusText = $"読み込み中: {result.FileName}";
            using var stream = await result.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var data = ms.ToArray();

            if (_saveService.TryLoadFromBytes(data, result.FileName, out var error))
            {
                await Shell.Current.GoToAsync(nameof(SaveEditorPage));
            }
            else
            {
                StatusText = error;
                await Shell.Current.DisplayAlert("エラー", error, "OK");
            }
        }
        catch (Exception ex)
        {
            StatusText = $"エラー: {ex.Message}";
            await Shell.Current.DisplayAlert("エラー", ex.Message, "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task OpenSaveEditorAsync()
    {
        if (!IsSaveLoaded) return;
        await Shell.Current.GoToAsync(nameof(SaveEditorPage));
    }

    [RelayCommand]
    private async Task ExportSaveFileAsync()
    {
        if (!IsSaveLoaded) return;

        try
        {
            var data = _saveService.ExportBytes();
            if (data == null) return;

            var fileName = $"PKHeX_save_{DateTime.Now:yyyyMMdd_HHmmss}.sav";
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
}
