# RESTful arhitektura (Web API)

Ovaj materijal je dio Ishoda 2 (minimalno).

## 4 Pohrana stanja

### 4.1 Postavljanje pristupa bazi podataka

1. Otvorite Microsoft SQL Server Management Studio

   - Povežite se s dostupnim poslužiteljem (localhost\SQLEXPRESS) korištenjem **SQL Server Autentifikacije** (korisnik:sa, lozinka: SQL)
   - Napravite bazu podataka `Exercise4` sa zadanim (default) postavkama
   - U ovoj bazi podataka stvorite tablicu Notification sa sljedećom shemom:

     ```SQL
     CREATE TABLE [dbo].[Notification](
       [Id] [int] IDENTITY(1,1) NOT NULL,
       [Guid] [uniqueidentifier] NOT NULL,
       [CreatedAt] [datetime2](7) NOT NULL,
       [UpdatedAt] [datetime2](7) NULL,
       [Receiver] [nvarchar](256) NOT NULL,
       [Subject] [nvarchar](256) NULL,
       [Body] [nvarchar](max) NOT NULL,
       [SentAt] [datetime2](7) NULL,
       CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED  (
         [Id] ASC
       )
     )

     ALTER TABLE [dbo].[Notification] ADD  CONSTRAINT [DF_Notification_Guid]  DEFAULT (newid()) FOR [Guid]
     GO

     ALTER TABLE [dbo].[Notification] ADD  CONSTRAINT [DF_Notification_CreatedAt]  DEFAULT (getutcdate()) FOR [CreatedAt]
     GO
     ```

   - Napunite tablicu podacima: https://pastebin.com/tG9m4guk

2. U Visual Studiju stvorite novu RESTful Web API aplikaciju sa sljedećim karakteristikama:

   - Naziv (solution): exercise-4
   - Tip projekta: Web API
   - Naziv projekta: exercise-4-1
   - Bez autentifikacije i bez HTTPS
   - Neka koristi kontrolere i Swagger

   Također:

   - Izbrišite nepotrebni kontroler i model `WeatherForecast`
     > Ponekad kada se ukloni posljednji kontroler, mapa `Controllers` nestaje. Možete jednostavno prebaciti `Solution Explorer` na `Show All Files` i uključiti mapu u projekt
   - Neka Web API port bude 5123

3. Instalirajte alate i pakete potrebne za pristup bazi podataka

   Kada projekt treba pristup bazi podataka putem Entity Frameworka, ova instalacija je prva stvar koju trebate napraviti. Morate instalirati potrebne pakete, inače nećete moći koristiti Entity Framework u svom projektu.  
   To je nešto što trebate učiniti za svaki projekt koji zahtijeva izravan pristup bazi podataka.  

   - Otvorite Package Manager Console u VS.Net (Tools > NuGet Package Manager > Packet Manager Console)
   - Instalirajte potrebni alat `ef` putem naredbe `dotnet`
     ```
     dotnet tool install --global dotnet-ef --version 8
     ```
     > Ako je taj alat već instaliran, dobit ćete poruku o pogrešci.  
     > `dotnet : Tool 'dotnet-ef' is already installed.`
     >
     > To je u redu.
   - `cd` u projektnu mapu
   - Instalirajte sljedeće pakete u projekt
     - Microsoft.EntityFrameworkCore
     - Microsoft.EntityFrameworkCore.Design
     - Microsoft.EntityFrameworkCore.SqlServer
     > Napomene:  
     > - Ako dobijete grešku `dotnet : Could not find any project in ...`, onda niste u mapi projekta
     > - Ako dobijete grešku `dotnet : Found more than one project in ...`, otvorite mapu projekta i izbrišite .csproj datoteku s `Backup` tekstom u nazivu
   - Primjer instalacije za jedan paket:
     ```
     dotnet add package Microsoft.EntityFrameworkCore --version 8
     ```
   - Učinite to za sva 3 paketa i provjerite jesu li instalirani u `Dependencies` mapi `Solution Explorera`, 
      > Morate odabrati `File > Save All` da biste spremili rješenje/projekt s dodanim zavisnostima
   - Provjerite jesu li sve 3 zavisnosti prisutne u `Dependencies > Packages` u Solution Exploreru

