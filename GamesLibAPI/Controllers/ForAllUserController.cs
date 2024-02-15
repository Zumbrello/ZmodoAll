using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GamesLibAPI.JwtHelpers;
using GamesLibAPI.Models;
using System.Text.Json.Serialization;
using GamesLibAPI.Context;
using GamesLibAPI.Models;

namespace GamesLibAPI.Controllers;
[ApiController]
[Route("api/[controller]")]

public class ForAllUserController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private int TokenTimeMinute = 5;
    private DateTime _tokenCreationTime;
    
    //Установка настроек Jwt
    private readonly JwtSettings jwtSettings;
    public ForAllUserController(JwtSettings jwtSettings, IConfiguration configuration) {
        this.jwtSettings = jwtSettings;
        _configuration = configuration;
    }

    //Данные о пользовательском аккаунте, для телефонов
    [Authorize]
    [HttpGet(Name = "GetUserMobileAccount")]
    public IActionResult Get(string login)
    {
        GtcymkznContext context = new GtcymkznContext();

        try
        {
            UserMobileAccount account = new UserMobileAccount();
            //Заполнение данных об аккаунте пользователя
            var user = context.Users.Where(u => u.Login == login).FirstOrDefault();
            account.Id = user.Id;
            account.Email = user.Email;
            account.Login = user.Login;
            account.Password = user.Password;

            //Если пользователь не найден
            if (account.Id == null)
            {
                return NotFound("AccountNotFound");
            }
            
            return Ok(JsonSerializer.Serialize(account, new JsonSerializerOptions() { 	ReferenceHandler = ReferenceHandler.IgnoreCycles }));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //Запрос на вывод всех игр
    [HttpGet("GetGame")]
    [Authorize]
    public ActionResult GetDataApi()
    {
        return Ok(JsonSerializer.Serialize(new GtcymkznContext().Games.ToList()));
    }
    
    //Запрос на вывод всех издателей
    [HttpGet("GetPublishers")]
    [Authorize]
    public ActionResult GetDataPublishers()
    {
        return Ok(JsonSerializer.Serialize(new GtcymkznContext().Publishers.ToList()));
    }
    
    //Запрос на вывод всех разработчиков
    [HttpGet("GetDevelopers")]
    [Authorize]
    public ActionResult GetDataDevelopers()
    {
        return Ok(JsonSerializer.Serialize(new GtcymkznContext().Developers.ToList()));
    }
    
    //Вывод жанров
    [HttpGet("GetGeners")]
    [Authorize]
    public ActionResult GetData()
    {
        return Ok(JsonSerializer.Serialize(new GtcymkznContext().Generes.ToList()));
    }
    
    //добавление игры в избранное
    [HttpPost("AddGameForFavorite")]
    [Authorize]
    public IActionResult AddBookForFavorite(int idGame)
    {
        GtcymkznContext Context = new GtcymkznContext();
        int idUser = GetUserIdFromToken(); //получаем id пользователя из токена
        Favorite favoriteModel = new Favorite //экземпляр класса избранного
        {
            IdUser = idUser,
            IdGame = idGame
        };

        Context.Favorites.Add(favoriteModel); //добавляем 
        Context.SaveChanges(); //сохраняем

        return Ok("Game add to favorite.");
    }
    
    [HttpGet("Favorite")]
    [Authorize]
    public OkObjectResult GetFavoriteBooksByUserId()
    {
        GtcymkznContext Context = new GtcymkznContext();
        int userId = GetUserIdFromToken();
        var favoriteBooks = Context.Favorites
            .Where(f => f.IdUser == userId) // Фильтрация по IdUser
            .Include(f => f.IdGameNavigation) // Предзагрузка связанной сущности Game
            .Select(f => new
            {
                IdGame = f.IdGameNavigation.Id,
                Name = f.IdGameNavigation.GameName,
                Publisher = f.IdGameNavigation.IdPublisherNavigation.Publisher1,
                Developer = f.IdGameNavigation.IdDeveloperNavigation.Developer1
            })
            .ToList();

        return Ok(favoriteBooks);
    }
    //запрос на информацию о конкретной игре
    [HttpGet("GetInformationAboutGame")]
    [Authorize]
    public async Task<ActionResult<ModelGameInformation>> GetInformationAboutGame(int gameId)
    {
        GtcymkznContext Context = new GtcymkznContext();
        int userId = GetUserIdFromToken(); //из токена получаем id пользователя
        
        //запрос к бд для нахождения игры соотвествующей введенному id
        var favoriteGame = await Context.GameGenres
            .Include(f => f.IdGameNavigation)
            .ThenInclude(game => game.IdPublisherNavigation)
            .Include(f =>f.IdGameNavigation)
            .ThenInclude(game => game.IdDeveloperNavigation)
            .Include(f => f.IdGameNavigation)
            .ThenInclude(game => game.GameGenres)
            .ThenInclude(gg => gg.IdGenreNavigation)
            .FirstOrDefaultAsync(f => f.IdGame == gameId);

        if (favoriteGame == null)
        {
            return NotFound("Game not found"); // если игра не найдена в избранном пользователя, возвращаем 404 Not Found
        }
        
        var gameInformation = new ModelGameInformation()
        {
            IdGame = favoriteGame.IdGameNavigation.Id,
            Name = favoriteGame.IdGameNavigation.GameName,
            Publisher = favoriteGame.IdGameNavigation.IdPublisherNavigation.Publisher1,
            Developer = favoriteGame.IdGameNavigation.IdDeveloperNavigation.Developer1,
            Geners = favoriteGame.IdGameNavigation.GameGenres.Select(gg => gg.IdGenreNavigation.Gener).ToArray(),
            Description = favoriteGame.IdGameNavigation.Description,
            SystemRequestMin = favoriteGame.IdGameNavigation.SystemRequestMin,
            SystemRequestRec = favoriteGame.IdGameNavigation.SystemRequestRec,
            ReleaseDate = favoriteGame.IdGameNavigation.ReleaseDate,
            MainImage = favoriteGame.IdGameNavigation.MainImage
        };

        return Ok(gameInformation); // возвращаем информацию о игре
    }
    //запрос на изменения пароля
    [HttpPut("ChangePassword")]
    [Authorize]
    public IActionResult ChangePassword(string password)
    {
        GtcymkznContext Context = new GtcymkznContext();
        int id = GetUserIdFromToken();
        // Проверяем, существует ли пользователь
        var user = Context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound("Пользователь не найден"); // Пользователь не найден
        }
        
        // Шифрование пароля
        string hashedPassword = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        user.Password = hashedPassword;
        Context.SaveChanges(); //сохранения нового пароля
        return Ok("Password changed");
    }
    
    //изменения email
    [HttpPut("ChangeEmail")]
    [Authorize]
    public IActionResult ChangeEmail(string email)
    {
        GtcymkznContext Context = new GtcymkznContext();
        int id = GetUserIdFromToken();
        // Проверяем, существует ли пользователь
        var user = Context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound("Пользователь не найден"); // Пользователь не найден
        }
        user.Email = email;
        
        Context.SaveChanges(); //сохранение нового мыла

        return Ok("Email changed");
    }
    
    //изменение логина
    [HttpPut("ChangeLogin")]
    [Authorize]
    public IActionResult ChangeLogin(string login)
    {
        GtcymkznContext Context = new GtcymkznContext();
        int id = GetUserIdFromToken();
        // Проверяем, существует ли пользователь
        var user = Context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound("Пользователь не найден"); // Пользователь не найден
        }
        
        // Проверяем, что новый логин не совпадает ни с одним из существующих логинов
        var existingUser = Context.Users.FirstOrDefault(u => u.Login == login);
        if (existingUser != null)
        {
            return BadRequest("Login already exists"); // Логин уже существует
        }

        user.Login = login;
        Context.SaveChanges(); //сохранение нового логина
        
        Console.WriteLine(id);
        return Ok("Login changed");
        
    }

    //изменение пользователя
    [HttpPut("ChangeUser")]
    [Authorize]
    public IActionResult ChangeUser(string login,  string email, string password, string id)
    {
        GtcymkznContext Context = new GtcymkznContext();
        // Проверяем, существует ли пользователь
        var user = Context.Users.FirstOrDefault(u => u.Id == int.Parse(id));
        if (user == null)
        {
            return NotFound("Пользователь не найден"); // Пользователь не найден
        }
        
        // Проверяем, что новый логин не совпадает ни с одним из существующих логинов
        var existingUser = Context.Users.FirstOrDefault(u => u.Login == login);
        if (existingUser != null && existingUser.Id != int.Parse(id))
        {
            return BadRequest("Login already exists"); // Логин уже существует
        }

        password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        user.Email = email;
        user.Password = password;
        user.Login = login;
        Context.SaveChanges(); //сохранение нового логина
        
        return Ok();
    }
    
    //удаляем игру из избранного
    [HttpDelete("RemoveGameFromFavorites")]
    [Authorize]
    public async Task<IActionResult> DeleteBookFromFavarite(int idGame)
    {
        GtcymkznContext Context = new GtcymkznContext();
        var game =  Context.Favorites.FirstOrDefault(b => b.IdGame == idGame); //находим игру по id

        if (game == null)
        {
            return NotFound("Игра не найдена"); // Если игра с указанным id не найден, возвращаем 404 Not Found
        }

        Context.Favorites.Remove(game); //удаляем
        await Context.SaveChangesAsync(); //сохраняем

        return Ok("Game deleted"); 
    }
    
    //Получение токенов
    [Route("Login")]
    [HttpPost]
    public IActionResult GetTokens(string login, string password)
    {
        try
        {
            GtcymkznContext Context = new GtcymkznContext();
            
            //Настройка токена
            var Token = new UserTokens();
            password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
            var User = Context.Users.Where(x => x.Login == login && x.Password == password).FirstOrDefault();
            if (User != null) {
                Token = GamesLibAPI.JwtHelpers.JwtHelpers.GenTokenkey(new UserTokens() {
                    UserName = User.Login
                }, jwtSettings);
            } else {
                return BadRequest($"wrong password");
            }

            //Запись данных в БД
            Context.UserJwts.Add(new UserJwt()
                { User = User.Id, JwtAccess = Token.AccessToken, JwtRefresh = Token.RefreshToken });
            Context.SaveChanges();
            
            return Ok(Token);
        } catch (Exception ex) {
            return BadRequest("Ошибка API");
        }
    }
    
    //Обновление токенов
    [Route("UpdateTokens")]
    [HttpGet]
    [Authorize]
    public IActionResult UpdateTokens()
    {
        GtcymkznContext Context = new GtcymkznContext();
        
        //Поиск старого токена в бвзе
        string token = Request.Headers.Authorization.ToString().Substring(7);
        var JwtRecord = Context.UserJwts.Where(x => x.JwtRefresh == token).FirstOrDefault();
        
        if (JwtRecord == null)
        {
            return NotFound("Токен отсутствует в базе"); 
        }
        
        User user = Context.Users.Where(u => u.Id == JwtRecord.User).FirstOrDefault();
        
        try
        {
            //Настройка токена
            var Token = new UserTokens();
            Token = GamesLibAPI.JwtHelpers.JwtHelpers.GenTokenkey(new UserTokens() {
                UserName = user.Login,
            }, jwtSettings);
            
            //Изменение данных в БД
            Context.UserJwts.Remove(JwtRecord);
            Context.SaveChanges();
            
            Context.UserJwts.Add(new UserJwt()
                { User = user.Id, JwtAccess = Token.AccessToken, JwtRefresh = Token.RefreshToken });
            Context.SaveChanges();
            
            return Ok(Token);
        } catch (Exception ex) {
            return BadRequest("Ошибка API");
        }
    }
    
    //регистрация
    [HttpPost("Registration")]
    public async Task<IActionResult> RegisterUser(string login, string email, string password)
    {
        GtcymkznContext Context = new GtcymkznContext();
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!email.Contains("@") || email.Length < 10)
        {
            return ValidationProblem("Почта не валидна");
        }
        
        // Проверяем, существует ли пользователь с таким же именем пользователя или email'ом
        if (await Context.Users.AnyAsync(u => u.Login == login || u.Email == email))
        {
            return ValidationProblem("Такая почта или логин уже заняты");
        }
            
        // Шифрование пароля
        string hashedPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        // Создаем нового пользователя
        var user = new User
        {
            Login = login,
            Email = email,
            Password = hashedPassword, 
            UserRole = 2
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        return Ok("User successfully registered");
    }
    
    //получение id пользователя из токена
    private int GetUserIdFromToken()
    {
        var token = GetTokenFromAuthorizationHeader(); //получаем токен
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        //полчение срока действия токена
        var now = DateTime.UtcNow;
        if (jwtToken.ValidTo < now)
        {
            // Токен истек, выполните необходимые действия, например, вызовите исключение
            throw new Exception("Expired token.");
        }
        // Извлечение идентификатора пользователя из полезной нагрузки токена
        var userId = int.Parse(jwtToken.Claims.First(c => c.Type == "userId").Value);

        return userId;
    }

    //получение токена из запроса
    private string GetTokenFromAuthorizationHeader()
    {
        var autorizationHeader = Request.Headers["Authorization"].FirstOrDefault();

        if (autorizationHeader != null && autorizationHeader.StartsWith("Bearer "))
        {
            var token = autorizationHeader.Substring("Bearer ".Length).Trim();
            return token;
        }

        return null;
    }
}