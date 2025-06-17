# MVC arhitektura

Ovaj materijal dio je ishoda učenja 3 (minimum).

## 8 ASP.NET MVC

Pregled ASP.NET Core MVC:

- https://learn.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-8.0


### 8.1 Izrada i pokretanje ASP.NET MVC projekta

U okolini Visual Studio 2022 stvaramo novi MVC projekt sa sljedećim svojstvima:

- Vrsta projekta: ASP.NET Core Web App (Model-View-Controller)
  - *NE Web API*
- Naziv (solution): exercise-8
- Naziv projekta: exercise-8-1
- Bez autentifikacije i HTTPS-a
- Enable Docker: Ne
- Do not use top-level statements: Ne

### 8.2 Promotrite strukturu projekta

- Web API ima vrlo sličnu strukturu mapa
- Mapa `wwwroot`
  – statičke datoteke dodane su 'po defaultu'
  - ovdje se može dodati još statičkih datoteka (skripti, CSS stilova, slika, dokumenata...)
- Datoteka `Program.cs`
  – zadana ruta je postavljena u parametru poziva `app.MapControllerRoute()`
  - zadani kontroler je `Home`, zadana akcija je `Index`
- Mapa `Kontroleri`
  - MVC akcije kontrolera vraćaju HTML stranice
  - otvorite `HomeController.cs` i pronađite akciju `Index`
  - dok ste u toj akciji, u kontekstnom izborniku odaberite "Go To View"
  - bit ćete prebačeni na taj prikaz (view)
  - očito, `Index.cshtml` je zadani prikaz
- Promijenite nešto HTML-a, ponovno napravite build aplikacije i promatrajte rezultat
- Mapa `Views`
  – Sadrži datoteke s kodom koji renderira HTML
  - Kod je poznat kao sintaksa **Razor**
  - Veliki dio toga **jest** HTML
- Mapa `Modeli` – dodana standardno, s jednim modelom
- Datoteka `launchSettings.json`:
  - Promijenite MVC aplikaciju da radi na portu 6555
- Datoteka `appsettings.json`:
  - Baza podataka, autentifikacija, prilagođene postavke...

### 8.3 Usmjeravanje

- Isprobajte sljedeće rute:
  - http://localhost:6555/
  - http://localhost:6555/home/
  - http://localhost:6555/home/index/
  - http://localhost:6555/home/privacy/
  - http://localhost:6555/home/error/

- Pronađite prikaz `Privacy.cshtml` i promijenite tekst u...
  "Korištenjem naših usluga pristajete na prikupljanje i korištenje vaših podataka kako je navedeno u ovim pravilima."
- Rukovanje greškama
  - Ruta pogreške samo je "placeholder" i za to možete koristiti vlastitu stranicu pogreške
  - Uklonite rutu rukovatelja iznimkama (blok s `UseExceptionHandler`)
  - Umjesto toga, koristite međuprogram za koji već znamo: `app.UseDeveloperExceptionPage()`
  - Bacite iznimku `NotImplementedException` u akciji `Privacy()` i pogledajte što se događa kada koristite tu rutu

### 8.4 Stvaranje akcije s prikazom

Sada ćete napraviti web stranicu.

- Napravite novi prazan MVC kontroler pod nazivom PersonController
- Napravite novu praznu akciju `Add()` unutar kontrolera
- Dodajte novi prikaz `Person/Add.cshtml`
  - U kontekstnom izborniku kliknite *Add View...*
  - Odaberite *Razor View – Empty*
  - Postavite naziv na `Add.cshtml`
- Zalijepite sljedeći kod u `Add.cshtml`: https://pastebin.com/kwknkp9C
- Pokrenite aplikaciju, idite na http://localhost:6555/Person/Add
- Promatrajte generirani HTML kod u pregledniku

### 8.5 Stvaranje HTTP POST akcije

Sada ćemo stvoriti funkcionalnost pohrane podataka.

- Potrebna nam je akcija koja obrađuje POST zahtjev kako bi poslužitelj dodao podatak ime u kolekciju
- Dodajte statičku listu stringova u kontroler
  ```C#
  private static List<string> _persons = new List<string>();
  ```
