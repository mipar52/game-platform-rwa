# RESTful arhitektura (Web API)

Ovaj materijal je dio Ishoda 1 (minimalno).

## 3 Akcije u stvarnom svijetu i CRUD

Osnove Language Integrated Query (LINQ) jezika moguće je naći na sljedećim stranicama:

- https://learn.microsoft.com/en-us/dotnet/standard/linq/
- https://github.com/dotnet/try-samples/tree/main/101-linq-samples/src

### 3.1 Spremanje u kolekciju

1. Napravite novu RESTful Web API aplikaciju sa sljedećim svojstvima:

    - Naziv (solution): vjezbe-3
    - Tip projekta: Web API
    - Naziv projekta: vjezbe-3-1
    - Bez autentifikacije i HTTPS-a
    - Neka koristi kontrolere i Swagger

2. Kreirajte novi API kontroler (Installed > Common > API, API Controller - Empty) naziva `CollectionController` i obrišite `WeatherForecastController`. 

3. Dodajte varijablu stanja kontroleru, koja može držati popis cijelih brojeva:  
   `private static List<int> State { get; set; } = new()`

   _NOTE: kao što vidite, možete koristiti skraćenu inicijalizaciju za tip - `new()`_

4. Kreirajte novu akciju `[HttpPost("[action]")] AddToState` koja kao parametar uzima polje integera - `int[] Numbers`. Akcija dodaje to polje integera u `State`.  

    > Možete koristiti izraz `.AddRange()`

    > Da biste testirali ovu promjenu, trebat će Vam breakpoint i unos JSON polja u Swagger, kao što je `[3, 5, 8]`.

5. Vratite `ActionResult` tip:

   - promijenite povratnu vrijednost u `ActionResult`
   - vratite `Ok()` iz metode

6. Zaštitite kod korištenjem try/catch bloka.   
   U slučaju pogreške vratite `StatusCode(500)`

### 3.2 Vraćanje kolekcije iz akcije

1. Implementirajte akciju `[HttpGet("[action]")] GetState` koja vraća stanje kao listu.

2. Tip koji se vraća treba biti `ActionResult<T>`:
   - koristite `ActionResult<List<int>>` kao povratni tip
   - vratite `Ok(State)` iz metode
   - implementirajte `try...catch` i vratite `StatusCode(500)` iz catch bloka

### 3.3 Dodavanje i uklanjanje pojedinačne stavke u/iz kolekcije

1. Implementirajte `[HttpPost("[action]")] AddItem(int number)` akciju koja dodaje broj u listu.

2. Implementirajte `[HttpDelete("[action]")] RemoveItem(int number)` akciju koja uklanja sve brojeve jednake broju koji je proslijeđen kao parametar iz liste. Koristite `.RemoveAll()` metodu za to.

### 3.4 Korištenje modela za spremanje stanja

Model je oblik podatka u aplikaciji (npr. zgrada koja ima svojstva: ulica, vrsta, kućni broj).  
Model često ima identifikator. Konvencija je da je naziv identifikatora bude `Id`.  
Modeli se često spremaju u posebnu mapu, npr. `Modeli`.  

1. Napravite novu RESTful Web API aplikaciju sa sljedećim karakteristikama:

    - Naziv (solution): vjezbe-3 (aktualna)
    - Project type: Web API
    - Naziv projekta: vjezbe-3-4
    - Nema autentifikacije i HTTPS-a
    - Neka koristi kontrolere i Swagger

2. U novoj `Models` mapi kreirajte klasu `Street` sa svojstvima:
     - `int Id`
     - `string Name`

3. Stvorite novi API kontroler pod nazivom `AddressController` odabirom opcije "Installed > Common > API, API Controller with read/write actions" tijekom izrade __(ne "MVC controller with read/write actions")__.

    > Primijetite da se sve HTTP metode implementiraju automatski. Također, implementirane su dvije `HttpGet` akcije: `Get()` i `Get(int id)`. Prva je namijenjena za dohvaćanje kolekcije svih stavki, a druga za dohvaćanje jedne određene stavke prema njenom identifikatoru.

4. Implementirajte popis ulica kao statičku varijablu `Ulice` novog kontrolera (ne zaboravite inicijalizaciju).

5. Promijenite akciju `Get()` tako da vraća popis ulica.

6. Promijenite akciju `Get(int id)` tako da vraća ulicu prema identifikatoru.

     > Za to upotrijebite LINQ izraz `.First()`. Na primjer: `Streets.First(x => x.Id == id)`.

7. Promijenite akciju `Post(...)` tako da dodaje ulicu koja joj je proslijeđena kao ulazni parametar. Zanemarite član `Id` koji je poslan i sami izračunajte koji je sljedeći identifikator. Neka ta akcija vrati i tu dodanu ulicu.

    > Za izračun sljedećeg identifikatora možete koristiti LINQ operator `.Max(x => x.Id)`
    >
    > Možete koristiti LINQ operator `.Any()` da biste provjerili postoji li neka ulica u kolekciji ulica

8. Promijenite akciju `Put(...)` tako da u skladu s proslijeđenim identifikatorom ažurira ulicu koja je također proslijeđena kao ulazni parametar. Potrebno je ažurirati samo član `Ime`. Neka ta akcija vrati ažuriranu ulicu.

9. Promijenite akciju `Delete(...)` tako da briše ulicu prema proslijeđenom identifikatoru.

10. Izbrišite `WeatherForecastController`.

11. Dodajte akciju `Search` za dohvaćanje svih ulica koje u svom nazivu sadrže vrijednost ulaznog parametra `text`.

12. Promijenite akciju `Search` na način da omogućite sortiranje izlaza prema članu `Id` ili `Name`.

