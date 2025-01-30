# FridayFilmClub

## Requirements:
- [.NET 6 or later](https://dotnet.microsoft.com/download)
- [SQL Server Management Studio]([https://www.microsoft.com/en-us/sql-server](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)) (or another database provider, if you're using a different one)

## Installation Steps:

1. **Clone the repository**:
   ```bash
   git clone https://github.com/jack-carruthers/FridayFilmClub.git
   cd FridayFilmClub

2. **Create SQL Tables**: 

  Connect using sql authentication with username "sa" and password "saroot" 
  In SQLQuerys folder you can find all sql queries to setup the database

3. **Install Depencencies**: 

  Open up project in vscode. Select open folder and select the project installed. 
  You can then open up a terminal by finding terminal and clicking the + or presseing " CTRL + ' " 
  in terminal run "dotnet restore" and it should install all depencies 

  IF THIS DOES NOT WORK THEN 
  run this in terminal instead
  
  ```bash
   
    dotnet add package Microsoft.AspNetCore.Authentication.Tools
    dotnet add package Microsoft.AspNetCore.Authentication
    dotnet add package Microsoft.Identity.Client
  

