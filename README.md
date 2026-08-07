# Mini Order App

Teknik değerlendirme çalışması kapsamında geliştirilen full-stack mini sipariş yönetim uygulamasıdır.

## Backend

Backend, ASP.NET Core Web API kullanılarak geliştirilmiştir. Projede sorumlulukların ayrılması ve kodun sürdürülebilir olması amacıyla sade bir katmanlı mimari tercih edilmiştir.

### Kullanılan Teknolojiler

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- FluentValidation
- IMemoryCache
- Swagger / OpenAPI
- xUnit

### Proje Mimarisi

Backend aşağıdaki projelerden oluşmaktadır:

- `MiniOrder.Domain` — Product, Order ve OrderItem gibi domain entity'lerini içerir.
- `MiniOrder.Application` — DTO'lar, interface'ler, validation kuralları, Result/Error modelleri ve uygulama sözleşmelerini içerir.
- `MiniOrder.Infrastructure` — EF Core persistence, entity configuration, mapping, cache entegrasyonu ve service implementasyonlarını içerir.
- `MiniOrder.Api` — REST controller'ları, dependency injection, Swagger ve global exception handling yapılarını içerir.
- `MiniOrder.Tests` — Validation ve kritik sipariş business rule'ları için otomatik testleri içerir.

Case kapsamı göz önünde bulundurularak CQRS, MediatR veya ek Repository katmanı gibi ihtiyaç duyulmayan abstraction'lar eklenmemiştir.

Amaç, gereksiz mimari karmaşıklık oluşturmadan sorumlulukları ayrılmış ve okunabilir bir yapı oluşturmaktır.

### Veritabanı

İlişkisel veritabanı olarak SQLite kullanılmıştır.

Veritabanı erişimi Entity Framework Core üzerinden gerçekleştirilmekte ve şema değişiklikleri migration'lar ile yönetilmektedir.

Temel entity'ler:

- `Product`
- `Order`
- `OrderItem`

`Product`, ürünün güncel fiyat ve stok bilgisini tutar.

`OrderItem` içerisinde sipariş oluşturulduğu andaki ürün fiyatı `UnitPrice` olarak saklanır. Böylece ürün fiyatının daha sonra değişmesi geçmiş siparişlerin tutarlarını etkilemez.

Uygulamanın ilk çalıştırılmasında örnek ürün verileri otomatik olarak oluşturulur.

### Sipariş Oluşturma Kuralları

Yeni bir sipariş oluşturulurken aşağıdaki kontroller uygulanır:

1. Request validation gerçekleştirilir.
2. Müşteri adı zorunludur.
3. Siparişte en az bir ürün bulunmalıdır.
4. Ürün miktarı sıfırdan büyük olmalıdır.
5. Aynı ürün bir sipariş içerisinde birden fazla kez gönderilemez.
6. Talep edilen tüm ürünlerin veritabanında bulunması gerekir.
7. Her ürün için yeterli stok bulunmalıdır.
8. Sipariş toplamı ürünlerin sipariş anındaki fiyatları üzerinden hesaplanır.
9. Ürün stokları sipariş miktarı kadar azaltılır.
10. Sipariş ve stok değişiklikleri aynı database transaction içerisinde kaydedilir.

Herhangi bir business validation başarısız olduğunda sipariş oluşturulmaz ve ürün stokları değiştirilmez.

### Transaction Yönetimi

Sipariş oluşturma ve stok azaltma işlemleri tek bir transaction içerisinde gerçekleştirilir.

Akış:

```text
Validation
    ↓
Ürün ve stok kontrolü
    ↓
Transaction başlatılır
    ↓
Sipariş oluşturulur
    ↓
Stoklar azaltılır
    ↓
SaveChanges
    ↓
Commit
```

İşlem sırasında beklenmeyen bir hata oluşması durumunda transaction rollback edilir. Böylece sipariş ile stok bilgilerinin birbirinden farklı duruma düşmesi engellenir.

### Cache Yaklaşımı

Ürün detay endpoint'i ASP.NET Core `IMemoryCache` kullanılarak cache'lenmektedir.

