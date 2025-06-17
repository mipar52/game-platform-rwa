# RESTful arhitektura (Web API)

Ovaj materijal je dio Ishoda 1 (minimalno).

## 2 Osnove kontrolera i akcija

Koncept usmjeravanja putem atributa i objašnjenje:

- https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-8.0#attribute-routing-for-rest-apis

Vezanje parametara je objašnjeno ovdje:

- https://learn.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/parameter-binding-in-aspnet-web-api

Mogućnosti za povrat vrijednosti iz akcije su objašnjene ovdje:

- https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-8.0

### 2.1 Akcija

Dobro napisana akcija ima sljedeća svojstva:

- koristi atribut HTTP metode (GET, POST, PUT, DELETE) koji može ako je potrebno definirati naziv akcije
- ako je potrebno, prima i jedan ili više parametara (vezanje parametara ili parameter binding)
- logiku štiti pomoću `try...catch` bloka
- vraća umotani (wrapped) generički rezultat `ActionResult<T>`, gdje je `T` tip podatka koji klijent stvarno očekuje
- na mjestu gdje se podatak vraća kao ispravan, "umota" ga u `2XX` ili `3XX` statusni kod odgovora; npr. `Ok(result)` ili `Redirect(url)`
- na mjestu gdje se podatak vraća kao neispravan, "umota" ga u `4XX` ili `5XX` statusni kod odgovora; npr. `BadRequest("Neuspjela operacija")` ili `StatusCode(500)`
- ne sadrži puno koda nego ako je potrebno npr. poziva metodu koja odrađuje posao

  ```C#
  [HttpGet("[action]")]
  public ActionResult<decimal> VratiKvocijent(decimal a, decimal b)
  {
      try
      {
          // Action logic (e.g. data handling)
          var result = a / b;

          // Return 200 OK
          return Ok(result);
      }
      catch (Exception ex)
      {
          // Exception handling code
          // ...

          // Return e.g. internal server error
          return StatusCode(500);
      }
  }
  ```

### 2.2 Dodavanje kontrolera i akcije

Da biste dodali kontroler i akciju, ne morate pisati kod od početka. Dovoljno je dodati prazan kontroler koristeći Solution Explorer.  
Desnim gumbom na "Controllers" otvara se kontekstni izbornik u kojem možete odabrati "Add", zatim "Controller..." Nakon toga iz upravo otvorenog panela _s lijeva_ odaberite "Installed > Common > API", a s desna opciju "API Controller > Empty" **(ne "MVC Controller > Empty")**.  
Nakon toga odaberete ime za kontroler i on će se pojaviti u listi kontrolera u projektu.

1. Kreirajte novu RESTful web api aplikaciju sa sljedećim karakteristikama:

- Naziv (solution): vjezbe-2
- Tip projekta: Web API
- Naziv projekta: vjezbe-2-1
- Bez autentifikacije i HTTPS-a
- Neka koristi kontrolere i Swagger

2.  Dodajte na navedeni način u projekt novu klasu `MatematikaController`.

    > Primijetite da `MatematikaController` nasljeđuje istu osnovnu klasu `ControllerBase` kao i klasa `WeatherForecastController`.

    > Da bi se kontroler pojavio u Swagger sučelju, mora biti označen atributom `ApiController`, kako je to napravljeno u `WeatherForecastController` i `MatematikaController` klasi. Osim toga kontroleru treba i atribut za usmjeravanje, `Route` - i on je prisutan u generiranom kontroleru i u `WeatherForecastController` klasi. Koja je razlika između njih?

3.  Dodajte u tu klasu novu metodu `Zbroji`. Neka metoda prima dva cjelobrojna parametra, a i b i vraća zbroj ta dva parametra.

    > Da bi se metoda pojavila u Swagger sučelju, mora biti označena atributom HTTP-metode, kao što je `[HttpGet]`. Kada pokrenete projekt, nova metoda je prikazana kao `api/Matematika`, bez naziva `Zbroji`. To je zato jer se naziv automatski ne uključuje u krajnju točku s obzirom da `HttpGet` atributu nismo dali parametar rute.

    > Dajte metodi parametar rute tako da promijenite `[HttpGet]` u `[HttpGet("[action]")]`.  
    > Što se događa u Swagger sučelju?

