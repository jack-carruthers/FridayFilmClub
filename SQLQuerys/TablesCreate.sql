Use FridayFilmClub; 

CREATE TABLE Users(
	UserID int IDENTITY(1,1) PRIMARY KEY, 
	Username varchar(15),
	Email varchar(255), 
	Hashed_password varchar(255),
	Salt varchar(255),
	PaidSubscription BIT DEFAULT,
	IsAdmin BIT DEFAULT 0,
	IsBanned BIT DEFAULT 0);


CREATE TABLE MovieRatings (
    RatingID INT PRIMARY KEY IDENTITY,
    MovieID NVARCHAR(50),
    UserID INT,
    Rating INT);

ALTER TABLE Users
ADD CONSTRAINT DF_Users_PaidSubscription DEFAULT 0 FOR PaidSubscription;

ALTER TABLE Users
ADD CONSTRAINT UQ_Users_Username UNIQUE (Username);



CREATE TABLE ChatMessages (
    MessageID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    Timestamp DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);



SELECT * FROM USERS

SELECT * FROM ChatMessages

SELECT * FROM MovieRatings



-- Make userid number 1 admin
UPDATE Users
SET Isbanned = 1
WHERE UserID = 10;



