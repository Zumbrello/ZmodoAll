package com.example.pokmob;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.ListView;
import android.widget.PopupMenu;

import androidx.appcompat.app.AppCompatActivity;

import java.util.ArrayList;

import models.Game;
import models.GameAdapter;
import retrofit.APIInterface;
import retrofit.GameLibAPI;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;
import retrofit2.Retrofit;

public class GamesListActivity extends AppCompatActivity implements View.OnClickListener {
    ListView mainListView;
    Button menuBtn;
    GameAdapter mArrayAdapter;
    ArrayList<Game> mNameList = new ArrayList();

    Retrofit retrofit;
    APIInterface api;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_games_list);

        menuBtn = findViewById(R.id.menuBtn);
        menuBtn.setOnClickListener(this);

        mainListView = findViewById(R.id.main_listview);
        mArrayAdapter = new GameAdapter(this,
                R.layout.games_list_item,
                mNameList);
        mainListView.setAdapter(mArrayAdapter);

        SharedPreferences myPreferences
                = GamesListActivity.this.getSharedPreferences("settings", Context.MODE_PRIVATE);

        //Настройка ретрофита
        retrofit = GameLibAPI.getClient();
        api = retrofit.create(APIInterface.class);

        Call<ArrayList<Game>> call = api.GetGame("Bearer " + myPreferences.getString("TOKEN", ""));

        Callback<ArrayList<Game>> callback = new Callback<ArrayList<Game>>() {
            @Override
            public void onResponse(Call<ArrayList<Game>> call, Response<ArrayList<Game>> response) {
                if(response.isSuccessful()){
                    mNameList.clear();
                    mNameList.addAll(response.body());
                    mArrayAdapter.notifyDataSetChanged(); // Обновление адаптера после получения данных
                }else{
                }
            }

            @Override
            public void onFailure(Call<ArrayList<Game>> call, Throwable t) {
                // Обработка ошибки, если необходимо
            }
        };

        call.enqueue(callback);
    }


    @Override
    public void onClick(View v) {
        if(v.getId() == R.id.menuBtn){
            showPopupMenu(v);
        }
    }

    private void showPopupMenu(View v) {
        PopupMenu popupMenu = new PopupMenu(this, v);
        popupMenu.inflate(R.menu.popupmenu);

        popupMenu
                .setOnMenuItemClickListener(new PopupMenu.OnMenuItemClickListener() {
                    @Override
                    public boolean onMenuItemClick(MenuItem item) {
                        if(item.getItemId() == R.id.account) {
                            Intent intent = new Intent(GamesListActivity.this, AccountActivity.class);
                            startActivity(intent);
                            GamesListActivity.this.finish();
                            return true;
                        }else if(item.getItemId() == R.id.catalog){
                            return true;
                        }else if(item.getItemId() == R.id.favorite){
                            Intent intent = new Intent(GamesListActivity.this, FavoriteActivity.class);
                            startActivity(intent);
                            GamesListActivity.this.finish();
                            return true;
                        }else if(item.getItemId() == R.id.logout){
                            Intent intent = new Intent(GamesListActivity.this, MainActivity.class);
                            startActivity(intent);
                            GamesListActivity.this.finish();
                            return true;
                        }
                        return false;
                    }
                });
        popupMenu.show();
    }
}