- Dodajte akciju `[HttpPost]` s istim nazivom kao akcija GET: `Add()`
- Dodajte joj parametar `string firstName` (ime mora biti isto kao ono u prikazu, u tekstualnom okviru unosa)
- U kodu akcije dodajte to ime u listu osoba
- Pokrenite programu debug načinu rada i pokažite da kada se pritisne gumb *Dodaj*, naziv se dodaje u statičku lisu

### 8.7 Prikaz podataka u listi

Ovdje ćemo napraviti funkcionalnost prikaza podataka pohranjenih u listi.

- Prikažimo imena dodana u kolekciju
- Za to možete koristiti akciju `Index` – dodajte prikaz `Index.cshtml`
- Za slanje podataka prikazu, u akciji postavite ViewData
  ```C#
  ViewData["persons"] = _persons;
  ```
- Za preuzimanje podataka u prikazu, na početku prikaza preuzmite zbirku iz ViewData:
  ```C#
  @{
    List<string> persons = ViewData["persons"] as List<string>;
  }
  ```
- Koristite sljedeći kod za generiranje ispravnog HTML-a: https://pastebin.com/T4KPh2UJ
- Nakon dodavanja podataka trebali biste moći vidjeti prikaz kolekcije ovdje: http://localhost:6555/Person

### 8.8 Preusmjeravanje s akcije POST na akciju prikaza liste podataka (PRG obrazac)

Ovdje ćemo preusmjeriti stranicu nakon što se podaci pohrane.

- Nakon dodavanja novog imena, preusmjerite na prikaz liste dodanih imena
  - Vratite rezultat RedirectToAction() umjesto View()
  ```C#
  return RedirectToAction("Index")
  ```
- Ispod tablice imena osoba dodajte gumb "Add person" koji će otvoriti URL "/Person/Add/"
 ```HTML
 <a href="/Person/Add/" class="btn btn-primary">Add person</a>
 ```
- Testirajte dodavanje imena

 > Napomena: ovo je jednostavna implementacija obrasca PRG (Post-Redirect-Get) koji se naširoko koristi u MVC-u

### 8.9 Koristite model za pohranu – dodajte prezime i e-mail

Ovdje ćemo se pozabaviti pohranjivanjem više podataka, a ne samo jednog stringa.

- Za to ćete očito morati spremiti različite podatke za popis, ne samo string
- Dodajte model `Person`
  - `string FirstName`
  - `string LastName`
  - `string Email`
- Promijenite `List<string>` u `List<Person>`
 - Za to morate promijeniti vrstu parametra akcije `Add()`
 - Također morate promijeniti tu listu u `Index.cshtml`: https://pastebin.com/DXhcxtzn
 - Također morate promijeniti obrazac u `Add.cshtml`: prilagodite kod tako da uključuje nove podatke

### 8.10 Stvorite prikaz za uređivanje

Ovdje ćemo stvoriti funkcionalnost uređivanja prikaza pohranjenih podataka.

- Za uređivanje očito trebamo novu akciju i novi prikaz
- Problem: nema identifikatora
  - Morat ćemo koristiti identifikator kako bismo mogli identificirati model
  - Dodajte svojstvo `int Id` modelu
  - Kada dodajete osobu, automatski generirajte taj `Id` koristeći `Max()` LINQ izraz na listi
- Dodajte prikaz za funkcionalnost uređivanja
  - Dodajte akciju `Edit(int id)`
  - Ispunite `ViewData` traženim modelom
    ```C#
    ViewData["person"] = person
    ```
  - Duplicirajte datoteku `Add.cshtml` u `Edit.cshtml`
  - U prikazu preuzmite podatke iz `ViewData`
    ```C#
    Person person = ViewData["person"] as Person;
    ```
 - Koristite svojstva osobe za vrijednosti
    ```C#
    @Html.TextBox("firstName", value: person.FirstName, htmlAttributes: ...)
    ```

    ```C#
    @{
      Person person = ViewData["person"] as Person;
    }

    @using (Html.BeginForm())
    {
      <div class="form-group">
        @Html.Label(labelText: "First Name", expression: "firstName")
        @Html.TextBox("firstName", value: person.FirstName, htmlAttributes: new { @class = "form-control" })
      </div>
      ...
    }
    ```
- Nakon uređivanja osobe, isprobajte prikaz: http://localhost:6555/Person/Edit/1

### 8.11 Stvaranje POST akcije za uređivanje

