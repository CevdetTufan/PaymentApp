PaymentApp Proje ve CI Yapısı Detaylı Açıklaması
Bu belge, PaymentApp projesinin genel yapısını (katmanlı mimarisi) ve GitHub Actions CI (.github/workflows/ci.yml) iş akışının detaylı açıklamasını içermektedir. Amacımız, projenin nasıl organize edildiğini, her bir bileşenin sorumluluğunu ve otomatik derleme/test sürecinin nasıl işlediğini kapsamlı bir şekilde ortaya koymaktır.

1. PaymentApp Proje Yapısı: Temiz Mimari Yaklaşımı
PaymentApp projesi, Robert C. Martin'in (Uncle Bob) "Temiz Mimari" (Clean Architecture) prensiplerine sıkı sıkıya bağlı kalarak tasarlanmıştır. Bu mimari, yazılım sistemlerini bağımsız, test edilebilir ve esnek katmanlara ayırarak karmaşıklığı azaltmayı ve uzun vadeli sürdürülebilirliği artırmayı hedefler.

Projenin ana dizin ve dosya yapısı aşağıdaki gibidir:

PaymentApp/
├── src/
│   ├── PaymentApp.Api/                   # 1. API Katmanı (Presentation Layer)
│   ├── PaymentApp.Application/           # 2. Uygulama Katmanı (Application Layer)
│   ├── PaymentApp.Domain/                # 3. Etki Alanı Katmanı (Domain Layer)
│   ├── PaymentApp.Infrastructure/        # 4. Altyapı Katmanı (Infrastructure Layer)
│   └── PaymentApp.SharedKernel/          # 5. Paylaşılan Çekirdek (Shared Kernel)
├── test/
│   └── PaymentApp.Test/                  # 6. Test Projesi
├── docker-compose.yml                    # Docker Konteyner Tanımlamaları
├── .github/workflows/ci.yml              # GitHub Actions CI İş Akışı
├── README.md                             # Proje Tanıtım Dosyası
└── .gitignore                            # Git İgnore Dosyası

Katmanların Detaylı Açıklaması ve Akış
Temiz mimarinin temel ilkesi, bağımlılık kuralıdır: iç katmanlar dış katmanlardan hiçbir şey bilmez. Dış katmanlar, iç katmanlara bağımlıdır. Bu, iş kurallarının (Domain katmanı) dış değişikliklerden (UI, veritabanı, harici servisler) etkilenmemesini sağlar.

1. src/PaymentApp.Api/ (API Katmanı / Presentation Layer)
Sorumluluk: Uygulamanın dış dünyaya açılan kapısıdır. RESTful API endpoint'lerini barındırır ve HTTP isteklerini karşılar. Kullanıcı arayüzleri, mobil uygulamalar veya diğer mikroservisler bu API üzerinden uygulama ile etkileşime girer. Bu katman, gelen isteği alır, doğrular ve PaymentApp.Application katmanına iletir. Yanıtı da uygun formatta (genellikle JSON) geri döndürür. Bu katman, iş mantığı içermez, sadece gelen/giden veriyi ve HTTP protokolünü yönetir.

İçerik Detayı:

Controllers/: ASP.NET Core MVC kontrolcüleri veya API kontrolcüleri burada bulunur. Her bir kontrolcü, belirli bir kaynak (örneğin, Ödemeler) için HTTP metotlarını (GET, POST, PUT, DELETE) tanımlar. Kontrolcüler, PaymentApp.Application katmanındaki özel işleyiciler (Handler) aracılığıyla komutları veya sorguları gönderir ve sonuçları HTTP yanıtı olarak döndürür.

Program.cs: ASP.NET Core uygulamasının başlangıç noktasıdır. Uygulama servis konteynerinin (Dependency Injection) yapılandırılması, HTTP istek hattına (middleware pipeline) middleware'lerin (kimlik doğrulama, yetkilendirme, hata yönetimi, loglama, CORS) eklenmesi ve uygulamanın HTTP isteklerini dinlemeye başlaması burada yapılır. PaymentApp.Application ve PaymentApp.Infrastructure katmanlarındaki bağımlılıklar burada kaydedilir.

