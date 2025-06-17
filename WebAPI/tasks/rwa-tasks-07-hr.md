# RESTful arhitektura (Web API)

Ovaj materijal je dio Ishoda 2 (željeno).

## 7 Korištenje Web API aplikacije

Statičke stranice u ASP.Net Core:

-   https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-8.0

JavaScript jQuery AJAX metode:

-   https://api.jquery.com/jQuery.ajax/
-   https://api.jquery.com/jQuery.get/

JavaScript Fetch API:

-   https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API

JavaScript localStorage:

-   https://developer.mozilla.org/en-US/docs/Web/API/Window/localStorage/

### Postavljanje baze podataka

SQL Server Management Studio:

-   stvorite bazu podataka `Exercise7`
-   stvoriti strukturu baze podataka s podacima
    -   struktura dio 1 (glavni entiteti): https://pastebin.com/5MTPzxrd
    -   struktura dio 2 (autentifikacija): https://pastebin.com/9wsPtAV1
    -   podaci: https://pastebin.com/iwuDcyKx

### Postavljanje Web API aplikacije

Raspakirajte arhivu `dwa-tasks-7-en.zip`.  
Koristit ćemo to rješenje kao Web API poslužitelj.

> Kako je napravljena ta aplikacija?  
> Iskoristili smo sve što smo naučili o Web API RESTful servisu za izradu ove aplikacije:
>
> -   kreirano novo Web API rješenje u Visual Studiju:
>     -   naziv rješenja i projekta: exercise-07
>     -   isključena HTTPS podrška
> -   instalirana EF podrška:
>     -   `dotnet tool install --global dotnet-ef --version 7`
>     -   `dotnet add package Microsoft.EntityFrameworkCore --version 7`
>     -   `dotnet add package Microsoft.EntityFrameworkCore.Design --version 7`
>     -   `dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 7`
> -   instalirana JWT podrška:
>     -   `dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 7`
>     -   `dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect --version 7`
> -   konfiguriran EF connection string u `appsettings.json`
>     ```JSON
>     "ConnectionStrings": {
>       "AppConnStr": "server={your server};Database=Exercise7;User=sa;Password=SQL;TrustServerCertificate=True;MultipleActiveResultSets=true"
>     }
>     ```
> -   "reverse engineered" baza podataka (stvorene klase entiteta i kontekst baze podataka prema stvarnoj bazi podataka)
>     -   `dotnet ef dbcontext scaffold "Name=ConnectionStrings:AppConnStr" Microsoft.EntityFrameworkCore.SqlServer -o Models --force`
> -   postavljen kontekst u `Program.cs` da bude dostupan DI spremniku
>     ```C#
>     builder.Services.AddDbContext<Exercise7Context>(options => {
>         options.UseSqlServer("name=ConnectionStrings:AppConnStr");
>     });
>     ```
> -   konfiguriran JWT u `appsettings.json` (samo sigurnosni ključ)
>     ```JSON
>     "JWT": {
>       "SecureKey": "1xvawozgzh78q2m9xpdlshegaqaspkpe"
>     }
>     ```
> -   konfigurirana JWT usluga i međuprogram u `Program.cs`:
>
>     ```C#
>     // Configure JWT security services
>     var secureKey = builder.Configuration["JWT:SecureKey"];
>     builder.Services
>         .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
>         .AddJwtBearer(o => {
>             var Key = Encoding.UTF8.GetBytes(secureKey);
>             o.TokenValidationParameters = new TokenValidationParameters
>             {
>                 ValidateIssuer = false,
>                 ValidateAudience = false,
>                 IssuerSigningKey = new SymmetricSecurityKey(Key)
>             };
>         });
>     ```
>
>     ```C#
>     // Use authentication / authorization middleware
>     app.UseAuthentication();
>     app.UseAuthorization(); // -> this should already be present
>     ```
>
> -   stvorena pomoćna klasa `JwtTokenProvider` koja obrađuje JWT: https://pastebin.com/ZnD36wjb
> -   u `Program.cs` dodana JWT podrška za Swagger login (za potrebe testiranja): https://pastebin.com/YdqbDfWy
> -   stvorena pomoćna klasa `PasswordHashProvider` za kriptografsku obradu raspršenja (engl. hash) lozinke: https://pastebin.com/buxhBfZx
> -   stvoren `UserController` koji upravlja registracijom i prijavom
>     -   vidi `UserController` za detalje (objašnjeno u prethodnim vježbama)
>     -   stvorene DTO klase za registraciju i prijavu: `UserRegisterDto` i `UserLoginDto`
> -   kreiran `AudioController` koji koristi `Audio` entitet s 1-na-N `Genre` i M-na-N `Tag` entitetima
>     -   stvoren `AudioDto`
>     -   stvorena klasa za mapiranje
> -   osigurano da neke radnje u `AudioControlleru` budu dostupne korisniku koji je prijavljen
>     -   koristeći atribut `[Authorize]`
>     -   osigurane su akcije `GetAll()`, `Post()`, `Put()` i `Delete()`
>     -   samo akcije `Search` i `Get(int id)` nisu osigurane

