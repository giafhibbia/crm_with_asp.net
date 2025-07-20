# crm_with_asp.net

A simple CRM (Customer Relationship Management) web application built with ASP.NET Core MVC and Entity Framework Core.

Sebuah aplikasi CRM sederhana berbasis web yang dibangun menggunakan ASP.NET Core MVC dan Entity Framework Core.

## Features / Fitur

- Login & authentication using cookie-based session  
  Login dan autentikasi berbasis cookie
- Basic role & permission system  
  Sistem role dan permission dasar
- Dashboard layout with sidebar and header  
  Layout dashboard dengan sidebar dan header
- User profile management  
  Manajemen profil pengguna
- Lead management  
  Manajemen data leads
- Success/error notifications (toast)  
  Notifikasi sukses dan error (toast)
- Styling using Tailwind CSS and custom CSS  
  Styling menggunakan Tailwind CSS dan custom CSS

## Technology Stack / Teknologi

- ASP.NET Core MVC
- Entity Framework Core (MySQL / MariaDB)
- Razor View Engine
- Tailwind CSS
- Custom CSS (`wwwroot/css/custom.css`)

## Getting Started / Cara Menjalankan

1. Clone the repository / Clone repositori:
   ```bash
   git clone https://github.com/yourusername/crm_with_asp.net.git
Configure your database connection in appsettings.json
Konfigurasi koneksi database di appsettings.json

Run database migration and seeding if available
Jalankan migrasi dan seed data jika tersedia:

bash
Copy
Edit
dotnet ef database update
Start the development server / Jalankan server:

bash
Copy
Edit
dotnet run
Open the application at http://localhost:5000 (default)

Folder Structure / Struktur Folder
rust
Copy
Edit
/Controllers     -> MVC controllers (User, Auth, Lead)
/Models          -> Data models (User, Role, Lead, etc.)
/Views           -> Razor views
/wwwroot/css     -> Custom styling (e.g., custom.css)
/Data            -> DbContext and seeders
Feel free to fork, modify, or contribute.