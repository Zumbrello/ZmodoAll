using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace GameLibDesktop.Views.UserControls;

public partial class MenuUC : UserControl
{
    public MenuUC()
    {
        InitializeComponent();
    }

    private void Catalog_OnClick(object? sender, RoutedEventArgs e)
    {
        MainWindow.GetInstance().ContentCC.Content = new GamesListUC();
    }

    private void Users_OnClick(object? sender, RoutedEventArgs e)
    {
        MainWindow.GetInstance().ContentCC.Content = new ChangeUserUC();
    }

    private void AddGame_OnClick(object? sender, RoutedEventArgs e)
    {
        MainWindow.GetInstance().ContentCC.Content = new AddGameUC(0);
    }

    private void Logout_OnClick(object? sender, RoutedEventArgs e)
    {
        MainWindow.GetInstance().ContentCC.Content = new AuthUC();
        MainWindow.GetInstance().MenuCC.IsVisible = false;
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }
}