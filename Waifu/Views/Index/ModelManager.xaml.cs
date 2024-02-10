using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Humanizer;
using Waifu.Data;
using Waifu.Utilities;
using Whisper.net.Ggml;

namespace Waifu.Views.Index;

public partial class ModelManager : UserControl, IPopup
{
    private readonly Data.Settings _settings;
    private readonly HuggingFaceModelDownloader _huggingFaceModelDownloader;

    public Models.Settings Settings { get; set; }

    public ModelManager(Data.Settings settings, HuggingFaceModelDownloader huggingFaceModelDownloader)
    {
        _settings = settings;
        _huggingFaceModelDownloader = huggingFaceModelDownloader;

        foreach (var ggmlValues in
                 Enum.GetValues(typeof(GgmlType)))
        {
            WhisperAvailableModels.Add(ggmlValues.ToString());
        }

        InitializeComponent();
    }

    public event EventHandler? CloseTriggered;

    public event EventHandler<FrameworkElement>? ReplaceTriggered;

    public ObservableCollection<string> ModelNames { get; set; } = new();
    public ObservableCollection<string> WhisperAvailableModels { get; set; } = new();

    private void CancelClicked(object sender, RoutedEventArgs e)
    {
        CloseTriggered?.Invoke(this, EventArgs.Empty);
    }

    private void ModelManagerLoaded(object sender, RoutedEventArgs e)
    {
        _ = Task.Run(async () =>
        {
            var settings = await _settings.GetOrCreateSettings();

            Settings = settings;

            Dispatcher.Invoke(() =>
            {
                CharacterAiTokenField.Password = settings.CharacterAiToken;
                WhisperModelList.Text = settings.WhisperModel.ToString();
            });
        });

        foreach (var model in _settings.GetModelsOnDirectory())
        {
            ModelNames.Add(model);
        }
    }

    private void OpenModelsFolder(object sender, RoutedEventArgs e)
    {
        try
        {
            // this doesnt work on ubuntu!!!!!!!!!
            var fullFolderPath = Path.GetFullPath(Constants.ModelsFolder);
            var tutorialFile = Path.Combine(fullFolderPath, "paste models here, .gguf preferably");

            if (!File.Exists(tutorialFile))
                File.Create(tutorialFile).Close();

            Process.Start("explorer.exe", fullFolderPath);
        }
        catch
        {
        }
    }

    private void ModelSave(object sender, RoutedEventArgs e)
    {
        var modelName = ModelsList.Text;
        var chaiToken = CharacterAiTokenField.Password;

        _ = Task.Run(async () =>
        {
            await _settings.ClearAndAddSettings(new Models.Settings()
            {
                LocalModel = modelName, CharacterAiToken = chaiToken
            });

            Dispatcher.Invoke(() =>
            {
                if (this.GetCurrentWindow() is MainWindow mainWindow)
                    mainWindow.ShowMessage("Model successfully added!");
            });
        });
    }

    private void UseCharacterAiChecked(object sender, RoutedEventArgs e)
    {
        CustomModelOptions.IsEnabled = false;
    }

    private void UseCharacterAiUnchecked(object sender, RoutedEventArgs e)
    {
        CustomModelOptions.IsEnabled = true;
    }

    private void SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && WhisperDownloadButton is { } whisperDownloadButton)
        {
            var modelFile = Path.Combine(HuggingFaceModelDownloader.ModelFolder, $"{comboBox.Text}.bin");
            if (File.Exists(modelFile))
            {
                whisperDownloadButton.Text = "Downloaded!";
            }
            else
                whisperDownloadButton.Text = "Download";
        }
    }

    private void DownloadWhisperModel(object sender, RoutedEventArgs e)
    {
        var modelFile = Path.Combine(HuggingFaceModelDownloader.ModelFolder, $"{WhisperModelList.Text}.bin");
        var senderButton = sender as Button;
        senderButton.IsEnabled = false;

        if (File.Exists(modelFile))
        {
            if (this.GetCurrentWindow() is MainWindow mainWindow)
                mainWindow.ShowMessage("We're re-downloading a model");
        }

        if (Enum.TryParse(WhisperModelList.Text, out GgmlType ggmlModel))
        {
            var progressChecker = _huggingFaceModelDownloader.DownloadWhisperModelInBackgroundAndSetAsModel(ggmlModel);
            senderButton.Content = "Fetching HuggingFace data...";
            progressChecker.OptimizedProgressChanged += (o, l) =>
            {
                if (l > 0)
                    Dispatcher.Invoke(() => { senderButton.Content = $"Downloading {l.Bytes().Humanize()}"; });
            };

            progressChecker.DownloadDone += (o, args) =>
            {
                Dispatcher.Invoke(() =>
                {
                    senderButton.Content = "Downloaded!";
                    senderButton.IsEnabled = true;
                });
            };
        }
    }
}