## 7.1 Omogućavanje statičkih stranica u projektu

Kada dohvaćamo podatke s web API poslužitelja, obično koristimo _statičke_ HTML stranice za dohvaćanje podataka s našeg poslužitelja.  
Statičke stranice su konvencionalno u mapi `wwwroot` korijenske mape projekta.

-   stvorite mapu `wwwroot` u korijenu projekta
    -   možete vidjeti da izgleda drugačije, radi se o posebnoj mapi
-   stvorite `audios.html` statičku HTML stranicu u mapi `wwwroot` (desni gumb, Add > New Item..., filtrirajte "html")
-   dodajte sadržaj `<h1>Audio list</h1><hr />` na stranicu
-   također morate podržati statičke datoteke pomoću odgovarajućeg međuprograma
    -   dodajte međuprogram `app.UseStaticFiles()` u Program.cs
    -   dodajte ga prije `app.UseAuthentication()` kako biste izbjegli provjeru JWT autentifikacijskog tokena prilikom dohvaćanja statičkih datoteka; ako biste ga dodali nakon toga, trebali biste slati JWT token za svaku od statičkih datoteka

Testirajte svoju stranicu:

-   pokrenite aplikaciju
-   otvorite http://localhost:5127/audios.html u pregledniku

Imajte na umu da ćete koristiti JavaScript za dohvaćanje podataka sa svoje krajnje točke.  
Uvijek možete koristiti jQuery da vam pomogne sa zahtjevima i rukovanjem DOM-om:

-   idite na https://releases.jquery.com/, odjeljak **jQuery 3.x**
-   kliknite minificirau verziju i kopirajte tag skripte
-   zalijepite ga u svoj HTML neposredno prije završetka body taga

## 7.2 Dohvaćanje podataka iz nezaštićenog izvora

1. Pronađite točnu adresu krajnje točke

    Lako ga je konstruirati pomoću atributa usmjeravanja, ali također možete koristiti Swagger.

    - pokrenite aplikaciju
    - koristite krajnju točku `/api/Audio/{id}` u Swaggeru da biste dobili jedan određeni audio sadržaj (npr. id=3)
    - obratite pažnju na polje `Request URL`, ono sadrži URL
        - e.g. `http://localhost:5127/api/Audio/3`

2. Provjerite radi li JavaScript

    Provjerite možete li stvarno koristiti JavaScript u svojim postavkama

    - dodajte tag Vaše skripte nakon prvog taga `jQuery` skripte
    - dodajte sljedeću skriptu:
        ```JavaScript
        $(function () {
            console.log("DOM ready");
        });
        ```
    - u Development Tools alatu preglednika provjerite je li odgovarajuća poruka nakon učitavanja stranice ispisana u konzoli