13. Promijenite akciju `Search` na način da omogućite pronalaženje prvih N proizvoda, drugih N proizvoda i tako dalje; u tu svrhu možete koristiti LINQ operatore `Skip()` i `Take()`. Za to će vam trebati dodatna dva parametra, npr. `start` i `count`.

### 3.5 Poboljšajte implementaciju akcija

Promijenite sve akcije u skladu s onim što smatramo dobrom implementacijom akcije:
- tip povratne vrijednosti `ActionResult<T>`
- zaštita akcije pomoću `try/catch`
- vraćanje rezultata kao što su `Ok(result)` ili `BadRequest()` i slično
   - vratiti 400 ako korisnik pokuša stvoriti ulicu s praznim imenom
   - vratiti 500 iz bilo koje metode gdje je primjenjivo
- logiranje grešaka unutar bloka `catch`
   - nije potrebno implementirati logiranje, samo ostavite TODO komentar umjesto toga

Primjer:

```C#
[HttpPost]
public ActionResult<Street> Post([FromBody] Street street)
{
    try
    {    
        int maxId = Streets.Any() ? Streets.Max(x => x.Id) : 0;
        street.Id = maxId + 1;

        Streets.Add(street);

        return Ok(street);
    }
    catch (Exception)
    {
        // TODO: Log error, use details from the exception

        return BadRequest("There was a problem while updating street");
    }
}
```

### 3.6 Returning case-specific codes

Uz `200 OK`, `400 Bad Request` ili `500 Internal Server Error`, u određenim slučajevima važno je vratiti neki drugi specifični HTTP kod.  

Na primjer:
- `201 Created` u slučaju kada je Web API "resurs" (stavka) kreiran POST metodom
- `204 No Content` ako ne želimo vratiti detalje, na primjer kada stvaramo ili ažuriramo resurs
- `401 Neautorizirano` ako klijent nije autentificiran
- `403 Forbidden` ako je klijent autentificiran, ali nije autoriziran (zbunjujuće zbog imena, ali istinito)
- `404 Not Found` ako resurs nije pronađen
- `500 Internal Server Error` ako nema posebnog objašnjenja zašto poslužitelj ne može izvršiti zahtjev, pa vraćamo "generički" razlog

---

1. Iskoristite `201 Created` (POST)  

Umjesto Ok(), koristite Created(). Za tu promjenu trebat će vam informacije o URL-u gdje se može pronaći stvoreni resurs.  

Na primjer:

    ```C#
    ...
    var location = Url.Action(nameof(Get), new { id = street.Id });
    return Created(location, street);

    ```

2. Iskoristite `404 Not Found` (GET za Id, PUT, DELETE)

Provjerite postoji li resurs i ako ne, vratite statusni kod 404 koristeći `NotFound()`.

### 3.7 Korištenje složenijeg modela za spremanje stanja

U ovoj vježbi pohranit ćemo i ulice i kućne brojeve u istu strukturu. Da bismo to učinili, stvorimo statičku klasu sa statičkim parametrom kao spremištem podataka.

1. Napravite mapu `Repository`

2. Napravite klasu `StreetRepository` u njoj i učinite je statičkom

3. Premjestite statičku kolekciju `Ulice` iz `AddressController` u `AddressRepository`.

4. U `AddressController`, zamijenite svu upotrebu `Streets` s `AddressRepository.Streets`.

Sada možete koristiti tu strukturu kao pohranu za svoje podatke iz različitih kontrolera.

5. Napravite kontroler `HouseNumberController` koji koristi model `HouseNumber` sa svojstvima `int Number` i `string Addendum`.

6. Neka kućni brojevi budu dio klase `Ulica`; npr. pohranite ih ü listu.

7. Implementirajte dobre akcije `GET`, `POST` i `DELETE` za određeni kućni broj.

     > NAPOMENA: trebat će vam dodatno svojstvo `StreetId` u modelu `HouseNumber` za POST zahtjev.
     >
     > Zašto nema smisla ovdje implementirati metodu `PUT`?

8. Implementirajte dobru akciju `GET` koja, za dani identifikator ulice, vraća sve kućne brojeve za tu ulicu. Neka kućni brojevi budu poredani prvo po broju, a zatim po dodatku.

### 3.8 Vježba: Krajnja točka za snimanje loga

Implementirajte krajnju točku koja bilježi logove i vraća ih na zahtjev. Krajnja točka je http://localhost:5123/api/admin/logs. Logovi se bilježe pomoću POST zahtjeva i dohvaćaju pomoću GET zahtjeva. Jedan log sadrži timestamp (datum i vrijeme), razinu loga (broj od 1 do 8) i poruku loga. Krajnja točka mora dohvatiti posljednjih N logova, a taj se parametar prosljeđuje pomoću `int last`. Ako korisnik nije proslijedio takav parametar, vratite 20 zapisa.

### 3.9 Vježba: Administracija proizvoda

Za rješavanje ovog zadatka upotrijebite isti projekt kao i za prošli zadatak.

Implementirajte krajnju točku za dohvaćanje, dodavanje, ažuriranje i brisanje proizvoda. Neka URL bude http://localhost:{port}/api/store/product. Neka proizvod ima naziv (name), opis (description), cijenu (price), datum izrade (creation date) i ocjenu (rating). Za identifikaciju proizvoda trebat će vam i identifikator.

Također implementirajte:
- preuzimanje pojedinačnog proizvoda
- pretraživanje
- straničenje (paging)

### 3.10 Vježba: Logiranje administracije proizvoda

Za rješavanje ovog zadatka upotrijebite isti projekt kao i za prošli zadatak.

Logirajte promjene i pogreške u administraciji proizvoda.

Testirajte API.
