# RESTful arhitektura (Web API)

Ovaj materijal je dio Ishoda 1 (željeno).

## 6 JSON Web Token (JWT)

JWT predstavlja tvrdnje (engl. claims) koji se prenose između klijenta i poslužitelja. To je kodirani skup brojeva i slova koji dokazuje da je korisnik onaj za kojeg se predstavlja. JWT token možete smatrati oblikom prijave.

JWT konfiguracija u obliku SecurityTokenDescriptor-a:

- https://learn.microsoft.com/en-us/dotnet/api/microsoft.identitymodel.tokens.securitytokendescriptor?view=msal-web-dotnet-latest

JWT debuger:
- https://jwt.io/

Korištenjem podrške za JWT u ASP.NET Core, možete:
- Zaustaviti neželjeni pristup resursima (kontrolorima i akcijama) u vašem projektu
- Dopustiti pristup sigurnim resursima samo registriranim korisnicima

---

Da biste zaustavili neželjeni pristup resursima u vašem projektu, morate:
- Instalirati NuGet pakete za JWT
- Koristiti međuprogram (engl. middleware) u `Program.cs` za postavljanje i konfiguraciju JWT sigurnosti
- Koristiti atribut `[Authorize]` na kontrolerima ili akcijama koje želite osigurati

Da biste omogućili pristup sigurnim resursima u vašem projektu, morate:
- Stvoriti i vratiti JWT token klijentu u nezaštićenoj krajnjoj točki
- Koristiti taj token u zaglavlju autorizacije kada izdajete zahtjeve zaštićenoj krajnjoj točki
- Dodatno: postaviti podršku za JWT u Swaggeru

Da biste omogućili pristup sigurnim resursima u vašem projektu samo registriranim korisnicima, morate:
- Podržati registraciju korisnika
- Stvoriti i vratiti JWT token za pojedinog korisnika
   - To se može smatrati izvođenjem prijave korisnika

### 6.1 Osiguranje kontrolera i akcija

1. Trebate instalirati sljedeće pakete u projekt:

    ```
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8
    dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect --version 8
    ```

2. U Program.cs, postavite međuprogram (engl. middleware) za konfiguraciju JWT sigurnosti. Ne zaboravite dodati usluge prije izgradnje web aplikacije (´var app = builder.Build()´), i međuprogram nakon toga.

    ```C#
    // Configure JWT security services
    var secureKey = "12345678901234567890123456789012";
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o => {
            var Key = Encoding.UTF8.GetBytes(secureKey);
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Key)
            };
        });
    ```

    ```
    // Use authentication / authorization middleware
    app.UseAuthentication();
    app.UseAuthorization(); // -> this should already be present
    ```

        > Ako pregledate kod, možete vidjeti da se za autentifikaciju JWT tokena koristi sigurni ključ. Ovaj ključ je nešto što je poznato samo vašem poslužitelju i ne smije se dijeliti ni s kim drugim. Također, ne bi trebao biti hardkodiran. Kasnije ćemo premjestiti ključ u konfiguraciju.

3. Stvorite Web API kontroler akciju za čitanje/pisanje pod nazivom `SecuredController`.
Dodajte atribut `[Authorize]` prije prve radnje `Get()`.
Isprobajte kontroler u Swaggeru.

     > Zaštićena radnja vratit će `Error: Unauthorized` jer je osigurana atributom `[Authorize]`. Neosigurana radnja vratit će podatke.

4. Sada označite cijeli kontroler atributom `[Authorize]`.
Isprobajte kontroler u Swaggeru.

     > Sve radnje osiguranog kontrolera vratit će `Error: Unauthorized` zbog osiguranja atributom `[Authorize]`.

### 6.2 Dozvoljavanje pristupa sigurnim resursima

1. Napravite Web API kontroler (prazan) `UserController` koji će podržavati stvaranje JWT tokena.

