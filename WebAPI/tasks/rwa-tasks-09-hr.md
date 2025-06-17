# MVC arhitektura

Ovaj materijal dio je ishoda učenja 3 (minimum).

## 9 ASP.NET MVC i prikazi

Prikazi:

- https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview?view=aspnetcore-8.0

Pregled sintakse Razora:

- https://learn.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-8.0

HTML pomoćnici (engl. HTML Helpers):

- https://learn.microsoft.com/en-us/dotnet/api/system.web.mvc.htmlhelper?view=aspnet-mvc-5.2

Tag pomoćnici (engl. Tag Helpers):

- https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro?view=aspnetcore-8.0

### 9.1 Postavljanje vježbe

Postavljanje baze podataka:

- kreirati bazu podataka `Exercise9` sa sljedećom strukturom

    > Možete kopirati skriptu sa ove poveznice: https://pastebin.com/xcQKDLN8

    ```SQL
    CREATE DATABASE Exercise9
    GO

    USE Exercise9
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

    CREATE TABLE Song (
      Id int NOT NULL IDENTITY (1, 1),
      [Name] nvarchar(256) NOT NULL,
      [Year] int NULL,
      GenreId int NOT NULL,
      DeletedAt datetime2(7) NULL,
      CONSTRAINT PK_Song
        PRIMARY KEY (Id),
      CONSTRAINT FK_Song_Genre
        FOREIGN KEY(GenreId)
        REFERENCES dbo.Genre (Id)
    )

    SET IDENTITY_INSERT Song ON
    GO

    INSERT INTO Song (Id, [Name], [Year], GenreId, DeletedAt)
    VALUES
      (1, 'A-ha - Take On Me', 1985, 8, NULL),
      (2, 'Tina Turner - What''s Love Got to Do with It', 1984, 8, NULL),
      (3, 'Van Halen - Jump', 1984, 1, NULL),
      (4, 'Franz Ferdinand - Take Me Out', 2004, 9, NULL),
      (5, 'DJ Snake - Lean On', 2015, 10, NULL),
      (6, 'Louis Armstrong - What a Wonderful World', 1967, 2, NULL),
      (7, 'Deleted Song', 1967, 2, '2024-04-27 11:41:00')
    GO

    SET IDENTITY_INSERT Song OFF
    GO
    ```

Postavljanje solutiona:

- Stvorite MVC rješenje bez HTTPS podrške

Postavljanje modela i repozitorija:

- Instalirajte EF pakete u projekt
    ```
    dotnet add package Microsoft.EntityFrameworkCore --version 7
    dotnet add package Microsoft.EntityFrameworkCore.Design --version 7
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 7
    ```
    > Ne zaboravite se prebaciti (`cd`) u mapu projekta!
- Konfigurirajte EF connection string u `appsettings.json`
    ```JSON
    "ConnectionStrings": {
      "ex9cs": "server=.;Database=Exercise9;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
    }
    ```
    > Postavite točan connection string!
- Napravite reverse engineering baze podataka i postavite servis u `Program.cs`

    ```
    dotnet ef dbcontext scaffold "Name=ConnectionStrings:ex9cs" Microsoft.EntityFrameworkCore.SqlServer -o Models --force
    ```

    > Ako `dotnet ef` nije instaliran, instalirajte taj alat!
    > - `dotnet tool install --global dotnet-ef --version 7`  
    > Možda ćete trebati ponovno pokrenuti Visual Studio ako ste u učionici.

    ```C#
    builder.Services.AddDbContext<Exercise9Context>(options => {
        options.UseSqlServer("name=ConnectionStrings:ex9cs");
    });
    ```

"Launch settings" postavke:

- Postavite port na 6555

Dodavanje kontrolera:

- Koristite predložak "MVC Controller with read/write actions" za stvaranje kontrolera
- Nazivi kontrolera: `GenreController`, `SongController`
- Proslijedite db kontekst kontroleru pomoću konstruktorskog ubacivanja zavisnosti (engl. dependency Injection - DI) u **oba** kontrolera.

 - Primjer za `GenreController`:

        ```C#
        private readonly Exercise9Context _context;

        public GenreController(Exercise9Context context)
        {
            _context = context;
        }
        ```

