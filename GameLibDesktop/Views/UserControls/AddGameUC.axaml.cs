using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GameLibDesktop.Models;
using GamesLibAPI.HelpersClasses;

namespace GameLibDesktop.Views.UserControls;

public partial class AddGameUC : UserControl
{
    private int _mode;
    private GameCard _game;
    private List<Developer> _developerList;
    private List<Publisher> _publisherList;

    public AddGameUC(int mode, GameCard game = null)
    {
        InitializeComponent();
        _mode = mode;
        TitleTB = this.FindControl<TextBox>("TitleTB");
        DescriptionTB = this.FindControl<TextBox>("DescriptionTB");
        ImageUrlTB = this.FindControl<TextBox>("ImageUrlTB");
        ReleaseDateTB = this.FindControl<TextBox>("ReleaseDateTB");
        MinReqTB = this.FindControl<TextBox>("MinReqTB");
        MaxReqTB = this.FindControl<TextBox>("MaxReqTB");
        DeveloperCB = this.FindControl<ComboBox>("DeveloperCB");
        PublisherCB = this.FindControl<ComboBox>("PublisherCB");
        AddGameBtn = this.FindControl<Button>("AddGameBtn");
        ChangeGameBtn = this.FindControl<Button>("ChangeGameBtn");
        DeleteGameBtn = this.FindControl<Button>("DeleteGameBtn");
        
        Program.wc.Headers.Clear();
        Program.wc.Headers.Add("Authorization", "Bearer " + MainWindow.GetInstance().config.AppSettings.Settings["AccessToken"].Value);
        _developerList = JsonSerializer.Deserialize<List<Developer>>(Program.wc.DownloadString(Program.HostAdress + "/api/ForAllUser/GetDevelopers"));
        _publisherList = JsonSerializer.Deserialize<List<Publisher>>(Program.wc.DownloadString(Program.HostAdress + "/api/ForAllUser/GetPublishers"));
        
        ReFillLists();
        
        // если mod == 1, идет изменение игры. если mod == 0, то добавление игры
        if (mode == 1)
        {
            TitleTB.Text = game.GameName;
            DescriptionTB.Text = game.Description;
            ImageUrlTB.Text = game.ImageURl;
            ReleaseDateTB.Text = game.ReleaseDate;
            MinReqTB.Text = game.SystemRequestMin;
            MaxReqTB.Text = game.SystemRequestRec;
            DeveloperCB.SelectedItem = game.Developer;
            PublisherCB.SelectedItem = game.Publisher;
            AddGameBtn.IsVisible = false;
            ChangeGameBtn.IsVisible = true;
            DeleteGameBtn.IsVisible = true;
            DeveloperCB.SelectedItem = game.Developer.Replace("Разработчик: ", "");
            PublisherCB.SelectedItem = game.Publisher.Replace("Издатель: ", "");
            _game = game;
        }
        else
        {
            AddGameBtn.IsVisible = true;
            ChangeGameBtn.IsVisible = false;
            DeleteGameBtn.IsVisible = false;

            DeveloperCB.SelectedIndex = 0;
            PublisherCB.SelectedIndex = 0;
        }
    }

    private void DeleteGame_OnClick(object? sender, RoutedEventArgs e)
    {
        Program.wc.Headers.Clear();
        Program.wc.Headers.Add("Authorization", "Bearer " + MainWindow.GetInstance().config.AppSettings.Settings["AccessToken"].Value);
        try
        {
            Program.wc.UploadString(Program.HostAdress + $"/api/ForAdmin/DeleteGame?id_game={_game.Id}", "DELETE",
                "");
            MainWindow.GetInstance().ContentCC.Content = new GamesListUC();
        }
        catch (Exception ex)
        {
            StatusText.Text = "Ошибка при удалении игры";
        }
        
    }

