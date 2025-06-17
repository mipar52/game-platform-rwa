# MVC arhitektura

Ovaj materijal dio je ishoda učenja 4 (minimum).

Atributi validacije:

-   https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-8.0#built-in-attributes

## 10 MVC arhitekturu i podrška za model

-   ViewModel
-   Model s povezanim podacima
-   Atributi označavanja
-   Validacija modela i atributi validacije
-   Poslužiteljska validacija modela 
-   Klijentska validacija modela
-   Oznake validacije i sažetak validacije

### 10.1 Postavljanje vježbe

Postavljanje baze podataka: kreirajte bazu podataka `Exercise10` sa sljedećom strukturom.

```SQL
CREATE DATABASE Exercise10
GO

USE Exercise10
GO

CREATE TABLE Genre (
  [Id] [int] IDENTITY(1,1) NOT NULL,
  [Name] [nvarchar](256) NOT NULL,
  [Description] [nvarchar](max) NOT NULL,
  PRIMARY KEY ([Id])
)
GO

SET IDENTITY_INSERT Genre ON
GO

INSERT INTO Genre (Id, [Name], [Description])
VALUES
  (1, 'Rock', 'Otherwise known as ‘Rock & Roll,’ rock music has been a popular genre since the early 1950s.'),
  (2, 'Jazz', 'Identifiable with blues and swing notes, Jazz has origins in European and West African culture.'),
  (3, 'Electronic Dance Music', 'Typically referred to as EDM, this type of music is created by DJs who mix a range of beats and tones to create unique music.'),
  (4, 'Dubstep', 'Dubstep is an electronic dance music subgenre that originated in the late 1990s’ in South London.'),
  (5, 'Techno', 'Techno is yet another sub-genre of electronic dance music. This genre became popular in Germany towards the end of the 1980s and was heavily influenced by house music, funk, synthpop, and futuristic fiction.'),
  (6, 'Rhythm and Blues (R&B)', 'R & B is one of the world’s top music genres combining gospel, blues, and jazz influences.'),
  (7, 'Country', 'Country music is another one of the world’s top music genres. Originating in the 1920s, Country has its roots in western music and American folk.'),
  (8, 'Pop', 'The term ‘Pop’ is derived from the word ‘popular.’ Therefore, Pop music is a genre that contains music generally favored throughout society.'),
  (9, 'Indie Rock', 'In terms of genre, Indie Rock lies somewhere between pop music and rock and roll.'),
  (10, 'Electro', 'Electro blends electronic music and hip hop to create music that is similar to disco in sound.')
GO

SET IDENTITY_INSERT Genre OFF
GO

CREATE TABLE Artist (
  Id int NOT NULL IDENTITY (1, 1),
  [Name] nvarchar(256) NOT NULL
  CONSTRAINT PK_Artist
    PRIMARY KEY (Id)
)

SET IDENTITY_INSERT Artist ON
GO

INSERT INTO Artist (Id, [Name])
VALUES
  (1, 'Tina Turner'),
  (2, 'Van Halen'),
  (3, 'DJ Snake'),
  (4, 'Louis Armstrong')
GO

SET IDENTITY_INSERT Artist OFF
GO

CREATE TABLE Song (
  Id int NOT NULL IDENTITY (1, 1),
  [Name] nvarchar(256) NOT NULL,
  [Year] int NULL,
  GenreId int NOT NULL,
  ArtistId int NOT NULL,
  DeletedAt datetime2(7) NULL,
  CONSTRAINT PK_Song
    PRIMARY KEY (Id),
  CONSTRAINT FK_Song_Genre
    FOREIGN KEY(GenreId)
    REFERENCES dbo.Genre (Id),
  CONSTRAINT FK_Song_Artist
    FOREIGN KEY(ArtistId)
    REFERENCES dbo.Artist (Id)
)

SET IDENTITY_INSERT Song ON
GO

INSERT INTO Song (Id, [Name], [Year], GenreId, ArtistId)
VALUES
  (1, 'What''s Love Got to Do with It', 1984, 8, 1),
  (2, 'The Best', 1989, 8, 1),
  (3, 'Jump', 1984, 1, 2),
  (4, 'Lean On', 2015, 10, 3),
  (5, 'What a Wonderful World', 1967, 2, 4),
  (6, 'We Have All The Time In The World', 1969, 2, 4)
GO

SET IDENTITY_INSERT Song OFF
GO
```

Sljedeće je već dovršeno kao "starter" projekta:

> Postavljanje modela i repozitorija (za detalje pogledajte prethodnu vježbu):
>
> -   Instalirajte EF pakete u projekt
> -   Konfigurirajte EF connection string u `appsettings.json`
> -   Napravite reverse engineering baze podataka i postavite servis u `Program.cs`
>
> "Launch settings" postavke:
>
> -   Postavite port na 6555
>
> Stvorite CRUD prikaze i funkcionalnost za `GenreController`
>
> -   Koristite "MVC Controller with read/write actions" predložak za stvaranje `GenreController`
> -   Proslijedite db kontekst kontroleru pomoću konstruktorskog DI
> -   Napravite viewmodel `GenreVM` (Id, Naziv, Opis) u mapi `ViewModels`
> -   Koristite `Add View`, `Razor View`, `Template: {required template}`, `Model`: `GenreVM`
> -   `Index`, `Template`: `List`
>     -   Popravite ovaj dio Razor predloška da ispravno stvorite akcijske poveznice
>         ```C#
>         @Html.ActionLink("Edit", "Edit", new { id=item.Id }) |
>         @Html.ActionLink("Details", "Details", new { id=item.Id }) |
>         @Html.ActionLink("Delete", "Delete", new { id=item.Id })
>         ```
>     -   U predlošku upotrijebite neki CSS stil ili Bootstrap za fino podešavanje prikaza
>     -   Implementirajte akciju GET
> -   `Details`: `Template`: `Details`
>     -   Popravite ovaj dio Razor predloška da ispravno stvorite akcijske poveznice
>         ```C#
>         @Html.ActionLink("Edit", "Edit", new { id = Model.Id })
>         ```
>     -   U predlošku upotrijebite neki CSS stil ili Bootstrap za fino podešavanje prikaza
>     -   Implementirajte GET akciju
> -   `Create`: `Template`: `Create`
>     -   Ažurirajte `POST` akciju tako da prihvaća viewmodel parametar
>     -   Implementirajte GET i POST akciju
> -   `Edit`: `Template`: `Edit`
>     -   Ažurirajte `POST` akciju tako da prihvaća viewmodel parametar
>     -   Implementirajte GET i POST akcije
> -   `Delete`: `Template`: `Delete`
>     -   Ažurirajte akciju `POST` da prihvaća viewmodel parametar
>     -   Implementirajte GET i POST akcije
> -   Ažurirajte navigaciju na stranici "layout" - u \_Layout.cshtml, dodajte vezu za navigaciju na stranicu popisa žanrova
>     ```HTML
>     <li class="nav-item">
>         <a class="nav-link text-dark" asp-area="" asp-controller="Genre" asp-action="Index">Genres</a>
>     </li>
>     ```
>     Stvorite CRUD prikaze i funkcionalnosti za `ArtistController` na isti način kao `GenreController`.

### 10.2 Viewmodel: Stvorite i postavite kontroler za entitet Song iz CRUD predloška

Koristite predložak "MVC Controller with read/write actions" za stvaranje kontrolera `SongController`.
Proslijedite db kontekst kontroleru pomoću konstruktorskog DI.
Stvorite viewmodel `SongVM` u mapi `ViewModels`:

-   int Id
-   string Name
-   int Year
-   int GenreId
-   int ArtistId

### 10.3 Implementirajte indeks akciju za prikaz popisa pjesama

Na akciju `Index` kontrolera `SongController` kliknite desnom tipkom miša i odaberite:

-   Add View, Razor View
-   Template: List
-   Model: `SongVM`

Implementirajte GET akciju.

> SAVJET: možete upotrijebiti kod koji je već dostupan u `GenreController` i prilagoditi ga za rad na `SongController`.

Ažurirajte navigaciju na stranici izgleda - u \_Layout.cshtml, dodajte navigacijsku poveznicu na stranicu popisa pjesama.

### 10.4 Popravite prikaz za popis pjesama

Postoje neki problemi s prikazom:

-   postoje "gole" nestilizirane poveznice
-   postoji `Id` u tablici

Da biste ih popravili, pogledajte `Views > Artists > Index.cshtml`.

### 10.5 Zamijenite ID-eve (strani ključevi) povezanim vrijednostima

Imamo model s podacima koji je povezan s drugim modelom - `Song` je povezan sa `Genre` i `Artist`.

Prvo riješimo problem gdje su podaci `Genre` i `Artist` u obliku `Id` umjesto teksta.
Uključite entitete `Genre` i `Artist` u LINQ upit.
Zatim proširite viewmodel pjesme kako biste uključili stringove za `Genre` i `Artist` (GenreName, ArtistName).