Dockerfile: Bu API projesi için bir Docker imajı oluşturma talimatlarını içerir. Uygulamanın izole edilmiş bir konteyner içinde çalışmasını ve farklı ortamlarda (geliştirme, test, üretim) tutarlı bir şekilde dağıtılmasını sağlar. Bu sayede "benim makinemde çalışıyor" sorunları ortadan kalkar.

appsettings.json, appsettings.Development.json, appsettings.Production.json: Uygulama yapılandırma ayarları (veritabanı bağlantı dizinleri, API anahtarları, loglama seviyeleri, harici servis URL'leri vb.) bu dosyalarda bulunur. Ortama özel ayarlar için farklı dosyalar kullanılır ve ASP.NET Core bunları otomatik olarak yükler.

2. src/PaymentApp.Application/ (Uygulama Katmanı / Application Layer)
Sorumluluk: Uygulamanın iş mantığını ve kullanım durumlarını (use cases) tanımlar. Etki alanı katmanındaki varlıkları ve iş kurallarını koordine eder. Dış dünya (API katmanı) ile etki alanı arasındaki arayüzü sağlar. Genellikle CQRS (Command Query Responsibility Segregation) deseni bu katmanda uygulanır. Bu katman, iş akışlarını yönetir ve Domain katmanındaki iş kurallarını tetikler.

İçerik Detayı:

DTOs/: Veri Transfer Nesneleri (Data Transfer Objects). Katmanlar arasında (API'den uygulama katmanına, uygulama katmanından API'ye) veri taşımak için kullanılır. Ham etki alanı varlıklarını doğrudan dışarıya açmak yerine, belirli kullanım durumları için optimize edilmiş, daha güvenli ve esnek veri yapıları sağlar. Örneğin, bir CreatePaymentRequest DTO'su veya bir PaymentResponse DTO'su.

Commands/: Veri değiştiren veya bir eylem başlatan istekleri temsil eden sınıflar (örn: CreatePaymentCommand, ProcessRefundCommand). Her komut, bir iş işlemini tanımlar ve genellikle bir CommandHandler tarafından işlenir. Komutlar genellikle void veya basit bir sonuç (örn: ID) döndürür.

Queries/: Veri okuma isteklerini temsil eden sınıflar (örn: GetPaymentByIdQuery, ListPaymentsQuery). Her sorgu, veri alma işlemini tanımlar ve genellikle bir QueryHandler tarafından işlenir. Sorgular genellikle veri döndürür ve sistemin durumunu değiştirmez.

Handlers/: Commands ve Queries tarafından tetiklenen iş mantığını içeren sınıflar. Bu işleyiciler, komutların ve sorguların ilgili işleyicilerine yönlendirilmesini sağlar, böylece işleyiciler arasında doğrudan bağımlılık oluşmaz ve kod daha modüler hale gelir. İşleyiciler, Domain katmanındaki repository'leri veya diğer etki alanı servislerini kullanır.

Interfaces/: Uygulama katmanının dış bağımlılıkları için arayüzler (örn: IPaymentService, IEventBus). Bu arayüzlerin somut implementasyonları Infrastructure katmanında bulunur.

İş akışları, validasyonlar (örn: FluentValidation ile) ve yetkilendirme kontrolleri de genellikle bu katmanda uygulanır. Bu katman, iş kurallarını doğrudan içermez ancak iş kurallarını uygulayan Domain katmanını koordine eder.

3. src/PaymentApp.Domain/ (Etki Alanı Katmanı / Domain Layer)
Sorumluluk: Projenin çekirdek iş mantığını, varlıklarını (entities), değer nesnelerini (value objects), etki alanı olaylarını (domain events) ve iş kurallarını barındırır. Bu katman, diğer katmanlara bağımlılığı olmayan, en bağımsız ve en önemli katmandır. İşin kalbi buradadır ve iş gereksinimlerinin doğrudan bir yansımasıdır.

İçerik Detayı:

Varlık sınıfları (örn: Payment, Transaction, Customer): Uygulamanın temel iş nesnelerini temsil eder. Genellikle bir kimliğe (ID) sahiptirler ve yaşam döngüleri boyunca durumları değişebilir. Varlıklar, iş kurallarını ve davranışlarını kapsüller.

Değer nesneleri (örn: Money, Address, PaymentStatus): Kimliği olmayan, değerine göre eşitliği belirlenen nesnelerdir. Genellikle değişmez (immutable) olurlar ve bir varlığın niteliklerini zenginleştirirler (örn: Money nesnesi, para birimi ve miktarını birlikte tutar).

Etki alanı olayları: Bir iş sürecinde meydana gelen önemli olayları temsil eder (örn: PaymentCreatedEvent, RefundProcessedEvent). Bu olaylar, sistemin diğer bölümlerinin bu değişikliklere tepki vermesini sağlar (örn: bir ödeme oluşturulduğunda bildirim gönderme).

Arayüzler: Repository gibi dış bağımlılıkların soyutlamaları burada tanımlanır (örn: IPaymentRepository, ICustomerRepository). Bu arayüzler, Domain katmanının belirli bir veritabanı teknolojisine veya harici servise bağımlı olmamasını sağlar. Somut implementasyonlar Infrastructure katmanında bulunur.

Etki alanı servisleri: Birden fazla varlığı veya etki alanı nesnesini içeren iş kurallarını barındırır.

4. src/PaymentApp.Infrastructure/ (Altyapı Katmanı / Infrastructure Layer)
Sorumluluk: Etki alanı ve uygulama katmanlarının ihtiyaç duyduğu dış bağımlılıkları (veritabanı erişimi, dosya sistemi, harici servis entegrasyonları, loglama, e-posta gönderme, mesaj kuyrukları vb.) yönetir. PaymentApp.Domain katmanında tanımlanan arayüzlerin somut implementasyonlarını içerir. Bu katman, uygulamanın teknik detaylarını ve dış sistemlerle entegrasyonunu ele alır.

İçerik Detayı:

DataModule.cs, ServiceModule.cs: Bağımlılık enjeksiyonu (Dependency Injection) için modül tanımlamaları. Bu dosyalar, Program.cs'de çağrılarak Infrastructure katmanındaki servislerin ve repository'lerin DI konteynerine kaydedilmesini sağlar. Bu, bağımlılıkların esnek bir şekilde yönetilmesine olanak tanır.

Repositories/: PaymentApp.Domain katmanında tanımlanan repository arayüzlerinin somut implementasyonları (örn: MongoDB ile veri erişimini sağlayan PaymentRepository). Bu sınıflar, veritabanı sorgularını, veri manipülasyonunu ve veri modelinin etki alanı varlıklarına dönüştürülmesini yönetir.

EventPublisher/, EventConsumer/: Olay odaklı mimari için olayların yayınlanması ve tüketilmesini sağlayan mekanizmalar. Bu, mikroservisler arası iletişimi veya asenkron işlemleri destekleyebilir (örn: Kafka, RabbitMQ veya basit bir in-memory dispatcher). Bu bileşenler, etki alanı olaylarını dinler ve ilgili işlemleri tetikler.

MongoDb/: MongoDB veritabanı ile bağlantı, yapılandırma ve özel yardımcı sınıfları içerir. MongoDB client'ının kurulumu, koleksiyonlara erişim, indeksleme gibi veritabanı spesifik detaylar burada yönetilir.

Services/: Harici servis entegrasyonları (örn: ödeme gateway'leri, e-posta servisleri) için somut implementasyonlar.

5. src/PaymentApp.SharedKernel/ (Paylaşılan Çekirdek / Shared Kernel)
Sorumluluk: Projenin tüm katmanları arasında paylaşılan ortak yapıları, temel sınıfları, yardımcıları ve genel tipleri içerir. Bu katman, diğer katmanlar arasında tekrarlayan kodun önüne geçmek ve DRY (Don't Repeat Yourself) prensibini uygulamak için kullanılır. Bağımlılıkları en az olan katmandır ve diğer katmanlar tarafından referans alınabilir.

İçerik Detayı:

Temel varlık sınıfları (BaseEntity, AuditableEntity gibi): Tüm varlıkların ortak özelliklerini (ID, Oluşturulma Tarihi vb.) tanımlar.

Ortak yardımcı metotlar veya uzantı metotları (örn: string manipülasyonu, tarih işlemleri, validasyon yardımcıları).

Genel sabitler veya numaralandırmalar (enums) (örn: PaymentStatus, TransactionType).

Özel istisna sınıfları (custom exceptions): Uygulamaya özgü hata durumlarını temsil eder.

Ortak değer nesneleri veya temel tipler.

6. test/PaymentApp.Test/ (Test Projesi)
Sorumluluk: Uygulamanın birim testlerini ve entegrasyon testlerini içerir. Kodun doğru çalıştığını ve beklendiği gibi davrandığını doğrulamak için kullanılır. Testler, kod kalitesini artırır ve gelecekteki değişikliklerin mevcut işlevselliği bozmadığını garanti eder.

İçerik Detayı:

Controllers/: API kontrolcülerinin testleri. Genellikle entegrasyon testleri olarak yazılır ve API endpoint'lerine gerçek HTTP istekleri göndererek yanıtları ve durum kodlarını doğrular. Test sunucusu (TestServer) kullanılarak gerçek HTTP pipeline'ı simüle edilebilir.

Services/: Uygulama katmanı servislerinin veya komut/sorgu işleyicilerinin birim testleri. Bu testlerde, bağımlılıklar mock'lanarak (sahte nesnelerle değiştirilerek) sadece test edilen birimin kendi mantığı doğrulanır. Bu, testlerin hızlı ve izole olmasını sağlar.

Repositories/: Repository implementasyonlarının entegrasyon testleri. Gerçek veritabanı bağlantısı kullanılarak (örn: test konteynerinde çalışan bir MongoDB) repository'lerin veritabanıyla doğru etkileşim kurduğu doğrulanır.

PaymentApp.Test.csproj: Test projesinin yapılandırma dosyası. Test framework'leri (örn: xUnit, NUnit, MSTest) ve mock kütüphaneleri (örn: Moq, NSubstitute) burada referans alınır.

Diğer Önemli Dosyalar
docker-compose.yml: Docker Compose, birden fazla Docker konteynerini (örn: PaymentApp.Api ve MongoDB) tek bir YAML dosyasıyla tanımlamanızı ve yönetmenizi sağlar. Geliştirme ortamını hızlıca ayağa kaldırmak ve bağımlılıkları yönetmek için idealdir. Servislerin birbirleriyle nasıl iletişim kuracağını, port eşlemelerini ve volume'leri tanımlar.

.github/workflows/ci.yml: GitHub Actions için Sürekli Entegrasyon (CI) iş akışını tanımlayan dosyadır. Kod güncellemelerinde otomatik derleme ve test süreçlerini tetikler.

README.md: Projenin genel tanıtımını, amacını, ana özelliklerini ve hızlı başlangıç bilgilerini içeren ana belgedir. Genellikle kullanıcıların projeyi anlaması ve kullanmaya başlaması için ilk başvuru noktasıdır.

.gitignore: Git versiyon kontrol sistemi tarafından izlenmeyecek dosyaları ve dizinleri belirtir (örn: derleme çıktıları (bin, obj), paketler (.vs), geçici dosyalar, hassas yapılandırma dosyaları). Bu, gereksiz dosyaların depoya eklenmesini engeller.

Bu katmanlı yapı, PaymentApp projesinin karmaşıklığını yönetmeye, farklı sorumlulukları ayırmaya ve her bir katmanın kendi içinde bağımsız olarak geliştirilmesine olanak tanır.

2. CI İş Akışı Dosyası (.github/workflows/ci.yml) Yapısı: Otomatik Kalite Güvencesi
Bu dosya, GitHub Actions kullanarak PaymentApp projeniz için bir Sürekli Entegrasyon (CI) iş akışını tanımlar. CI, yazılım geliştirme sürecinde kod değişikliklerinin otomatik olarak derlenmesi, test edilmesi ve entegre edilmesi pratiğidir. Bu, hataların erken aşamada tespit edilmesini ve kod kalitesinin korunmasını sağlar.

Genel Yapı
name: CI

on:
  push:
    branches: [ master ]
  pull_request:
    branches: [ master ]

jobs:
  build-test:
    runs-on: ubuntu-latest
    steps:
      # ... adımlar buraya gelir ...



name: CI: Bu, GitHub Actions arayüzünde iş akışınızın görünen adıdır. Anlaşılır bir isim, iş akışlarını kolayca ayırt etmenizi sağlar.

on:: Bu bölüm, iş akışının hangi GitHub olayları (events) tetiklendiğinde çalışacağını tanımlar.

push:: Kod deposuna bir push işlemi yapıldığında iş akışını tetikler.

branches: [ master ]: Bu iş akışının sadece master dalına yapılan push işlemlerinde çalışmasını sağlar. Bu, ana dalın her zaman kararlı kalmasını hedefler ve her commit'in test edilmesini garanti eder.

pull_request:: Bir pull request (PR) açıldığında veya mevcut bir PR güncellendiğinde iş akışını tetikler.

branches: [ master ]: Bu iş akışının sadece master dalını hedefleyen pull request'lerde çalışmasını sağlar. Bu, yeni kodun ana dala birleştirilmeden önce derleme ve test kontrollerinden geçmesini sağlayarak kod kalitesini artırır.

jobs:: İş akışının gerçekleştireceği ana görevleri (jobs) tanımlar. Bir iş akışı birden fazla iş içerebilir ve bunlar paralel veya sıralı çalışabilir. Bu iş akışında tek bir iş (build-test) bulunmaktadır.

build-test Görevi
Bu görev, PaymentApp projesinin derlenmesi ve test edilmesi sorumluluğunu üstlenir.

jobs:
  build-test:
    runs-on: ubuntu-latest
    steps:
      # ... adımlar ...



runs-on: ubuntu-latest: Bu görevin çalışacağı sanal sunucu ortamını belirtir. GitHub Actions, farklı işletim sistemleri (Ubuntu, Windows, macOS) ve donanım yapılandırmalarına sahip "runner"lar (koşucular) sağlar. ubuntu-latest, en son Ubuntu Linux sürümünü kullanan bir GitHub Actions runner'ıdır. Bu, çoğu .NET Core projesi için standart ve uygun bir seçimdir ve tutarlı bir derleme ortamı sağlar.

Adımlar (steps)
build-test görevi içindeki her bir step, belirli bir eylemi gerçekleştiren sıralı bir komut veya eylemdir. Bir adım başarısız olursa, genellikle tüm görev başarısız olur ve iş akışı durur, böylece sorunlu kodun ana dala birleşmesi engellenir.

name: Checkout code

- name: Checkout code
  uses: actions/checkout@v3



Açıklama: Bu adım, projenizin kaynak kodunu GitHub deposundan sanal sunucuya (runner) kopyalar. Bu, sonraki adımların kod dosyalarınıza erişebilmesi ve üzerinde işlem yapabilmesi için ilk ve temel adımdır. Kodun doğru versiyonunun çekildiğinden emin olunur.

uses: actions/checkout@v3: GitHub tarafından sağlanan hazır bir eylemi (checkout) kullanır. Bu eylem, depoyu klonlama işlemini otomatikleştirir ve genellikle hızlı ve güvenilirdir.

name: Setup .NET

- name: Setup .NET
  uses: actions/setup-dotnet@v3
  with:
    dotnet-version: '9.0.x'



Açıklama: Bu adım, .NET SDK'sını sanal sunucuya kurar. .NET projelerini derlemek, geri yüklemek ve test etmek için dotnet komutlarına (CLI araçları) ihtiyaç duyulur.

uses: actions/setup-dotnet@v3: .NET SDK'sını kurmak için özel olarak tasarlanmış hazır bir eylemi kullanır. Bu eylem, belirtilen .NET sürümünü indirir ve ortam değişkenlerini ayarlar, böylece dotnet komutları sistem PATH'ine eklenir.

with: dotnet-version: '9.0.x': Kurulacak .NET SDK sürümünü belirtir. 9.0.x ifadesi, 9.0 serisindeki en son kararlı sürümün (örn: 9.0.301) kurulmasını sağlar, bu da uyumluluk sorunlarını minimize eder ve her zaman güncel bir ortamda çalışmayı garanti eder.

name: Restore dependencies

- name: Restore dependencies
  run: dotnet restore PaymentApp.sln



Açıklama: Bu adım, PaymentApp.sln çözüm dosyasında belirtilen tüm .NET projelerinin NuGet paket bağımlılıklarını geri yükler. Bu, kodu derlemeden veya test etmeden önce tüm gerekli kütüphanelerin (örneğin, özel işleyiciler, MongoDB sürücüsü, test framework'leri vb.) internetten indirilip yerel önbelleğe alınmasını sağlar.

run:: Belirtilen kabuk komutunu (bu durumda dotnet restore) çalıştırır. Bu komut, csproj dosyalarındaki <PackageReference> öğelerini okur ve ilgili paketleri indirir. Eğer paketler önbellekte varsa, indirme işlemi atlanır ve bu da CI süresini hızlandırır.

name: Build solution

- name: Build solution
  run: dotnet build PaymentApp.sln --configuration Release --no-restore



Açıklama: Bu adım, PaymentApp.sln çözümünü derler. Bu, tüm projelerin (API, Application, Domain, Infrastructure, SharedKernel) başarılı bir şekilde derlenebildiğini ve herhangi bir sözdizimi veya derleme hatası olmadığını doğrular. Derleme hataları, kodun çalıştırılamayacağı anlamına gelir ve bu adımda yakalanmaları önemlidir.

--configuration Release: Derlemenin "Release" yapılandırmasında yapılacağını belirtir. "Release" yapılandırması genellikle performans optimizasyonları içerir ve hata ayıklama sembollerini içermez, bu da üretim ortamı için daha uygun ve daha küçük derlenmiş çıktılar sağlar.

--no-restore: Bağımlılıkların tekrar geri yüklenmemesini sağlar. Bir önceki adımda zaten geri yükleme yapıldığı için bu, derleme süresini kısaltır ve gereksiz ağ trafiğini önler.

name: Run unit tests

- name: Run unit tests
  run: dotnet test test/PaymentApp.Test/PaymentApp.Test.csproj \
    --configuration Release \
    --no-build \
    --verbosity normal



Açıklama: Bu adım, projenizin birim testlerini çalıştırır. Testlerin başarılı olması, kodunuzun beklenen şekilde çalıştığını ve iş mantığının doğru olduğunu doğrular. Bu, yeni eklenen özelliklerin mevcut işlevselliği bozmadığını (regresyon testleri) kontrol etmek için kritik öneme sahiptir.

dotnet test test/PaymentApp.Test/PaymentApp.Test.csproj: test/PaymentApp.Test/PaymentApp.Test.csproj yolundaki test projesini hedef alarak testleri çalıştırır. Bu, sadece belirtilen test projesindeki testlerin çalıştırılmasını sağlar ve diğer projelerin testlerini atlar (eğer varsa).

--configuration Release: Testlerin "Release" yapılandırmasında derlenip çalıştırılmasını sağlar. Bu, testlerin üretim ortamına yakın bir yapılandırmada çalışmasını garanti eder.

--no-build: Testleri çalıştırmadan önce test projesinin tekrar derlenmesini engeller, çünkü ana çözüm zaten derlenmişti. Bu da CI süresini optimize eder ve gereksiz derleme adımlarını önler.

--verbosity normal: Test çıktısının normal ayrıntı düzeyinde olmasını sağlar. Başarısız testler hakkında yeterli bilgi verirken, başarılı testler için çok fazla çıktı üretmez. Bu, CI günlüklerini okunabilir tutar.