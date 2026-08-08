# Picklr — Phase 2

ASP.NET Core MVC app for browsing and reserving pickleball programs, with an
admin area for managing clubs, programs, and users.

## Setup — run this first

The SQLite database file (`picklr.db`) is **not** in this repository — it's
generated, so it's excluded by `.gitignore`. You have to create it before the
app will run.

Open a terminal in the project folder and run:

```
dotnet tool install --global dotnet-ef
dotnet ef database update
dotnet run
```

Then open **http://localhost:5000**.

> If you skip `dotnet ef database update`, the app builds and starts fine but
> every page throws `SQLite Error 1: 'no such table: Clubs'` — that just means
> the database hasn't been created yet.

> If `dotnet ef` isn't found after installing, add the tools folder to your PATH:
> - **macOS/Linux:** `export PATH="$PATH:$HOME/.dotnet/tools"`
> - **Windows:** restart the terminal

### In Visual Studio (Windows)

Same thing through the IDE:

1. Open `Picklr.csproj`
2. Tools → NuGet Package Manager → **Package Manager Console**
3. Run `Update-Database`
4. Press Ctrl+F5

## What's in Phase 2

- **Program search** — filter by club and date on the home page, using model
  binding on the query string
- **Shopping cart** — Reserve adds a program to a cart held in Session; nothing
  is written to the database until checkout
- **Pay & Confirm** — turns each cart line into a `Reservation` row
- **Admin** — programs now belong to a Club and run on selected days of the week

## Project structure

```
Areas/Admin/         Admin area (controllers + views), [Area("Admin")]
Controllers/         Public site: HomeController, CartController
Models/              Entities, PicklrContext, CartItem, SessionCart helper
Migrations/          EF Core migrations
Views/               Public site views
Properties/          launchSettings.json (sets Development environment)
```

## Notes

- Database: SQLite (`picklr.db`), created by the migrations above with seed data
  for 3 clubs, 5 programs, and 2 users.
- The cart lives in Session, not the database. Restarting the server clears any
  in-progress carts — expected, since Session uses an in-memory store here.