2. Dodajte novu mapu pod nazivom `Security`, i stvorite klasu `JwtTokenProvider` u toj mapi.

    ```C#
    public class JwtTokenProvider
    {
        public static string CreateToken(string secureKey, int expiration, string subject = null)
        {
            // Get secret key bytes
            var tokenKey = Encoding.UTF8.GetBytes(secureKey);

            // Create a token descriptor (represents a token, kind of a "template" for token)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(expiration),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)                
            };

            if (!string.IsNullOrEmpty(subject))
            {
                tokenDescriptor.Subject = new ClaimsIdentity(new System.Security.Claims.Claim[]
                {
                    new System.Security.Claims.Claim(ClaimTypes.Name, subject),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, subject),
                });
            }

            // Create token using that descriptor, serialize it and return it
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(token);
            
            return serializedToken;
        }
    }
    ```

    Koristit ćete ovu klasu za stvaranje JWT tokena pomoću sigurnog ključa.

    > Napomena: Možete koristiti tu klasu u svom projektu i mijenjati je na način koji želite. Imajte na umu da `SecurityTokenDescriptor` dopušta dodatne parametre. Slobodno istražite ove parametre.

2. Stvorite akciju `GetToken()` koja vraća JWT token

    ```C#
    [HttpGet("[action]")]
    public ActionResult GetToken()
    {
        try
        {
            // The same secure key must be used here to create JWT,
            // as the one that is used by middleware to verify JWT
            var secureKey = "12345678901234567890123456789012";
            var serializedToken = JwtTokenProvider.CreateToken(secureKey, 10);

            return Ok(serializedToken);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    ```

    Testirajte ovu akciju u Swaggeru.

    > Koristite https://jwt.io/ da biste vidjeli sadržaj tokena. Postoji poveznica `Debugger` i tekstualni okvir za unos tokena. Sadržaj tokena možete vidjeti na desnoj strani.

3. Premjestite sigurnosni ključ u konfiguraciju.

    U `appsettings.json` stvorite sekciju `JWT` i u tu sekciju dodajte `SecureKey` koji Vam je potreban i u međuprogramu i tamo gdje stvarate JWT.

    ```JSON
    "JWT": {
      "SecureKey": "12345678901234567890123456789012"
    }
    ```

    > U stvarnosti, kada koristite JWT vjerojatno će vam trebati sigurniji ključ kao što je, na primjer `E(H+MbQeThWmZq4t6w9z$C&F)J@NcRfU` :)

4. Za čitanje konfiguracije u vašem `UserControlleru` (gdje vam je potrebna), morate dopustiti DI spremniku da je proslijedi konstruktoru vašeg kontrolera:

    ```C#
    private readonly IConfiguration _configuration;

    public UserController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    ```

5. Sada pročitajte ključ pomoću konfiguracije umjesto pomoću hardkodirane vrijednosti

    ```C#
    var secureKey = _configuration["JWT:SecureKey"];
    var serializedToken = JwtTokenProvider.CreateToken(secureKey, 10);
    ```

6. Da biste pročitali konfiguraciju u `Program.cs` (gdje vam je također potrebna), jednostavno je pročitajte izravno iz "app buildera":

    ```C#
    var secureKey = builder.Configuration["JWT:SecureKey"];
    ```

7. Kako biste omogućili Swaggeru da prihvati JWT token, trebate konfigurirati njegovo korisničko sučelje pomoću konfiguracije usluge Swagger u `Program.cs`

    ```C#
    builder.Services.AddSwaggerGen(option =>
    {
        option.SwaggerDoc("v1",
            new OpenApiInfo { Title = "RWA Web API", Version = "v1" });

        option.AddSecurityDefinition("Bearer",
            new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter valid JWT",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

        option.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }
            });
    });
    ```

