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

public class ChangeUser extends AppCompatActivity implements View.OnClickListener {

    Button exitBtn, changeBtn;
    EditText passwordEdit, emailEdit, loginEdit;
    Retrofit retrofit;
    APIInterface api;

    TextView warningTV;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_change_user);

        passwordEdit = findViewById(R.id.passwordEdit);
        emailEdit = findViewById(R.id.emailEdit);
        loginEdit = findViewById(R.id.loginEdit);
        exitBtn = findViewById(R.id.exitBtn);
        changeBtn = findViewById(R.id.changeBtn);
        warningTV = findViewById(R.id.warningTV);
        SharedPreferences myPreferences
                = ChangeUser.this.getSharedPreferences("settings", Context.MODE_PRIVATE);
        emailEdit.setText(myPreferences.getString("EMAIL", ""));
        loginEdit.setText(myPreferences.getString("LOGIN", ""));
        passwordEdit.setText(myPreferences.getString("PASSWORD", ""));

        exitBtn.setOnClickListener(this);
        changeBtn.setOnClickListener(this);

        warningTV.setVisibility(View.INVISIBLE);

        //Настройка ретрофита
        retrofit = GameLibAPI.getClient();
        api = retrofit.create(APIInterface.class);
    }

    @Override
    public void onClick(View view) {
        if (view.getId() == R.id.exitBtn){
            Intent intent = new Intent(ChangeUser.this, AccountActivity.class);
            startActivity(intent);
            ChangeUser.this.finish();
        }else if(view.getId() == R.id.changeBtn){
            SharedPreferences myPreferences
                    = ChangeUser.this.getSharedPreferences("settings", Context.MODE_PRIVATE);

            //Запрос на получение токена (Авторизация)
            Call<Void> call = api.ChangeUser(
                    loginEdit.getText().toString(),
                    emailEdit.getText().toString(),
                    passwordEdit.getText().toString(),
                    myPreferences.getString("USER", ""),
                    "Bearer " + myPreferences.getString("TOKEN", "")
            );

            Callback<Void> callback = new Callback<Void>() {
                @Override
                public void onResponse(Call<Void> call, retrofit2.Response<Void> response) {
                    if (response.isSuccessful())
                    {
                        myPreferences.edit().clear().commit();
                        Intent intent = new Intent(ChangeUser.this, MainActivity.class);
                        startActivity(intent);
                        ChangeUser.this.finish();
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