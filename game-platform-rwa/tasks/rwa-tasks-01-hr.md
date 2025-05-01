# Zbirka zadataka iz Razvoja web aplikacija

## 1 RESTful arhitektura (Web API)

### 1.1 Web API aplikacija - osnovni koncepti

Osnovne (i druge) koncepte moguće je naći na sljedećim stranicama:

- https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio
- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-8.0

### 1.2 Izrada nove Web API aplikacije

Kreirajte novu RESTful web api aplikaciju sa sljedećim karakteristikama:
  - Naziv: MojPrviWebApi
  - Bez autentifikacije i HTTPS-a
  - Neka koristi kontrolere i Swagger

Pokrenite aplikaciju u pregledniku Chrome, a zatim ju pokrenite u pregledniku Edge.

> Koje su pristupne točke implementirane u aplikaciji?

Isprobajte pristupnu točku koristeći Postman sučelje.  
Isprobajte pristupnu točku koristeći Swagger sučelje.  

> Koji je naziv za oblik podatka kojeg vraća pristupna točka?

### 1.3 Upravljanje startanjem aplikacije putem launchSettings.json

U RESTful Web API aplikaciji koju ste napravili u prošlom zadatku napravite sljedeće promjene:
  - postavite port aplikacije na 5123
  - obrišite "IIS Express" profil
  - promijenite naziv profila vaše aplikacije
  - dodajte novi profil koji ne starta preglednik kod starta aplikacije

> Čemu služe profili u Visual Studio .NET okruženju?

### 1.4 Igra s WeatherForecast kontrolerom

U RESTful Web API aplikaciji koju ste izradili u prethodnom zadatku napravite sljedeće promjene:
  - promijenite informacije rute kontrolera  
    `[Ruta("MojiPodaci")]`

    > Koje promjene vidite?

  - promijenite informacije HTTP metode akcije  
    `[HttpGet("...")]` => `[HttpPost()]`

    > Koje promjene vidite?

  - promijeniti povratnu vrijednost akcije
    - vratite string iz akcije
    - vratite broj iz akcije
    - vratite polje stringova iz akcije
    - vratite polje brojeva iz akcije
    - vratite listu brojeva iz akcije
    - implementirajte jednostavnu klasu s brojem, stringom i poljem, inicijalizirajte je i vratite iz akcije

    > Što možete reći o vrijednosti koju vraća akcija?