3. Izdajte zahtjev i logirajte odgovor

    Sada možete zamijeniti jedan poziv `console.log()` dohvaćanjem podataka s vaše krajnje točke poslužitelja:

    ```JavaScript
    $(function () {
        let url = "http://localhost:5127/api/Audio/3";
        $.get(url, function (data) {
            console.log(data);
        });
    });
    ```

    > Imajte na umu da također možete koristiti u preglednik ugrađeni (engl. native) JavaScript API za dohvaćanje bez jQueryja. Za to koristite `async` funkciju u svojoj skripti
    >
    > ```JavaScript
    > document.addEventListener("DOMContentLoaded", async function () {
    >     let url = "http://localhost:5127/api/Audio/3";
    >     const response = await fetch(url);
    >     const data = await response.json();
    >     console.log(data);
    > });
    > ```

## 7.3 Rukovanje greškama

Za rukovanje pogreškama u jQuery zahtjevu morate dodati `.fail()` rukovatelj pogreškama odmah nakon `$.get()`. To se radi pomoću tzv. "fluent interface" API-a.

```JavaScript
$(function () {
    let url = "http://localhost:5127/api/Audio/5";
    $.get(url, function (data) {
      console.log(data);
    }).fail(function() {
      console.error("There was an error while trying to load your data");
    })
});
```

> Napomena: kada koristite Fetch API, rukovanje pogreškama se vrši pomoću standardne sintakse try/catch.
> Primjer:
>
> ```JavaScript
> try {
>   const response = await fetch(url);
>   const data = await response.json();
>   console.log(data);
> } catch (error) {
>   console.error(`Request error: ${error.message}`);
> }
> ```

## 7.4 Korištenje DOM-a

Upotrijebimo rukovanje DOM-om za prikaz podataka na našoj stranici.  
Dodajte rezervirano mjesto (engl. placeholder) za prikaz podataka na vašoj stranici.  

```HTML
<div id="placeholder"></div>
```

Umjesto upotrebe `console.log`, generirajte podatke pomoću jQueryja.  
Na primjer:

```JavaScript
$("#placeholder").append("<ul>");
$ul = $("#placeholder ul");
$ul.append(`<li>Id: ${data.id}</li>`);
$ul.append(`<li>Name: ${data.title}</li>`);
$ul.append(`<li><a href="${data.url}">Play song</a></li>`);
```

## 7.5 Dohvaćanje podataka iz zaštićenog izvora

Da biste dohvatili podatke iz zaštićenog izvora, prvo trebate dohvatiti JWT i zatim ga upotrijebiti u svom zahtjevu.

1. Dohvatite JWT

    Prvo registrirajmo korisnika koristeći Swagger, a zatim se prijavimo (dohvatimo JWT) koristeći JavaScript.

    Na primjer, koristite ove podatke za registraciju:

    ```
    {
      "username": "johnny1234",
      "password": "qwertzuiop",
      "firstName": "John",
      "lastName": "Smith",
      "email": "johnsmith1234@example.com",
      "phone": "0987654321"
    }
    ```

    Sada koristite Swagger za prijavu i pogledajte kako krajnja točka prijave stvarno izgleda.
    Na primjer, izgledati će ovako: `http://localhost:5127/api/User/Login`.
    Obratite pažnju na to da se radi o POST krajnjoj točki, pa morate koristiti jQuery Ajax POST za login iz JavaScripta:

    ```JavaScript
    let loginUrl = "http://localhost:5127/api/User/Login";
    let loginData = {
        "username": "johnny1234",
        "password": "qwertzuiop"
    }
    $.ajax({
        method: "POST",
        url: loginUrl,
        data: JSON.stringify(loginData),
        contentType: 'application/json' // important, otherwise it's sent as form data
    }).done(function (data) {
        console.log(data);
    }).fail(function () {
        console.error("There was an error while trying to load your data");
    });
    ```

    Sada biste trebali vidjeti JWT dohvaćen i prikazan u konzoli.

    > Napomena: Kada radite na Web API aplikaciji, često biste mogli dobivati automatsku grešku 400. Možete ju isključiti pomoću već poznate tehnike u `Program.cs`:
    >
    > ```C#
    > builder.Services.Configure<ApiBehaviorOptions>(options =>
    > {
    >   options.SuppressModelStateInvalidFilter = true;
    > });
    > ```

