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
