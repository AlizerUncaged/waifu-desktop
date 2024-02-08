using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CharacterAI.Models;
using Waifu.Data;
using Waifu.Models;

namespace Waifu.Views.Shared.Popups;

public partial class AtarashiCharacterAi : UserControl, IPopup, INotifyPropertyChanged
{
    private readonly CharacterAiApi _characterAiApi;
    private readonly ImageHashing _imageHashing;
    private readonly Characters _characters;
    private Character? _character = null;

    public AtarashiCharacterAi(CharacterAiApi characterAiApi, ImageHashing imageHashing, Characters characters)
    {
        _characterAiApi = characterAiApi;
        _imageHashing = imageHashing;
        _characters = characters;
        InitializeComponent();
    }

    public Character? Character
    {
        get => _character;
        set
        {
            if (Equals(value, _character)) return;
            _character = value;
            OnPropertyChanged();
        }
    }

    public event EventHandler? CloseTriggered;
    public event EventHandler<FrameworkElement>? ReplaceTriggered;

    private void SaveCharacter(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement frameworkElement) return;

        SearchProgressBar.Visibility = Visibility.Visible;

        if (Character is not null)
        {
            _ = Task.Run(async () =>
            {
                var localFileDownload = await _imageHashing.StoreImageFromWebAsync(Character.AvatarUrlFull);

                var character = new RoleplayCharacter()
                {
                    CharacterName = Character.Name,
                    Description = Character.Description,
                    SampleMessages = Character.Greeting,
                    ProfilePictureHash = $".\\{localFileDownload}",
                    IsCharacterAi = true,
                    CharacterAiId = Character.Id,
                    CharacterAiTargetPersona = Character.Tgt
                };

                await _characters.AddCharacterAsync(character);

                Dispatcher.Invoke(() => { CloseTriggered?.Invoke(this, EventArgs.Empty); });
            });
            return;
        }

        frameworkElement.IsEnabled = false;

        var characterId = CharacterId.Text;


        _ = Task.Run(async () =>
        {
            var characterData = await _characterAiApi.GetCharacterDataFromId(characterId);

            Dispatcher.Invoke(() =>
            {
                Character = characterData;

                CharacterImage.ImageUrl = characterData.AvatarUrlMini;

                frameworkElement.IsEnabled = true;

                CharacterInfo.Visibility = Visibility.Visible;

                SearchText.Text = "Save";

                SearchProgressBar.Visibility = Visibility.Collapsed;
            });
        });
    }

    private void AtarashiCharacterAiLoaded(object sender, RoutedEventArgs e)
    {
        CharacterInfo.Visibility = Visibility.Collapsed;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}