4.  Isprobajte kako radi nova krajnja točka u Swagger sučelju.  
    Vidimo da je moguće do nje doći iz Swaggera. Nazivamo ju _krajnja točka_ (endpoint), a metodu kontrolera koja se poziva kada se pozove krajnja točka nazivamo _akcija_.

        > Vidimo da nova pristupna točka ne vraća JSON. Kod primitivnih vrijednosti, JSON se ne vraća. JSON se vraća samo u slučaju da se radi o kolekcijama, objektima ili kolekcijama objekata.

### 2.3 Usmjeravanje kontrolera

Promijenite rutu usmjeravanja kontrolera dodavanjem segmenta "operations" u URL nakon prefiksa "api".

`Route("api/[controller]")` => `Route("api/operations/[controller]")`

Pogledajte promjenu u Swaggeru.

### 2.4 Usmjeravanje akcije

1. Implementirajte akcije `Pomnozi` i `Potenciraj` za množenje i potenciranje dva decimalna broja. Koristite atribute `[HttpGet]`, znači bez informacije za usmjeravanje.

   Kada pokrenete aplikaciju pretraživač će umjesto uobičajenog sučelja pokazati grešku!

   `Failed to load API definition.`

2. Grešku možete dijagnosticirati u pretraživaču, Developer Toolsima. Otvorite Developer Tools u pretraživaču (F12), odaberite `Network` karticu, osvježite stranicu i odaberite `swagger.json` zahtjev koji pokazuje grešku. Nakon toga s desne strane možete otvoriti odgovor sa servera i u njemu vidjeti detalje greške.

   `Conflicting method/path combination "GET api/operations/Matematika" for actions - ...`

   > _Objašnjenje_: sada imamo dvije metode koje su ujedno i dvije `HttpGet` akcije. Web API framework sam ne zna razlikovati dvije akcije prema nazivima metoda (zovu se `Pomnozi` i `Potenciraj`). Zato ćete kada pokrenete aplikaciju dobiti grešku `Failed to load API definition.`.

3. Zato je je potrebno dodati informaciju za usmjeravanje:

   - za metodu `Pomnozi`: `[HttpGet("Pomnozi")]` ili `[HttpGet("[action]")]`
   - za metodu `Potenciraj`: `[HttpGet("Potenciraj")]` ili `[HttpGet("[action]")]`

   Nakon toga Swagger više ne prikazuje pogrešku, a informacija o usmjeravanju je ažurirana.

   > Kao što vidite, moguće je za usmjeravanje koristiti i token `[action]`. Tako u naziv akcije ulazi i ime metode: `[HttpGet("[action]")]`.

### 2.5 Vezanje ulaznih parametara (parameter binding)

1.  Web API akcije mogu se pozivati izravno iz pretraživača.  
    Otvorite novu karticu u pretraživaču i u nju unesite adresu:

        http://localhost:5123/api/operations/Matematika/Zbroji?a=1&b=2

        > Kakav se rezultat dobiva?

2.  Nije potrebno slati parametre, ali moramo znati što u tom slučaju dobivamo

    http://localhost:5123/api/operations/Matematika/Zbroji

    > Što ako ne pošaljemo parametre?  
    > Parametri će dobiti default vrijednosti.

    > Može li akcija deklarirati parametar s default vrijednosti?  
    > Pokušajte.

3.  Što se događa kada pošaljemo npr. `string` umjesto `int`?

    http://localhost:5123/api/operations/Matematika/Zbroji?a=a&b=b

    > Što ako pošaljemo parametre neispravnog tipa?  
    > Doći će do greške prilikom vezanja parametara (parameter bindinga).  
    > `One or more validation errors occurred...`

### 2.6 Spremanje stanja

