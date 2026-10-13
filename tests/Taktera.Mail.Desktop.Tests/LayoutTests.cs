using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Taktera.Mail.Desktop;
using Taktera.Mail.Desktop.Models;
using Taktera.Mail.Desktop.ViewModels;
using Xunit;

[assembly: AvaloniaTestApplication(typeof(Taktera.Mail.Desktop.Tests.TestAppBuilder))]

namespace Taktera.Mail.Desktop.Tests;

public static class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}

public sealed class LayoutTests
{
    [AvaloniaTheory]
    [InlineData(true, 850, 520)]
    [InlineData(false, 850, 520)]
    [InlineData(true, 1220, 780)]
    [InlineData(false, 1220, 780)]
    public void EmptyStatesHideReadingActionsAndRecover(bool clearInbox, int width, int height)
    {
        var vm = new MainWindowViewModel();
        var original = vm.Messages[0];
        var window = new MainWindow { Width = width, Height = height, DataContext = vm };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            if (clearInbox)
                vm.Messages.Clear();
            else
                vm.SelectedMessage = null;

            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.Null(vm.SelectedMessage);
            var emptyInbox = window.FindControl<TextBlock>("EmptyInbox")!;
            var emptySelection = window.FindControl<TextBlock>("EmptySelection")!;
            var reader = window.FindControl<ScrollViewer>("MessageScroll")!;
            Assert.Equal(clearInbox, emptyInbox.IsVisible);
            Assert.True(emptySelection.IsEffectivelyVisible);
            AssertInsideWindow(emptySelection, window);
            if (clearInbox)
                AssertInsideWindow(emptyInbox, window);
            Assert.False(reader.IsEffectivelyVisible);
            Assert.DoesNotContain(reader.GetVisualDescendants().OfType<Button>(), b => b.IsEffectivelyVisible);
            Assert.Equal(clearInbox ? "▣  Входящие · 0" : "▣  Входящие · 3",
                window.FindControl<TextBlock>("InboxCount")!.Text);

            if (clearInbox)
                vm.Messages.Add(original);
            var list = Assert.Single(window.GetVisualDescendants().OfType<ListBox>());
            list.SelectedItem = original;
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.Same(original, vm.SelectedMessage);
            Assert.False(emptyInbox.IsVisible);
            Assert.False(emptySelection.IsVisible);
            Assert.True(reader.IsEffectivelyVisible);
            Assert.Contains(reader.GetVisualDescendants().OfType<TextBlock>(), t => t.Text == original.Body);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void RemovingSelectedMessageBeforeOpeningWindowClearsSelection()
    {
        var vm = new MainWindowViewModel();
        vm.Messages.Remove(vm.SelectedMessage!);
        Assert.Null(vm.SelectedMessage);
        Assert.False(vm.HasSelectedMessage);
        Assert.False(vm.IsInboxEmpty);
        vm.Messages.Clear();
        Assert.True(vm.IsInboxEmpty);
    }

    [AvaloniaFact]
    public void SelectingSyntheticMessageUpdatesReadingPane()
    {
        var vm = new MainWindowViewModel();
        var window = new MainWindow { DataContext = vm };
        try
        {
            window.Show();
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();
            var list = Assert.Single(window.GetVisualDescendants().OfType<ListBox>());
            Assert.Equal(3, list.ItemCount);
            Assert.Same(vm.Messages[0], list.SelectedItem);

            list.SelectedItem = vm.Messages[1];
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Assert.Same(vm.Messages[1], vm.SelectedMessage);
            var reader = window.FindControl<ScrollViewer>("MessageScroll")!;
            var texts = reader.GetVisualDescendants().OfType<TextBlock>().Select(t => t.Text).ToArray();
            Assert.Contains(vm.Messages[1].Subject, texts);
            Assert.Contains(vm.Messages[1].Body, texts);
            Assert.DoesNotContain(vm.Messages[0].Body, texts);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaTheory]
    [InlineData(850, 520)]
    [InlineData(1220, 780)]
    public void LongMessageScrollsAndControlsStayInsideWindow(int width, int height)
    {
        var vm = new MainWindowViewModel();
        var longMessage = new MailItem(new string('S', 300), new string('T', 300), "Preview", "10:24",
            string.Join("\n", Enumerable.Repeat(new string('B', 200), 100)));
        vm.Messages.Clear();
        vm.Messages.Add(longMessage);
        vm.SelectedMessage = longMessage;
        var window = new MainWindow { Width = width, Height = height, DataContext = vm };
        try
        {
            window.Show();
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();

            var scroll = window.FindControl<ScrollViewer>("MessageScroll")!;
            Assert.True(scroll.Viewport.Width > 200);
            Assert.True(scroll.Extent.Height > scroll.Viewport.Height);
            Assert.True(scroll.Extent.Width <= scroll.Viewport.Width + 1);
            scroll.ScrollToEnd();
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();
            Assert.True(scroll.Offset.Y > 0);

            var search = window.FindControl<TextBox>("SearchBox")!;
            Assert.True(search.Bounds.Width >= 120);
            AssertInsideWindow(search, window);
            var buttons = window.GetVisualDescendants().OfType<Button>()
                .Where(b => b.Content is string text && (text == "Ответить" || text == "Переслать"))
                .ToArray();
            Assert.Equal(2, buttons.Length);
            foreach (var button in buttons)
                AssertInsideWindow(button, window);

            var list = Assert.Single(window.GetVisualDescendants().OfType<ListBox>());
            var item = Assert.Single(list.GetVisualDescendants().OfType<ListBoxItem>());
            Assert.True(item.Bounds.Width <= list.Bounds.Width);
            foreach (var text in item.GetVisualDescendants().OfType<TextBlock>())
                AssertInsideWindow(text, window);
        }
        finally
        {
            window.Close();
        }
    }

    private static void AssertInsideWindow(Control control, Window window)
    {
        var position = control.TranslatePoint(default, window);
        Assert.NotNull(position);
        Assert.True(position.Value.X >= 0 && position.Value.Y >= 0);
        Assert.True(position.Value.X + control.Bounds.Width <= window.Bounds.Width + 1);
        Assert.True(position.Value.Y + control.Bounds.Height <= window.Bounds.Height + 1);
    }
}