"Index" akcija:

- U `GenreController` koristite akciju `Index()` za prikaz žanrova

 - Dodajte prazan Razor prikaz `Index.cshtml`
 - Koristite `ViewBag` za prosljeđivanje žanrova u prikaz

        ```
        public ActionResult Index()
        {
            ViewBag.Genres = _context.Genres;

            return View();
        }
        ```

 - U `Index.cshtml` prikazu preuzmite žanrove iz ViewBaga
        ```
        @{
            var genres = ViewBag.Genres as IEnumerable<Genre>;
        }
        ```
 - U `Index.cshtml` prikazu generirajte HTML kod iz tih podataka - koristite jednostavan ul/li HTML popis
        ```HTML
        <ul class="genre-list">
            @foreach (var genre in genres)
            {
                <li>
                    @genre.Name: @genre.Description
                </li>
            }
        </ul>
        ```

- U `SongController` koristite akciju `Index()` za prikaz pjesama
  - za svaku pjesmu prikažite naziv pjesme i godinu kada je pjesma izdana
  - primjer: "A-ha - Take On Me (1985)"

Ažurirajte navigaciju na "layout" stranici:

- u \_Layout.cshtml dodajte navigacijske poveznice na popis žanrova i popis pjesama.
  Primjer za kontroler `Genre`:
    ```HTML
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="" asp-controller="Genre" asp-action="Index">Genres</a>
    </li>
    ```

Testirajte navigaciju i stranice.

### 9.2 Stvorite prikaz s CSS stilom

Ovdje ćete napraviti podršku za određeni stil u prikazu u kojem Vam je to potrebno.

- Dodajte imenovanu sekciju u prikaz

    ```C#
    @await RenderSectionAsync("Styles", required: false)
    ```

    > Ovo je način da u "layout" prikazu rezervirate mjesto za JavaScript kod.

- Dodajte stil za žanr/indeks: dodajte podmapu `wwwroot/css/genres` i dodajte joj datoteku `index.css`

    ```CSS
    ul.genre-list {
        list-style-type: none;
        margin: 0;
        padding: 0;
    }

        ul.genre-list li {
            border: 1px solid black;
            margin: 1em;
            padding: 1em;
            color: #fff;
            background: rgb(2,0,36);
            background: linear-gradient(0deg, rgba(2,0,36,1) 0%, rgba(150,174,180,1) 100%);
            box-shadow: rgba(150,174,180,1) 5px 5px 5px;
        }
    ```

- Dodajte poveznicu na stil u prikaz - prikazat će se u rezerviranoj sekciji na "layout" prikazu
    ```HTML
    @section Styles {
        <link rel="stylesheet" href="~/css/genres/index.css" />
    }
    ```
- Za stiliziranje stranice s pjesmama možete koristiti potpuno isti tijek rada: dodajte stylesheet datoteku i poveznicu u predložak prikaza pjesama `Index.cshtml`

### 9.3 Upotrijebite ViewData i ViewBag za prijenos podataka s kontrolera na prikaz

- što god želite proslijediti prikazu, možete to učiniti iz akcije putem `ViewData` ili `ViewBag` strukture
- na primjer, u kodu akcije, prosljeđivanje podataka iz akcije u prikaz
    ```C#
    ViewData["genres"] = _context.Genres;
    //...is equivalent to...
    ViewBag.Genres = _context.Genres;
    ```
- na primjer, referenciranje podataka u prikazu
    ```C#
    @{
      var genres = ViewData["genres"] as IEnumerable<Genre>;
      //...is equivalent to...
      var genres = ViewBag.Genres as IEnumerable<Genre>;
    }
    ```
- tada je renderiranje HTML-a jednostavno
    ```HTML
    <ul class="genre-list">
        @foreach (var genre in genres)
        {
        <li>@genre.Name: @genre.Description</li>
        }
    </ul>
    ```

### 9.4 Prikaz podataka kao uobičajenih elemenata HTML obrasca

Upotrijebite akciju `Index` kontrolera `Song` za prikaz nekih uobičajenih elemenata HTML obrasca.

Akcija:

```C#
ViewBag.Songs = _context.Songs;
ViewBag.ExampleText = "Some text";
ViewBag.ExampleNumber = 1987;
ViewBag.Genres = _context.Genres;
```

