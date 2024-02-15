using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using GameLibDesktop.Models;


namespace GameLibDesktop.Views.UserControls;

public partial class GamesListUC : UserControl
{
    private int _mode;
    private GameCard _game;
    private List<Developer> _developerList;
    private List<Publisher> _publisherList;
    
    public GamesListUC()
    {
        InitializeComponent();
        
        Program.wc.Headers.Clear();
        Program.wc.Headers.Add("Authorization", "Bearer " + MainWindow.GetInstance().config.AppSettings.Settings["AccessToken"].Value);
        
        _developerList = JsonSerializer.Deserialize<List<Developer>>(Program.wc.DownloadString(Program.HostAdress + "/api/ForAllUser/GetDevelopers"));
        _publisherList = JsonSerializer.Deserialize<List<Publisher>>(Program.wc.DownloadString(Program.HostAdress + "/api/ForAllUser/GetPublishers"));
        List<string> developersCBList = new List<string>();
        developersCBList.Add("Все разработчики");
        developersCBList.AddRange(_developerList.Select(d => d.Developer1).ToList());
        
        List<string> publishersCBList = new List<string>();
        publishersCBList.Add("Все издатели");
        publishersCBList.AddRange(_publisherList.Select(d => d.Publisher1).ToList());
        
        DeveloperCB.ItemsSource = developersCBList;
        PublisherCB.ItemsSource = publishersCBList;
        
        FillList();
        
        DeveloperCB.SelectedIndex = 0;
        PublisherCB.SelectedIndex = 0;
        //DataContext = GamesListVM.GetInstance();
    }

    private void FillList()
    {
        try
        {
            Program.wc.Headers.Clear();
            Program.wc.Headers.Add("Authorization", "Bearer " + MainWindow.GetInstance().config.AppSettings.Settings["AccessToken"].Value);
            var resultGames = Program.wc.DownloadString(Program.HostAdress + "/api/ForAllUser/GetGame");
            var gamesList = JsonSerializer.Deserialize<List<Game>>(resultGames);

            List<GameCard> gameCardsList = new List<GameCard>();
            
            if (PublisherCB.SelectedItem != null && PublisherCB.SelectedIndex != 0)
            {
                gamesList = gamesList.Where(g =>
                    g.IdPublisher == _publisherList.Where(p => p.Publisher1 == PublisherCB.SelectedItem.ToString()).First().Id).ToList();
            }
            
            if (DeveloperCB.SelectedItem != null && DeveloperCB.SelectedIndex != 0)
            {
                gamesList = gamesList.Where(g =>
                    g.IdDeveloper == _developerList.Where(p => p.Developer1 == DeveloperCB.SelectedItem.ToString()).First().Id).ToList();
            }
            
            if (SearchTB.Text != "" && SearchTB.Text != null)
            {
                gamesList = gamesList.Where(g => g.GameName.ToLower().Contains(SearchTB.Text.ToLower())).ToList();
            }
            
            foreach (var game in gamesList)
            {
                try
                {
                    gameCardsList.Add(new GameCard()
                    {

                        GameName = game.GameName, 
                        Description = game.Description,
                        Developer = "Разработчик: " +
                                    _developerList.Where(p => p.Id == game.IdDeveloper).First().Developer1,
                        Publisher = "Издатель: " +
                                    _publisherList.Where(p => p.Id == game.IdPublisher).First().Publisher1,
                        ImageURl = game.MainImage,
                        MainImage = new Bitmap(new MemoryStream(Program.wc.DownloadData(game.MainImage))),
                        ReleaseDate = game.ReleaseDate,
                        SystemRequestMin = game.SystemRequestMin, 
                        SystemRequestRec = game.SystemRequestRec,
                        Id = game.Id
                    });
                }
                catch (Exception e)
                {
                    gameCardsList.Add(new GameCard()
                    {

                        GameName = game.GameName, 
                        Description = game.Description,
                        Developer = "Разработчик: " +
                                    _developerList.Where(p => p.Id == game.IdDeveloper).First().Developer1,
                        Publisher = "Издатель: " +
                                    _publisherList.Where(p => p.Id == game.IdPublisher).First().Publisher1,
                        ImageURl = "https://нормебель.рф/image/cache/no_image-2000x1500_0.jpg",
                        MainImage = new Bitmap(new MemoryStream(Program.wc.DownloadData("https://нормебель.рф/image/cache/no_image-2000x1500_0.jpg"))),
                        ReleaseDate = game.ReleaseDate,
                        SystemRequestMin = game.SystemRequestMin, 
                        SystemRequestRec = game.SystemRequestRec,
                        Id = game.Id
                    });
                }
            }
            
            GamesList.ItemsSource = gameCardsList;

        }
        catch (Exception ex)
        {
            GamesList.ItemsSource = new ObservableCollection<GameCard>();
        }
    }
    
    private void SearchTB_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        FillList();
    }

    private void DeveloperCB_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        FillList();
    }

    private void PublisherCB_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
       FillList();
    }

    private void GamesList_OnTapped(object? sender, TappedEventArgs e)
    {
        MainWindow.GetInstance().ContentCC.Content = new AddGameUC(1, (sender as ListBox).SelectedItem as GameCard);
    }
}