Akcija može u jednom pozivu spremiti stanje i upotrijebiti ga u drugom. Npr. takav način rada se koristi kod spremanja u bazu podataka.

> Mi ćemo za sada koristiti statičku varijablu umjesto baze podataka.

Osnove Language Integrated Query (LINQ) jezika moguće je naći na sljedećim stranicama:

- https://learn.microsoft.com/en-us/dotnet/standard/linq/
- https://github.com/dotnet/try-samples/tree/main/101-linq-samples/src

---

1. Kreirajte novu RESTful web api aplikaciju sa sljedećim karakteristikama:

- Naziv (solution): vjezbe-2
- Tip projekta: Web API
- Naziv projekta: vjezbe-2-6
- Bez autentifikacije i HTTPS-a
- Neka koristi kontrolere i Swagger

2. Kreirajte kontroler naziva `StatefulController`

3. Kreirajte unutar kontrolera statičku varijablu

   `private static int State { get; set; } = 0;`

4. Kreirajte novu akciju `Add` koja nema parametra niti vraća neku vrijednost, već samo dodaje uvećava varijablu `State` za 1. Proslijedite naziv akcije kao parametar za usmjeravanje.

   > Koju HTTP metodu bismo trebali koristiti za mijenjanje stanja?

5. Kreirajte novu `HttpGet` akciju `GetState` koja vraća vrijednost stanja.

   > Isprobajte sada te dvije akcije.

6. Pokušajte maknuti `static` iz deklaracije varijable.

   Kad isprobate kako to radi, varijabla će uvijek biti 0.

   > _Razlog_: kod svakog poziva metode, kontroler se ponovno instancira i koristi se nova instanca. Staru instancu sakupi Garbage Collector. Ali sjetite se statičkih varijabli! Statičke varijable kod instanciranja objekta klase "preživljavaju", to jest njihova se vrijednost ne mijenja samim instanciranjem!

   Zato možemo koristiti statičku varijablu za spremanje stanja za vrijeme rada aplikacije.

   _Važno_: Kada ponovno pokrenete aplikaciju statičke (i ostale) varijable se resetiraju na početnu vrijednost, jer se proces restarta i memorija koju aplikacija koristi je obrisana.

### 2.7 Vježba: Prosjek

Napravite u kontroleru `Matematika` akciju `Prosjek` koja uzima jedan `int` parametar, a vraća prosječnu vrijednost svih brojeva od 1 do tog broja.

### 2.8 Vježba: Fibonacci

Napravite u kontroleru `Matematika` akciju `Fibonacci` koja uzima jedan parametar N, redni broj Fibonaccijevog broja. Akcija vraća N-ti fibonaccijev broj. Spremite taj broj kao stanje i napravite akciju `LastFibonacci` koji dohvaća zadnje kreirani Fibonaccijev broj.

### 2.9 Vježba: Ponavljajući tekst

Napravite novi kontroler `TextOperation` i akciju `RepeatText` koja uzima jedan tekstualni parametar T i jedan cjelobrojni parametar N. Akcija vraća N puta ponovljeni tekst T.

### 2.10 Vježba: Najčešće slovo

Napravite u kontroleru `TextOperation` akciju `MostFrequentCharacter` koja uzima jedan parametar `sentence`. Akcija treba vratiti slovo koje se načešće pojavljuje u ulaznom parametru.

### 2.11 Vježba: Broj slova u rečenici

Napravite u kontroleru `TextOperation` akciju `SetSentence` koja uzima jedan parametar `sentence` kao rečenicu i sprema ga. Napravite također akciju `CharacterFrequency` koja prima slovo kao ulazni parametar i vraća koliko se takvih slova pojavljuje u spremljenoj rečenici.

### 2.12 Vježba: ROT13

Napravite u kontroleru `TextOperation` akciju `Rot13` koja uzima jedan parametar `input`. Akcija treba vratiti ROT13 kriptirani niz znakova.

> Potražite kako napraviti ROT13 enkripciju.  
> Napomena: ROT13 enkripcija je ujedno i ROT13 dekripcija.