8. Sada testirajte JWT u Swaggeru

    - Koristite radnju `GetToken()` da biste dobili JWT
    - Koristite gumb `Authorize` na vrhu stranice da biste spremili JWT u preglednik
    - Sada isprobajte zaštićenu krajnju točku

    > Napomena: JWT se šalje pomoću zaglavlja `Authorization` u HTTP zahtjevu. To možete primijetiti ispitivanjem zaglavlja zahtjeva u Development Tools alatu preglednika

### 6.3 Podrška za registraciju korisnika

Ne želite izdati JWT nepoznatom korisniku jer svaki korisnik s JWT-om može imati pristup vašim osiguranim resursima. To znači da trebate imati popis svojih registriranih korisnika (čitaj: tablicu baze podataka) s lozinkama.  

Važno je imati lozinku u bazi podataka ne kao čisti tekst, već kao kriptografski hash te lozinke. Kada se korisnik pokuša prijaviti, lozinka koju unese uspoređivat će se s kriptografskim hashom i ako vrijednosti međusobno odgovaraju, korisniku će biti dopušteno dobivanje JWT-a.  

Dakle, prvo podržavamo EF bazu podataka u našem projektu.  
To je ista stvar koju smo radili u vježbama 4.  

1. Kreirajmo bazu podataka `Vježba6` i tablicu za korisnike

    ```SQL
    CREATE TABLE [dbo].[User](
      [Id] [int] IDENTITY(1,1) NOT NULL,
      [Username] [nvarchar](50) NOT NULL, -- Use this for login
      [PwdHash] [nvarchar](256) NOT NULL, -- Use to check password hash
      [PwdSalt] [nvarchar](256) NOT NULL, -- Additional level of security (random string)
      [FirstName] [nvarchar](256) NOT NULL,
      [LastName] [nvarchar](256) NOT NULL,
      [Email] [nvarchar](256) NOT NULL,
      [Phone] [nvarchar](256) NULL,
      CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED (
        [Id] ASC
      )
    )
    ```

2. Upotrijebimo `Package Manager Console` da instaliramo podršku za bazu podataka i konfiguriramo je. **Obratite pozornost na pravilnu promjenu connection stringa.**

    ```
    dotnet tool install --global dotnet-ef --version 8

    dotnet add package Microsoft.EntityFrameworkCore --version 8
    dotnet add package Microsoft.EntityFrameworkCore.Design --version 8
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8

    dotnet ef dbcontext scaffold "server=.;Database=GamePlatformRWA;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true" Microsoft.EntityFrameworkCore.SqlServer -o Models --force
    ```

3. Izrežite/zalijepite connection string iz generiranog konteksta baze podataka `Exercise6Context` u `appsettings.json`:

    ```JSON
    "ConnectionStrings": {
      "ex6cs": "{ovdje-ide-zalijepljeni-connection-string}"
    }
    ```

4. U `Exercise6Context.cs` trebali biste imati ovo:

    ```C#
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=ConnectionStrings:ex6cs");
    ```

5. U `Program.cs` trebate dodati ovo:

    ```C#
    builder.Services.AddDbContext<Exercise6Context>(options => {
        options.UseSqlServer("name=ConnectionStrings:ex6cs");
    });
    ```

6. Sada možemo podržati registraciju korisnika u `UserController.cs`. Prvo koristimo DI da dobijemo kontekst baze podataka.

    ```C#
    private readonly IConfiguration _configuration;
    private readonly Exercise6Context _context;

    public UserController(IConfiguration configuration, Exercise6Context context)
    {
        _configuration = configuration;
        _context = context;
    }
    ```

7. Trebamo DTO klasu za korisnika, pa kreiramo mapu `Dtos` i unutra klasu `UserDto`:

    ```C#
    public class UserDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name should be between 2 and 50 characters long")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name should be between 2 and 50 characters long")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Provide a correct e-mail address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Provide a correct phone number")]
        public string Phone { get; set; }
    }
    ```

