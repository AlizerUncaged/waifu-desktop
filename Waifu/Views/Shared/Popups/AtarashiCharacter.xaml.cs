using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Waifu.Data;
using Waifu.Models;

namespace Waifu.Views.Shared.Popups;

public partial class AtarashiCharacter : UserControl, IPopup
{
    private readonly ImageHashing _imageHashing;
    private readonly Characters _characters;

    public AtarashiCharacter(ImageHashing imageHashing, Characters characters)
    {
        _imageHashing = imageHashing;
        _characters = characters;
        InitializeComponent();
    }

    public event EventHandler? CloseTriggered;
    public event EventHandler<FrameworkElement>? ReplaceTriggered;

    private void NewCharacterPopupLoaded(object sender, RoutedEventArgs e)
    {
        // had to set it here so xaml wont break
        HintAssist.SetHint(SampleTextField,
            "{user}: vanilla...?" + $"{Environment.NewLine}Vanilla: master, wake up nyaa~~ it's morning");
    }

    private string? characterImage = null;

    private void BrowserForImages(object sender, RoutedEventArgs e)
    {
        OpenFileDialog op = new OpenFileDialog();

        op.Title = "Select a picture";
        op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                    "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                    "Portable Network Graphic (*.png)|*.png";

        if (op.ShowDialog() == true)
        {
            characterImage = op.FileName;
            CharacterImage.Source = new BitmapImage(new Uri(characterImage));
        }
    }

    public void CharacterSaved() =>
        CloseTriggered?.Invoke(this, EventArgs.Empty);


    private void SaveCharacter(object sender, RoutedEventArgs e)
    {
        var character = new RoleplayCharacter()
        {
            CharacterName = CharacterNameField.Text.Trim(),
            Description = DescriptionField.Text.Trim(),
            SampleMessages = SampleTextField.Text.Trim()
        };

        // must not..freeze the UI!!
        _ = Task.Run(async () =>
        {
            // save image!!!
            if (!string.IsNullOrWhiteSpace(characterImage) && File.Exists(characterImage))
            {
                var fileHash = await _imageHashing.StoreImageAsync(characterImage);

                character.ProfilePictureHash = $".\\{fileHash}";

                await _characters.AddCharacterAsync(character);

                this.Dispatcher.Invoke(CharacterSaved);
            }
        });
    }
}