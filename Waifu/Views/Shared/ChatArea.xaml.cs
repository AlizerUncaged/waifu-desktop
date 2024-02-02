using System.Windows.Controls;
using Waifu.Models;

namespace Waifu.Views.Shared;

public partial class ChatArea : UserControl
{
    private readonly RoleplayCharacter _character;

    public ChatArea(RoleplayCharacter character)
    {
        _character = character;
        
        InitializeComponent();
    }

    public RoleplayCharacter RoleplayCharacter => _character;
}