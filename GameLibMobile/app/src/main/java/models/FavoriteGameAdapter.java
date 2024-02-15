package models;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.example.pokmob.FavoriteActivity;
import com.example.pokmob.R;
import com.squareup.picasso.Picasso;

import java.util.List;

import retrofit.APIInterface;
import retrofit.GameLibAPI;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import retrofit2.Retrofit;

public class FavoriteGameAdapter extends ArrayAdapter<Game> {
    Context mContext;
    int mResource;

    public FavoriteGameAdapter(Context context, int resource, List<Game> objects) {
        super(context, resource, objects);
        mContext = context;
        mResource = resource;
    }

    @NonNull
    @Override
    public View getView(int position, @Nullable View convertView, @NonNull ViewGroup parent) {

        Retrofit retrofit = GameLibAPI.getClient();
        APIInterface api = retrofit.create(APIInterface.class);

        Button deleteLikeButton;

        SharedPreferences myPreferences
                = mContext.getSharedPreferences("settings", Context.MODE_PRIVATE);

        // Получаем объект Game для текущей позиции
        Game game = getItem(position);

        // Проверяем, есть ли макет для переиспользования
        if (convertView == null) {
            LayoutInflater inflater = LayoutInflater.from(mContext);
            convertView = inflater.inflate(mResource, parent, false);
        }

        deleteLikeButton = convertView.findViewById(R.id.add_to_favorites_button);

        // Устанавливаем тег для кнопки лайка, чтобы запомнить позицию игры
        deleteLikeButton.setTag(position);

        // Устанавливаем обработчик нажатия на кнопку лайка
        deleteLikeButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Получаем позицию игры из тега кнопки
                int position = (int) v.getTag();

                // Получаем объект Game для этой позиции
                Game clickedGame = getItem(position);

                Call<Void> call = api.RemoveGameFromFavorites(
                        clickedGame.getId(),
                        "Bearer " + myPreferences.getString("TOKEN", "")

                );

                Callback<Void> callback = new Callback<Void>() {
                    @Override
                    public void onResponse(Call<Void> call, Response<Void> response) {
                        if(response.isSuccessful()){
                            Toast.makeText(mContext, "Удалено из избранного", Toast.LENGTH_SHORT).show();
                            Intent intent = new Intent( mContext, FavoriteActivity.class);
                            mContext.startActivity(intent);
                            ((FavoriteActivity) mContext).finish();
                        }else{
                            Toast.makeText(mContext, "Ошибка удаления из избранного", Toast.LENGTH_SHORT).show();
                        }
                    }

                    @Override
                    public void onFailure(Call<Void> call, Throwable t) {
                        Toast.makeText(mContext, "Ошибка сервера", Toast.LENGTH_SHORT).show();
                    }
                };

                call.enqueue(callback);
            }
        });

        // Находим TextView в макете элемента списка
        TextView gameNameTextView = convertView.findViewById(R.id.game_name);
        TextView descriptionTextView = convertView.findViewById(R.id.game_description);
        ImageView gameLogoImageView = convertView.findViewById(R.id.game_image);
        // Найдите другие TextView, если необходимо

        // Установите значения полей объекта Game в TextView
        if (game != null) {
            gameNameTextView.setText(game.getGameName());
            descriptionTextView.setText(game.getDescription());
            Picasso.get().load(game.getMainImage()).into(gameLogoImageView);
            // Установите другие значения TextView, если необходимо
        }

        return convertView;
    }
}