4. Pripremite EF kontekst i modele pomoću alata `dotnet ef`

   Ovaj je korak obično dio scenarija "database first", kada se počinje s bazom podataka i iz nje se generiraju modeli.

    > Napomena: kada koristite scenarij "code first", ovo nije potrebno. Nećemo koristiti scenarij "code first" pa nam je ipak potreban ovaj korak.

   - Provjerite radi li alat ispravno koristeći sljedeću naredbu u konzoli Package Manager (ne smije ispisivati pogrešku): `dotnet ef`

    > Napomene:
    > - Ako korištenje alata završava pogreškom, ponovno pokrenite Visual Studio i ponovno otvorite projekt.  
    > - Može se dogoditi da na sustavima gdje nemate administratorske ovlasti ipak ne možete ovako koristiti alat. Alat je instaliran u mapi `C:\Users\{your-username}\.dotnet\tools\dotnet-ef.exe`. Tada ga možete koristiti na način `{...full path...}\dotnet-ef`
    >   - koristite naredbu `whoami` da biste saznali koje je vaše korisničko ime
    > - ako posumnjate da uopće nemate instaliran alat, možete koristiti `dotnet tool list -g` da biste to provjerili

   - Automatski generiraj modele za bazu podataka pomoću odgovarajućeg connection stringa (provjerite i uredite ako je potrebno)

     ```
     # Authentication using windows user (not in classroom, probably your PC at home)
     dotnet ef dbcontext scaffold "server=.;Database=GamePlatformRWA;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true" Microsoft.EntityFrameworkCore.SqlServer -o Models --force

     # If dotnet ef is not available...
     C:\Users\lecturerf6\.dotnet\tools\dotnet-ef.exe dbcontext scaffold ...
     ```

     > Napomene:
     > - ako se pojavi pogreška `No project was found. Change the current working directory or use the --project option.`, ako se pojavi pogreška `Nijedan projekt nije pronađen. Promijenite trenutni radni direktorij ili koristite opciju --project.`, to znači da niste u mapi projekta
     > - ako dobijete pogrešku `Cannot open database ... requested by the login. The login failed.`, najvjerojatnije imate netočan naziv baze podataka u connection stringu

5. U Solution Exploreru trebali biste vidjeti novu mapu `Models`. Otvorite mapu `Models` i promatrajte rezultat:

   - Models/Exercise4Context.cs
     - promotrite `DbSet` kolekcije koje predstavljaju zapise baze podataka
     - promotrite `OnConfiguring()` i hardkodirani connection string
     - promotrite `OnModelCreating()` i konfiguraciju sheme baze podataka (generiranu)
   - Models/Notification.cs
     - promotrite generirane klase

6. Registrirajte EF kontekst u DI spremniku

   Kada vaše rješenje treba pristup bazi podataka putem Entity Frameworka, ovo također trebate napraviti. Međutim, u slučaju da postoji više projekata, ovaj dio se radi samo u **startup** projektu. Morate konfigurirati connection string u svojim postavkama i riješiti se hardkodiranog connection stringa. Također, trebate dodati dbcontext uslugama za DI spremnik (kontejner) da biste pravilno riješili svoj db kontekst.

   - otvorite appsettings.json konfiguracijsku datoteku i dodajte sekciju "ConnectionStrings" u konfiguraciju, s Vašim postavkama connection stringa (promijenite ako je potrebno)

     ```
     {
       ...
       "AllowedHosts": "*",
       "ConnectionStrings": {
         "Exercise4ConnStr": "server=.;Database=Exercise4;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
       }
     }
     ```

   - koristite tu konfiguraciju u OnConfiguring() umjesto hardkodiranog connection stringa; također možete ukloniti poruku upozorenja (warning)

     ```
     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
       => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Exercise4ConnStr");
     ```

   - `Program.cs`: dodajte registraciju konteksta baze podataka u DI kontejner

     > O ubacivanju zavisnosti govorit ćemo u našim narednim predavanjima i koristiti ga u našim daljnjim vježbama.
     >
     > Dodajte sljedeći blok koda nakon redaka koji počinju s `builder.Services...`

     ```
     builder.Services.AddDbContext<Exercise4Context>(options => {
         options.UseSqlServer("name=ConnectionStrings:Exercise4ConnStr");
     });
     ```

     Sada je projekt spreman za pristup bazi podataka.