8. Kako bismo podržali kriptografsku operaciju stvaranja salt i hash vrijednosti, stvorit ćemo pomoćnu klasu `PasswordHashProvider` u mapi `Security`:

    ```C#
    public class PasswordHashProvider
    {
        public static string GetSalt()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
            string b64Salt = Convert.ToBase64String(salt);

            return b64Salt;
        }

        public static string GetHash(string password, string b64salt)
        {
            byte[] salt = Convert.FromBase64String(b64salt);

            byte[] hash =
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8);
            string b64Hash = Convert.ToBase64String(hash);

            return b64Hash;
        }
    }
    ```

9. Sada možemo registrirati korisnika koristeći kreirani DTO i pomoćnu kriptografsku klasu. Imajte na umu da pazimo da izbjegnemo dvostruku registraciju korisnika tako da provjerimo postojeće korisničko ime.

    ```C#
    [HttpPost("[action]")]
    public ActionResult<UserDto> Register(UserDto userDto)
    {
        try
        {
            // Check if there is such a username in the database already
            var trimmedUsername = userDto.Username.Trim();
            if (_context.Users.Any(x => x.Username.Equals(trimmedUsername)))
                return BadRequest($"Username {trimmedUsername} already exists");

            // Hash the password
            var b64salt = PasswordHashProvider.GetSalt();
            var b64hash = PasswordHashProvider.GetHash(userDto.Password, b64salt);

            // Create user from DTO and hashed password
            var user = new User
            {
                Id = userDto.Id,
                Username = userDto.Username,
                PwdHash = b64hash,
                PwdSalt = b64salt,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Phone = userDto.Phone,
            };

            // Add user and save changes to database
            _context.Add(user);
            _context.SaveChanges();

            // Update DTO Id to return it to the client
            userDto.Id = user.Id;

            return Ok(userDto);

        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    ```

    > Napomena: krajnja točka `Registracija` namijenjena je za samoregistraciju.  
    > Ako osigurate krajnju točku `Registracija`, registracija korisnicima neće biti dopuštena.

10. Sada možete testirati registraciju korisnika pomoću Swaggera i nove krajnje točke `Register()`.  
JSON DTO primjer:

    ```JSON
    {
      "username": "johnny1234",
      "password": "qwertzuiop",
      "firstName": "John",
      "lastName": "Smith",
      "email": "johnsmith1234@example.com",
      "phone": "0987654321"
    }
    ```

### 6.4 Podrška za prijavu korisnika

Kao što je već spomenuto, proces prijave uključuje provjeru postoji li korisnik i ako postoji, dobivanje tokena. Token je obično personaliziran, što znači da sadrži tvrdnje (engl. claims) o korisniku (korisničko ime, uloga, itd...)  
Nakon prijave, korisnik dobiva token kojeg će poslati natrag poslužitelju kada pristupa zaštićenoj krajnjoj točki.  

> Napomena: da bi ovo radilo, morate stvoriti klasu `UserLoginDto`

  ```C#
  [HttpPost("[action]")]
  public ActionResult Login(UserLoginDto userDto)
  {
      try
      {
          var genericLoginFail = "Incorrect username or password";

          // Try to get a user from database
          var existingUser = _context.Users.FirstOrDefault(x => x.Username == userDto.Username);
          if (existingUser == null)
              return BadRequest(genericLoginFail);

          // Check is password hash matches
          var b64hash = PasswordHashProvider.GetHash(userDto.Password, existingUser.PwdSalt);
          if(b64hash != existingUser.PwdHash)
              return BadRequest(genericLoginFail);

          // Create and return JWT token
          var secureKey = _configuration["JWT:SecureKey"];
          var serializedToken = JwtTokenProvider.CreateToken(secureKey, 120, userDto.Username);

          return Ok(serializedToken);
      }
      catch (Exception ex)
      {
          return StatusCode(500, ex.Message);
      }
  }
  ```

  > Ako osigurate `Login` krajnju točku, korisnicima neće biti dopuštena prijava.

  > Koristite https://jwt.io/ da vidite sadržaj tokena. Sada možete vidjeti korisnikovo ime. Također, kada korisnik izvrši autentificirani zahtjev, međuprogram se brine o sadržaju JWT-a i može ga pružiti kada vam je potreban, zajedno s imenom i drugim podacima koji su u JWT-u.

