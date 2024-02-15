package retrofit;

import java.util.ArrayList;

import calls.GetUser;
import models.Game;
import retrofit2.Call;
import retrofit2.http.DELETE;
import retrofit2.http.GET;
import retrofit2.http.Header;
import retrofit2.http.POST;
import retrofit2.http.PUT;
import retrofit2.http.Query;
//Интерфейс апи
public interface APIInterface {
    //Запрос на получение токена
    @POST("/api/ForAllUser/Login")
    Call<GetUser.GetToken> Login(
            @Query("login") String login,
            @Query("password") String password
    );

    //Запрос получения данных об аккаунте для телефона
    @GET("/api/ForAllUser/GetGame")
    Call<ArrayList<Game>> GetGame(
            @Header("Authorization") String authHeader
    );

    //Запрос получения данных об аккаунте для телефона
    @GET("/api/ForAllUser/GetUsers")
    Call<Game> GetUsers(
            @Header("Authorization") String authHeader
    );

    //Запрос данных о достижении
    @POST("api/ForAllUser/AddGameForFavorite")
    Call<Void> AddGameForFavorite(
            @Query("idGame") int login,
            @Header("Authorization") String authHeader
    );

    @GET("api/ForAllUser/Favorite")
    Call<ArrayList<Game>> Favorite(
            @Header("Authorization") String authHeader
    );

    @DELETE("api/ForAllUser/RemoveGameFromFavorites")
    Call<Void> RemoveGameFromFavorites(
            @Query("idGame") int login,
            @Header("Authorization") String authHeader
    );

    @PUT("api/ForAllUser/ChangeUser")
    Call<Void> ChangeUser(
            @Query("login") String login,
            @Query("email") String email,
            @Query("password") String password,
            @Query("id") String id,
            @Header("Authorization") String authHeader
    );

    @POST("api/ForAllUser/Registration")
    Call<Void> Registration(
            @Query("login") String login,
            @Query("email") String email,
            @Query("password") String password
    );
}
