package retrofit;

import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;
//Настройки ретрофита для работы с апи
public class GameLibAPI {
    private static Retrofit retrofit = null;

    public static Retrofit getClient() {
        String url;
        url = "http://5.144.96.227:5555/";
        retrofit = new Retrofit.Builder()
                .baseUrl(url)
                .addConverterFactory(GsonConverterFactory.create())
                .build();

        return retrofit;
    }
}