### 6.5 Middleware podrška za JWT podatke 

Middleware brine o identitetu korisnika proslijeđenom putem JWT-a.  
Prosljeđuje korisničke podatke putem `HttpContext.User.Identity`, otkuda ih možete dohvatiti.  

  ```C#
  // Just an example in SecuredController
  [HttpGet]
  public ActionResult<string> Get()
  {
      var identity = HttpContext.User.Identity as ClaimsIdentity;
      return identity.FindFirst(ClaimTypes.Name).Value;
  }
  ```

### 6.6 Podrška za uloge

Možete podržati kontrolu pristupa temeljenu na ulogama putem JWT-a.  
Za resurse koje je potrebno zaštititi ulogama, upotrijebite isti atribut `Authorize` i dodajte mu uloge.

  ```C#
  [Authorize(Roles = "Admin")]
  ```

  ```C#
  [Authorize(Roles = "Admin,User")]
  ```

Da bi to funkcioniralo, trebate dodati tvrdnju `Role` prilikom generiranja tokena.

  ```C#
  var role = "Admin" // for example...
  tokenDescriptor.Subject = new ClaimsIdentity(new System.Security.Claims.Claim[]
  {
      new System.Security.Claims.Claim(ClaimTypes.Name, subject),
      new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, subject),
      new System.Security.Claims.Claim(ClaimTypes.Role, role),
  });
  ```

### 6.7 Vježba: Stvorite ASP.NET Web API projekt sa zaštićenim kontrolerom

1. Stvorite novi Web API projekt koji koristi istu bazu podataka `Exercise6`.
2. Stvorite podršku u bazi podataka za entitete `User`, `Product`, `Receipt` i `ReceiptItem` i povežite projekt sa svojom bazom podataka.  

    Atributi entiteta `Product`:
    - Id (int)
    - Title (string)
    - Price (decimal)

    Atributi entiteta `Receipt`:
    - Id (int)
    - Code (string)
    - Total (decimal)
    - IssuedAt (DateTime)

    Atributi entiteta `ReceiptItem`:
    - Id (int)
    - ProductId (int)
    - ReceiptId (int)
    - Quantity (int)
    - Price (decimal)
    - Value (decimal)

    > Napomena: pazite na pravilno postavljanje stranih ključeva.

3. Napravite kontroler za registraciju i prijavu korisnika (dobivanje JWT-a).  
4. Postavite JWT podršku u Swaggeru.  
5. Napravite CRUD kontroler za entitet `Product` i CRUD kontroler za entitet `Receipt`
6. Koristite entitete `ReceiptItem` kada dohvaćate/pohranjujete entitet `Product`
   - dodajte zbirku `ReceiptItems` klasi entiteta `Product`
7. Osigurajte `Receipt` CRUD kontroler.  

### 6.8 Vježba: Promjena lozinke

Za prethodni zadatak dodajte krajnju točku `ChangePassword` koja korisniku omogućuje promjenu lozinke.

### 6.9 Vježba: Podrška za uloge 

Za prethodni zadatak, podržite entitet uloge (`Roles`) u svojoj bazi podataka i članstvo korisnika u pojedinoj ulozi. Uloge bi trebale biti `Admin` i `User`. Članstvo u `Admin` ulozi treba omogućiti čitanje i pisanje svih entiteta. Članstvo u `User` ulozi omogućuje čitanje proizvoda.  
Prilikom samoregistracije korisnika, postavite njegovu ulogu na `Korisnik`.  
Dodajte zaštićenu krajnju točku `PromoteUser` koja omogućuje promicanje registriranog korisnika u administratora.
