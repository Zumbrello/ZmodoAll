package calls;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class GetUserMobileAccount {

    @SerializedName("Id")
    @Expose
    private Integer id;
    @SerializedName("User")
    @Expose
    private Integer user;
    @SerializedName("Login")
    @Expose
    private String login;
    @SerializedName("Email")
    @Expose
    private String email;

    public Integer getId() {
        return id;
    }

    public void setId(Integer id) {
        this.id = id;
    }

    public Integer getUser() {
        return user;
    }

    public void setUser(Integer user) {
        this.user = user;
    }

    public String getLogin() {
        return login;
    }

    public void setLogin(String login) {
        this.login = login;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

}
