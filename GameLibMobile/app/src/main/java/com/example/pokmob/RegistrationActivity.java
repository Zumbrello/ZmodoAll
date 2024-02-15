package com.example.pokmob;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

import retrofit.APIInterface;
import retrofit.GameLibAPI;
import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Retrofit;

public class RegistrationActivity extends AppCompatActivity implements View.OnClickListener {

    Button exitButton, registerButton;
    EditText loginText, passwordText, emailEdit;
    TextView warningTV;
    Retrofit retrofit;
    APIInterface api;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_registration);

        //Инициализация переменных, связанных с активити
        exitButton = (Button)findViewById(R.id.exitBtn);
        registerButton = (Button)findViewById(R.id.registerBtn);
        loginText = (EditText)findViewById(R.id.loginEdit);
        passwordText = (EditText)findViewById(R.id.passwordEdit);
        emailEdit = (EditText)findViewById(R.id.emailEdit);
        warningTV = (TextView)findViewById(R.id.warningTV);

        warningTV.setVisibility(View.INVISIBLE);

        //Установка обработчиков нажатия
        exitButton.setOnClickListener(this);
        registerButton.setOnClickListener(this);

        //Настройка ретрофита
        retrofit = GameLibAPI.getClient();
        api = retrofit.create(APIInterface.class);

        SharedPreferences myPreferences
                = RegistrationActivity.this.getSharedPreferences("settings", Context.MODE_PRIVATE);

        //Автозаполнение полей, если ранее был произведён вход
        if(myPreferences.getString("PASSWORD", "") != "" &&
                myPreferences.getString("LOGIN", "") != ""){
            loginText.setText(myPreferences.getString("LOGIN", ""));
            passwordText.setText(myPreferences.getString("PASSWORD", ""));
        }
    }

    @Override
    public void onClick(View view) {

        if(view.getId() == R.id.exitBtn){
            System.exit(0);
        }else if(view.getId() == R.id.registerBtn){

            Call<Void> call = api.Registration(
                    loginText.getText().toString(),
                    emailEdit.getText().toString(),
                    passwordText.getText().toString()
            );
            Callback<Void> callback = new Callback<Void>() {
                @Override
                public void onResponse(Call<Void> call, retrofit2.Response<Void> response) {
                    if (response.isSuccessful()) {
                        Intent intent = new Intent(RegistrationActivity.this, MainActivity.class);
                        startActivity(intent);
                        RegistrationActivity.this.finish();
                    }else{
                        warningTV.setVisibility(View.VISIBLE);
                    }
                }

                @Override
                public void onFailure(Call<Void> call, Throwable t) {
                    System.out.println("Network Error :: " + t.getLocalizedMessage());
                }
            };

            call.enqueue(callback);
        }
    }
}