7. Sada ćete stvoriti kontroler koji može pristupiti podacima baze podataka putem EF konteksta. Pročitajte upute.

   - Da bi vaš kontroler mogao raditi s bazom podataka, morate primiti parametar db konteksta preko konstruktora. Framework će osigurati da dobijete parametar konteksta - za to koristi DI.

   - Stvorite novi API kontroler "with read/write actions" pod nazivom `NotificationsController`
   - Dodajte konstruktor tom API kontroleru, s jednim parametrom `Exercise4Context context`; pohranite taj parametar u lokalno readonly polje.

     ```
     private readonly Exercise4Context _context;

     public NotificationsController(Exercise4Context context)
     {
         _context = context;
     }
     ```

     > Napomena: ovo je tipično rukovanje parametrima proslijeđenim preko DI (ubacivanja zavisnosti). Dobivate ih preko konstruktora i pohranjujete kao readonly članove klase.

8. Dohvaćanje podataka i vraćanje klijentu:

   - koristite `Get()` akciju za to

     ```
     [HttpGet]
     public ActionResult<IEnumerable<Notification>> Get()
     {
         try
         {
             return Ok(_context.Notifications);
         }
         catch (Exception ex)
         {
             return StatusCode(500, ex.Message);
         }
     }
     ```

   - testirajte novu krajnju točku u Swaggeru
   - koristite debugger da biste vidjeli što se točno događa u akciji

### 4.2 Ažuriranje modela kada se struktura baze podataka promijeni

Obično biste promijenili strukturu baze podataka ili njezin dio (kao inkrementalnu promjenu), dok se zahtjevi mijenjaju. To može uključivati modificiranje ili uklanjanje postojećih tablica, dodavanje novih tablica, promjenu ključeva itd. Morate ažurirati model baze podataka kako bi odražavao vaše promjene.

1. Ažurirajte strukturu tablice baze podataka

   - Uklonite `UpdatedAt` stupac iz tablice
   - Ažurirajte `Body` stupac da bude tipa `nvarchar(2048) NOT NULL`
   - Dodajte `Priority` stupac tipa `int NULL`

2. Regenerirajte modele

   - Ponovno generiraj modele za bazu podataka - upotrijebi dodatnu zastavicu `--force` da biste pisali preko postojećih modela
     ```
     dotnet ef dbcontext scaffold "server=.;Database=Exercise4;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true" Microsoft.EntityFrameworkCore.SqlServer -o Models --force
     ```
     > Napomene: 
     > - bez zastavice `--force` dobit ćete odgovor poput "The following file(s) already exist in directory", a nijedan model ili kontekst neće biti osvježen
     > - after the update, database context class will again contain hardcoded connection string. To avoid that, you should be able to use parameterized connection string
     > - nakon ažuriranja, klasa db konteksta ponovno će sadržavati hardkodirani niz veze. Da biste to izbjegli, trebali biste koristiti parametrizirani connection string
     ```
     dotnet ef dbcontext scaffold "name=ConnectionStrings:Exercise4ConnStr" Microsoft.EntityFrameworkCore.SqlServer -o Models --force
     ```
   - Podržane zastavice:
     - `--context NewDatabaseContext` - stvaranje drugog db konteksta
     - `--data-annotations` - umjesto "fluent" API-ja, koristite validacijske anotacije kod izrade modela
     - `--no-build` - sprječava buildanje aplikacije prije izvršavanja naredbe
     - `--verbose` - ispis detalja o izvršavanja naredbe
     - `--help` - ispisuje pomoć