Razor predložak:

```HTML
@{
  var genres = ViewBag.Genres as IEnumerable<Genre>;
}

<!-- ...Razor kod za popis pjesama... -->

<hr />

<form>
    <label>Text input: <input type="text" value="@ViewBag.ExampleText"></label><br />
    <label>Numeric input: <input type="number" value="@ViewBag.ExampleNumber"></label><br />
    <label>Select genre:
        <select>
            @foreach (var genre in genres)
            {
                <option value="@genre.Id">@genre.Name</option>
            }
        </select>
    </label>
</form>
```

### 9.5 Prikazivanje podataka kao HTML elemenata obrasca pomoću HTML pomoćnika

HTML pomoćnici (engl. HTML helpers) mogu smanjiti količinu koda koji pišete za prikaz HTML-a u vašem predlošku. Postoje pomoćnici koje jednostavno možete koristiti, poput `@Html.TextBox()`. Postoje i drugi koje trebate putem parametara _nahraniti_ određenim instancama objekata. Primjer je `@Html.DropDownList()` koji prima zbirku instanci `SelectListItem`.

Dakle, dodajmo kolekciju instanci objekta `SelectListItem` u `Index` akciju `Song` kontrolera i upotrijebimo to u predlošku.

```C#
// Akcija
ViewBag.Songs = _context.Songs;
ViewBag.ExampleText = "Some text";
ViewBag.ExampleNumber = 1987;
ViewBag.Genres = _context.Genres;
ViewBag.GenreDropDownItems =
    _context.Genres.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
```

```HTML
<!-- ...postojeći Razor kod... -->

<hr />

@using(Html.BeginForm())
{
    <text>Text input: </text>
    @Html.TextBox("ExampleText")<br />
    <text>Numeric input: </text>
    @Html.TextBox("ExampleNumber")<br />
    <text>Select genre: </text>
    @Html.DropDownList("GenreDropDownItems")
    <button type="submit">Send data</button>
}
```

### 9.6 Slanje podataka na poslužitelj pomoću HTML obrasca

Zapamtite:

- Podaci se šalju na poslužitelj pomoću atributa `method` navedenog u `<form>` (`GET` ili `POST`)
- Podaci se šalju krajnjoj točki poslužitelja koja je navedena u `<form>` atributu `action`  
- Na poslužitelj se šalju samo podaci koji se nalaze unutar elemenata obrasca s atributom `name`
-   Primjer:
    ```HTML
    <form action="/Song/Index" method="POST">
      <input type="text" name="Example"><!--This is sent to server, key is Example--><br />
      <input type="text"><!--This is NOT sent to server--><br />
      <select name="SelectedGenre"><!--This is sent to server, key is SelectedGenre-->
        <option value="1">One</option>
        <option value="2">Two</option>
      </select>
      <button type="submit">Send data</button>
    </form>
    ```

Napišimo kod koji može poslati (submit) podatke:

- dodajte `action` atribut u obrazac: "/Genre/Index"
- dodajte `method` atribut u obrazac: "POST"
- dodajte `name` atribute elementima obrasca
- dodajte gumb za slanje u obrazac

Primjer:
```HTML
<form action="/Song/Index" method="POST">
  <label>Text input: <input type="text" name="ExampleTxt" value="@ViewBag.ExampleText"></label><br />
  <label>Numeric input: <input type="number" name="ExampleNum" value="@ViewBag.ExampleNumber"></label><br />
  <label>
    Select genre:
    <select name="SelectedGenre">
      @foreach (var genre in genres)
      {
        <option value="@genre.Id">@genre.Name</option>
      }
    </select>
  </label>
  <button type="submit">Send data</button>
</form>
```

POST zahtjev treba obraditi odgovarajućom akcijom:

```C#
[HttpPost]
public ActionResult Index(string ExampleTxt, int ExampleNum, string SelectedGenre)
{
    return RedirectToAction();
}
```

### 9.7 Korištenje HTML pomoćnika za smanjenje koda

Pogledajmo kako korištenje HTML pomoćnika smanjuje kod. Za HTML pomoćnike imenovanje je važno.
**Isto ime se koristi i za ViewBag ključ za prikaz vrijednosti i za generirani atribut `name`.**