```C#
var songVms = _context.Songs
    .Include(x => x.Genre)
    .Include(x => x.Artist)
    .Select(x => new SongVM
        {
            Id = x.Id,
            Name = x.Name,
            Year = x.Year ?? 0,
            ArtistId = x.ArtistId,
            ArtistName = x.Artist.Name,
            GenreId = x.GenreId,
            GenreName = x.Genre.Name,
    })
    .ToList();
```

Sada možete zamijeniti `GenreId` s `GenreName` u `Song/Index.cshtml`.
Učinite isto za `Artist`.

> Napomena: to trebate učiniti na dva mjesta u prikazu - naslov (labela) i podaci.

### 10.6 Atributi labeliranja/označavanja: postavite nazive u viewmodelu

U `Index.cshtml`, sljedeća sintaksa će ispisati podatke:

```C#
@Html.DisplayFor(modelItem => item.Name)
```

Sljedeća sintaksa će ispisati labelu za podatak:

```C#
@Html.DisplayNameFor(modelItem => item.Name)
```

Prema zadanim postavkama, labela podataka je ista kao naziv svojstva podataka.  
Pomoću atributa [Display] možete promijeniti labelu podataka.

```C#
public class SongVM
{
    public int Id { get; set; }
    [Display(Name = "Song Name")]
    public string Name { get; set; }
    public int Year { get; set; }
    public int GenreId { get; set; }
    [Display(Name = "Genre")]
    public string GenreName { get; set; }
    public int ArtistId { get; set; }
    [Display(Name = "Artist")]
    public string ArtistName { get; set; }
}
```

Promatrajte promjenu u zaglavlju tablice popisa pjesama.

### 10.7 Koristite labelu postavljenu u viewmodelu za druge prikaze

Dodajte `Create` prikaz (`Create.cshtml`) za entitet `Song`.

-   _Add View_, _Razor View_, _Template: Create_, _Model class: SongVM_
-   ne trebamo ID, uklonite taj odjeljak iz HTML-a

Upotrijebite predložak `Create`, ažurirajte akciju `POST Create` tako da prihvaća kao parametar model prikaza `SongVM`.
Dodajte novu pjesmu u bazu podataka.

-   za novu pjesmu postavite svojstva: `Name`, `Year`, `GenreId`, `ArtistId`
-   pogledajte kako je to napravljeno u `POST Create` za `GenreVM`

Sada možete stvoriti novu pjesmu, ali stvaranje je malo nezgrapno jer koristi FK ID-eve.

> Kada pokrenete i testirate funkcionalnost stvaranja, primijetit ćete da `GenreName` i `ArtistName` nemaju smisla, stoga ih uklonite iz predloška.  
> Također ćete zamijeniti `GenreId` i `ArtistId` padajućim izbornicima.

### 10.8 Related data: Replace ID textboxes with dropdowns

Za npr. `GenreId`, umjesto oznake `<input>` potrebna vam je oznaka `<select>` s opcijama.

-   zamijenite `<input>` oznakom `<select>` (oznaka "select" mora biti pravilno zatvorena: `<select ... ></select>`)
-   zamijenite `class="form-control"` s `class="form-select"` da bi se ispravno prikazao Bootstrap element za odabir
-   sada su vam potrebni podaci za `<option>` oznake da biste ih prikazali u samom padajućem izborniku
    -   popunite te podatke u `GET Create`
        ```C#
        ViewBag.GenreDdlItems =_context.Genres
          .Select(x => new SelectListItem
          {
              Text = x.Name,
              Value = x.Id.ToString()
          });
        ```
    -   referencirajte te podatke u Razor predlošku, atribut `asp-items`; opcije će biti automatski generirane  
        ```HTML
        <select asp-for="GenreId" asp-items="ViewBag.GenreDdlItems" class="form-select"></select>
        ```
    -   možete dodati "(select item)" s praznom vrijednošću kako biste prisilili korisnika da odabere stavku
        ```HTML
        <select asp-for="GenreId" asp-items="ViewBag.GenreDdlItems" class="form-select">
          <option value="">(select item)</option>
        </select>
        ```
-   konačno, dodajte u model "display" atribut za GenreId u viewmodel, za prikaz odgovarajuće oznake (kao što ste učinili u prethodnom zadatku)

Učinite isto za entitet `Artist`.

