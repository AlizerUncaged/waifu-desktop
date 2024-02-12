using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Waifu.Data;
using Waifu.Models;
using Waifu.Utilities;

namespace Waifu.Views.Shared.Popups;

public partial class CharacterEdit : UserControl, IPopup
{
    private readonly ElevenlabsVoiceGenerator _elevenlabsVoiceGenerator;

    public CharacterEdit(ElevenlabsVoiceGenerator elevenlabsVoiceGenerator)
    {
        _elevenlabsVoiceGenerator = elevenlabsVoiceGenerator;
        InitializeComponent();
    }

    public ObservableCollection<string> ElevenlabsVoice { get; set; } = new();

    #region Character Object Property

    public RoleplayCharacter RoleplayCharacter
    {
        get { return (RoleplayCharacter)GetValue(RoleplayCharacterProperty); }
        set { SetValue(RoleplayCharacterProperty, value); }
    }

    public static readonly DependencyProperty RoleplayCharacterProperty =
        DependencyProperty.Register(nameof(RoleplayCharacter), typeof(RoleplayCharacter), typeof(CharacterEdit),
            new FrameworkPropertyMetadata(null)
            {
            });

    #endregion


    public event EventHandler<RoleplayCharacter> RoleplayCharacterChanged;


    private void SaveCharacter(object sender, RoutedEventArgs e)
    {
        RoleplayCharacterChanged?.Invoke(this, RoleplayCharacter);
        CloseTriggered?.Invoke(this, EventArgs.Empty);
    }

    private void CharacterEditLoaded(object sender, RoutedEventArgs e)
    {
        _ = RefreshVoices();
    }

    public async Task RefreshVoices(bool force = false)
    {
        Dispatcher.Invoke(() => { SearchProgressBar.Visibility = Visibility.Visible; });
        IEnumerable<string> voices;


        var voicesRaw = _elevenlabsVoiceGenerator.VoicesCache;

        if (voicesRaw is null || force)
        {
            voices = await _elevenlabsVoiceGenerator.GetElevenlabsVoices();
        }
        else voices = voicesRaw.Select(x => x.Name);


        Dispatcher.Invoke(() =>
        {
            foreach (var voice in voices)
            {
                ElevenlabsVoice.Add(voice);
            }

            VoiceList.ItemsSource = ElevenlabsVoice;

            if (!string.IsNullOrWhiteSpace(RoleplayCharacter.ElevenlabsSelectedVoice))
            {
                VoiceList.SelectedIndex = ElevenlabsVoice.IndexOf(RoleplayCharacter.ElevenlabsSelectedVoice);
            }

            // VoiceList.Text = string.IsNullOrWhiteSpace()
            //     ? voices.FirstOrDefault()
            //     : RoleplayCharacter.ElevenlabsSelectedVoice;


            SearchProgressBar.Visibility = Visibility.Collapsed;
        });
    }

    public event EventHandler? CloseTriggered;
    public event EventHandler<FrameworkElement>? ReplaceTriggered;

    private void VoiceSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
   
    }

    private void VoiceSelectionClicked(object sender, MouseButtonEventArgs e)
    {
    }

    private void CancelClicked(object sender, RoutedEventArgs e)
    {
        CloseTriggered?.Invoke(this, EventArgs.Empty);
    }

    private void RefreshVoicesClicked(object sender, RoutedEventArgs e)
    {
        _ = RefreshVoices(true);
    }

    private void RealSelectionChanged(object? sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(VoiceList.Text))
            RoleplayCharacter.ElevenlabsSelectedVoice = VoiceList.Text;
    }
}