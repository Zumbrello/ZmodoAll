package com.example.pokmob;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.PopupMenu;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

public class AccountActivity extends AppCompatActivity implements View.OnClickListener{

    TextView emailText, loginText;
    Button changeBtn, menuBtn;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_account);
    }

    @Override
    protected void onResume() {
        super.onResume();

        //Инициализация переменных, связанных с активити
        emailText = findViewById(R.id.emailText);
        loginText = findViewById(R.id.loginText);
        changeBtn = findViewById(R.id.changeBtn);
        menuBtn = findViewById(R.id.menuBtn);

        //Установка обработчиков нажатия
        changeBtn.setOnClickListener(this);
        menuBtn.setOnClickListener(this);

        //Установка данных в полях активити
        Bundle extras = getIntent().getExtras();
        SharedPreferences myPreferences
                = AccountActivity.this.getSharedPreferences("settings", Context.MODE_PRIVATE);

        emailText.setText(myPreferences.getString("EMAIL", ""));
        loginText.setText(myPreferences.getString("LOGIN", ""));
    }

    public AccountActivity(){

    }

    @Override
    public void onClick(View view) {
        if (view.getId() == R.id.changeBtn){
            Intent intent = new Intent(AccountActivity.this, ChangeUser.class);
            startActivity(intent);
            AccountActivity.this.finish();
        }else if(view.getId() == R.id.menuBtn){
            showPopupMenu(view);
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
                            return true;
                        }else if(item.getItemId() == R.id.catalog){
                            Intent intent = new Intent(AccountActivity.this, GamesListActivity.class);
                            startActivity(intent);
                            AccountActivity.this.finish();
                            return true;
                        }else if(item.getItemId() == R.id.favorite){
                            Intent intent = new Intent(AccountActivity.this, FavoriteActivity.class);
                            startActivity(intent);
                            AccountActivity.this.finish();
                            return true;
                        }else if(item.getItemId() == R.id.logout){
                            Intent intent = new Intent(AccountActivity.this, MainActivity.class);
                            startActivity(intent);
                            AccountActivity.this.finish();
                            return true;
                        }
                        return false;
                    }
                });
        popupMenu.show();
    }
}
