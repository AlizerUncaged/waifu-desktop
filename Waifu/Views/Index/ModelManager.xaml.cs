using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Waifu.Views.Index;

public partial class ModelManager : UserControl, IPopup
{
    public ModelManager()
    {
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
        Directory.CreateDirectory(Constants.ModelsFolder);

        var filesInModelsFolder = Directory.GetFiles(Constants.ModelsFolder, "*", SearchOption.AllDirectories);

        foreach (var file in filesInModelsFolder)
            ModelNames.Add(Path.GetFileName(file));
    }
}