```C#
using(Html.BeginForm())
{
    <text>Text input: </text>
    @Html.TextBox("ExampleText")<!--Atribut imena je ExampleText--><br />
    <text>Numeric input: </text>
    @Html.TextBox("ExampleNumber")<!--Atribut imena je ExampleNumber--><br />
    <text>Select genre: </text>
    @Html.DropDownList("GenreDropDownItems")<!--Atribut imena je GenreDropDownItems-->
    <button type="submit">Send data</button>
}
```

```C#
[HttpPost]
public ActionResult Index(string ExampleText, int ExampleNumber, int GenreDropDownItems)
{
    return RedirectToAction();
}
```

> Usporedite količinu koda kada ne koristite HTML pomoćnike i kada koristite HTML pomoćnike.
>
> Podržani su sljedeći HTML pomoćnici:
>
> Html.ActionLink() renderira `<a></a>`  
> Html.TextBox() / Html.TextBoxFor() renderira `<input type="textbox">`  
> Html.TextArea() / Html.TextAreaFor() renderira `<input type="textarea">`  
> Html.CheckBox() / Html.CheckBoxFor() renderira `<input type="checkbox">`  
> Html.RadioButton() / Html.RadioButtonFor() renderira `<input type="radio">`  
> Html.DropDownList() / Html.DropDownListFor() renderira `<select><option>...</select>`  
> Html.ListBox() / Html.ListBoxFor() renderira multi-select `<select><option>...</select>`  
> Html.Hidden() / Html.HiddenFor() renderira `<input type="hidden">`  
> Html.Password() / Html.PasswordFor() renderira `<input type="password">`  
> Html.Label() / Html.LabelFor() renderira `<label>`  
> Html.Editor() / Html.EditorFor() renderira Html kontrole temeljene na tipu podataka određenog svojstva modela, npr. tekstualni okvir za string, numeričko polje za int, double ili drugu numeričku vrstu.  
> Html.Display() / Html.DisplayFor() renderira tekst umjesto Html kontrola, podatke koje sadrži svojstvo modela.

### 9.8 Strogo tipizirani prikaz

Preferirani način prosljeđivanja podataka u prikaz je korištenje strogo tipiziranog prikaza.
Za to vam je potrebna klasa modela. Obično za takve modele kažemo da su _viewmodeli_. Nazivamo ih npr. `GenreViewModel` ili `GenreVM`. Postoje i druge konvencije imenovanja.

-   stvorite mapu `ViewModels`
-   kreirajte sljedeće klase unutar te mape

    ```C#
    public class GenreVM
    {
      public int Id { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
    }
    ```

    ```C#
    public class SongVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Year { get; set; }
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public bool IsActive { get; set; }
    }
    ```

-   za kontroler `Genre`, akciju `Details`, dodajte prazan prikaz `Details.cshtml`
-   u `Details` akciji  dohvatite žanr iz baze podataka prema parametru `id` i proslijedite ga strogo tipiziranom prikazu

    ```C#
    public ActionResult Details(int id)
    {
      var genre = _context.Genres.FirstOrDefault(x => x.Id == id);

      // Map to VM
      var genreVM = new GenreVM
      {
        Id = genre.Id,
        Name = genre.Name,
        Description = genre.Description,
      };

      return View(genreVM);
    }
    ```

-   da bi to funkcioniralo, morate očekivati ​​taj tip u samom prikazu - koristite ključnu riječ `@model` Razor na samom vrhu prikaza `Details.cshtml`
    ```C#
    @model exercise_8_1.ViewModels.GenreVM
    ```
-   sada taj model možete koristiti na razne načine
    - s referencom `@Model`
    - s HTML pomoćnicima
    - s Tag pomoćnicima
-   na primjer, pomoću reference `@Model`:

    ```HTML
    @model exercise_8_1.ViewModels.GenreVM

    <p>@Model.Name</p>
    <p>@Model.Description</p>
    ```

-   koristite http://localhost:6555/Genre/Details/1 da vidite rezultat ove akcije

