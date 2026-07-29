INTERNSHIP MANAGEMENT SYSTEM
============================

SYSTEM REQUIREMENTS:
- Windows 7 or later (64-bit or 32-bit)
- SQL Server LocalDB (comes with Visual Studio) or SQL Server Express
- No additional .NET installation required

INSTALLATION & RUNNING:

1. Extract all files to a folder
2. Double-click "InternshipManagementSystem.exe"
3. The application will start automatically
4. Open your browser and go to: https://localhost:5001
   (or http://localhost:5048 if port 5001 is in use)

FIRST TIME SETUP:
- The database will be created automatically on first run
- Wait for the database setup to complete (may take 30-60 seconds)

DEFAULT ADMIN LOGIN:
- Email: admin@internship.com
- Password: Admin@123
- Login As: Admin/Supervisor

CREATING NEW USERS:

INTERNS:
1. On the login page, click "Register as Intern"
2. Fill in all required information
3. Click "Register"
4. Use your email and password to login

ADMINS/SUPERVISORS:
1. On the login page, click "Register as Admin"
2. Select your role (Admin or Supervisor)
3. Fill in all required information
4. Click "Register"
5. Use your email and password to login

TROUBLESHOOTING:

If the application won't start:
- Make sure you have SQL Server LocalDB or Express installed
- Check that port 5001 is not in use by another application
- Try port 5048 instead (check the console window for the actual port)

If you get a database connection error:
- Ensure SQL Server LocalDB is running
- Check your appsettings.json connection string
- You may need to change the server name in appsettings.json

For more help, contact the system administrator.