2. Koristite JWT za dohvaćanje zaštićenih podataka

    Upotrijebite Swagger da pronađete osiguranu krajnju točku `GetAll()`.  
    Na primjer, to će biti: http://localhost:5127/api/Audio/GetAll

    Nakon što dobijete JWT, upotrijebite ga za dohvaćanje podataka sa zaštićene krajnje točke:

    ```JavaScript
    let loginUrl = "http://localhost:5127/api/User/Login";
    let allAudiosUrl = "http://localhost:5127/api/Audio/GetAll";

    let loginData = {
        "username": "johnny1234",
        "password": "qwertzuiop"
    }
    $.ajax({...})
    .done(function (tokenData) {
        console.log(tokenData);

        $.ajax({
            url: allAudiosUrl,
            headers: { "Authorization": `Bearer ${tokenData}` } // this is how you send JWT token using JavaScript
        }).done(function (allAudioData) {
            console.log(allAudioData);
        }).fail(function () {
            console.error("There was an error while trying to load your data");
        });
    })
    .fail(...);
    ```

## 7.6 Using localStorage

LocalStorage je svojstvo koje sprema podatke kako bi oni ostali zapamćeni nakon zatvaranja prozora preglednika.  
Primjer:

```
localStorage.setItem("myCat", "Tom");
const cat = localStorage.getItem("myCat");
console.log(cat);
```

Možete ukloniti stavku iz localStorage-a:

```
localStorage.removeItem("myCat");
```

## 7.7 Korištenje localStorage-a za podršku JWT prijave/odjave

Korištenjem localStorage-a možete podržati funkcionalnost prijave (login) i odjave (logout):

-   Login: dohvaćanje i pohranjivanje JWT-a u localStorage
-   Logout: uklanjanje JWT-a iz localStorage

> Primjer će također uključivati ​​HTML i CSS jer nam je potreban izgled.

Primjer:

-   `login.html`: kreirajte stranicu `login.html` s unosom korisničkog imena i lozinke i gumbom `Login`
-   `login.html`: na klik gumba, ili prikazuje pogrešku prijave ili pohranjuje primljeni JWT kao localStorage stavku i preusmjerava na stranicu `audios.html`

    ```HTML
    <div class="login-container">
        <label for="username"><b>Username</b></label>
        <input type="text" placeholder="Enter Username" name="username" id="username" required>

        <label for="password"><b>Password</b></label>
        <input type="password" placeholder="Enter Password" name="password" id="password" required>

        <button onclick="jwtLogin()">Login</button>
    </div>
    ```

    ```CSS
    input[type=text], input[type=password] {
        width: 100%;
        padding: 12px 20px;
        margin: 8px 0;
        display: block;
        border: 1px solid #ccc;
        box-sizing: border-box;
    }

    button {
        width: 100%;
        background-color: #04AA6D;
        color: white;
        padding: 14px 20px;
        margin: 8px 0;
        display: block;
        border: none;
        cursor: pointer;
    }

        button:hover {
            opacity: 0.8;
        }

    .login-container {
        width: 50%;
        padding: 16px;
        border: 3px solid #f1f1f1;
        margin: auto;
    }
    ```

    ```JavaScript
    function jwtLogin() {
        let loginData = {
            "username": $("#username").val(),
            "password": $("#password").val()
        }
        $.ajax({
            method: "POST",
            url: loginUrl,
            data: JSON.stringify(loginData),
            contentType: 'application/json'
        }).done(function (tokenData) {
            //console.log(tokenData);
            localStorage.setItem("JWT", tokenData);

            // redirect
            window.location.href = "audios.html";
        }).fail(function (err) {
            alert(err.responseText);
            localStorage.removeItem("JWT");
        });
    }
    ```