-   možete učiniti isto za akciju `Details` kontrolera `Song`

    -   stvorite `Details.cshtml`
    -   u akciji: dohvatite pjesmu prema ID-u, mapirajte je u VM objekt, proslijedite je prikazu
    -   example:

    ```C#
    public ActionResult Details(int id)
    {
      var song = _context.Songs.Include(x => x.Genre).FirstOrDefault(x => x.Id == id);

      // Map to VM
      var songVM = new SongVM
      {
        Id = song.Id,
        Name = song.Name,
        Year = song.Year,
        GenreId = song.GenreId,
        GenreName = song.Genre.Name,
        IsActive = !song.DeletedAt.HasValue,
      };

      return View(songVM);
    }
    ```

    -   u prikazu: koristite `@Model` za prikaz pojedinosti pjesme
    -   example:

    ```HTML
    @model exercise_8_1.ViewModels.SongVM

    <p>@Model.Name (@Model.Year)</p>
    <p>Genre: @Model.GenreName</p>
    @if (!Model.IsActive)
    {
        <i>This song is deleted</i>
    }
    ```

-   koristite http://localhost:6555/Song/Details/1 ili http://localhost:6555/Song/Details/7 da bi vidjeli rezultat ove akcije

### 9.9 Strogo tipizirani prikaz i POST akcija

Strogo tipiziran prikaz ima sve podatke o modelu. Ako je obrazac pravilno konfiguriran, može proslijediti ispravne informacije modela u POST zahtjevu kada se obrazac pošalje.

Za obradu dostavljenih podataka trebate implementirati POST akciju s istim nazivom kao što je GET akcija i modelom kao jedinim parametrom.

-   izradite strogo tipizirani prikaz za GET `Edit` akciju `Genre` kontrolera, kao što ste učinili u prethodnom zadatku
    -   model je isti kao u prikazu `Details.cshtml` - `exercise_8_1.ViewModels.GenreVM`
-   koristite sljedeći kod za generiranje obrasca u prikazu `Edit.cshtml`:

    ```HTML
    @using (Html.BeginForm())
    {
        @Html.HiddenFor(m => m.Id)
        @Html.TextBoxFor(m => m.Name)
        <br />
        @Html.TextAreaFor(m => m.Description)
        <br />
        <button type="submit">Send data</button>
    }
    ```

    > Ovaj kod s HTML pomoćnicima stvara isti obrazac kao i sljedeći kod s Tag pomoćnicima:
    >
    > ```HTML
    > <form asp-controller="Genre" asp-action="Edit" method="POST">
    >     <input type="hidden" asp-for="Id" />
    >     <input type="text" asp-for="Name" />
    >     <br />
    >     <textarea asp-for="Description"></textarea>
    >     <br />
    >     <button type="submit">Send data</button>
    > </form>
    > ```
    >
    > Tag pomoćnici slični su HTML oznakama, ali se mogu koristiti za smanjenje koda potrebnog za pisanje prikaza baš kao i HTML pomoćnici.

-   kopirajte kod iz akcije `Detalji`
    -   kod dobiva podatke iz baze podataka i prosljeđuje te podatke prikazu
-   koristite http://localhost:6555/Genre/Edit/1 da biste prikazali rezultat
-   POST akcija već postoji, pa je ne morate dodavati

    -   promijenite `Genre` tip parametra u `GenreVM`, jer očekujemo tip viewmodela od strogo tipiziranog prikaza
    -   dodajte breakpoint na početak akcije i testirajte akciju, provjerite parametre
        -   parametar `id` se treba vezati za podatak, isto kao i cijeli model `Genre`
    -   implementirajte ažuriranje podataka u bazi podataka

    ```C#
    var dbGenre = _context.Genres.FirstOrDefault(x => x.Id == id);
    dbGenre.Name = genre.Name;
    dbGenre.Description = genre.Description;

    _context.SaveChanges();

    return RedirectToAction(nameof(Index));
    ```

-   koristite http://localhost:6555/Genre/Edit/1 za testiranje funkcije uređivanja

### 9.10 Automatsko generiranje strogo tipiziranog prikaza - Create prikaz

Lako je automatski generirati strogo tipizirani prikaz za određeni model.

-   idite na akciju `Stvori` kontrolera `Genre`, kliknite desnom tipkom miša i "Add View"
-   izaberite "Razor View"
    -   NE "Razor View - Empty"