    private void ChangeGame_OnClick(object? sender, RoutedEventArgs e)
    {
        Program.wc.Headers.Clear();
        Program.wc.Headers.Add("Authorization", "Bearer " + MainWindow.GetInstance().config.AppSettings.Settings["AccessToken"].Value);
        GameEdit newChangeGame = new GameEdit();

        newChangeGame.Id = _game.Id;
        newChangeGame.GameName = TitleTB.Text;
        newChangeGame.IdDeveloper = _developerList.Where(d => d.Developer1 == DeveloperCB.SelectedItem.ToString()).First().Id;
        newChangeGame.IdPublisher = _publisherList.Where(p => p.Publisher1 == PublisherCB.SelectedItem.ToString()).First().Id;
        newChangeGame.Description = DescriptionTB.Text;
        newChangeGame.MainImage = ImageUrlTB.Text;
        newChangeGame.ReleaseDate = ReleaseDateTB.Text;
        newChangeGame.SystemRequestMin = MinReqTB.Text;
        newChangeGame.SystemRequestRec = MaxReqTB.Text;
        
        Program.wc.Headers[HttpRequestHeader.ContentType] = "application/json";
        try
        {
            Program.wc.UploadString(Program.HostAdress + "/api/ForAdmin/EditGame", "POST",
                JsonSerializer.Serialize(newChangeGame));
            StatusText.Text = "Игра успешно редактирована";
        }
        catch (Exception ex)
        {
            StatusText.Text = "Ошибка при редактировании игры";
        }
        MainWindow.GetInstance().ContentCC.Content = new GamesListUC();
    }

    private void AddGame_OnClick(object? sender, RoutedEventArgs e)
    {
        Program.wc.Headers.Clear();
        Program.wc.Headers.Add("Authorization", "Bearer " + MainWindow.GetInstance().config.AppSettings.Settings["AccessToken"].Value);
        GameAdd newGame = new GameAdd();
        
        newGame.Name = TitleTB.Text;
        newGame.IdDeveloper = _developerList.Where(d => d.Developer1 == DeveloperCB.SelectedItem.ToString()).First().Id;
        newGame.IdPublisher = _publisherList.Where(p => p.Publisher1 == PublisherCB.SelectedItem.ToString()).First().Id;
        newGame.Description = DescriptionTB.Text;
        newGame.MainImage = ImageUrlTB.Text;
        newGame.ReleaseDate = ReleaseDateTB.Text;
        newGame.SystemRequestMin = MinReqTB.Text;
        newGame.SystemRequestRec = MaxReqTB.Text;
        
        Program.wc.Headers[HttpRequestHeader.ContentType] = "application/json";
        try
        {
            Program.wc.UploadString(Program.HostAdress + "/api/ForAdmin/AddGame", "POST",
                JsonSerializer.Serialize(newGame));
            StatusText.Text = "Игра успешно добавлена";
        }
        catch (Exception ex)
        {
            StatusText.Text = "Ошибка при добавлении игры";
        }
        
        MainWindow.GetInstance().ContentCC.Content = new GamesListUC();
    }
    
    private void ReFillLists()
    {
        Program.wc.Headers.Clear();
        Program.wc.Headers.Add("Authorization", "Bearer " + MainWindow.GetInstance().config.AppSettings.Settings["AccessToken"].Value);
        _publisherList = new List<Publisher>();
        _developerList = new List<Developer>();
        
        var resultDevelopers = Program.wc.DownloadString( Program.HostAdress + "/api/ForAllUser/GetDevelopers");
        _developerList = JsonSerializer.Deserialize<List<Developer>>(resultDevelopers);
            
        var resultPublishers = Program.wc.DownloadString(Program.HostAdress + "/api/ForAllUser/GetPublishers");
        _publisherList = JsonSerializer.Deserialize<List<Publisher>>(resultPublishers);

        PublisherCB.ItemsSource = _publisherList.Select(p => p.Publisher1).ToList();
        DeveloperCB.ItemsSource = _developerList.Select(d => d.Developer1).ToList();
    }
}