-   `audios.html`: prilikom učitavanja stranice provjerite postoji li JWT stavka u localStorageu i ako ne, preusmjerite na `login.html`
-   `audios.html`: dodajte gumb na stranicu `audios.html` koji uklanja JWT stavku iz localStoragea i preusmjerava na `login.html`

    ```HTML
    <nav>
        <h2>Audio list</h2>
        <ul>
            <li><a href="audios.html">Audios</a></li>
            <li><a href="javascript:void(0);" onclick="jwtLogout()">Logout</a></li>
        </ul>
    </nav>
    <div id="placeholder"></div>
    ```

    ```CSS
    * {
        padding: 0;
        margin: 0;
    }

    body {
        font-family: Arial, Tahoma, Serif;
        color: #263238;
    }

    nav {
        display: flex; /* 1 */
        justify-content: space-between; /* 2 */
        padding: 1rem 2rem; /* 3 */
        background: #cfd8dc; /* 4 */
    }

        nav ul {
            display: flex; /* 5 */
            list-style: none; /* 6 */
        }

        nav li {
            padding-left: 1rem; /* 7! */
        }

        nav a {
            text-decoration: none;
            color: #0d47a1
        }

            nav a:hover {
                opacity: 0.5;
            }
    ```

    ```JavaScript
    function jwtLogout() {
        localStorage.removeItem("JWT");

        // redirect
        window.location.href = "login.html";
    }
    ```

## 7.8 Primjer: JavaScript i Web API funkcionalnost registracije/prijave/odjave

Implementirajte funkcionalnost registracije.  
Koristite stranicu `register.html` i integrirajte je u shemu stranice za login (npr. poveznica "Registracija novog korisnika").

> Napomena: kada je korisnik registriran, klijent može pozvati login funkciju i spremiti JWT isto kao kada se prijavljuje.

## 7.9 Primjer: JavaScript interaktivnost pomoću CRUD kontrolera

Zadaci:

1. Prikažite audio sadržaj u obliku tablice / popisa / kartice
2. Dodajte opciju pretraživanja zapisa
3. Podržite dodavanje novog audio sadržaja
4. Podržite uklanjanje audio sadržaja
5. Podržite ažuriranje audio sadržaja

Pravila interaktivnosti za dodavanje, modificiranje i brisanje stavki:

-   koristite AJAX GET za dohvaćanje podataka
-   koristite AJAX POST za dodavanje podataka
-   koristite AJAX PUT za izmjenu podataka
-   koristite AJAX DELETE za uklanjanje podataka

> Savjet: koristite modalni dijalog za stvaranje ili izmjenu stavki bez navigacije na drugu stranicu. Možete koristiti Bootstrap modal.

Koristite sljedeći predložak za osnovnu komunikaciju s vašim poslužiteljem:

```JavaScript
const requestUrl = "http://localhost:5127/api/{controller}"

function templateToDoSomething() {
  const requestData = {} // data that you want to send to server
  $.ajax({
      method: "{GET or POST or PUT or DELETE}",
      url: requestUrl,
      data: JSON.stringify(requestData), // for POST or PUT
      contentType: 'application/json', // for POST or PUT
      headers: { "Authorization": `Bearer ${jwt}` } // for secured endpoints
  }).done(function (responseData) {
      console.log(responseData);

      // Example 1: modify DOM
      // Example 2: fill inputs with data
      // Example 3: use your imagination :)
  }).fail(function (err) {
      console.error(err.responseText);

      // ...or: alert(err.responseText) to show error to user
      // ...or: modify DOM to show error to user
  });
}
```

**Rješenje**:

Pojedinosti potražite u arhivi rješenja.