Sada ćemo izraditi funkcionalnost za pohranu uređenih podataka.

- Započnite kopiranjem akcije `GET Edit()` u akciju POST
- Promijenite ulazni argument u tip `Person`, da biste vezali podatak koji dolazi iz korisničkog sučelja
- Pronađite osobu s popisa prema ID-u i ažurirajte njeno ime i prezime te e-mail
- Preusmjeravanje na akciju "Indeks", isto kao i za akciju `Add()`
- Nakon što dodate osobu, isprobajte funkciju uređivanja: http://localhost:6555/Person/Edit/1

### 8.12 Stvorite prikaz brisanja

Ovdje ćemo stvoriti funkcionalnost prikaza pohranjenih podataka za brisanje.

- Za brisanje također trebamo novu akciju i novi prikaz
- Dodajte prikaz za funkcionalnost brisanja
  - Kopirajte (duplicirajte) akciju `Edit(int id)` kao akciju `Delete(int id)`
  - Kopirajte (duplicirajte) prikaz `Edit.cshtml` kao prikaz `Delete.cshtml`
  - U prikazu, zamijenite
    ```C#
    @Html.TextBox("firstName", value: person.FirstName, htmlAttributes: new { @class = "form-control" })
    ```
    ...s...
    ```C#
    @Html.Display("person.FirstName")
    ```
  - Napomena: Razor izraz će zaviriti u `ViewData` i izvući informacije iz njega
  - Učinite to za sve tekstualne okvire
- Nakon dodavanja osobe, isprobajte prikaz: http://localhost:6555/Person/Delete/1
- Provjerite kakav je učinak zamjene `TextBox()` s `Display()` u HTML kodu

### 8.13 Stvorite POST akciju za brisanje

Sada ćemo stvoriti funkcionalnost brisanja podataka.

- Započnite kopiranjem GET akcije `Delete()`
- Promijenite ulazni argument u tip `Osoba`, da biste povezali podatak poslan s korisničkog sučelja
- Pronađite osobu s popisa prema ID-u i uklonite je iz liste
- Preusmjerite na akciju "Index", isto kao i za akciju `Add()`
- Nakon dodavanja osobe, isprobajte funkciju brisanja: http://localhost:6555/Person/Delete/1

### 8.14 Pospajajte veze

Ovdje ćemo koristiti `_Layout.cshtml` (može se smatrati kao "master page") za generiranje navigacijskih veza.

- Otvorite `_Layout.cshtml`, zamijenite postojeću vezu *Privacy* vezom na `Person/Index` s tekstom *People*
- Provjerite radi li poveznica na index stranicu: http://localhost:6555
- U `Index.cshtml` dodajte dva "placeholder" gumba za `Uredi` i `Izbriši` funkcionalnosti
  ```HTML
  <a href="@editUrl" class="btn btn-primary">Edit</a>
  <a href="@deleteUrl" class="btn btn-danger">Delete</a>
  ```
- Nakon što program uđe u petlju `@foreach`, deklarirajte i inicijalizirajte potrebne varijable
  ```C#
  var editUrl = "/Person/Edit/" + person.Id;
  var deleteUrl = "/Person/Delete/" + person.Id;
  ```
- Isprobajte funkcionalnost
- Imajte na umu da ne postoji gumb Odustani na prikazima Add/Edit/Delete – implementirajte ga

### Kako implementiramo CRUD?

Možete koristiti sljedeći popis za kontrolu:
> - Dodaj model
> - Dodaj kontroler
> - Dodaj repozitorij podataka (ovdje smo koristili statičku kolekciju)
> - Stvori akciju dodavanja (GET)
> - Stvori Add.cshtml
> - Stvori akciju dodavanja (POST)
> - Stvarori index (lista) akciju (GET)
> - Stvorite Index.cshtml
> - Stvori akciju uređivanja (GET)
> - Stvori Edit.cshtml
> - Stvori akciju uređivanja (POST)
> - Stvori akciju brisanja (GET)
> - Stvori Delete.cshtml
> - Stvori akciju brisanja (POST)
> - Poveži veze u _Layout.cshtml

### 8.15 Vježba: Implementirajte CRUD za proizvode

- Model Product
  - Id (int)
  - Name (string)
  - Description (string)
  - Price (decimal)
  - URL (string)