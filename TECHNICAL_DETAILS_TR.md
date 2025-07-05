# Teknik Doküman - PaymentApp

Bu doküman, mevcut **PaymentApp** deposunun yapısını, katmanlarını, önemli sınıf ve arayüzlerini, veri erişim ve test stratejilerini ayrıntılı olarak açıklar. Kod örnekleri ve test tanımları da yer almaktadır.

---

## 1. Proje Katmanları ve Dizin Yapısı

```plain
PaymentApp/
├── src/
│   ├── PaymentApp.Api/            # API Katmanı (Presentation)
│   ├── PaymentApp.Application/    # Uygulama Katmanı (Use Cases)
│   ├── PaymentApp.Domain/         # Domain Katmanı (Entities, Value Objects, Interfaces)
│   ├── PaymentApp.Infrastructure/ # Infrastructure Katmanı (EF Core, Repositories)
│   └── PaymentApp.SharedKernel/   # Shared Kernel (ortak sınıf ve yardımcılar)
├── test/
│   └── PaymentApp.Test/           # Test Projeleri (xUnit & Moq)
├── docker/                        # Docker Compose ve Dockerfile
├── .github/workflows/ci.yml       # CI İş Akışı
└── README.md
```

---

## 2. Domain Katmanı

### 2.1 Entity: `Payment`

```csharp
public class Payment
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    public Payment(Guid customerId, Money amount)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkCompleted() { /* ... */ }
    public void MarkFailed()    { /* ... */ }
    public void MarkRefunded()  { /* ... */ }
}
```

- **İş akışı:** `Status` sahası yalnızca `MarkCompleted()`, `MarkFailed()` ve `MarkRefunded()` metotlarıyla değişir.
- **Kapsülleme:** `private set` ile dış müdahaleyi engeller.

### 2.2 Value Object: `Money`

```csharp
public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0) throw new ArgumentException(...);
        Currency = currency ?? throw new ArgumentNullException(...);
        Amount = amount;
    }

    public override bool Equals(object? obj) => Equals(obj as Money);
    public bool Equals(Money? other) { /* ... */ }
    public override int GetHashCode()  => HashCode.Combine(Amount, Currency);
    public static bool operator ==(Money? a, Money? b) => ...;
    public static bool operator !=(Money? a, Money? b) => ...;
}
```

- **Değer tabanlı eşitlik:** `Equals` ve `GetHashCode` override edilmiştir.

### 2.3 Repository Arayüzleri

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken token = default);
    Task AddAsync(T entity, CancellationToken token = default);
    Task UpdateAsync(T entity, CancellationToken token = default);
    Task DeleteAsync(T entity, CancellationToken token = default);
}

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IReadOnlyList<Payment>> ListByCustomerAsync(Guid customerId, CancellationToken token = default);
}
```

- Arayüzler **Domain** katmanında tanımlıdır.

---

## 3. Infrastructure Katmanı

### 3.1 EF Core DbContext

```csharp
public class PaymentDbContext : DbContext
{
    public DbSet<Payment> Payments { get; set; }

