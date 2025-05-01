# RESTful arhitektura (Web API)

Ovaj materijal je dio Ishoda 2 (minimalno).

## 5 Pohrana stanja (nastavak)

Kontekst u Entity Frameworku:

-   https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/

navigacijska svojstva u Entity Frameworku:

-   https://learn.microsoft.com/en-us/ef/core/modeling/relationships
-   https://learn.microsoft.com/en-us/ef/core/modeling/relationships/navigations

Validacija modela u ASP.NET Web API:

-   https://learn.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/model-validation-in-aspnet-web-api
-   https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=net-8.0

### 5.1 Postavljanje vježbe

Prisjetimo se posljednje vježbe, napravljeni su sljedeći koraci za postavljanje pristupa bazi podataka u projektu:

- osigurali ste da je instaliran alat `dotnet ef`
- instalirali ste potrebne NuGet pakete u svoj projekt
- koristili ste naredbu `dotnet ef dbcontext scaffold` za generiranje konteksta baze podataka i modela
- konfigurirali ste connection string u konfiguraciji
- registrirali ste potrebni tip (kontekst baze podataka) u ubacivanju zavisnosti
- upotrijebili ste kontekst baze podataka u svom kontroleru za dohvaćanje podataka

Iskoristite projekt koji ste izradili i postavili tijekom zadnje vježbe. Dostupan je na Infoeduki.

> Imajte na umu da ćete morati ponovno postaviti bazu podataka (brisati/stvoriti) i koristiti skriptu iz prošle vježbe za ponovno stvaranje strukture baze podataka. Također, koristite skriptu koja se nalazi na PasteBin URL-u da biste popunili tablicu podacima.
>
> Morat ćete pravilno konfigurirati pristup bazi podataka pomoću konfiguracijske datoteke appsettings.json.
>
> Vaš projekt i baza podataka pripremljeni su za pristup bazi podataka kada uspješno koristite GET krajnju točku za dohvaćanje podataka.

Također, dodajmo u bazi podataka postojećoj tablici još jednu tablicu s relacijom 1-na-N. Sljedeća skripta će dodati i tablicu i relaciju prema postojećoj tablici u bazu. Napunite ju podacima. Dodati će i reference iz `Notification` prema `NotificationType`.

```SQL
CREATE TABLE dbo.NotificationType (
  Id int NOT NULL IDENTITY (1, 1),
  [Name] nvarchar(256) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE dbo.NotificationType
ADD CONSTRAINT PK_NotificationType PRIMARY KEY CLUSTERED (
  Id
)
GO

ALTER TABLE dbo.[Notification]
ADD NotificationTypeId int NULL
GO

ALTER TABLE dbo.[Notification]
ADD CONSTRAINT FK_Notification_NotificationType
FOREIGN KEY (NotificationTypeId)
REFERENCES dbo.NotificationType(Id)
GO

SET IDENTITY_INSERT [dbo].[NotificationType] ON
GO
INSERT [dbo].[NotificationType] ([Id], [Name]) VALUES (1, N'Normal')
INSERT [dbo].[NotificationType] ([Id], [Name]) VALUES (2, N'Prioritized')
INSERT [dbo].[NotificationType] ([Id], [Name]) VALUES (3, N'Urgent')
GO
SET IDENTITY_INSERT [dbo].[NotificationType] OFF
GO

UPDATE [Notification]
SET NotificationTypeId = 3
WHERE CreatedAt BETWEEN '2023-03-01' AND '2023-03-02'
GO

UPDATE [Notification]
SET NotificationTypeId = 2
WHERE CreatedAt BETWEEN '2023-03-02' AND '2023-03-05'
GO

UPDATE [Notification]
SET NotificationTypeId = 1
WHERE NotificationTypeId IS NULL
GO
```

Konačno, upotrijebite naredbu `dotnet ef` za ponovno generiranje modela. Obratite pažnju na postavljanje ispravnog connection stringa. Također, obratite pažnju na pokretanje naredbe u mapi projekta.

```
dotnet ef dbcontext scaffold "server=.;Database=Exercise4;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true" Microsoft.EntityFrameworkCore.SqlServer -o Models --force
```

### 5.2 Implementacija CRUD-a

Osnovna implementacija CRUD-a u biti je ista kao kod korištenja statičkog polja. Važna razlika je spremanje promjena u bazu podataka nakon izvođenja promjena.

