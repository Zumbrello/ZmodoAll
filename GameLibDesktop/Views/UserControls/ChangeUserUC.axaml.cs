using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GameLibDesktop.Models;


namespace GameLibDesktop.Views.UserControls;

public partial class ChangeUserUC : UserControl
{
    private User currentUser;
    public ChangeUserUC()
    {
        InitializeComponent();
    }

    private void DeleteUser_OnClick(object? sender, RoutedEventArgs e)
    {
        Program.wc.Headers.Clear();
        Program.wc.Headers.Add("Authorization", "Bearer " + MainWindow.GetInstance().config.AppSettings.Settings["AccessToken"].Value);
        try
        {
            var resultUsers = Program.wc.UploadString(
                Program.HostAdress + $"/api/ForAdmin/DeleteUser?" +
                $"id_user={currentUser.Id}", "DELETE", "");
            
            StatusTB.Text = "Пользователь успешно удалён";
        }
        catch (Exception ex)
        {
            StatusTB.Text = "Ошибка удаления пользователя";
        }
    }

    private void ChangeUser_OnClick(object? sender, RoutedEventArgs e)
    {
        Program.wc.Headers.Clear();
        Program.wc.Headers.Add("Authorization", "Bearer " + MainWindow.GetInstance().config.AppSettings.Settings["AccessToken"].Value);
        try
        {
            var resultUsers = Program.wc.UploadString(
                Program.HostAdress + $"/api/ForAllUser/ChangeUser?" +
                $"login={LoginTB.Text}" +
                $"&email={EmailTB.Text}" +
                $"&password={System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(PasswordTB.Text))}" +
                $"&id={currentUser.Id}", "PUT",
                "");
            StatusTB.Text = "Данные успешно обновлены";
        }
        catch (Exception ex)
        {
            StatusTB.Text = "Ошибка обновления данных";
        }
    }

    private void FindUser_OnClick(object? sender, RoutedEventArgs e)
    {
        Program.wc.Headers.Clear();
        Program.wc.Headers.Add("Authorization", "Bearer " + MainWindow.GetInstance().config.AppSettings.Settings["AccessToken"].Value);
        var resultUsers = Program.wc.DownloadString(Program.HostAdress + "/api/ForAdmin/GetUsers");
        var usersList = JsonSerializer.Deserialize<List<User>>(resultUsers);

        currentUser = usersList.Where(u => u.Login == LoginTB.Text).FirstOrDefault();
        if (currentUser == null)
        {
            StatusTB.Text = "Нет пользователя с таким логином!";
            EmailTB.Text = "";
            PasswordTB.Text = "";
            
            HideStatus();
            return;
        }
        else
        {
            StatusTB.Text = "Пользователь найден !";
        }

        EmailTB.Text = currentUser.Email;
        PasswordTB.Text = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(currentUser.Password));
        
        HideStatus();
    }
    
    private async void HideStatus()
    {
        await Task.Delay(5000);
        StatusTB.Text = "";
    }
}