# FridayFilmClub

## Requirements:

- [.NET 6 or later](https://dotnet.microsoft.com/download)
- [SQL Server Management Studio](https://www.microsoft.com/en-us/sql-server)

## Installation Steps:

1. **Clone the repository**:

   ```bash
   git clone https://github.com/jack-carruthers/FridayFilmClub.git
   cd FridayFilmClub
   ```

2. **Create SQL Tables**:

   Connect using SQL authentication with username "sa" and password "saroot".  
   In `FridayFilmClub\SQLQuerys` folder, you can find all SQL queries to set up the database.

3. **Install Dependencies**:

   Open up the project in VSCode. Select **Open Folder** and choose the project directory.  
   Open a terminal by clicking the **+** in the terminal tab or pressing `CTRL + '`.  
   Run the following command to install all dependencies:

   ```bash
   dotnet restore
   ```

   **If this does not work, run these commands instead:**

   ```bash
   dotnet add package Microsoft.AspNetCore.Authentication.Tools
   dotnet add package Microsoft.AspNetCore.Authentication
   dotnet add package Microsoft.Identity.Client
   ```
