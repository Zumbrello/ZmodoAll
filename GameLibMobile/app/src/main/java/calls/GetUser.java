package calls;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import models.User;

public class GetUser {

    private Boolean result;
    private List<User> data;
    private Object error;
    private Map<String, Object> additionalProperties = new LinkedHashMap<String, Object>();

    public Boolean getResult() {
        return result;
    }

    public void setResult(Boolean result) {
        this.result = result;
    }

    public List<User> getData() {
        return data;
    }

    public void setData(List<User> data) {
        this.data = data;
    }

    public Object getError() {
        return error;
    }

    public void setError(Object error) {
        this.error = error;
    }

    public Map<String, Object> getAdditionalProperties() {
        return this.additionalProperties;
    }

    public void setAdditionalProperty(String name, Object value) {
        this.additionalProperties.put(name, value);
    }

    public class GetToken {

        @SerializedName("accessToken")
        @Expose
        private String accessToken;
        @SerializedName("error")
        @Expose
        private String error;
        @SerializedName("userName")
        @Expose
        private String userName;
        @SerializedName("refreshToken")
        @Expose
        private String refreshToken;

        @SerializedName("user")
        @Expose
        private Integer user;
        @SerializedName("login")
        @Expose
        private String login;
        @SerializedName("email")
        @Expose
        private String email;

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

        public String getAccessToken() {
            return accessToken;
        }

        public void setAccessToken(String token) {
            this.accessToken = token;
        }

        public String getUserName() {
            return userName;
        }

        public void setUserName(String userName) {
            this.userName = userName;
        }

        public String getRefreshToken() {
            return refreshToken;
        }

        public void setRefreshToken(String refreshToken) {
            this.refreshToken = refreshToken;
        }

        public String getError() {
            return error;
        }

        public void setError(String error) {
            this.error = error;
        }

    }
}
