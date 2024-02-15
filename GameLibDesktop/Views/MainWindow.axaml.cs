using System;
using System.Configuration;
using Avalonia.Controls;
using GameLibDesktop.Views.UserControls;

namespace GameLibDesktop.Views;

public partial class MainWindow : Window
{
    private static MainWindow _instance;
    public Configuration config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.BaseDirectory + "GameLibDesktop");

    public static MainWindow GetInstance()
    {
        if (_instance == null)
        {
            _instance = new MainWindow();
        }
        return _instance;
    }
    private MainWindow()
    {
        InitializeComponent();
        ContentCC = this.FindControl<ContentControl>("ContentCC");
        MenuCC = this.FindControl<ContentControl>("MenuCC");

        MenuCC.Content = new MenuUC();
        ContentCC.Content = new AuthUC();
        MenuCC.IsVisible = false;
    }
}