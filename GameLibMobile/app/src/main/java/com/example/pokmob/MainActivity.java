package com.example.pokmob;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import androidx.appcompat.app.AppCompatActivity;

import calls.GetUser;
import retrofit.APIInterface;
import retrofit.GameLibAPI;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Retrofit;

public class MainActivity extends AppCompatActivity implements View.OnClickListener {

    Button loginButton, exitButton, registerButton;
    EditText loginText, passwordText;
    Retrofit retrofit;
    APIInterface api;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        //Инициализация переменных, связанных с активити
        loginButton = (Button)findViewById(R.id.loginBtn);
        exitButton = (Button)findViewById(R.id.exitBtn);
        registerButton = (Button)findViewById(R.id.registerBtn);
        loginText = (EditText)findViewById(R.id.loginEdit);
        passwordText = (EditText)findViewById(R.id.passwordEdit);

        //Установка обработчиков нажатия
        loginButton.setOnClickListener(this);
        exitButton.setOnClickListener(this);
        registerButton.setOnClickListener(this);

        //Настройка ретрофита
        retrofit = GameLibAPI.getClient();
        api = retrofit.create(APIInterface.class);

        SharedPreferences myPreferences
                = MainActivity.this.getSharedPreferences("settings", Context.MODE_PRIVATE);

        //Автозаполнение полей, если ранее был произведён вход
        if(myPreferences.getString("PASSWORD", "") != "" &&
                myPreferences.getString("LOGIN", "") != ""){
            loginText.setText(myPreferences.getString("LOGIN", ""));
            passwordText.setText(myPreferences.getString("PASSWORD", ""));
        }
    }

    @Override
    public void onClick(View view) {

        if (view.getId() == R.id.loginBtn){

            //Запрос на получение токена (Авторизация)
            Call<GetUser.GetToken> jwt = api.Login(
                    loginText.getText().toString(),
                    passwordText.getText().toString()
            );

            Callback<GetUser.GetToken> jwtCall = new Callback<GetUser.GetToken>() {
                @Override
                public void onResponse(Call<GetUser.GetToken> call, retrofit2.Response<GetUser.GetToken> response) {
                    if (response.isSuccessful())
                    {
                        GetUser.GetToken apiResponse = response.body();
                        if (apiResponse.getError() == null){
                            SharedPreferences myPreferences
                                    = MainActivity.this.getSharedPreferences("settings", Context.MODE_PRIVATE);
                            SharedPreferences.Editor editor = myPreferences.edit();
                            editor.putString("TOKEN", response.body().getAccessToken());
                            editor.putString("REFRESH_TOKEN", response.body().getRefreshToken());
                            editor.putString("LOGIN", apiResponse.getLogin().toString());
                            editor.putString("EMAIL", apiResponse.getEmail().toString());
                            editor.putString("USER", apiResponse.getUser().toString());
                            editor.putString("PASSWORD", passwordText.getText().toString());
                            editor.commit();
                            Intent intent = new Intent(MainActivity.this, GamesListActivity.class);
                            startActivity(intent);
                            MainActivity.this.finish();
                        }
                    }else{
                        MainActivity.this.loginText.setText("");
                        MainActivity.this.passwordText.setText("");
                    }
                }

                @Override
                public void onFailure(Call<GetUser.GetToken> call, Throwable t) {
                    System.out.println("Network Error :: " + t.getLocalizedMessage());
                }
            };

            jwt.enqueue(jwtCall);

        } else if(view.getId() == R.id.exitBtn){
            System.exit(0);
        }else if(view.getId() == R.id.registerBtn){
            Intent intent = new Intent(MainActivity.this, RegistrationActivity.class);
            startActivity(intent);
            MainActivity.this.finish();
        }
    }
}