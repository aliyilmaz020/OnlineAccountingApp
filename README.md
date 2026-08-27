<a id="turkce"></a>

# OnlineAccountingApp

**Türkçe** | [English](#english)

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![EF Core](https://img.shields.io/badge/EF%20Core-10.0.10-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927)
![Architecture](https://img.shields.io/badge/architecture-Clean%20%2B%20CQRS-blue)

Her şirketin **kendi veritabanına** sahip olduğu, çok kiracılı (multi-tenant) bir ön muhasebe
API'si. Clean Architecture katmanlaması, MediatR ile CQRS ve EF Core üzerine kuruludur.

## İçindekiler

1. [Genel Bakış](#genel-bakış)
2. [Teknolojiler](#teknolojiler)
3. [Gereksinimler ve Kurulum](#gereksinimler-ve-kurulum)
4. [Veritabanı ve Migration'lar](#veritabanı-ve-migrationlar)
5. [Mimari](#mimari)
6. [Çok Kiracılılık Modeli](#çok-kiracılılık-modeli)
7. [API Referansı](#api-referansı)
8. [Örnek Veri Doldurma (Seed)](#örnek-veri-doldurma-seed)
9. [gRPC Servisi](#grpc-servisi)
10. [Yanıt Zarfı ve Hata Kodları](#yanıt-zarfı-ve-hata-kodları)
11. [Yeni Feature Ekleme](#yeni-feature-ekleme)
12. [Yol Haritası](#yol-haritası)

## Genel Bakış

Sistemde iki tür veritabanı vardır:

- **Master veritabanı** (`AccountingMasterDb`) — şirket kayıtlarını ve kullanıcı/rol bilgilerini
  (ASP.NET Identity) tutar. Her `Company` kaydı, o şirketin kendi veritabanına ait bağlantı
  bilgilerini de saklar.
- **Şirket veritabanları** — her şirket için ayrı bir veritabanı. Muhasebe verileri
  (örn. tekdüzen hesap planı) burada tutulur ve şirketler birbirinin verisini asla göremez.

Bir istek hangi şirkete ait olduğunu `X-Company-Id` başlığı ile bildirir; API o şirketin
veritabanına bağlanır. Başlıktaki şirket, token'daki kullanıcının `UserCompany` üzerinden
gerçekten erişim hakkı olduğu bir şirket olmak zorundadır.

> **Kimlik doğrulama zorunludur.** `/api/Auth/*` dışındaki tüm uç noktalar
> `Authorization: Bearer <token>` başlığı ister.

## Teknolojiler

| Alan | Kullanılan |
| --- | --- |
| Hedef framework | .NET 10 (`net10.0`) |
| ORM | Entity Framework Core 10.0.10 (SQL Server) |
| CQRS / Mediator | MediatR 14.2.0 |
| Nesne eşleme | Mapster 10.0.11 |
| Doğrulama | FluentValidation 12.1.1 |
| Kimlik doğrulama | ASP.NET Core Identity + JWT Bearer 10.0.10 |
| Refresh token deposu | Redis (StackExchange.Redis 3.1.13) |
| API dokümantasyonu | Swashbuckle 10.2.3 / Microsoft.OpenApi 2.9.0 |

## Gereksinimler ve Kurulum

**Gereksinimler**

- .NET 10 SDK
- SQL Server (LocalDB, Express veya Docker)
- Redis (refresh token deposu için gerekli — Docker önerilir, bkz. [Auth](#api-referansı))
- EF Core CLI: `dotnet tool install --global dotnet-ef`

**Adımlar**

```bash
# 1. Depoyu klonlayın
git clone https://github.com/aliyilmaz020/OnlineAccountingApp.git
cd OnlineAccountingApp/api

# 2. Bağlantı dizesini yapılandırın (aşağıdaki nota bakın)

# 3. Master veritabanını oluşturun
dotnet ef database update \
  --project OnlineAccountingApp.Persistence \
  --startup-project OnlineAccountingApp.WebApi \
  --context AppDbContext

# 4. API'yi çalıştırın
dotnet run --project OnlineAccountingApp.WebApi
```

| Adres | Açıklama |
| --- | --- |
| `http://localhost:5251` | HTTP |
| `https://localhost:7025` | HTTPS |
| `http://localhost:5251/swagger` | Swagger UI (yalnızca Development ortamında) |

**Bağlantı dizesi.** `OnlineAccountingApp.WebApi/appsettings.json` içindeki `ConnectionStrings:SqlServer`
master veritabanını gösterir:

```json
"ConnectionStrings": {
  "SqlServer": "Server=localhost;Database=AccountingMasterDb;User Id=sa;Password=Password1;TrustServerCertificate=True;"
}
```

> **Güvenlik notu:** Depoda yerel geliştirme için bir `sa` parolası bulunuyor. Gerçek bir ortamda
> bu bilgiyi dosyada tutmayın; `dotnet user-secrets set "ConnectionStrings:SqlServer" "..."`
> kullanın. Aynı şey şirket kayıtlarında saklanan `ServerPassword` alanı için de geçerlidir —
> şu an düz metin olarak saklanmaktadır.

**JWT ayarları.** Aynı dosyadaki `Jwt` bölümü token üretimini yapılandırır:

```json
"Jwt": {
  "Issuer": "OnlineAccountingApp",
  "Audience": "OnlineAccountingApp.Client",
  "SecretKey": "dev-only-secret-change-me-with-user-secrets-at-least-32-bytes",
  "AccessTokenMinutes": 15,
  "RefreshTokenDays": 7
}
```

> `SecretKey` de bağlantı dizesi gibi depoya girmemelidir:
> `dotnet user-secrets set "Jwt:SecretKey" "..."`. HMAC-SHA256 için en az 32 baytlık bir anahtar
> kullanın.

**Redis ayarları.** Refresh token'lar Redis'te tutulur; bağlantı bilgisi `Redis:ConnectionString`
alanından okunur:

```json
"Redis": {
  "ConnectionString": "localhost:6379,password=secret,abortConnect=false"
}
```

Yerelde Docker ile hızlıca ayağa kaldırmak için (yukarıdaki varsayılan bağlantı dizesiyle birebir
uyumludur):

```bash
docker run -d --name c_redis -p 6379:6379 redis:7 redis-server --requirepass secret
```

## Veritabanı ve Migration'lar

Projede iki ayrı `DbContext` ve iki ayrı migration klasörü vardır.

**Master veritabanı (`AppDbContext` → `Migrations/AppDb`)**

```bash
# Yeni migration ekleme
dotnet ef migrations add <Ad> \
  --project OnlineAccountingApp.Persistence \
  --startup-project OnlineAccountingApp.WebApi \
  --context AppDbContext -o Migrations/AppDb

# Uygulama
dotnet ef database update \
  --project OnlineAccountingApp.Persistence \
  --startup-project OnlineAccountingApp.WebApi \
  --context AppDbContext
```

**Şirket veritabanları (`CompanyDbContext` → `Migrations/CompanyDb`)**

```bash
# Yeni migration ekleme
dotnet ef migrations add <Ad> \
  --project OnlineAccountingApp.Persistence \
  --startup-project OnlineAccountingApp.WebApi \
  --context CompanyDbContext -o Migrations/CompanyDb
```

Şirket migration'ları `dotnet ef database update` ile **uygulanmaz**; çünkü her şirketin bağlantı
dizesi farklıdır. Bunun yerine çalışma zamanında şu uç nokta çağrılır:

```
GET /api/Companies/MigrateCompanyDb
```

Bu uç nokta master veritabanındaki tüm şirketleri dolaşır, her biri için kendi bağlantı dizesiyle
bir `CompanyDbContext` açar ve bekleyen migration'ları uygular. Yeni bir şirket oluşturduktan sonra
da, şema değişikliğini tüm kiracılara yaydıktan sonra da bu uç nokta çağrılmalıdır.

> **Not:** `CompanyDbContext.CompanyDbContextFactory`, tasarım zamanında (migration üretirken)
> master veritabanına ait bağlantı dizesini kod içinde sabit tutar ve ilk şirket kaydını şablon
> olarak kullanır. Farklı bir yerel kurulumunuz varsa bu dizeyi güncellemeniz gerekir.

> **Refresh token'lar artık master veritabanında değil.** `RefreshTokens` tablosu kaldırıldı (bkz.
> [Auth](#api-referansı)); mevcut bir kurulumdan güncelleme yapıyorsanız tabloyu düşürecek bir
> migration eklemeniz gerekir:
>
> ```bash
> dotnet ef migrations add RemoveRefreshTokens \
>   --project OnlineAccountingApp.Persistence \
>   --startup-project OnlineAccountingApp.WebApi \
>   --context AppDbContext
> dotnet ef database update \
>   --project OnlineAccountingApp.Persistence \
>   --startup-project OnlineAccountingApp.WebApi \
>   --context AppDbContext
> ```
>
> Bu migration'la birlikte tablodaki mevcut kayıtlar (ve dolayısıyla aktif oturumlar) kaybolur.

## Mimari

Clean Architecture; bağımlılıklar daima içe doğru akar.

```
api/
├── OnlineAccountingApp.Domain/          # Bağımlılıksız çekirdek
│   ├── Abstracts/BaseEntity.cs          # Id, CreateDate, EditDate, Status, Deleted
│   ├── AppEntities/                     # Master DB: Company, UserCompany, AppUser, AppRole
│   ├── CompanyEntities/                 # Şirket DB: UniformChartOfAccount
│   └── Exceptions/                      # BusinessException, ValidationException, AppErrorCodes
│
├── OnlineAccountingApp.Framework/       # Genel amaçlı MediatR altyapısı (yalnızca Domain'e bağlı)
│   ├── Services/                        # IRepository, IUnitOfWork, PagedResult
│   └── MedatR/Create|Update|Delete|GetById|GetList/
│                                         # BaseXCommand/Query, BaseXCommandHandler, BaseXCommandValidator
│
├── OnlineAccountingApp.Application/     # Kullanım senaryoları (Domain + Framework'e bağlı)
│   ├── Features/AppFeatures/            # Master DB feature'ları (CQRS)
│   ├── Features/CompanyFeatures/        # Şirket DB feature'ları (CQRS)
│   ├── Services/AppServices/            # ICompanyService, IRoleService, IRefreshTokenService, ...
│   ├── Services/CompanyServices/        # ICompanyContext, ICompanyUnitOfWork, IUniformChartOfAccountService
│   ├── Behaviors/ValidationBehavior.cs  # MediatR pipeline doğrulaması
│   └── Mapper/MapsterConfig.cs
│
├── OnlineAccountingApp.Persistence/     # EF Core gerçeklemesi
│   ├── Context/AppDbContext.cs          # Master DB (IdentityDbContext)
│   ├── Context/CompanyDbContext.cs      # Şirket DB (çalışma zamanında kurulur)
│   ├── Services/Repository.cs           # Repository<TEntity, TContext> — her iki DB'ye de hizmet eder
│   ├── Services/UnitOfWork.cs           # Master DB
│   ├── Services/AppServices/            # CompanyService
│   ├── Services/CompanyServices/        # UniformChartOfAccountService, CompanyUnitOfWork
│   ├── Configurations/ + Constants/     # Şirket DB tablo yapılandırmaları
│   └── Migrations/AppDb + Migrations/CompanyDb
│
├── OnlineAccountingApp.Infrastructure/  # Dış dünya gerçeklemeleri
│   ├── Options/JwtOptions.cs, RedisOptions.cs   # "Jwt" ve "Redis" yapılandırma bölümleri
│   └── Services/JwtTokenService.cs, RedisCacheService.cs, RedisRefreshTokenService.cs
│                                         # ITokenService, ICacheService, IRefreshTokenService (Redis tabanlı)
│
├── OnlineAccountingApp.Grpc/            # İkinci sunum katmanı: gRPC host'u
│   ├── Protos/                          # auth, companies, roles, uniform_chart_of_accounts
│   ├── Services/                        # *GrpcService — aynı MediatR komut/sorgularını dispatch eder
│   ├── Interceptors/                    # BusinessExceptionInterceptor
│   └── DependencyInjections/            # WebApi'nin DI kaydının elle senkronize edilen kopyası
│
└── OnlineAccountingApp.WebApi/          # REST sunum katmanı
    ├── Controllers/                     # İnce MediatR dispatcher'ları
    ├── Tenancy/                         # HttpCompanyContext, RequiresCompanyHeaderAttribute
    ├── DependencyInjections/            # AddApplication(), AddPersistence()
    ├── Configurations/                  # Swagger, hata yönetimi
    ├── ExceptionHandling/               # GlobalExceptionHandler
    ├── Filters/ApiResultFilter.cs       # Yanıtları ApiResponse ile sarar
    └── Models/ApiResponse.cs
```

Controller'lar iş mantığı içermez; yalnızca komutu/sorguyu MediatR'a iletir:

```csharp
[HttpPost("[action]")]
public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyCommand command, CancellationToken cancellationToken)
{
    var result = await mediator.Send(command, cancellationToken);
    return Ok(result);
}
```

### `OnlineAccountingApp.Framework`

`Domain`'in hemen üstünde, `Application`'ın altında yer alan bağımsız bir katmandır. İki şey sağlar:

- `Services/IRepository<T>`, `IUnitOfWork`, `PagedResult<T>` — genel veri erişim soyutlamaları.
- `MedatR/Create|Update|Delete|GetById|GetList/` — her aksiyon için şablon-metot (template method)
  tabanlı base command/query, handler ve validator sınıfları (`BaseCreateCommand<TResponse>`,
  `BaseCreateCommandHandler<TCommand, TEntity, TResponse>`, `BaseCreateCommandValidator<TCommand>`
  ve diğer dört aksiyon için eşleniği). Handler'ların `Handle()` akışı sabittir; özelleştirme
  `virtual` metotlar üzerinden yapılır:

  | Metot | Ne için |
  | --- | --- |
  | `GetExistsPredicate` (Create) / `GetConflictPredicate` (Update) | Benzersizlik kontrolü |
  | `BuildPredicate`, `GetIncludes` (GetById / GetList) | Sorgu filtresi, eager-load |
  | `Before/AfterCreateAsync`, `Before/AfterUpdateAsync`, `Before/AfterDeleteAsync` | Persist öncesi/sonrası ek mantık |
  | `GetNotFoundErrorCode/Message`, `GetAlreadyExistsErrorCode/Message`, `GetConflictErrorCode/Message` | Entity'ye özgü `AppErrorCodes` ve mesaj |

  `Company` ve `UniformChartOfAccount` feature'ları bu altyapıyı kullanır (bkz.
  `CreateCompanyCommandHandler`, `CreateUniformChartOfAccountCommandHandler`).

> **Not:** `AppRole`, `BaseEntity`'den değil ASP.NET Identity'nin `IdentityRole<string>`'ından
> türer ve `IRoleService`, `IRepository<T>` implemente etmez. Bu yüzden `RoleFeature` bu base
> sınıflara uymaz ve doğrudan `IRequest<T>` / `IRequestHandler<,>` / `AbstractValidator<T>`
> kullanmaya devam eder.

## Çok Kiracılılık Modeli

Master veritabanındaki her `Company` kaydı, kendi veritabanının bağlantı bilgilerini taşır:
`ServerName`, `DatabaseName`, `ServerUserId`, `ServerPassword`.

`AppDbContext` DI'da sabit bir bağlantı dizesiyle kayıtlıdır. `CompanyDbContext` ise **isteğe göre**
kurulur: hangi şirkete bağlanılacağı `X-Company-Id` başlığından okunur.

```
İstek
  │  Authorization: Bearer <token>
  │  X-Company-Id: 16e1818a-...
  ▼
HttpCompanyContext ──► CompanyId (başlıktan)  +  UserId (token claim'inden)
  │
  ▼
AddCompanyTenancy() (PersistenceDependencyInjection)
  │  Master DB'den Company kaydını bulur
  │  Başlık yoksa       → 03400
  │  Şirket bulunamazsa  → 03404
  │  UserCompany kaydı yoksa → 04403   ← erişim kontrolü
  ▼
CompanyDbContext (o şirketin bağlantı dizesiyle)
  │
  ▼
UniformChartOfAccountService  +  ICompanyUnitOfWork
```

Başlığın tek başına bir yetki kanıtı olmadığına dikkat edin: kullanıcının o şirkete
`UserCompany` üzerinden bağlı olması gerekir, aksi halde kimliği doğrulanmış herhangi bir kullanıcı
başka bir kiracının verisini okuyabilirdi.

> ⚠ **Kullanıcıyı şirkete bağlama.** Henüz `UserCompany` kaydı oluşturan bir uç nokta yok. Yeni
> kayıt olan bir kullanıcı hiçbir şirkete bağlı olmadığı için tüm kiracı uçlarından `04403` alır.
> Bu uç nokta yazılana kadar kaydı elle eklemeniz gerekir:
>
> ```sql
> INSERT INTO UserCompanies (Id, AppUserId, CompanyId, CreateDate, Status, Deleted)
> VALUES (NEWID(), '<user-id>', '<company-id>', GETDATE(), 1, 0);
> ```

`CompanyDbContext` yalnızca ona ihtiyaç duyan bir servis çözümlendiğinde oluşturulur; master
veritabanıyla çalışan uç noktalar bu maliyeti ödemez.

| Arayüz | Hangi veritabanı |
| --- | --- |
| `IUnitOfWork` / `UnitOfWork` | Master (`AppDbContext`) |
| `ICompanyUnitOfWork` / `CompanyUnitOfWork` | Aktif şirket (`CompanyDbContext`) |

Kiracıya özel uç noktalar `[RequiresCompanyHeader]` ile işaretlenir; bu sayede başlık Swagger UI'da
da zorunlu alan olarak görünür.

**Örnek**

```bash
curl -X POST http://localhost:5251/api/UniformChartOfAccounts/CreateUniformChartOfAccount \
  -H "Authorization: Bearer <token>" \
  -H "X-Company-Id: 16e1818a-6e3e-47cf-8807-b3ddb65b0260" \
  -H "Content-Type: application/json" \
  -d '{"code":"100","name":"KASA","type":"Aktif"}'
```

## API Referansı

### Auth — kimlik doğrulama

Tek anonim grup; token gerekmez.

| Metot | Yol | Açıklama |
| --- | --- | --- |
| `POST` | `/api/Auth/Register` | Kullanıcı oluşturur ve token çifti döner |
| `POST` | `/api/Auth/Login` | E-posta/parola ile giriş, token çifti döner |
| `POST` | `/api/Auth/RefreshToken` | Refresh token'ı yenisiyle değiştirir (rotasyon) |
| `POST` | `/api/Auth/Logout` | Refresh token'ı Redis'ten siler (sunucu tarafında oturumu kapatır) |

```bash
curl -X POST http://localhost:5251/api/Auth/Login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"Passw0rd!"}'
# -> { accessToken, refreshToken, accessTokenExpiresAt }
```

Access token varsayılan olarak 15 dakika geçerlidir. Refresh token ise **DB'de değil Redis'te**
tutulur (`refresh-token:{token}` anahtarı) ve süresi **ilk giriş anından itibaren** `RefreshTokenDays`
(varsayılan 7) gün olarak hesaplanır: `RefreshToken` uç noktası her çağrıldığında token rotate
edilir (eski değer Redis'ten silinir, yenisi yazılır) ama ilk giriş zamanı (`IssuedAtUtc`) korunur —
yani aktif kullanım süresi her seferinde "şimdi + 7 gün"e ötelenmez, oturum tam olarak ilk
girişten `RefreshTokenDays` gün sonra Redis'in kendi TTL mekanizmasıyla kendiliğinden düşer. Süresi
dolmuş veya zaten tüketilmiş (ikinci kez kullanılan) bir refresh token `04401` döner. `POST
/api/Auth/Logout` çağrısı aynı token'ı sunucu tarafında da anında geçersiz kılar.

### Companies — master veritabanı

Token gerekir; `X-Company-Id` başlığı **gerekmez**.

| Metot | Yol | Açıklama |
| --- | --- | --- |
| `POST` | `/api/Companies/CreateCompany` | Yeni şirket kaydı oluşturur |
| `GET` | `/api/Companies/GetCompanies` | Sayfalı şirket listesi |
| `GET` | `/api/Companies/GetCompanyById/{id}` | Tek şirket getirir |
| `PUT` | `/api/Companies/UpdateCompany/{id}` | Şirketi günceller |
| `DELETE` | `/api/Companies/DeleteCompany/{id}` | Şirketi siler (soft delete) |
| `GET` | `/api/Companies/MigrateCompanyDb` | Tüm şirket veritabanlarına migration uygular |

### Roles — master veritabanı

Token gerekir; `X-Company-Id` başlığı **gerekmez**. `AppRole` ASP.NET Identity'den geldiği için bu
feature Framework base sınıflarını kullanmaz (bkz. [Mimari](#mimari)).

| Metot | Yol | Açıklama |
| --- | --- | --- |
| `POST` | `/api/Roles/CreateRole` | Yeni rol oluşturur (`Name`, `Code`) |
| `POST` | `/api/Roles/CreateAllRoles` | Tanımlı tüm rolleri toplu olarak oluşturur (başlangıç seed'i) |
| `GET` | `/api/Roles/GetRoles` | Sayfalı rol listesi |
| `GET` | `/api/Roles/GetRoleById/{id}` | Tek rol getirir |
| `PUT` | `/api/Roles/UpdateRole/{id}` | Rolü günceller |
| `DELETE` | `/api/Roles/DeleteRole/{id}` | Rolü siler (soft delete) |
| `POST` | `/api/Roles/AssignRoleToUser` | Kullanıcıya rol atar (`UserId`, `RoleCode`) |
| `DELETE` | `/api/Roles/RemoveRoleFromUser` | Kullanıcıdan rolü kaldırır (`UserId`, `RoleName`) |
| `GET` | `/api/Roles/GetUserRoles/{userId}` | Kullanıcının sahip olduğu rolleri listeler |

### MainRoles — master veritabanı

Token gerekir; `X-Company-Id` başlığı **gerekmez**. `MainRole` (`Title`, `IsRoleCreateByAdmin`,
`CompanyId`) `BaseEntity`'den türer ve Framework base sınıflarını kullanır.

| Metot | Yol | Açıklama |
| --- | --- | --- |
| `POST` | `/api/MainRoles/CreateMainRole` | Yeni ana rol oluşturur |
| `GET` | `/api/MainRoles/GetMainRoles` | Sayfalı ana rol listesi |
| `GET` | `/api/MainRoles/GetMainRoleById/{id}` | Tek ana rol getirir |
| `PUT` | `/api/MainRoles/UpdateMainRole/{id}` | Ana rolü günceller |
| `DELETE` | `/api/MainRoles/DeleteMainRole/{id}` | Ana rolü siler (soft delete) |

### MainRoleAndRoleRelationships — master veritabanı

`MainRole` ile `AppRole` arasındaki ilişkiyi (`RoleId`, `MainRoleId`) yönetir. Token gerekir;
`X-Company-Id` başlığı gerekmez.

| Metot | Yol | Açıklama |
| --- | --- | --- |
| `POST` | `/api/MainRoleAndRoleRelationships/CreateMainRoleAndRoleRelationship` | Yeni ilişki kaydı ekler |
| `GET` | `/api/MainRoleAndRoleRelationships/GetMainRoleAndRoleRelationships` | Sayfalı liste |
| `GET` | `/api/MainRoleAndRoleRelationships/GetMainRoleAndRoleRelationshipById/{id}` | Tek kayıt getirir |
| `PUT` | `/api/MainRoleAndRoleRelationships/UpdateMainRoleAndRoleRelationship/{id}` | Kaydı günceller |
| `DELETE` | `/api/MainRoleAndRoleRelationships/DeleteMainRoleAndRoleRelationship/{id}` | Kaydı siler (soft delete) |

### MainRoleAndUserRelationships — master veritabanı

`MainRole` ile `AppUser` arasındaki ilişkiyi (`UserId`, `MainRoleId`, `CompanyId`) yönetir. Token
gerekir; `X-Company-Id` başlığı gerekmez.

| Metot | Yol | Açıklama |
| --- | --- | --- |
| `POST` | `/api/MainRoleAndUserRelationships/CreateMainRoleAndUserRelationship` | Yeni ilişki kaydı ekler |
| `GET` | `/api/MainRoleAndUserRelationships/GetMainRoleAndUserRelationships` | Sayfalı liste |
| `GET` | `/api/MainRoleAndUserRelationships/GetMainRoleAndUserRelationshipById/{id}` | Tek kayıt getirir |
| `PUT` | `/api/MainRoleAndUserRelationships/UpdateMainRoleAndUserRelationship/{id}` | Kaydı günceller |
| `DELETE` | `/api/MainRoleAndUserRelationships/DeleteMainRoleAndUserRelationship/{id}` | Kaydı siler (soft delete) |

### Seed — master veritabanı

Geliştirme ortamı için örnek veri oluşturur. Token gerekir; `X-Company-Id` başlığı gerekmez.

| Metot | Yol | Açıklama |
| --- | --- | --- |
| `POST` | `/api/Seed/SeedSampleData` | Master veritabanına örnek şirket/kullanıcı/rol verisi ekler |

Statik UCAF izin rollerini, iki örnek şirketi, şirket başına iki kullanıcıyı ve bunları
birbirine bağlayan `MainRole` / ilişki kayıtlarını oluşturur. İşlem idempotenttir: her adım önce
doğal anahtara göre var olup olmadığını kontrol eder, bu yüzden tekrar çağırmak kayıt
çoğaltmaz. Şirket veritabanları (tenant) kapsam dışıdır — bunun için erişilebilir bir
per-company SQL Server bağlantısı gerekirdi. Oluşturulan örnek kullanıcıların parolası
`Test.123`'tür.

```json
{
  "success": true,
  "data": {
    "permissionRolesCreated": 0,
    "companiesCreated": 2,
    "usersCreated": 4,
    "userCompanyLinksCreated": 4,
    "mainRolesCreated": 4,
    "mainRoleRoleLinksCreated": 0,
    "mainRoleUserLinksCreated": 4
  },
  "errorCode": null,
  "message": null,
  "errors": null
}
```

> Sayaçlar yalnızca o çağrıda **yeni oluşturulan** kayıtları sayar; kayıtlar zaten varsa (örn.
> roller `CreateAllRoles` ile önceden oluşturulmuşsa) ilgili alan `0` döner.

### UniformChartOfAccounts — şirket veritabanı

Tüm uç noktalar token'a ek olarak `X-Company-Id` başlığını **zorunlu** kılar ve kullanıcının o
şirkete üyeliğini doğrular.

| Metot | Yol | Açıklama |
| --- | --- | --- |
| `POST` | `/api/UniformChartOfAccounts/CreateUniformChartOfAccount` | Hesap planı kaydı ekler |
| `GET` | `/api/UniformChartOfAccounts/GetUniformChartOfAccounts` | Sayfalı liste |
| `GET` | `/api/UniformChartOfAccounts/GetUniformChartOfAccountById/{id}` | Tek kayıt getirir |
| `PUT` | `/api/UniformChartOfAccounts/UpdateUniformChartOfAccount/{id}` | Kaydı günceller |
| `DELETE` | `/api/UniformChartOfAccounts/DeleteUniformChartOfAccount/{id}` | Kaydı siler (soft delete) |

**Sayfalama parametreleri** (listeleme uç noktaları için):

| Parametre | Varsayılan | Kural |
| --- | --- | --- |
| `pageNumber` | `1` | `>= 1` |
| `pageSize` | `20` | `1` – `100` |
| `searchTerm` | — | Şirketlerde ve rollerde `Name`; ana rollerde `Title`; hesap planında `Code` veya `Name` içinde arar |

`GetMainRoleAndRoleRelationships` ve `GetMainRoleAndUserRelationships` sayfalıdır ama `searchTerm`
desteklemez. Silinen (soft delete) kayıtlar listelerde ve tekil sorgularda dönmez.

## gRPC Servisi

REST API'nin yanında, aynı Application/Persistence/Infrastructure katmanlarını kullanan ayrı bir
`OnlineAccountingApp.Grpc` host'u vardır. Kendi `Program.cs`'i, kendi portları ve kendi DI
kayıtlarıyla (`DependencyInjections/Grpc*DependencyInjection.cs`) çalışır — bunlar WebApi'nin
`AddPersistence`/`AddConfigureAuthentication`/`AddInfrastructure`/`AddApplication` metodlarının elle
senkronize edilen birebir kopyalarıdır; WebApi projesine referans vermez.

| Adres | Açıklama |
| --- | --- |
| `http://localhost:5158` | HTTP/2 (gRPC) |
| `https://localhost:7293` | HTTPS/2 (gRPC) |

Development ortamında gRPC reflection açıktır (`grpcurl`, Postman gibi istemcilerle keşif için).

> gRPC host'u da `Login`/`RefreshToken` için REST ile aynı `IRefreshTokenService` (Redis tabanlı)
> gerçeklemesini kullanır; Redis erişilemezse gRPC'nin `Auth` servisi de çalışmaz.

### Servisler

Her gRPC servisi, ilgili REST controller'ıyla aynı MediatR command/query'lerini dispatch eder —
iş mantığı Application katmanında tek yerde yazılır, iki protokol de aynı handler'lara gider.

| Servis | `.proto` | REST karşılığı |
| --- | --- | --- |
| `Auth` | `Protos/auth.proto` | `AuthController` (Login/Register/RefreshToken, anonim) |
| `Companies` | `Protos/companies.proto` | `CompaniesController` |
| `Roles` | `Protos/roles.proto` | `RolesController` |
| `UniformChartOfAccounts` | `Protos/uniform_chart_of_accounts.proto` | `UniformChartOfAccountsController` |

`Auth` dışındaki tüm servisler `[Authorize]`'dır; kimlik doğrulama ve çok kiracılılık
(`X-Company-Id`) aynı şekilde işler, yalnızca HTTP başlığı yerine **gRPC metadata** üzerinden taşınır.

### Hata işleme

REST tarafındaki `GlobalExceptionHandler` / `ApiResponse` yerine `BusinessExceptionInterceptor`
kullanılır: `BusinessException` / `ValidationException` yakalanıp uygun `StatusCode`'a sahip bir
`RpcException`'a çevrilir. gRPC'nin JSON gövdesi olmadığından `AppErrorCode` (ve doğrulama
hatalarında alan bazlı hatalar) **trailing metadata**'da taşınır: `error-code`, doğrulama
hatalarında ayrıca `errors-json`.

| HTTP durumu (`AppErrorCodes`) | gRPC `StatusCode` |
| --- | --- |
| 400 | `InvalidArgument` |
| 401 | `Unauthenticated` |
| 403 | `PermissionDenied` |
| 404 | `NotFound` |
| 409 | `AlreadyExists` |
| `03400` (`Tenant.CompanyNotSpecified`) | `FailedPrecondition` (özel durum) |
| Beklenmeyen hata | `Internal` |

## Yanıt Zarfı ve Hata Kodları

Tüm yanıtlar `ApiResponse` ile sarılır.

**Başarılı**

```json
{
  "success": true,
  "data": { "id": "a295007b-...", "code": "100", "name": "KASA", "type": "Aktif" },
  "errorCode": null,
  "message": null,
  "errors": null
}
```

**İş kuralı hatası**

```json
{
  "success": false,
  "data": null,
  "errorCode": "02409",
  "message": "A uniform chart of account with the same code already exists.",
  "errors": null
}
```

**Doğrulama hatası**

```json
{
  "success": false,
  "data": null,
  "errorCode": "00400",
  "message": "One or more validation errors occurred.",
  "errors": { "Code": ["'Code' boş olmamalı."] }
}
```

### Hata kodları

Kod biçimi: **`{2 haneli servis kodu}{3 haneli HTTP durum kodu}`**. Son üç hane HTTP durumunu verir.

| Kod | HTTP | Anlamı |
| --- | --- | --- |
| `00400` | 400 | Doğrulama hatası (genel) |
| `01400` | 400 | Şirket doğrulama hatası |
| `01404` | 404 | Şirket bulunamadı |
| `01409` | 409 | Aynı isimde şirket zaten var |
| `02400` | 400 | Hesap planı doğrulama hatası |
| `02404` | 404 | Hesap planı kaydı bulunamadı |
| `02409` | 409 | Aynı kodlu hesap planı kaydı zaten var |
| `03400` | 400 | `X-Company-Id` başlığı gönderilmedi |
| `03404` | 404 | Başlıktaki şirket bulunamadı |
| `04400` | 400 | Kimlik doğrulama doğrulama hatası |
| `04401` | 401 | Token yok/geçersiz, hatalı parola veya geçersiz refresh token |
| `04403` | 403 | Kullanıcının bu şirkete erişim yetkisi yok |
| `04409` | 409 | Bu e-posta ile kayıtlı kullanıcı zaten var |
| `05400` | 400 | Rol doğrulama hatası |
| `05404` | 404 | Rol bulunamadı |
| `05409` | 409 | Aynı isimde/kodda rol zaten var |
| `06400` | 400 | Ana rol (MainRole) doğrulama hatası |
| `06404` | 404 | Ana rol bulunamadı |
| `06409` | 409 | Aynı başlıkta ana rol zaten var |
| `07400` | 400 | MainRole-Role ilişkisi doğrulama hatası |
| `07404` | 404 | MainRole-Role ilişkisi bulunamadı |
| `07409` | 409 | Aynı MainRole-Role ilişkisi zaten var |
| `08400` | 400 | MainRole-User ilişkisi doğrulama hatası |
| `08404` | 404 | MainRole-User ilişkisi bulunamadı |
| `08409` | 409 | Aynı MainRole-User ilişkisi zaten var |

## Yeni Feature Ekleme

Feature'lar hedef veritabanına göre ayrılır:

- Master veritabanı → `Application/Features/AppFeatures/<Feature>/<Action>/`
- Şirket veritabanı → `Application/Features/CompanyFeatures/<Feature>/<Action>/`

**Entity `BaseEntity`'den türüyorsa** (çoğu durum budur), `OnlineAccountingApp.Framework`'teki base
sınıfları kullanın — örnek için `CompanyFeature/Create` veya
`UniformChartOfAccountFeature/Create` klasörlerine bakın:

```
Create/
├── CreateXCommand.cs           # : BaseCreateCommand<TResponse>
├── CreateXCommandHandler.cs    # : BaseCreateCommandHandler<CreateXCommand, TEntity, TResponse>
└── CreateXCommandValidator.cs  # : BaseCreateCommandValidator<CreateXCommand>
```

Update/Delete/GetById/GetList için sırasıyla `Base{Update,Delete,GetById,GetList}Command/Query`,
`...Handler`, `...Validator` ailesini kullanın (bkz.
[`OnlineAccountingApp.Framework`](#onlineaccountingappframework)).
Handler'da yalnızca ihtiyacınız olan `virtual` metotları override edin — `Handle()` akışının
kendisine dokunmanız gerekmez. Şirket veritabanıyla çalışan handler'lar constructor'da
`ICompanyUnitOfWork` alıp base sınıfa geçirmelidir (`IUnitOfWork` değil); aksi halde
`Repository<T>()` yanlışlıkla master veritabanına bağlanır.

**Entity `BaseEntity`'den türemiyorsa** (örn. `AppRole`, ASP.NET Identity'den geliyor), base
sınıflar uygun değildir — `RoleFeature`'daki gibi doğrudan `IRequest<TResponse>` /
`IRequestHandler<TCommand, TResponse>` / `AbstractValidator<TCommand>` kullanmaya devam edin.

Handler'lar MediatR tarafından otomatik bulunur (`RegisterServicesFromAssembly`), validator'lar da
`AddValidatorsFromAssembly` ile kaydedilir ve `ValidationBehavior` üzerinden çalışır. Geriye kalan
adımlar:

1. Mapster eşlemelerini `Application/Mapper/MapsterConfig.cs` içine ekleyin ve `AddApplication()`
   içinden çağrılan bir `Register...Mappings()` metoduna bağlayın.
2. Entity'ye özgü ek sorgular gerekiyorsa servis arayüzünü/gerçeklemesini
   `ApplicationDependencyInjection` içinde kaydedin (base sınıflar için bu adım gerekli değildir).
3. Şirket veritabanıyla çalışıyorsa controller'ı `[RequiresCompanyHeader]` ile işaretleyin.
4. Controller'lar `BaseApiController`'dan türediği için otomatik olarak `[Authorize]` olur;
   anonim erişim gerekiyorsa `[AllowAnonymous]` ekleyin (`AuthController` gibi).

## Yol Haritası

- [x] JWT kimlik doğrulama — register / login / refresh token, tüm uçlar `[Authorize]`
- [x] `X-Company-Id` başlığının `UserCompany` üzerinden doğrulanması (kiracılar arası erişim
      kapatıldı)
- [x] `OnlineAccountingApp.Framework` — Create/Update/Delete/GetById/GetList için jenerik,
      şablon-metot tabanlı MediatR base sınıfları (`Company`, `UniformChartOfAccount` bu altyapıyı
      kullanır)
- [x] Rol yönetimi — `RolesController` (CRUD, kullanıcıya rol atama/kaldırma) ve `CreateAllRoles`
      ile toplu başlangıç seed'i
- [x] `MainRole`, `MainRoleAndRoleRelationship`, `MainRoleAndUserRelationship` feature'ları — master
      DB'de, Framework base sınıfları üzerine kurulu tam CRUD
- [x] `OnlineAccountingApp.Application.Tests` — xUnit + Moq ile Auth, Company, Role, MainRole ve
      ilişki, UniformChartOfAccount feature'larının handler testleri
- [x] `POST /api/Seed/SeedSampleData` — master veritabanı için idempotent örnek veri doldurma
      uç noktası (bkz. [Örnek Veri Doldurma (Seed)](#örnek-veri-doldurma-seed))
- [x] Refresh token'lar Redis'e taşındı (`IRefreshTokenService` → `RedisRefreshTokenService`),
      rotasyonlarda ilk giriş anından itibaren mutlak süre sınırı korunuyor ve `POST
      /api/Auth/Logout` ile sunucu tarafında anında iptal ekleniyor
- [ ] **Kullanıcıyı şirkete atama uç noktaları** — şu an `UserCompany` kayıtları elle
      ekleniyor (yukarıdaki nota bakın)
- [ ] `Me` ucu — kullanıcının erişebildiği şirketleri listeler
- [ ] `MainRole` feature'larının gRPC'ye taşınması — şu an yalnızca REST üzerinden erişilebiliyor
- [ ] `Logout` uç noktasının gRPC'ye eklenmesi — şu an yalnızca REST üzerinden erişilebiliyor
- [ ] `Infrastructure` katmanının kalanının doldurulması (e-posta, dosya, dış servisler)
- [ ] Şirket bağlantı parolalarının şifrelenmesi

---

<a id="english"></a>

# OnlineAccountingApp

[Türkçe](#turkce) | **English**

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![EF Core](https://img.shields.io/badge/EF%20Core-10.0.10-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927)
![Architecture](https://img.shields.io/badge/architecture-Clean%20%2B%20CQRS-blue)

A multi-tenant accounting API in which **every company gets its own database**. Built on Clean
Architecture layering, CQRS via MediatR, and EF Core.

## Table of Contents

1. [Overview](#overview)
2. [Tech Stack](#tech-stack)
3. [Requirements and Setup](#requirements-and-setup)
4. [Database and Migrations](#database-and-migrations)
5. [Architecture](#architecture)
6. [Multi-Tenancy Model](#multi-tenancy-model)
7. [API Reference](#api-reference)
8. [Sample Data Seeding](#sample-data-seeding)
9. [gRPC Service](#grpc-service)
10. [Response Envelope and Error Codes](#response-envelope-and-error-codes)
11. [Adding a Feature](#adding-a-feature)
12. [Roadmap](#roadmap)

## Overview

There are two kinds of database in the system:

- **Master database** (`AccountingMasterDb`) — holds company records plus users and roles
  (ASP.NET Identity). Each `Company` row also stores the connection details of that company's
  own database.
- **Company databases** — one database per company. Accounting data (e.g. the uniform chart of
  accounts) lives here, and no company can ever see another's data.

A request states which company it belongs to through the `X-Company-Id` header, and the API
connects to that company's database. The company named in the header must be one the token's user
actually has access to, via `UserCompany`.

> **Authentication is mandatory.** Every endpoint except `/api/Auth/*` requires an
> `Authorization: Bearer <token>` header.

## Tech Stack

| Area | Used |
| --- | --- |
| Target framework | .NET 10 (`net10.0`) |
| ORM | Entity Framework Core 10.0.10 (SQL Server) |
| CQRS / mediator | MediatR 14.2.0 |
| Object mapping | Mapster 10.0.11 |
| Validation | FluentValidation 12.1.1 |
| Authentication | ASP.NET Core Identity + JWT Bearer 10.0.10 |
| Refresh token store | Redis (StackExchange.Redis 3.1.13) |
| API documentation | Swashbuckle 10.2.3 / Microsoft.OpenApi 2.9.0 |

## Requirements and Setup

**Requirements**

- .NET 10 SDK
- SQL Server (LocalDB, Express, or Docker)
- Redis (required for the refresh token store — Docker recommended, see [Auth](#api-reference))
- EF Core CLI: `dotnet tool install --global dotnet-ef`

**Steps**

```bash
# 1. Clone the repository
git clone https://github.com/aliyilmaz020/OnlineAccountingApp.git
cd OnlineAccountingApp/api

# 2. Configure the connection string (see the note below)

# 3. Create the master database
dotnet ef database update \
  --project OnlineAccountingApp.Persistence \
  --startup-project OnlineAccountingApp.WebApi \
  --context AppDbContext

# 4. Run the API
dotnet run --project OnlineAccountingApp.WebApi
```

| Address | Description |
| --- | --- |
| `http://localhost:5251` | HTTP |
| `https://localhost:7025` | HTTPS |
| `http://localhost:5251/swagger` | Swagger UI (Development environment only) |

**Connection string.** `ConnectionStrings:SqlServer` in
`OnlineAccountingApp.WebApi/appsettings.json` points at the master database:

```json
"ConnectionStrings": {
  "SqlServer": "Server=localhost;Database=AccountingMasterDb;User Id=sa;Password=Password1;TrustServerCertificate=True;"
}
```

> **Security note:** the repository contains an `sa` password for local development. Do not keep
> credentials in the file for a real environment — use
> `dotnet user-secrets set "ConnectionStrings:SqlServer" "..."` instead. The same applies to the
> `ServerPassword` field stored on company records, which is currently kept in plain text.

**JWT settings.** The `Jwt` section in the same file configures token issuing:

```json
"Jwt": {
  "Issuer": "OnlineAccountingApp",
  "Audience": "OnlineAccountingApp.Client",
  "SecretKey": "dev-only-secret-change-me-with-user-secrets-at-least-32-bytes",
  "AccessTokenMinutes": 15,
  "RefreshTokenDays": 7
}
```

> `SecretKey` does not belong in the repository either:
> `dotnet user-secrets set "Jwt:SecretKey" "..."`. Use a key of at least 32 bytes for HMAC-SHA256.

**Redis settings.** Refresh tokens live in Redis; the connection info comes from
`Redis:ConnectionString`:

```json
"Redis": {
  "ConnectionString": "localhost:6379,password=secret,abortConnect=false"
}
```

To spin one up quickly with Docker locally (matches the default connection string above exactly):

```bash
docker run -d --name c_redis -p 6379:6379 redis:7 redis-server --requirepass secret
```

## Database and Migrations

The project has two separate `DbContext` types and two separate migration folders.

**Master database (`AppDbContext` → `Migrations/AppDb`)**

```bash
# Add a migration
dotnet ef migrations add <Name> \
  --project OnlineAccountingApp.Persistence \
  --startup-project OnlineAccountingApp.WebApi \
  --context AppDbContext -o Migrations/AppDb

# Apply
dotnet ef database update \
  --project OnlineAccountingApp.Persistence \
  --startup-project OnlineAccountingApp.WebApi \
  --context AppDbContext
```

**Company databases (`CompanyDbContext` → `Migrations/CompanyDb`)**

```bash
# Add a migration
dotnet ef migrations add <Name> \
  --project OnlineAccountingApp.Persistence \
  --startup-project OnlineAccountingApp.WebApi \
  --context CompanyDbContext -o Migrations/CompanyDb
```

Company migrations are **not** applied with `dotnet ef database update`, because every company has
a different connection string. They are applied at runtime through this endpoint instead:

```
GET /api/Companies/MigrateCompanyDb
```

It iterates every company in the master database, opens a `CompanyDbContext` against each one's own
connection string, and applies pending migrations. Call it after creating a new company, and again
after adding a schema change that must reach all existing tenants.

> **Note:** `CompanyDbContext.CompanyDbContextFactory` hardcodes the master connection string at
> design time (when generating migrations) and uses the first company record as a template. Adjust
> that string if your local setup differs.

> **Refresh tokens no longer live in the master database.** The `RefreshTokens` table was removed
> (see [Auth](#api-reference)); if you're updating an existing setup, add a migration to drop it:
>
> ```bash
> dotnet ef migrations add RemoveRefreshTokens \
>   --project OnlineAccountingApp.Persistence \
>   --startup-project OnlineAccountingApp.WebApi \
>   --context AppDbContext
> dotnet ef database update \
>   --project OnlineAccountingApp.Persistence \
>   --startup-project OnlineAccountingApp.WebApi \
>   --context AppDbContext
> ```
>
> This migration drops any existing rows (and therefore any active sessions) along with the table.

## Architecture

Clean Architecture; dependencies always point inward.

```
api/
├── OnlineAccountingApp.Domain/          # Dependency-free core
│   ├── Abstracts/BaseEntity.cs          # Id, CreateDate, EditDate, Status, Deleted
│   ├── AppEntities/                     # Master DB: Company, UserCompany, AppUser, AppRole
│   ├── CompanyEntities/                 # Company DB: UniformChartOfAccount
│   └── Exceptions/                      # BusinessException, ValidationException, AppErrorCodes
│
├── OnlineAccountingApp.Framework/       # General-purpose MediatR plumbing (depends on Domain only)
│   ├── Services/                        # IRepository, IUnitOfWork, PagedResult
│   └── MedatR/Create|Update|Delete|GetById|GetList/
│                                         # BaseXCommand/Query, BaseXCommandHandler, BaseXCommandValidator
│
├── OnlineAccountingApp.Application/     # Use cases (depends on Domain + Framework)
│   ├── Features/AppFeatures/            # Master DB features (CQRS)
│   ├── Features/CompanyFeatures/        # Company DB features (CQRS)
│   ├── Services/AppServices/            # ICompanyService, IRoleService, IRefreshTokenService, ...
│   ├── Services/CompanyServices/        # ICompanyContext, ICompanyUnitOfWork, IUniformChartOfAccountService
│   ├── Behaviors/ValidationBehavior.cs  # MediatR pipeline validation
│   └── Mapper/MapsterConfig.cs
│
├── OnlineAccountingApp.Persistence/     # EF Core implementation
│   ├── Context/AppDbContext.cs          # Master DB (IdentityDbContext)
│   ├── Context/CompanyDbContext.cs      # Company DB (constructed at runtime)
│   ├── Services/Repository.cs           # Repository<TEntity, TContext> — serves both databases
│   ├── Services/UnitOfWork.cs           # Master DB
│   ├── Services/AppServices/            # CompanyService
│   ├── Services/CompanyServices/        # UniformChartOfAccountService, CompanyUnitOfWork
│   ├── Configurations/ + Constants/     # Company DB table configuration
│   └── Migrations/AppDb + Migrations/CompanyDb
│
├── OnlineAccountingApp.Infrastructure/  # Outside-world implementations
│   ├── Options/JwtOptions.cs, RedisOptions.cs   # the "Jwt" and "Redis" configuration sections
│   └── Services/JwtTokenService.cs, RedisCacheService.cs, RedisRefreshTokenService.cs
│                                         # ITokenService, ICacheService, IRefreshTokenService (Redis-backed)
│
├── OnlineAccountingApp.Grpc/            # Second presentation layer: the gRPC host
│   ├── Protos/                          # auth, companies, roles, uniform_chart_of_accounts
│   ├── Services/                        # *GrpcService — dispatch the same MediatR commands/queries
│   ├── Interceptors/                    # BusinessExceptionInterceptor
│   └── DependencyInjections/            # hand-synced copy of WebApi's DI registration
│
└── OnlineAccountingApp.WebApi/          # REST presentation layer
    ├── Controllers/                     # Thin MediatR dispatchers
    ├── Tenancy/                         # HttpCompanyContext, RequiresCompanyHeaderAttribute
    ├── DependencyInjections/            # AddApplication(), AddPersistence()
    ├── Configurations/                  # Swagger, error handling
    ├── ExceptionHandling/               # GlobalExceptionHandler
    ├── Filters/ApiResultFilter.cs       # Wraps responses in ApiResponse
    └── Models/ApiResponse.cs
```

Controllers hold no business logic; they only dispatch the command or query to MediatR:

```csharp
[HttpPost("[action]")]
public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyCommand command, CancellationToken cancellationToken)
{
    var result = await mediator.Send(command, cancellationToken);
    return Ok(result);
}
```

### `OnlineAccountingApp.Framework`

An independent layer sitting just above `Domain` and below `Application`. It provides two things:

- `Services/IRepository<T>`, `IUnitOfWork`, `PagedResult<T>` — generic data-access abstractions.
- `MedatR/Create|Update|Delete|GetById|GetList/` — template-method base classes for each action:
  a base command/query, handler, and validator per action (`BaseCreateCommand<TResponse>`,
  `BaseCreateCommandHandler<TCommand, TEntity, TResponse>`, `BaseCreateCommandValidator<TCommand>`,
  and the equivalent trio for the other four actions). Each handler's `Handle()` flow is fixed;
  customization happens through `virtual` methods:

  | Method | Purpose |
  | --- | --- |
  | `GetExistsPredicate` (Create) / `GetConflictPredicate` (Update) | Uniqueness check |
  | `BuildPredicate`, `GetIncludes` (GetById / GetList) | Query filter, eager-loading |
  | `Before/AfterCreateAsync`, `Before/AfterUpdateAsync`, `Before/AfterDeleteAsync` | Extra logic before/after persistence |
  | `GetNotFoundErrorCode/Message`, `GetAlreadyExistsErrorCode/Message`, `GetConflictErrorCode/Message` | Entity-specific `AppErrorCodes` and message |

  The `Company` and `UniformChartOfAccount` features are built on this plumbing (see
  `CreateCompanyCommandHandler`, `CreateUniformChartOfAccountCommandHandler`).

> **Note:** `AppRole` derives from ASP.NET Identity's `IdentityRole<string>`, not `BaseEntity`, and
> `IRoleService` does not implement `IRepository<T>`. `RoleFeature` therefore doesn't fit these base
> classes and keeps using `IRequest<T>` / `IRequestHandler<,>` / `AbstractValidator<T>` directly.

## Multi-Tenancy Model

Every `Company` row in the master database carries the connection details of its own database:
`ServerName`, `DatabaseName`, `ServerUserId`, `ServerPassword`.

`AppDbContext` is registered in DI with a fixed connection string. `CompanyDbContext`, by contrast,
is constructed **per request**: which company to connect to is read from the `X-Company-Id` header.

```
Request
  │  Authorization: Bearer <token>
  │  X-Company-Id: 16e1818a-...
  ▼
HttpCompanyContext ──► CompanyId (from header)  +  UserId (from token claim)
  │
  ▼
AddCompanyTenancy() (PersistenceDependencyInjection)
  │  Looks the Company up in the master DB
  │  Header missing        → 03400
  │  Company not found     → 03404
  │  No UserCompany row    → 04403   ← access check
  ▼
CompanyDbContext (built with that company's connection string)
  │
  ▼
UniformChartOfAccountService  +  ICompanyUnitOfWork
```

Note that the header alone is not proof of access: the user must be linked to that company through
`UserCompany`, otherwise any authenticated user could read another tenant's data.

> ⚠ **Linking a user to a company.** There is no endpoint yet that creates `UserCompany` rows. A
> newly registered user belongs to no company and therefore gets `04403` from every tenant
> endpoint. Until that endpoint exists, insert the row by hand:
>
> ```sql
> INSERT INTO UserCompanies (Id, AppUserId, CompanyId, CreateDate, Status, Deleted)
> VALUES (NEWID(), '<user-id>', '<company-id>', GETDATE(), 1, 0);
> ```

`CompanyDbContext` is only created when a service that needs it is resolved, so endpoints working
against the master database never pay that cost.

| Interface | Which database |
| --- | --- |
| `IUnitOfWork` / `UnitOfWork` | Master (`AppDbContext`) |
| `ICompanyUnitOfWork` / `CompanyUnitOfWork` | Current company (`CompanyDbContext`) |

Tenant-scoped endpoints are marked `[RequiresCompanyHeader]`, which also makes the header show up
as required in Swagger UI.

**Example**

```bash
curl -X POST http://localhost:5251/api/UniformChartOfAccounts/CreateUniformChartOfAccount \
  -H "Authorization: Bearer <token>" \
  -H "X-Company-Id: 16e1818a-6e3e-47cf-8807-b3ddb65b0260" \
  -H "Content-Type: application/json" \
  -d '{"code":"100","name":"KASA","type":"Aktif"}'
```

## API Reference

### Auth — authentication

The only anonymous group; no token required.

| Method | Path | Description |
| --- | --- | --- |
| `POST` | `/api/Auth/Register` | Creates a user and returns a token pair |
| `POST` | `/api/Auth/Login` | Signs in with email/password, returns a token pair |
| `POST` | `/api/Auth/RefreshToken` | Exchanges a refresh token for a new pair (rotation) |
| `POST` | `/api/Auth/Logout` | Deletes the refresh token from Redis (server-side sign-out) |

```bash
curl -X POST http://localhost:5251/api/Auth/Login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"Passw0rd!"}'
# -> { accessToken, refreshToken, accessTokenExpiresAt }
```

The access token is valid for 15 minutes by default. The refresh token lives **in Redis, not the
database** (key `refresh-token:{token}`), and its lifetime is computed as `RefreshTokenDays`
(7 by default) from the **moment of the original login** — every call to `RefreshToken` rotates the
token (the old Redis key is deleted, a new one written) but the original issue time (`IssuedAtUtc`)
is carried over unchanged, so active use never pushes the expiry forward. The session drops on its
own, via Redis's own TTL, exactly `RefreshTokenDays` days after the first login. An expired or
already-used refresh token returns `04401`. Calling `POST /api/Auth/Logout` revokes the same token
on the server immediately.

### Companies — master database

Requires a token; the `X-Company-Id` header is **not required**.

| Method | Path | Description |
| --- | --- | --- |
| `POST` | `/api/Companies/CreateCompany` | Creates a new company record |
| `GET` | `/api/Companies/GetCompanies` | Paged company list |
| `GET` | `/api/Companies/GetCompanyById/{id}` | Returns a single company |
| `PUT` | `/api/Companies/UpdateCompany/{id}` | Updates a company |
| `DELETE` | `/api/Companies/DeleteCompany/{id}` | Deletes a company (soft delete) |
| `GET` | `/api/Companies/MigrateCompanyDb` | Applies migrations to every company database |

### Roles — master database

Requires a token; the `X-Company-Id` header is **not required**. `AppRole` comes from ASP.NET
Identity, so this feature does not use the Framework base classes (see [Architecture](#architecture)).

| Method | Path | Description |
| --- | --- | --- |
| `POST` | `/api/Roles/CreateRole` | Creates a new role (`Name`, `Code`) |
| `POST` | `/api/Roles/CreateAllRoles` | Bulk-creates every defined role (initial seed) |
| `GET` | `/api/Roles/GetRoles` | Paged role list |
| `GET` | `/api/Roles/GetRoleById/{id}` | Returns a single role |
| `PUT` | `/api/Roles/UpdateRole/{id}` | Updates a role |
| `DELETE` | `/api/Roles/DeleteRole/{id}` | Deletes a role (soft delete) |
| `POST` | `/api/Roles/AssignRoleToUser` | Assigns a role to a user (`UserId`, `RoleCode`) |
| `DELETE` | `/api/Roles/RemoveRoleFromUser` | Removes a role from a user (`UserId`, `RoleName`) |
| `GET` | `/api/Roles/GetUserRoles/{userId}` | Lists the roles a user has |

### MainRoles — master database

Requires a token; the `X-Company-Id` header is **not required**. `MainRole` (`Title`,
`IsRoleCreateByAdmin`, `CompanyId`) derives from `BaseEntity` and uses the Framework base classes.

| Method | Path | Description |
| --- | --- | --- |
| `POST` | `/api/MainRoles/CreateMainRole` | Creates a new main role |
| `GET` | `/api/MainRoles/GetMainRoles` | Paged main role list |
| `GET` | `/api/MainRoles/GetMainRoleById/{id}` | Returns a single main role |
| `PUT` | `/api/MainRoles/UpdateMainRole/{id}` | Updates a main role |
| `DELETE` | `/api/MainRoles/DeleteMainRole/{id}` | Deletes a main role (soft delete) |

### MainRoleAndRoleRelationships — master database

Manages the relationship between a `MainRole` and an `AppRole` (`RoleId`, `MainRoleId`). Requires a
token; the `X-Company-Id` header is not required.

| Method | Path | Description |
| --- | --- | --- |
| `POST` | `/api/MainRoleAndRoleRelationships/CreateMainRoleAndRoleRelationship` | Adds a new relationship |
| `GET` | `/api/MainRoleAndRoleRelationships/GetMainRoleAndRoleRelationships` | Paged list |
| `GET` | `/api/MainRoleAndRoleRelationships/GetMainRoleAndRoleRelationshipById/{id}` | Returns a single entry |
| `PUT` | `/api/MainRoleAndRoleRelationships/UpdateMainRoleAndRoleRelationship/{id}` | Updates an entry |
| `DELETE` | `/api/MainRoleAndRoleRelationships/DeleteMainRoleAndRoleRelationship/{id}` | Deletes an entry (soft delete) |

### MainRoleAndUserRelationships — master database

Manages the relationship between a `MainRole` and an `AppUser` (`UserId`, `MainRoleId`,
`CompanyId`). Requires a token; the `X-Company-Id` header is not required.

| Method | Path | Description |
| --- | --- | --- |
| `POST` | `/api/MainRoleAndUserRelationships/CreateMainRoleAndUserRelationship` | Adds a new relationship |
| `GET` | `/api/MainRoleAndUserRelationships/GetMainRoleAndUserRelationships` | Paged list |
| `GET` | `/api/MainRoleAndUserRelationships/GetMainRoleAndUserRelationshipById/{id}` | Returns a single entry |
| `PUT` | `/api/MainRoleAndUserRelationships/UpdateMainRoleAndUserRelationship/{id}` | Updates an entry |
| `DELETE` | `/api/MainRoleAndUserRelationships/DeleteMainRoleAndUserRelationship/{id}` | Deletes an entry (soft delete) |

### Seed — master database

Fills the master database with development sample data. Requires a token; the `X-Company-Id`
header is not required.

| Method | Path | Description |
| --- | --- | --- |
| `POST` | `/api/Seed/SeedSampleData` | Seeds the master database with sample companies/users/roles |

Creates the static UCAF permission roles, two sample companies, two users per company, and the
`MainRole` / relationship rows that link them together. The operation is idempotent: every step
checks by natural key first, so calling it again never duplicates rows. Company (tenant)
databases are out of scope, since that would require a reachable per-company SQL Server
connection. The password for every seeded sample user is `Test.123`.

```json
{
  "success": true,
  "data": {
    "permissionRolesCreated": 0,
    "companiesCreated": 2,
    "usersCreated": 4,
    "userCompanyLinksCreated": 4,
    "mainRolesCreated": 4,
    "mainRoleRoleLinksCreated": 0,
    "mainRoleUserLinksCreated": 4
  },
  "errorCode": null,
  "message": null,
  "errors": null
}
```

> The counters only reflect rows **newly created** by that call; if the rows already exist (e.g.
> the roles were already created via `CreateAllRoles`), the matching field returns `0`.

### UniformChartOfAccounts — company database

Every endpoint **requires** the `X-Company-Id` header in addition to the token, and verifies the
user's membership of that company.

| Method | Path | Description |
| --- | --- | --- |
| `POST` | `/api/UniformChartOfAccounts/CreateUniformChartOfAccount` | Adds a chart-of-accounts entry |
| `GET` | `/api/UniformChartOfAccounts/GetUniformChartOfAccounts` | Paged list |
| `GET` | `/api/UniformChartOfAccounts/GetUniformChartOfAccountById/{id}` | Returns a single entry |
| `PUT` | `/api/UniformChartOfAccounts/UpdateUniformChartOfAccount/{id}` | Updates an entry |
| `DELETE` | `/api/UniformChartOfAccounts/DeleteUniformChartOfAccount/{id}` | Deletes an entry (soft delete) |

**Paging parameters** (for list endpoints):

| Parameter | Default | Rule |
| --- | --- | --- |
| `pageNumber` | `1` | `>= 1` |
| `pageSize` | `20` | `1` – `100` |
| `searchTerm` | — | Matches `Name` for companies and roles; `Title` for main roles; `Code` or `Name` for chart of accounts |

`GetMainRoleAndRoleRelationships` and `GetMainRoleAndUserRelationships` are paged but don't support
`searchTerm`. Soft-deleted records are excluded from both list and single-record queries.

## gRPC Service

Alongside the REST API there's a separate `OnlineAccountingApp.Grpc` host that uses the same
Application/Persistence/Infrastructure layers. It has its own `Program.cs`, its own ports, and its
own DI registrations (`DependencyInjections/Grpc*DependencyInjection.cs`) — hand-synced copies of
WebApi's `AddPersistence`/`AddConfigureAuthentication`/`AddInfrastructure`/`AddApplication`; it does
not reference the WebApi project.

| Address | Description |
| --- | --- |
| `http://localhost:5158` | HTTP/2 (gRPC) |
| `https://localhost:7293` | HTTPS/2 (gRPC) |

gRPC reflection is enabled in Development (for discovery with tools like `grpcurl` or Postman).

> The gRPC host uses the same Redis-backed `IRefreshTokenService` as REST for `Login`/
> `RefreshToken`; if Redis is unreachable, gRPC's `Auth` service won't work either.

### Services

Each gRPC service dispatches the same MediatR commands/queries as its REST controller counterpart
— business logic is written once in the Application layer and both protocols hit the same handlers.

| Service | `.proto` | REST equivalent |
| --- | --- | --- |
| `Auth` | `Protos/auth.proto` | `AuthController` (Login/Register/RefreshToken, anonymous) |
| `Companies` | `Protos/companies.proto` | `CompaniesController` |
| `Roles` | `Protos/roles.proto` | `RolesController` |
| `UniformChartOfAccounts` | `Protos/uniform_chart_of_accounts.proto` | `UniformChartOfAccountsController` |

Every service except `Auth` is `[Authorize]`. Authentication and multi-tenancy (`X-Company-Id`)
work the same way, just carried through **gRPC metadata** instead of an HTTP header.

### Error handling

Instead of REST's `GlobalExceptionHandler` / `ApiResponse`, a `BusinessExceptionInterceptor` catches
`BusinessException` / `ValidationException` and turns them into an `RpcException` with a matching
`StatusCode`. Since gRPC has no JSON response body, `AppErrorCode` (and, for validation failures,
the per-field errors) travel as **trailing metadata**: `error-code`, plus `errors-json` for
validation errors.

| HTTP status (`AppErrorCodes`) | gRPC `StatusCode` |
| --- | --- |
| 400 | `InvalidArgument` |
| 401 | `Unauthenticated` |
| 403 | `PermissionDenied` |
| 404 | `NotFound` |
| 409 | `AlreadyExists` |
| `03400` (`Tenant.CompanyNotSpecified`) | `FailedPrecondition` (special case) |
| Unexpected error | `Internal` |

## Response Envelope and Error Codes

Every response is wrapped in `ApiResponse`.

**Success**

```json
{
  "success": true,
  "data": { "id": "a295007b-...", "code": "100", "name": "KASA", "type": "Aktif" },
  "errorCode": null,
  "message": null,
  "errors": null
}
```

**Business rule error**

```json
{
  "success": false,
  "data": null,
  "errorCode": "02409",
  "message": "A uniform chart of account with the same code already exists.",
  "errors": null
}
```

**Validation error**

```json
{
  "success": false,
  "data": null,
  "errorCode": "00400",
  "message": "One or more validation errors occurred.",
  "errors": { "Code": ["'Code' must not be empty."] }
}
```

### Error codes

Code format: **`{2-digit service code}{3-digit HTTP status code}`**. The last three digits are the
HTTP status.

| Code | HTTP | Meaning |
| --- | --- | --- |
| `00400` | 400 | Validation error (general) |
| `01400` | 400 | Company validation error |
| `01404` | 404 | Company not found |
| `01409` | 409 | A company with the same name already exists |
| `02400` | 400 | Chart-of-accounts validation error |
| `02404` | 404 | Chart-of-accounts entry not found |
| `02409` | 409 | An entry with the same code already exists |
| `03400` | 400 | `X-Company-Id` header was not supplied |
| `03404` | 404 | The company in the header was not found |
| `04400` | 400 | Authentication validation error |
| `04401` | 401 | Missing/invalid token, wrong password, or invalid refresh token |
| `04403` | 403 | The user has no access to this company |
| `04409` | 409 | A user with this email already exists |
| `05400` | 400 | Role validation error |
| `05404` | 404 | Role not found |
| `05409` | 409 | A role with the same name/code already exists |
| `06400` | 400 | Main role validation error |
| `06404` | 404 | Main role not found |
| `06409` | 409 | A main role with the same title already exists |
| `07400` | 400 | MainRole-Role relationship validation error |
| `07404` | 404 | MainRole-Role relationship not found |
| `07409` | 409 | The same MainRole-Role relationship already exists |
| `08400` | 400 | MainRole-User relationship validation error |
| `08404` | 404 | MainRole-User relationship not found |
| `08409` | 409 | The same MainRole-User relationship already exists |

## Adding a Feature

Features are split by which database they target:

- Master database → `Application/Features/AppFeatures/<Feature>/<Action>/`
- Company database → `Application/Features/CompanyFeatures/<Feature>/<Action>/`

**If the entity derives from `BaseEntity`** (the common case), use the base classes in
`OnlineAccountingApp.Framework` — see `CompanyFeature/Create` or
`UniformChartOfAccountFeature/Create` for a worked example:

```
Create/
├── CreateXCommand.cs           # : BaseCreateCommand<TResponse>
├── CreateXCommandHandler.cs    # : BaseCreateCommandHandler<CreateXCommand, TEntity, TResponse>
└── CreateXCommandValidator.cs  # : BaseCreateCommandValidator<CreateXCommand>
```

Use the matching `Base{Update,Delete,GetById,GetList}Command/Query`, `...Handler`, `...Validator`
family for the other actions (see [`OnlineAccountingApp.Framework`](#onlineaccountingappframework)).
Only override the `virtual` methods you actually need — the `Handle()` flow itself never changes.
Handlers that target a company database must take `ICompanyUnitOfWork` in the constructor (not
`IUnitOfWork`) and pass it to the base class; otherwise `Repository<T>()` would wrongly resolve
against the master database.

**If the entity does not derive from `BaseEntity`** (e.g. `AppRole`, which comes from ASP.NET
Identity), the base classes don't apply — keep using `IRequest<TResponse>` /
`IRequestHandler<TCommand, TResponse>` / `AbstractValidator<TCommand>` directly, as `RoleFeature`
does.

Handlers are discovered automatically by MediatR (`RegisterServicesFromAssembly`), and validators
are registered via `AddValidatorsFromAssembly` and run through `ValidationBehavior`. The remaining
steps:

1. Add Mapster mappings to `Application/Mapper/MapsterConfig.cs` and hook them into a
   `Register...Mappings()` method called from `AddApplication()`.
2. If the entity needs extra queries beyond the generic repository, register a service
   interface/implementation in `ApplicationDependencyInjection` (not needed when the base classes
   are enough on their own).
3. If it targets a company database, mark the controller `[RequiresCompanyHeader]`.
4. Controllers inherit `[Authorize]` from `BaseApiController`; add `[AllowAnonymous]` if anonymous
   access is needed (as `AuthController` does).

## Roadmap

- [x] JWT authentication — register / login / refresh token, every endpoint `[Authorize]`
- [x] Validate the `X-Company-Id` header against `UserCompany` (cross-tenant access closed)
- [x] `OnlineAccountingApp.Framework` — generic, template-method MediatR base classes for
      Create/Update/Delete/GetById/GetList (`Company` and `UniformChartOfAccount` are built on it)
- [x] Role management — `RolesController` (CRUD, assign/remove role for a user) plus
      `CreateAllRoles` for bulk initial seeding
- [x] `MainRole`, `MainRoleAndRoleRelationship`, `MainRoleAndUserRelationship` features — full CRUD
      in the master DB, built on the Framework base classes
- [x] `OnlineAccountingApp.Application.Tests` — xUnit + Moq handler tests for the Auth, Company,
      Role, MainRole and relationship, and UniformChartOfAccount features
- [x] `POST /api/Seed/SeedSampleData` — idempotent sample-data seeding endpoint for the master
      database (see [Sample Data Seeding](#sample-data-seeding))
- [x] Refresh tokens moved to Redis (`IRefreshTokenService` → `RedisRefreshTokenService`); rotation
      preserves the absolute expiry from the original login, and `POST /api/Auth/Logout` now
      revokes a token server-side immediately
- [ ] **Assign-user-to-company endpoints** — `UserCompany` rows are currently inserted by hand
      (see the note above)
- [ ] A `Me` endpoint listing the companies a user can access
- [ ] Move the `MainRole` features to gRPC — currently REST-only
- [ ] Add a `Logout` RPC to gRPC — currently REST-only
- [ ] Fill in the rest of the `Infrastructure` layer (email, files, external services)
- [ ] Encrypt company connection passwords