    public PaymentDbContext(DbContextOptions<PaymentDbContext> opts) : base(opts) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Payment>().OwnsOne(p => p.Amount);
    }
}
```

### 3.2 Generic ve Özel Repository

```csharp
public class Repository<T> : IRepository<T> where T: class
{ /* CRUD implementasyonu */ }

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{ /* ListByCustomerAsync implementasyonu */ }
```

### 3.3 DI Kaydı (Autofac)

```csharp
public class DataModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<PaymentDbContext>().AsSelf();
        builder.RegisterGeneric(typeof(Repository<>))
               .As(typeof(IRepository<>));
        builder.RegisterType<PaymentRepository>().As<IPaymentRepository>();
    }
}
```

---

## 4. Application Katmanı

### 4.1 DTO’lar

```csharp
public record CreatePaymentDto(Guid CustomerId, decimal Amount, string Currency);
public record PaymentDto(Guid Id, Guid CustomerId, decimal Amount, string Currency, string Status, DateTime CreatedAt, DateTime? ProcessedAt);
```

### 4.2 Command & Query Pattern

- **Command**: `CreatePaymentCommand : ICommand<PaymentDto>`
- **Handler**: `CreatePaymentCommandHandler.HandleAsync(...)`
- **Query**: `GetPaymentByIdQuery : IQuery<PaymentDto>`
- **Handler**: `GetPaymentByIdQueryHandler.HandleAsync(...)`

---

## 5. Test Katmanı

### 5.1 Domain Testleri

- **MoneyTests**: negatif değer, null currency, eşitlik operatörleri.
- **PaymentEntityTests**: ctor, `MarkCompleted/Failed/Refunded` metotları.

### 5.2 Repository Entegrasyon Testleri

```csharp
public class PaymentRepositoryTests : IDisposable
{ /* In-Memory DB ile Add, GetById, ListByCustomer, Update testleri */ }
```

### 5.3 Handler Birim Testleri

- **CreatePaymentCommandHandlerTests**: doğru DTO üretimi, repo çağrısı.
- **GetPaymentByIdQueryHandlerTests**: bulunma ve bulunmama senaryoları.

---

## 6. CI / CD

- **.github/workflows/ci.yml**: `dotnet build`, `dotnet vstest` adımları.

---

Bu doküman, projedeki ana bileşenleri ve kod akışını detaylıca anlatır. Projeye yeni katılan geliştiriciler veya dışarıdan bakacaklar için rehber niteliğindedir.


Bu GitHub Actions iş akışı, bir .NET projesi için sürekli entegrasyon (CI) sürecini otomatikleştirmek üzere tasarlanmıştır. Temel olarak, kodunuzda bir değişiklik olduğunda (belirli dallara push veya pull request yapıldığında) otomatik olarak çalışır, bağımlılıkları geri yükler, projeyi derler ve birim testlerini çalıştırır.

**İş Akışının Bölümleri:**

- `name: CI`: Bu, iş akışının adıdır. GitHub Actions arayüzünde bu isimle görünür.
- `on:`: Bu bölüm, iş akışının ne zaman tetikleneceğini tanımlar.
  - `push:`: master dalına yapılan her push işleminde çalışır.
  - `pull_request:`: master dalına yapılan her pull request işleminde çalışır.
- `jobs:`: İş akışının gerçekleştireceği görevleri tanımlar. Burada tek bir görev var: `build-test`.
- `build-test`: Bu, projenizi derleyip test edecek olan ana görevdir.
- `runs-on: ubuntu-latest`: Bu görev için kullanılacak işletim sistemini belirtir. ubuntu-latest en son Ubuntu sürümünü kullanır.
- `steps:`: Görevin sırayla gerçekleştireceği adımlardır.

**Adımlar (Steps):**

1. **Checkout code:**
   ```yaml
   uses: actions/checkout@v3
   ```
   Bu adım, GitHub deposundaki kodunuzu sanal sunucuya (runner) kopyalar. Böylece diğer adımlar kod dosyalarınıza erişebilir.

2. **Setup .NET:**
   ```yaml
   uses: actions/setup-dotnet@v3
   with:
     dotnet-version: '9.0.x'
   ```
   Bu adım, .NET SDK'sını (burada '9.0.x' sürümü) sanal sunucuya kurar. Bu, dotnet komutlarını kullanabilmeniz için gereklidir.

3. **Restore dependencies:**
   ```bash
   run: dotnet restore PaymentApp.sln
   ```
   Bu komut, PaymentApp.sln çözüm dosyasında belirtilen tüm projelerin NuGet paket bağımlılıklarını geri yükler. Bu, kodu derlemeden veya test etmeden önce tüm gerekli kütüphanelerin mevcut olmasını sağlar.

4. **Build solution:**
   ```bash
   run: dotnet build PaymentApp.sln --configuration Release --no-restore
   ```
   Bu komut, PaymentApp.sln çözümünü "Release" yapılandırmasında derler.

   - `--configuration Release`: Derlemenin "Release" modunda yapılacağını belirtir (genellikle optimizasyonlar içerir).
   - `--no-restore`: Bağımlılıkların tekrar geri yüklenmemesini sağlar, çünkü bir önceki adımda zaten geri yüklenmişlerdi.

5. **Debug and Run unit tests (Hata Ayıklama ve Birim Testlerini Çalıştırma):**

   Bu adım, bir önceki yanıtta aldığınız "MSBUILD : error MSB1008: Only one project can be specified." hatasını gidermek ve daha fazla bilgi toplamak için değiştirilmiş kısımdır.

   ```bash
   run: |
     echo "--- Current Directory Contents (Recursive) ---" && ls -R

     echo "--- Environment Variables ---" && env | sort

     echo "--- Attempting to run unit tests with diagnostic verbosity ---"

     dotnet test test/PaymentApp.Test/PaymentApp.Test.csproj \
       --configuration Release \
       --no-build \
       --verbosity normal \
       /p:VSTestVerbosity=detailed

     # /p:MSBuildLogVerbosity=Diagnostic
     # Bu satır şu an yorum satırı halindedir. Eğer detailed ayrıntı düzeyi yeterli olmazsa, bu satırı etkinleştirerek MSBuild'in kendisinden çok daha ayrıntılı (tanısal) günlükler alabilirsiniz. Bu, hatanın kök nedenini bulmak için çok güçlü bir araçtır, ancak çok fazla çıktı üretebilir.

     echo "--- If the above failed, trying with solution file ---"

     dotnet test PaymentApp.sln \
       --filter "FullyQualifiedName~PaymentApp.Test" \
       --configuration Release \
       --no-build \
       --verbosity normal
   ```

   - Bu, birim testlerini çalıştırmak için kullanılan ana komuttur.
   - `test/PaymentApp.Test/PaymentApp.Test.csproj`: Test projesinin yolunu belirtir.
   - `--configuration Release`: Testlerin "Release" yapılandırmasında çalıştırılacağını belirtir.
   - `--no-build`: Testleri çalıştırmadan önce projenin tekrar derlenmemesini sağlar (çünkü zaten derlenmişti).
   - `--verbosity normal`: Test çıktısının normal ayrıntı düzeyinde olmasını sağlar.
   - `/p:VSTestVerbosity=detailed`: VSTest test çalıştırıcısının daha ayrıntılı günlükler üretmesini sağlar. Bu, hatanın nedenini anlamak için ek bilgi sağlayabilir.

   ```bash
   # Alternatif komut
   dotnet test PaymentApp.sln \
     --filter "FullyQualifiedName~PaymentApp.Test" \
     --configuration Release \
     --no-build \
     --verbosity normal
   ```
   Bu, testleri çalıştırmanın alternatif bir yoludur.
