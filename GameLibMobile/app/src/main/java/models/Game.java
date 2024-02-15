package models;

public class Game {
    private int Id;
    private String GameName;
    private int IdDeveloper;
    private int IdPublisher;
    private String Description;
    private String SystemRequestMin;
    private String SystemRequestRec;
    private String ReleaseDate;
    private String MainImage;

    public int getId() {
        return Id;
    }

    public void setId(int id) {
        Id = id;
    }

    public String getGameName() {
        return GameName;
    }

    public void setGameName(String gameName) {
        GameName = gameName;
    }

    public int getIdDeveloper() {
        return IdDeveloper;
    }

    public void setIdDeveloper(int idDeveloper) {
        IdDeveloper = idDeveloper;
    }

    public int getIdPublisher() {
        return IdPublisher;
    }

    public void setIdPublisher(int idPublisher) {
        IdPublisher = idPublisher;
    }

    public String getDescription() {
        return Description;
    }

    public void setDescription(String description) {
        Description = description;
    }

    public String getSystemRequestMin() {
        return SystemRequestMin;
    }

    public void setSystemRequestMin(String systemRequestMin) {
        SystemRequestMin = systemRequestMin;
    }

    public String getSystemRequestRec() {
        return SystemRequestRec;
    }

    public void setSystemRequestRec(String systemRequestRec) {
        SystemRequestRec = systemRequestRec;
    }

    public String getReleaseDate() {
        return ReleaseDate;
    }

    public void setReleaseDate(String releaseDate) {
        ReleaseDate = releaseDate;
    }

    public String getMainImage() {
        return MainImage;
    }

    public void setMainImage(String mainImage) {
        MainImage = mainImage;
    }
}