Implementirajte CRUD za tablicu `NotificationType`. Ne zaboravite koristiti konstruktor za dobivanje konteksta baze podataka iz DI spremnika (pogledajte kako se to radi u kontroleru `Notification`).

1. Implementirajte akciju `Get()`
     - umjesto statičkog člana koristite `_context.NotificationTypes` kao kolekciju
2. Implementirajte akciju `Get(int id)`
     - umjesto statičkog člana koristite `_context.NotificationTypes` kao kolekciju
3. Implementirajte akciju `Post([FromBody] NotificationType value)`
     - ne morate izračunati sljedeći id, baza podataka će to učiniti umjesto vas (pogledajte: `IDENTITY(1,1)`)
     - nakon dodavanja nove vrste obavijesti (NotificationType), pozovite `_context.SaveChanges()`
4. Implementirajte akciju `Put(int id, [FromBody] NotificationType value)`
     - nakon izmjene tipa obavijesti (NotificationType), pozovite `_context.SaveChanges()`
5. Implementirajte akciju `Delete(int id)`
     - nakon uklanjanja vrste obavijesti (NotificationType), pozovite `_context.SaveChanges()`

> Primijetite da postoji član `Notifications` u klasi `NotificationType`. Ovo je **navigacijsko svojstvo**. Ponekad je to korisna značajka, ali ponekad nam nije potrebna
>
> - kada želimo vidjeti povezane podatke, trebamo to navigacijko svojstvo i tada moramo **uključiti** te referencirane podatke
> - kada ne želimo vidjeti povezane podatke, moramo mapirati podatke u DTO koji nema navigacijko svojstvo

### 5.3 Uključivanje referenciranih podataka

Za uključivanje navedenih podataka u rezultat upotrijebite `.Include()`

```C#
[HttpGet("{id}")]
public ActionResult<NotificationType> Get(int id)
{
    try
    {
        var result =
            _context.NotificationTypes
                .Include(x => x.Notifications)
                .FirstOrDefault(x => x.Id == id);

        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}
```

> Napomena: kada to učinite, Swagger će vam pokazati pogrešku: `A possible object cycle was detected.` Razlog je serijalizacija ciklusa objekta. `U Program.cs` trebate dati upute ASP.NET Web API-ju da zanemari cikluse referenciranja objekata.
>
> ```
> builder.Services.AddControllers().AddJsonOptions(x =>
>   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
> ```

Sada uredite `NotificationsController` da biste dobili podatke i uključili referencirane `NotificationTypes` podatke.

> Imajte na umu da ima više smisla uključiti `NotificationType` u podatke `Notification` nego obrnuto.

### 5.4 Korištenje DTO-a za izuzimanje referenciranih podataka iz rezultata

To exclude the referenced data, you will have to create the class that doesn't have the data you want to exclude. The class is usually referenced as **Data Transfer Object** or DTO.

Da biste isključili referencirane podatke, morat ćete stvoriti klasu koja ne sadrži podatke koje želite izuzeti. Objekt takve klase se obično naziva **Data Transfer Object** ili DTO.

```C#
// Create this e.g. in new folder DTOs
public class NotificationTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// You need this in your controller
[HttpGet]
public ActionResult<IEnumerable<NotificationTypeDto>> Get()
{
    try
    {
        var result = _context.NotificationTypes;
        var mappedResult = result.Select(x =>
            new NotificationTypeDto
            {
                Id = x.Id,
                Name = x.Name
            });

        return Ok(mappedResult);
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}

[HttpGet("{id}")]
public ActionResult<NotificationTypeDto> Get(int id)
{
    try
    {
        var result =
            _context.NotificationTypes
                .FirstOrDefault(x => x.Id == id);

        var mappedResult = new NotificationTypeDto
        {
            Id = result.Id,
            Name = result.Name
        };

        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}
```

### 5.5 Korištenje DTO klasa za stvaranje, modificiranje i brisanje

Korištenje EF klasa sa svojstvima navigacije kao inputa za stvaranje, modificiranje i brisanje je loše. Radije biste trebali poslati DTO klase kao ulaze, a zatim mapirati te klase u EF klase.

Dakle, trebali biste imati na umu:

- Dohvat kolekcije podataka: mapirajte zbirku EF klasa u zbirku DTO klasa
- Dohvat jednog podatka: mapirajte EF klasu u DTO klasu
- Stvaranje podatke: prihvatite DTO klasu, mapirajte je u EF klasu
- Ažuriranje podatke: prihvatite DTO klasu, mapirajte je u EF klasu
- Brisanje podataka: nema mapiranja

> Napomena: korištenje DTO-a kao klase koja prenosi podatke između klijenta i poslužitelja je **uvijek** dobra ideja. Čak i kada su vam potrebna navigacijska svojstva - na isti način možete mapirati navigacijska svojstva.

### 5.6 Korištenje DTO klasa za validaciju

DTO klase također se koriste i za validaciju.
Za to možete koristiti imenski prostor (engl. namespace) `DataAnnotation`.
Atributi u imenskom prostoru DataAnnotation koriste se za označavanje validiranih polja u DTO-u.

```C#
public class NotificationTypeDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "You need to enter the name")]
    public string Name { get; set; }
}

public class NotificationDto
{
    public int Id { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Required(ErrorMessage = "Receiver is required.")]
    public string Receiver { get; set; } = null!;

    [Required(AllowEmptyStrings = true)]
    [StringLength(256, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
    public string? Subject { get; set; }

    [StringLength(2048, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
    public string Body { get; set; } = null!;

    public DateTime? SentAt { get; set; }

    [Range(1, int.MaxValue)]
    public int? NotificationTypeId { get; set; }
}
```

Da biste validirali DTO, morate:

-   isključiti automatsku grešku 400

```
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
  options.SuppressModelStateInvalidFilter = true;
});

```

-   provjeriti svojstvo `ModelState.IsValid` u svojoj akciji

```
[HttpPost]
public ActionResult<NotificationTypeDto> Post([FromBody] NotificationTypeDto value)
{
    try
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newNotificationType = new NotificationType
        {
            Name = value.Name,
        };

        _context.NotificationTypes.Add(newNotificationType);

        _context.SaveChanges();

        value.Id = newNotificationType.Id;

        return Ok(value);
    }
    catch (Exception)
    {
        return BadRequest();
    }
}

[HttpPost]
public ActionResult<NotificationDto> Post([FromBody] NotificationDto value)
{
    if(!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var newNotification = new Notification
    {
        UpdatedAt = value.UpdatedAt,
        Receiver = value.Receiver,
        Subject = value.Subject,
        Body = value.Body,
        SentAt = value.SentAt,
        NotificationTypeId = value.NotificationTypeId,
    };

    _context.Notifications.Add(newNotification);

    _context.SaveChanges();

    value.Id = newNotification.Id;

    return value;
}
```

### 5.7 Implementacija pretraživanja, sortiranja i straničenja (engl. paging)

Operacija pretraživanja sa sortiranjem i straničenjem može se implementirati na isti način na koji smo to učinili za statičku kolekciju.

> _NAPOMENA: ne morate navesti `StringComparison.OrdinalIgnoreCase` za usporedbu, ignoriranje velikih i malih slova u tekstualnoj usporedbi je zadano za SQL Server._

### 5.8 Vježba: Podržite CRUD za tablicu obavijesti

Podržite CRUD za tablicu `Obavijesti`.
Koristite klasu `NotificationDto` kao DTO da biste izbjegli navigacijska svojstva u komunikaciji između klijenta i poslužitelja.

### 5.9 Vježba: Log tablica

Kreirajte novo rješenje koje podržava bilježenje (logiranje) u bazu podataka.

Log tablica se sastoji od:

- identifikatora (autogenerani, primarni ključ)
- vremenska oznaka (timestamp, datum i vrijeme),
- razina loga (broj od 1 do 5),
- poruka (tekst, 1024 znaka)
- tekst greške (tekst, najveći podržani broj znakova)

Napravite `LogController` s akcijama:

- Post(Log log) - dodaje log u tablicu baze podataka
- Post(Log[] logs) - dodaje više zapisa u tablicu baze podataka
- Delete(int n) - briše prvih n zapisa
- Get(int n, int orderBy) - dohvaća posljednjih n zapisa, poredanih po ID-u, vremenskoj oznaci ili poruci

### 5.10 Vježba: Baza adresa

Podržite bazu podataka adresa sa sljedećim entitetima:

- ulica (CRUD)
- kućni broj (CRUD)

Koristite DTO.
Koristite validaciju u DTO-ovima.