### 10.9 Podržite funkciju uređivanja

Dodajte prikaz za uređivanje za entitet `Song`.  
Ažurirajte prikaz (upotrijebi "select" umjesto "input" itd.)  
Ažurirajte akciju `GET Edit` da dohvatite odgovarajuće podatke iz baze podataka i proslijedite ih prikazu (pogledajte kako se to radi u `GET Edit` za `GenreVM`).  
Za `Get Edit` također koristite ista `ViewBag` svojstva `GenreDdlItems` i `ArtistDdlItems` kao što ste to učinili za `GET Create`.  
Ažurirajte `Edit.cshtml` prikaz `<select>` za prikaz stavki (`asp-items`). 
Ažurirajte akciju `POST Edit` da biste prihvatili `SongVM` viewmodel parametar (pogledajte kako se to radi u `POST Edit` za `GenreVM`).

### 10.10 Podrška funkcionalnosti prikaza/brisanja

Možete koristiti ono što ste već naučili da biste podržali ove potrebne funkcionalnosti.

### 10.11 Validacija modela i atributi validacije

_Klijentska validacija_

Kada izradite model pomoću automatskog generiranja i zadanih vrijednosti, uključena je klijentska validacija. To znači da ako korisnik ne unese potrebne vrijednosti, prikazat će se pogreške:

-   `The Song Name field is required.`
-   `The Year field is required.`

Skripta koja je odgovorna za ovu funkcionalnost je `_ValidationScriptsPartial.cshtml`. Ako **zakomentirate** ovu skriptu iz predloška, klijentska validacija se NE izvodi i morate se osloniti na poslužiteljsku (serversku) validaciju.

Klijentska validacija se izvodi odmah na klijentu, što znači da se podaci ne moraju slati poslužitelju da bi bili validirani.

_Poslužiteljska validacija_

Do sada nismo koristili poslužiteljsku validaciju. Način da to učinite je da provjerite zastavicu `ModelState.IsValid`. Ovisno o toj zastavici, ili nastavljate kreirati instancu entiteta, ili zaustavljate i vraćate korisnika na točku gdje nije unio ispravne podatke (samo vratite View()).

```C#
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Create(SongVM song)
{
    try
    {
        if (!ModelState.IsValid) {
            return View();
        }

        var newSong = new Song
        {
            Name = song.Name,
            Year = song.Year,
            GenreId = song.GenreId,
            ArtistId = song.ArtistId,
        };

        _context.Songs.Add(newSong);

        _context.SaveChanges();

        return RedirectToAction(nameof(Index));
    }
    catch
    {
        return View();
    }
}
```

> NAPOMENA: sada, kada korisnik unese netočne podatke, nema stavki <opcija>.  
> Ispunite ih prije vraćanja prikaza.

### 10.12 Odgovarajuća validacija modela korištenjem validacijskih atributa

Možete koristiti ugrađene validacijske atribute u viewmodelu kako biste automatski provjerili što Vam je potrebno i dali odgovarajuće povratne informacije u slučaju pogreške.

Npr. za `Name` i `Year`:

```C#
[Required(ErrorMessage = "There's not much sense of having a song without the name, right?")]
public string Name { get; set; }

[Range(1000, 2024, ErrorMessage = "Invalid year for a song")]
public int Year { get; set; }
```

> Napomena: Ako **odkomentirate** validacijsku skriptu iz predloška, ​​možete vidjeti da i klijentska validacija također radi.

### 10.13 Validacijske oznake i sažetak validacije

Sljedeće automatski generirane oznake odgovorne su za povratne informacije o validaciji:

```HTML
<span asp-validation-for="Name" class="text-danger"></span>
```

Također, postoji još jedan tag koji je odgovoran za povratnu informaciju:

```HTML
<div asp-validation-summary="ModelOnly" class="text-danger"></div>
```

Ako se prebacite s `ModelOnly` na `All`, dobit ćete sve povratne informacije o validaciji u ul/li popisu koji možete oblikovati prema vlastitoj želji.

> Napomena: Što je `ModelOnly`? U slučaju da otkrijete pogrešku, možete dodati opću poruku o pogrešci koja nije povezana ni s jednim svojstvom modela.
>
> ```C#
> ModelState.AddModelError("", "Failed to create song");
> ```
>
> Dakle, `ModelOnly` sada će prikazivati ​​ove vrste pogrešaka.
>
> Važno: `asp-validation-summary` radi na ovaj način samo za poslužiteljsku validaciju.