-   za "Name" ostavite `Stvori`
-   za "Template" izaberite `Create` (za drugu akciju odabrali biste drugi predložak)
-   za "Model class" odaberite svoj `GenreVM` model prikaza
-   ostavite "DbContext class" prazan
-   ostavite ostala svojstva kao zadana

Sada se otvara cshtml prikaz za kreiranje. Pregledajte ga.

> Obratite pažnju na Tag pomoćnike.

Kada pokrenete aplikaciju i odete na http://localhost:6555/Genre/Create, aplikacija prikazuje prazna polja koja je potrebno ispuniti.

> Taj predložak je grubog izgleda. Od vas se očekuje da koristite znanje o oblikovanju CSS-a i uređivanju HTML-a kako biste vizualno prilagodili automatski generirani predložak i učinili ga atraktivnijim.

Implementirajte POST akciju.

-   ulazni parametar treba biti tipa `GenreVM`
-   dodajte mapirani žanr u kontekst baze podataka (engl. database context) i spremite kontekst

```C#
  var newGenre = new Genre
  {
    Name = genre.Name,
    Description = genre.Description,
  };

  _context.Genres.Add(newGenre);

  _context.SaveChanges();

  return RedirectToAction(nameof(Index));
```

-   pokušajte s akcijom `Create`: http://localhost:6555/Genre/Create

> Imajte na umu da akcija prihvaća parametar `Id`; treba ga ukloniti iz predloška `Create.cshtml` jer ga baza podataka automatski generira

### 9.11 Vježba: Automatsko generiranje svih strogo tipiziranih prikaza za pjesmu

Za kontroler `Song`, automatski generirajte prikaze `Create`, `Edit`, `Delete` i `Details`.

> Također možete automatski generirati prikaz `List`, ali ćete ga morati prepisati preko `Index.cshtml` kojeg ste stvorili.

Koristite `SongVM` model prikaza i nemojte koristiti kontekst baze podataka.
Pregledajte svaki automatski generirani prikaz.

> Savjeti:
>
> -   uklonite "form group" za unos Id iz `Create` prikaza
> -   u prikazu `Uredi`, postavite ID unosa kao `type="hidden"`, premjestite ga izvan svake "form group" i izbrišite taj "form group" 
> -   u `Create` i `Edit`, `GenreId` se generira kao slobodan unos, no taj unos moramo napraviti padajućim izbornikom pomoću oznake `<select>`
>     -   u akcijama GET dohvatite sve žanrove iz baze podataka kao kolekciju instanci `SelectListItem`
>         ```C#
>         ViewBag.GenreSelect =
>           _context.Genres.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
>         ```
>     -   u prikazima uklonite `GenreName`
>     -   u prikazima, umjesto `<input>`, neka `GenreId` bude `<select>`
>     -   da `<select>` ne smije biti samozatvarajući i mora imati završnu oznaku (engl. tag)
>     -   u oznaci `<select>`, umjesto `class="form-control"` koristite `class="form-select"`
>     -   koristite `asp-items` za prosljeđivanje podataka iz ViewBaga za renderiranje oznaka `<option>`
>         ```HTML
>         <select asp-for="GenreId" asp-items="@ViewBag.GenreSelect" class="form-select"></select>
>         ```
>         ```HTML
>         <select asp-for="GenreId" asp-items="@ViewBag.GenreSelect" class="form-select">
>           <option>(select genre)</option>
>         </select>
>         ```
> -   u `Delete`, `GenreId` nije važan, ali trebamo `GenreName`

Test:

-   http://localhost:6555/Song/Index
-   http://localhost:6555/Song/Details/1
-   http://localhost:6555/Song/Create
-   http://localhost:6555/Song/Edit/1
-   http://localhost:6555/Song/Delete/1

### 9.12 Vježba: korištenje automatski generiranih strogo tipiziranih prikaza za CRUD funkcionalnosti

Implementirajte POST akcije za svaku od funkcija `Create`, `Edit` i `Delete` za kontroler `Song`.

### 9.13 Vježba: Prilagodba automatski generiranih prikaza

Vizualno prilagodite sve automatski generirane predloške kako biste ih učinili atraktivnijima.