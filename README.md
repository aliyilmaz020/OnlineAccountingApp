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
8. [Yanıt Zarfı ve Hata Kodları](#yanıt-zarfı-ve-hata-kodları)
9. [Yeni Feature Ekleme](#yeni-feature-ekleme)
10. [Yol Haritası](#yol-haritası)

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
| API dokümantasyonu | Swashbuckle 10.2.3 / Microsoft.OpenApi 2.9.0 |

## Gereksinimler ve Kurulum

**Gereksinimler**

- .NET 10 SDK
- SQL Server (LocalDB, Express veya Docker)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

**Adımlar**

```bash
# 1. Depoyu klonlayın
git clone https://github.com/aliyilmaz020/OnlineAccountingApp.git
cd OnlineAccountingApp

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

## Mimari

Clean Architecture; bağımlılıklar daima içe doğru akar.

```
OnlineAccountingApp/
├── OnlineAccountingApp.Domain/          # Bağımlılıksız çekirdek
│   ├── Abstracts/BaseEntity.cs          # Id, CreateDate, EditDate, Status, Deleted
│   ├── AppEntities/                     # Master DB: Company, UserCompany, AppUser, AppRole
│   ├── CompanyEntities/                 # Şirket DB: UniformChartOfAccount
│   └── Exceptions/                      # BusinessException, ValidationException, AppErrorCodes
│
├── OnlineAccountingApp.Application/     # Kullanım senaryoları (yalnızca Domain'e bağlı)
│   ├── Features/AppFeatures/            # Master DB feature'ları (CQRS)
│   ├── Features/CompanyFeatures/        # Şirket DB feature'ları (CQRS)
│   ├── Services/                        # IRepository, IUnitOfWork, PagedResult
│   ├── Services/AppServices/            # ICompanyService
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
│   ├── Options/JwtOptions.cs            # "Jwt" yapılandırma bölümü
│   └── Services/JwtTokenService.cs      # ITokenService — token üretimi
│
└── OnlineAccountingApp.WebApi/          # Sunum katmanı
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

```bash
curl -X POST http://localhost:5251/api/Auth/Login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"Passw0rd!"}'
# -> { accessToken, refreshToken, accessTokenExpiresAt }
```

Access token varsayılan olarak 15 dakika, refresh token 7 gün geçerlidir. Refresh işleminde eski
token iptal edilir; aynı refresh token'ı ikinci kez kullanmak `04401` döner.

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

**Sayfalama parametreleri** (her iki listeleme uç noktası için):

| Parametre | Varsayılan | Kural |
| --- | --- | --- |
| `pageNumber` | `1` | `>= 1` |
| `pageSize` | `20` | `1` – `100` |
| `searchTerm` | — | Şirketlerde `Name`; hesap planında `Code` veya `Name` içinde arar |

Silinen (soft delete) kayıtlar listelerde ve tekil sorgularda dönmez.

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

## Yeni Feature Ekleme

Feature'lar hedef veritabanına göre ayrılır:

- Master veritabanı → `Application/Features/AppFeatures/<Feature>/<Action>/`
- Şirket veritabanı → `Application/Features/CompanyFeatures/<Feature>/<Action>/`

Her aksiyon klasöründe üç dosya bulunur:

```
Create/
├── CreateXCommand.cs           # IRequest<TResponse>
├── CreateXCommandHandler.cs    # IRequestHandler<CreateXCommand, TResponse>
└── CreateXCommandValidator.cs  # AbstractValidator<CreateXCommand>
```

Handler'lar MediatR tarafından otomatik bulunur (`RegisterServicesFromAssembly`), validator'lar da
`AddValidatorsFromAssembly` ile kaydedilir ve `ValidationBehavior` üzerinden çalışır. Geriye kalan
adımlar:

1. Mapster eşlemelerini `Application/Mapper/MapsterConfig.cs` içine ekleyin ve `AddApplication()`
   içinden çağrılan bir `Register...Mappings()` metoduna bağlayın.
2. Servis arayüzünü/gerçeklemesini `ApplicationDependencyInjection` içinde kaydedin.
3. Şirket veritabanıyla çalışıyorsa: `ICompanyUnitOfWork` enjekte edin (`IUnitOfWork` değil) ve
   controller'ı `[RequiresCompanyHeader]` ile işaretleyin.
4. Controller'lar `BaseApiController`'dan türediği için otomatik olarak `[Authorize]` olur;
   anonim erişim gerekiyorsa `[AllowAnonymous]` ekleyin (`AuthController` gibi).

Ortak veri erişimi için hazır altyapıyı kullanın: `Repository<TEntity, TContext>` sınıfı
oluşturma, güncelleme, soft delete, sayfalama (`GetPagedAsync`), varlık kontrolü ve sayım
metotlarını zaten sağlar.

## Yol Haritası

- [x] JWT kimlik doğrulama — register / login / refresh token, tüm uçlar `[Authorize]`
- [x] `X-Company-Id` başlığının `UserCompany` üzerinden doğrulanması (kiracılar arası erişim
      kapatıldı)
- [ ] **Kullanıcıyı şirkete atama uç noktaları** — şu an `UserCompany` kayıtları elle
      ekleniyor (yukarıdaki nota bakın)
- [ ] Rol yönetimi ve başlangıç seed'i (Admin/User)
- [ ] `Me` ucu — kullanıcının erişebildiği şirketleri listeler
- [ ] `Infrastructure` katmanının kalanının doldurulması (e-posta, dosya, dış servisler)
- [ ] Şirket bağlantı parolalarının şifrelenmesi
- [ ] Test projesi

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
8. [Response Envelope and Error Codes](#response-envelope-and-error-codes)
9. [Adding a Feature](#adding-a-feature)
10. [Roadmap](#roadmap)

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
| API documentation | Swashbuckle 10.2.3 / Microsoft.OpenApi 2.9.0 |

## Requirements and Setup

**Requirements**

- .NET 10 SDK
- SQL Server (LocalDB, Express, or Docker)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

**Steps**

```bash
# 1. Clone the repository
git clone https://github.com/aliyilmaz020/OnlineAccountingApp.git
cd OnlineAccountingApp

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

## Architecture

Clean Architecture; dependencies always point inward.

```
OnlineAccountingApp/
├── OnlineAccountingApp.Domain/          # Dependency-free core
│   ├── Abstracts/BaseEntity.cs          # Id, CreateDate, EditDate, Status, Deleted
│   ├── AppEntities/                     # Master DB: Company, UserCompany, AppUser, AppRole
│   ├── CompanyEntities/                 # Company DB: UniformChartOfAccount
│   └── Exceptions/                      # BusinessException, ValidationException, AppErrorCodes
│
├── OnlineAccountingApp.Application/     # Use cases (depends on Domain only)
│   ├── Features/AppFeatures/            # Master DB features (CQRS)
│   ├── Features/CompanyFeatures/        # Company DB features (CQRS)
│   ├── Services/                        # IRepository, IUnitOfWork, PagedResult
│   ├── Services/AppServices/            # ICompanyService
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
│   ├── Options/JwtOptions.cs            # the "Jwt" configuration section
│   └── Services/JwtTokenService.cs      # ITokenService — token issuing
│
└── OnlineAccountingApp.WebApi/          # Presentation layer
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

```bash
curl -X POST http://localhost:5251/api/Auth/Login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"Passw0rd!"}'
# -> { accessToken, refreshToken, accessTokenExpiresAt }
```

The access token is valid for 15 minutes by default, the refresh token for 7 days. Refreshing
revokes the presented token; replaying the same refresh token returns `04401`.

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

**Paging parameters** (for both list endpoints):

| Parameter | Default | Rule |
| --- | --- | --- |
| `pageNumber` | `1` | `>= 1` |
| `pageSize` | `20` | `1` – `100` |
| `searchTerm` | — | Matches `Name` for companies; `Code` or `Name` for chart of accounts |

Soft-deleted records are excluded from both list and single-record queries.

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

## Adding a Feature

Features are split by which database they target:

- Master database → `Application/Features/AppFeatures/<Feature>/<Action>/`
- Company database → `Application/Features/CompanyFeatures/<Feature>/<Action>/`

Each action folder holds three files:

```
Create/
├── CreateXCommand.cs           # IRequest<TResponse>
├── CreateXCommandHandler.cs    # IRequestHandler<CreateXCommand, TResponse>
└── CreateXCommandValidator.cs  # AbstractValidator<CreateXCommand>
```

Handlers are discovered automatically by MediatR (`RegisterServicesFromAssembly`), and validators
are registered via `AddValidatorsFromAssembly` and run through `ValidationBehavior`. The remaining
steps:

1. Add Mapster mappings to `Application/Mapper/MapsterConfig.cs` and hook them into a
   `Register...Mappings()` method called from `AddApplication()`.
2. Register the service interface and implementation in `ApplicationDependencyInjection`.
3. If it targets a company database: inject `ICompanyUnitOfWork` (not `IUnitOfWork`) and mark the
   controller `[RequiresCompanyHeader]`.
4. Controllers inherit `[Authorize]` from `BaseApiController`; add `[AllowAnonymous]` if anonymous
   access is needed (as `AuthController` does).

Use the existing data-access plumbing: `Repository<TEntity, TContext>` already provides create,
update, soft delete, paging (`GetPagedAsync`), existence checks, and counting.

## Roadmap

- [x] JWT authentication — register / login / refresh token, every endpoint `[Authorize]`
- [x] Validate the `X-Company-Id` header against `UserCompany` (cross-tenant access closed)
- [ ] **Assign-user-to-company endpoints** — `UserCompany` rows are currently inserted by hand
      (see the note above)
- [ ] Role management and initial seeding (Admin/User)
- [ ] A `Me` endpoint listing the companies a user can access
- [ ] Fill in the rest of the `Infrastructure` layer (email, files, external services)
- [ ] Encrypt company connection passwords
- [ ] Test project
