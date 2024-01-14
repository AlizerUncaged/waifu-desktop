using System.Windows;
using System.Windows.Controls;

namespace Waifu.Views.Shared;

public partial class CharacterItem : UserControl
{
    public CharacterItem()
    {
        InitializeComponent();
    }

    #region Character Name Property

    public String CharacterName
    {
        get { return (String)GetValue(CharacterNameProperty); }
        set { SetValue(CharacterNameProperty, value); }
    }

    public static readonly DependencyProperty CharacterNameProperty =
        DependencyProperty.Register(nameof(CharacterName), typeof(String), typeof(CharacterItem),
            new FrameworkPropertyMetadata(null)
            {
                //  It's read-write, so make it bind both ways by default
                BindsTwoWayByDefault = true
            });

    #endregion
}