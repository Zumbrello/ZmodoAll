using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GameLibDesktop.Models;


namespace GameLibDesktop.Views.UserControls;

public partial class AuthUC : UserControl
{
    public AuthUC()
    {
        InitializeComponent();

        LoginTB = this.FindControl<TextBox>("LoginTB");
        PasswordTB = this.FindControl<TextBox>("PasswordTB");
        StatusTB = this.FindControl<TextBlock>("StatusTB");
        
        LoginTB.Text = "Zombus";
        PasswordTB.Text = "12";
        StatusTB.Text = "";
        //DataContext = AuthVM.GetInstance();
    }

    private void EnterBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        string Token;
        string url = Program.HostAdress + "/api/ForAllUser/Login?login=" + LoginTB.Text + "&password=" + PasswordTB.Text;

        JsonDocument request;
        try
        {
            request = JsonDocument.Parse(Program.wc.UploadString(url, "POST", ""));
        }
        catch (Exception ex)
        {
            StatusTB.Text = "Неверный логин или пароль";
            return;
        }

        Token = Convert.ToString(request.RootElement.GetProperty("accessToken").ToString());
        Program.wc.Headers.Clear();
        Program.wc.Headers.Add("Authorization", "Bearer " + Token);
        User CurrentUser = JsonSerializer.Deserialize<List<User>>(JsonDocument
            .Parse(Program.wc.DownloadString(
                    Program.HostAdress + "/api/ForAdmin/GetUsers")
            ))
            .Where(u => u.Login == LoginTB.Text && u.Password == Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(PasswordTB.Text)))
            .FirstOrDefault();
        
        //Проверка сохранённых данных входа
        if (MainWindow.GetInstance().config.AppSettings.Settings["AccessToken"]?.Value == null)
        {
            MainWindow.GetInstance().config.AppSettings.Settings.Add("AccessToken", request.RootElement.GetProperty("accessToken").ToString()); 
            MainWindow.GetInstance().config.AppSettings.Settings.Add("RefreshToken", request.RootElement.GetProperty("refreshToken").ToString());
            MainWindow.GetInstance().config.Save();
        }
        else
        {
            MainWindow.GetInstance().config.AppSettings.Settings.Remove("AccessToken");
            MainWindow.GetInstance().config.AppSettings.Settings.Remove("RefreshToken");
            MainWindow.GetInstance().config.AppSettings.Settings.Add("AccessToken", request.RootElement.GetProperty("accessToken").ToString());
            MainWindow.GetInstance().config.AppSettings.Settings.Add("RefreshToken", request.RootElement.GetProperty("refreshToken").ToString());
            MainWindow.GetInstance().config.Save();
        }
        
        Program.timer.Start();
        MainWindow.GetInstance().MenuCC.IsVisible = true;
        MainWindow.GetInstance().MenuCC.Content = new MenuUC();
        MainWindow.GetInstance().ContentCC.Content = new GamesListUC();
    }
}