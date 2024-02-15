using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using GamesLibAPI.Context;
using GamesLibAPI.HelpersClasses;
using GamesLibAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamesLibAPI.JwtHelpers;
using GamesLibAPI.Models;

namespace GameLib.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]

public class ForAdminController : ControllerBase
{
    //Подключение к БД
    private GtcymkznContext Context = new GtcymkznContext();

    //Конструктор контроллера
    public ForAdminController() 
    {}
    
    //Вывод всех пользователей
    [HttpGet("GetUsers")]
    [Authorize]
    public ActionResult GetDataApi()
    {
        string token = Request.Headers.Authorization.ToString().Substring(7);
        if (!IsAdmin(token))
        {
            return BadRequest("Доступно только администраторам");
        }
        //вывод данных в api
        return Ok(JsonSerializer.Serialize(Context.Users.ToList()));
    }
    
    //Удаление пользователя
    [HttpDelete("DeleteUser")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(int id_user)
    {
        string token = Request.Headers.Authorization.ToString().Substring(7);
        if (!IsAdmin(token))
        {
            return BadRequest("Доступно только администраторам");
        }
        var user = Context.Users.Where(u => u.Id == id_user).Include(u => u.UserJwts).FirstOrDefault();  //находим пользователя по id

        if (user == null)
        {
            return NotFound("User not found"); // Если пользователь с указанным id не найден, возвращаем 404 Not Found
        }
        
        if (user.UserRole == 1)
        {
            return BadRequest("Ты не можешь удалять других администраторов!");
        }

	try{
	    var jwtRecs = Context.UserJwts.Where(j => j.User == user.Id).ToList();
            Context.RemoveRange(jwtRecs);
            Context.Users.Remove(user); //удаляем пользователя
            Context.SaveChanges(); //сохраняем
	}
	catch(Exception ex){
		return BadRequest("Ошибка удаления пользователя");
	}

        return Ok("User deleted"); 
    }
    
    //Удаление игры
    [HttpDelete("DeleteGame")]
    [Authorize]
    public async Task<IActionResult> DeliteGame(int id_game)
    {
        string token = Request.Headers.Authorization.ToString().Substring(7);
        if (!IsAdmin(token))
        {
            return BadRequest("Доступно только администраторам");
        }
        
        //Находим игру по id
        var game = await Context.Games.FindAsync(id_game); 
        if (game == null)
        {
            return NotFound("Игра не найдена");
        }

        List<GameGenre> gameGenresList = Context.GameGenres.Where(g => g.IdGame == id_game).ToList();

        if (gameGenresList.Count != 0)
        {
            Context.GameGenres.RemoveRange(gameGenresList);    
        }

        Context.SaveChanges();
        
        Context.Games.Remove(game);
        await Context.SaveChangesAsync();
        return Ok("Игра удалена");
    }
    
    //Добавление игры
    [HttpPost("AddGame")]
    [Authorize]
    public async Task<ActionResult> AddGame([FromBody] GameAdd newGameadd)
    {
        string token = Request.Headers.Authorization.ToString().Substring(7);
        if (!IsAdmin(token))
        {
            return BadRequest("Доступно только администраторам");
        }
        int idUser = JwtHelpers.GetUserIdByToken(token);

        User user = Context.Users.Where(u => u.Id == idUser).FirstOrDefault();
        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }

        //Проверка дублирования записей
        Game game = Context.Games.Where(g => g.GameName == newGameadd.Name).FirstOrDefault();
        if (game != null)
        {
            return BadRequest("Игра с таким название уже существует");
        }

        try
        {
            // Создаем новый объект игры
            var newGame = new Game
            {
                GameName = newGameadd.Name,
                Description = newGameadd.Description,
                IdDeveloper = newGameadd.IdDeveloper,
                IdPublisher = newGameadd.IdPublisher,
                SystemRequestMin = newGameadd.SystemRequestMin,
                SystemRequestRec = newGameadd.SystemRequestRec,
                ReleaseDate = newGameadd.ReleaseDate,
                MainImage = newGameadd.MainImage
            };

            // Добавляем новую игру в базу данных
            Context.Games.Add(newGame);
            await Context.SaveChangesAsync();

            return Ok("Game added successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    //Изменение игры
    [HttpPost("EditGame")]
    [Authorize]
    public async Task<ActionResult> EditGame([FromBody] GameEdit editGame)
    {
        string token = Request.Headers.Authorization.ToString().Substring(7);
        if (!IsAdmin(token))
        {
            return BadRequest("Доступно только администраторам");
        }

        var game = Context.Games.Where(g => g.Id == editGame.Id).FirstOrDefault();

        //Проверяем, что игра существует
        if (game == null)
        {
            return NotFound("Игра не найдена");
        }

        try
        {
            //Изменяем данные об игре
            game.Description = editGame.Description;
            game.IdPublisher = editGame.IdPublisher;
            game.IdDeveloper = editGame.IdDeveloper;
            game.GameName = editGame.GameName;
            game.MainImage = editGame.MainImage;
            game.ReleaseDate = editGame.ReleaseDate;
            game.SystemRequestMin = editGame.SystemRequestMin;
            game.SystemRequestRec = editGame.SystemRequestRec;
        }
        catch (Exception e)
        {
            return BadRequest("Ошибка при выполнении запроса");
        }
        
        await Context.SaveChangesAsync();

        return Ok("Game edited successfully");

    }

    //Проверка на админа
    private bool IsAdmin(string token)
    {
        User user = new GtcymkznContext().Users.Where(u => u.Id == JwtHelpers.GetUserIdByToken(token)).FirstOrDefault();

        if (user == null)
        {
            return false;
        }
        
        return user.UserRole == 1;
    }
}