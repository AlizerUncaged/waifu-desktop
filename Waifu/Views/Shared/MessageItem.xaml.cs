using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MdXaml;
using Waifu.Models;

namespace Waifu.Views.Shared;

public partial class MessageItem : UserControl
{
    public MessageItem()
    {
        InitializeComponent();
    }

    public void AddMessageContent(string partialMessage)
    {
        MessageContent += partialMessage;
    }

    public String MessageContent
    {
        get { return (String)GetValue(MessageContentProperty); }
        set { SetValue(MessageContentProperty, value); }
    }

    public static readonly DependencyProperty MessageContentProperty =
        DependencyProperty.Register(nameof(MessageContent), typeof(String), typeof(MessageItem),
            new FrameworkPropertyMetadata(null)
            {
                //  It's read-write, so make it bind both ways by default
                BindsTwoWayByDefault = true
            });


    public ChatMessage ChatMessage
    {
        get { return (ChatMessage)GetValue(ChatMessageProperty); }
        set { SetValue(ChatMessageProperty, value); }
    }

    public static readonly DependencyProperty ChatMessageProperty =
        DependencyProperty.Register(nameof(ChatMessage), typeof(ChatMessage), typeof(MessageItem));

    private Markdown _engine = new();

    private void MessageItemLoaded(object sender, RoutedEventArgs e)
    {
        if (ChatMessage.SentByUser) SetToRight();

        MarkdownView.Markdown = MessageContent;
        MarkdownView.UpdateLayout();

        this.UpdateLayout();

        if (FindResource("EntranceAnimation") is Storyboard storyboard &&
            storyboard.Children.OfType<DoubleAnimation>().FirstOrDefault() is { } doubleAnimation)
        {
            doubleAnimation.To = MessageGrid.ActualHeight + MarkdownView.ActualHeight;

            storyboard.Begin();
        }
    }

    public void SetToRight()
    {
        Grid.SetColumn(ChatComponent, 1);
        Grid.SetColumn(Extra, 0);

        ChatParent.Children.Remove(Extra);
        ChatParent.Children.Insert(0, Extra);
        MainBorder.HorizontalAlignment = HorizontalAlignment.Right;
    }

    private void ItemClicked(object sender, MouseButtonEventArgs e)
    {
        ;
    }

    private void MessageGridLoaded(object sender, RoutedEventArgs e)
    {
    }
}