Kullanılan strateji:

- Cache edilen veri: `ProductResponse`
- Cache key: `product:{id}`
- Cache süresi: 5 dakika
- Yaklaşım: Cache-aside

İlk ürün detay isteğinde veri veritabanından okunarak cache'e eklenir. Sonraki isteklerde cache süresi dolmadığı sürece veri doğrudan cache üzerinden döndürülür.

Bulunamayan ürünler cache'e eklenmez.

Sipariş başarıyla tamamlandığında sipariş içerisinde bulunan ürünlerin:

```text
product:{id}
```

cache kayıtları temizlenir.

Böylece sipariş nedeniyle stok miktarı değişen bir ürün için eski `StockQuantity` değerinin cache üzerinden dönmesi engellenir.

Ürün arama sonuçları bilinçli olarak cache'lenmemiştir. Bu sayede farklı search parametreleri için gereksiz cache key üretimi ve karmaşık invalidation yönetimi önlenmiştir.

### Validation

Request validation işlemleri FluentValidation ile gerçekleştirilmektedir.

Kontrol edilen başlıca kurallar:

- Müşteri adı zorunluluğu
- Müşteri adı maksimum uzunluğu
- Sipariş ürünlerinin zorunlu olması
- Siparişte en az bir ürün bulunması
- Aynı ürünün tekrarlanmaması
- ProductId değerinin pozitif olması
- Quantity değerinin pozitif olması

### Hata Yönetimi

Beklenen business hataları uygulamadaki `Result<T>` ve `Error` modelleri üzerinden yönetilmektedir.

Örnek hatalar:

- Ürün bulunamadı
- Sipariş bulunamadı
- Yetersiz stok
- Geçersiz sipariş isteği

Örneğin:

```json
{
  "code": "Product.NotFound",
  "message": "Product with id '999' was not found."
}
```

Beklenmeyen sistem hataları ise merkezi `GlobalExceptionHandler` üzerinden yakalanmaktadır.

Bu durumlarda ASP.NET Core `ProblemDetails` kullanılarak `500 Internal Server Error` cevabı oluşturulur. Teknik exception ve stack trace bilgileri API response içerisinde kullanıcıya açılmaz; uygulama loglarında tutulur.

### API Endpointleri

#### Products

```http
GET /api/products
GET /api/products?search={searchTerm}
GET /api/products/{id}
```

#### Orders

```http
POST /api/orders
GET /api/orders
GET /api/orders/{id}
```

Örnek sipariş oluşturma isteği:

```json
{
  "customerName": "Doğukan",
  "items": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 2,
      "quantity": 3
    }
  ]
}
```

Başarılı sipariş oluşturma işlemi `201 Created` döndürür.

Business validation hataları `400 Bad Request`, bulunamayan kaynaklar ise `404 Not Found` olarak döndürülür.

### Testler

Backend otomatik testleri xUnit kullanılarak hazırlanmıştır.

Test sayısını gereksiz şekilde artırmak yerine kritik business davranışlarının doğrulanmasına odaklanılmıştır.

Mevcut test kapsamı:

- Boş sipariş validation kontrolü
- Duplicate ürün kontrolü
- Geçersiz ürün miktarı kontrolü
- Müşteri adı zorunluluk kontrolü
- Yetersiz stok durumunda sipariş oluşturulmaması ve stokların değişmemesi
- Başarılı sipariş oluşturulduğunda toplam tutarın doğru hesaplanması ve stokların doğru azaltılması

Testleri çalıştırmak için `backend` dizininde:

```bash
dotnet test MiniOrder.Tests/MiniOrder.Tests.csproj
```

### Backend'i Çalıştırma

Öncelikle `backend` dizinine geçilir:

```bash
cd backend
```

Bağımlılıklar yüklenir:

```bash
dotnet restore
```

Solution build edilir:

```bash
dotnet build MiniOrder.sln
```

API çalıştırılır:

```bash
dotnet run --project MiniOrder.Api
```

Development ortamında API endpointleri Swagger üzerinden görüntülenebilir ve test edilebilir.