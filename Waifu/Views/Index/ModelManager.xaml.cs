﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Waifu.Utilities;

namespace Waifu.Views.Index;

public partial class ModelManager : UserControl, IPopup
{
    private readonly Data.Settings _settings;

    public ModelManager(Data.Settings settings)
    {
        _settings = settings;

        InitializeComponent();
    }

    public event EventHandler? CloseTriggered;

    public event EventHandler<FrameworkElement>? ReplaceTriggered;

    public ObservableCollection<string> ModelNames { get; set; } = new();

    private void CancelClicked(object sender, RoutedEventArgs e)
    {
        CloseTriggered?.Invoke(this, EventArgs.Empty);
    }

    private void ModelManagerLoaded(object sender, RoutedEventArgs e)
    {
        _ = Task.Run(async () =>
        {
            var settings = await _settings.GetOrCreateSettings();

            Dispatcher.Invoke(() => { CharacterAiTokenField.Password = settings.CharacterAiToken ?? String.Empty; });
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
        _ = Task.Run(async () =>
        {
            await _settings.ClearAndAddSettings(new Models.Settings()
            {
                LocalModel = modelName
            });

            if (this.GetCurrentWindow() is MainWindow mainWindow)
                mainWindow.ShowMessage("Model successfully added!");
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
}