# Mini Order App

Teknik değerlendirme kapsamında geliştirilen full-stack mini sipariş yönetim uygulamasıdır.

Uygulama; ürün görüntüleme ve arama, birden fazla ürün ile sipariş oluşturma, stok yönetimi, sipariş listeleme ve sipariş detaylarını görüntüleme akışlarını içerir.

## Kullanılan Teknolojiler

### Backend

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- FluentValidation
- IMemoryCache
- Swagger / OpenAPI
- xUnit

### Frontend

- React
- TypeScript
- Vite
- React Router
- Native Fetch API
- CSS

## Proje Yapısı

```text
mini-order-app
├── backend
│   ├── MiniOrder.Api
│   ├── MiniOrder.Application
│   ├── MiniOrder.Domain
│   ├── MiniOrder.Infrastructure
│   ├── MiniOrder.Tests
│   └── MiniOrder.sln
│
├── frontend
│   ├── src
│   └── package.json
│
└── README.md
```

### Backend Katmanları

- `MiniOrder.Domain`

  - Product, Order ve OrderItem entity'lerini içerir.

- `MiniOrder.Application`

  - DTO'lar
  - Interface'ler
  - Validation kuralları
  - Result ve Error modelleri

- `MiniOrder.Infrastructure`

  - Entity Framework Core
  - SQLite
  - Entity configuration'ları
  - Migration'lar
  - Service implementasyonları
  - Mapping
  - Cache kullanımı

- `MiniOrder.Api`

  - REST Controller'ları
  - Dependency Injection
  - Swagger
  - Global Exception Handling
  - CORS

- `MiniOrder.Tests`
  - Kritik business kuralları ve validation testleri

Case kapsamı nedeniyle CQRS, MediatR, Generic Repository veya global state management gibi ek abstraction'lar kullanılmamıştır.

Amaç, gereksiz mimari karmaşıklık oluşturmadan okunabilir ve sorumlulukları ayrılmış bir çözüm oluşturmaktır.

---

# Uygulama Özellikleri

## Ürünler

- Ürünleri listeleme
- Ürün detayını görüntüleme
- Ürün ismine göre arama
- Stok koduna göre arama
- Fiyat görüntüleme
- Mevcut stok miktarını görüntüleme

## Siparişler

- Müşteri adı ile sipariş oluşturma
- Birden fazla ürün seçme
- Her ürün için miktar belirleme
- Toplam tutarı görüntüleme
- Sipariş oluşturma sonucunu kullanıcıya bildirme
- Siparişleri listeleme
- Sipariş detayını görüntüleme

---

# Business Kuralları

Yeni bir sipariş oluşturulurken:

1. Müşteri adı zorunludur.
2. Siparişte en az bir ürün bulunmalıdır.
3. Ürün miktarı sıfırdan büyük olmalıdır.
4. Aynı ürün bir siparişte birden fazla kez bulunamaz.
5. Ürünlerin veritabanında bulunması gerekir.
6. Her ürün için yeterli stok bulunmalıdır.
7. Sipariş oluşturulduğunda ürün stokları azaltılır.
8. Sipariş tutarı ürünlerin sipariş anındaki fiyatları üzerinden hesaplanır.
9. Sipariş anındaki fiyat `OrderItem.UnitPrice` içerisinde saklanır.
10. Sipariş ve stok değişiklikleri tek bir database transaction içerisinde gerçekleştirilir.

Herhangi bir üründe yeterli stok bulunmadığında:

- Sipariş oluşturulmaz.
- Hiçbir ürünün stoğu azaltılmaz.
- Kullanıcıya anlaşılır hata mesajı döndürülür.

---

# Database

İlişkisel veritabanı olarak SQLite kullanılmıştır.

Temel entity'ler:

```text
Product
Order
OrderItem
```

`OrderItem`, `Product` ile sipariş arasındaki ilişkiyi temsil eder ve sipariş anındaki:

- Quantity
- UnitPrice
- LineTotal

bilgilerini saklar.

Bu sayede ürün fiyatı daha sonra değişse bile geçmiş sipariş kayıtları etkilenmez.

Uygulama ilk çalıştırıldığında örnek ürünler otomatik olarak eklenir.

---

# Transaction Yönetimi

Sipariş oluşturma ve stok azaltma işlemleri aynı transaction içerisinde gerçekleştirilir.

```text
Request Validation
       ↓
Product Control
       ↓
Stock Control
       ↓
Begin Transaction
       ↓
Create Order
       ↓
Decrease Stock
       ↓
SaveChanges
       ↓
Commit
```

İşlem sırasında beklenmeyen bir hata oluşursa transaction rollback edilir.

---

# Cache Yaklaşımı

Ürün detay endpoint'i `IMemoryCache` ile cache'lenmektedir.

Kullanılan strateji:

```text
Cache Key: product:{id}
Cache Duration: 5 dakika
Cached Value: ProductResponse
Strategy: Cache-aside
```

İlk istekte ürün veritabanından okunarak cache'e yazılır.

Sonraki isteklerde cache süresi dolmadıysa ürün doğrudan cache üzerinden döndürülür.

Bulunamayan ürünler cache'e eklenmez.

Sipariş başarıyla oluşturulduktan sonra siparişte bulunan ürünlerin cache kayıtları temizlenir:

```text
product:{id}
```

Böylece stok değiştikten sonra eski `StockQuantity` değerinin cache üzerinden dönmesi engellenir.

Arama sonuçları bilinçli olarak cache'lenmemiştir. Bu sayede farklı arama parametreleri için gereksiz cache key üretimi ve karmaşık invalidation yönetimi önlenmiştir.

---

# Validation

Request validation işlemleri FluentValidation ile yapılmaktadır.

Kontroller:

- Customer name zorunluluğu
- Customer name maksimum uzunluğu
- En az bir order item bulunması
- Duplicate product kontrolü
- ProductId değerinin pozitif olması
- Quantity değerinin pozitif olması

Frontend tarafında kullanıcı deneyimi için temel validation uygulanır.

Asıl business rule doğrulamaları backend tarafından gerçekleştirilir.

---

# Hata Yönetimi

Beklenen business hataları:

```text
Result<T>
+
Error
```

yapısı üzerinden yönetilir.

Örnek:

```json
{
  "code": "Product.InsufficientStock",
  "message": "'Wireless Mouse' has insufficient stock. Requested: 999, Available: 97."
}
```

Beklenmeyen sistem hataları merkezi `GlobalExceptionHandler` tarafından yakalanır.

Bu hatalarda ASP.NET Core `ProblemDetails` kullanılarak `500 Internal Server Error` response'u oluşturulur.

Teknik exception detayları ve stack trace kullanıcıya açılmaz; log içerisinde tutulur.

---

# REST API

## Products

```http
GET /api/products
GET /api/products?search={searchTerm}
GET /api/products/{id}
```

## Orders

```http
POST /api/orders
GET /api/orders
GET /api/orders/{id}
```

Örnek sipariş isteği:

```json
{
  "customerName": "Example Customer",
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

Başarılı sipariş oluşturma isteği:

```text
201 Created
```

Geçersiz business request:

```text
400 Bad Request
```

Bulunamayan kaynak:

```text
404 Not Found
```

---

# Frontend

Uygulama açıldığında teknik değerlendirme için hazırlanmış bir karşılama ekranı gösterilir.

Ana ekranlar:

```text
/
└── Welcome

/products
└── Product List + Search

/orders/create
└── Create Order

/orders
└── Order List

/orders/:id
└── Order Detail
```

Ürün aramasında gereksiz API çağrılarını azaltmak için `400 ms` debounce uygulanmıştır.

Frontend içerisinde:

- Loading state
- Empty state
- API error state
- Form validation
- Success / error alert
- Responsive temel görünüm

bulunmaktadır.

Global state management kütüphanesine ihtiyaç duyulmadığı için Redux veya benzeri bir yapı kullanılmamıştır.

---

# Testler

Backend testleri xUnit ile hazırlanmıştır.

Mevcut testler kritik business davranışlarını doğrulamaktadır:

- Boş sipariş validation kontrolü
- Duplicate ürün validation kontrolü
- Quantity validation kontrolü
- Customer name validation kontrolü
- Yetersiz stok durumunda sipariş oluşturulmaması ve stokların değişmemesi
- Başarılı sipariş oluşturulduğunda toplam tutarın ve stokların doğru güncellenmesi

Testleri çalıştırmak için:

```bash
cd backend
dotnet test MiniOrder.Tests/MiniOrder.Tests.csproj
```

---

# Uygulamayı Çalıştırma

## Backend

Repository içerisindeki `backend` klasörüne geçin:

```bash
cd backend
```

Bağımlılıkları yükleyin:

```bash
dotnet restore
```

Solution'ı build edin:

```bash
dotnet build MiniOrder.sln
```

API'yi çalıştırın:

```bash
dotnet run --project MiniOrder.Api
```

API local ortamda çalışmaktadır.

Swagger Development ortamında kullanılabilir.

## Frontend

Yeni bir terminal açarak:

```bash
cd frontend
```

Bağımlılıkları yükleyin:

```bash
npm install
```

React uygulamasını başlatın:

```bash
npm run dev
```

Frontend:

```text
http://localhost:5173
```

üzerinden açılır.

Backend'in de aynı anda çalışıyor olması gerekir.

---

# Teknik Değerlendirme Soruları

## 1. Uygulama nasıl çalıştırılır?

Backend:

```bash
cd backend
dotnet restore
dotnet run --project MiniOrder.Api
```

Frontend:

```bash
cd frontend
npm install
npm run dev
```

Frontend `http://localhost:5173` üzerinden kullanılabilir.

---

## 2. Problemi hangi parçalara ayırdınız?

Çalışmayı temel olarak:

- Domain ve database modeli
- Application sözleşmeleri ve validation
- Persistence ve business service'leri
- REST API
- Cache
- Testler
- React frontend
- Backend / frontend entegrasyonu

şeklinde parçalara ayırdım.

---

## 3. Database modelini neden bu şekilde oluşturdunuz?

`Product`, ürünün güncel fiyat ve stok bilgisini temsil eder.

`Order`, siparişin üst bilgisini tutar.

`OrderItem`, sipariş ile ürün arasındaki ilişkiyi ve sipariş anındaki miktar/fiyat bilgisini saklar.

Fiyat bilgisinin `OrderItem` içerisinde ayrıca tutulmasının nedeni, ürün fiyatı gelecekte değişse bile geçmiş siparişlerin değişmemesidir.

---

## 4. Kod organizasyonunu neden bu şekilde tercih ettiniz?

Business logic'in controller içerisine yığılmaması ve sorumlulukların açık şekilde ayrılması için Domain, Application, Infrastructure ve API katmanlarına ayrılmış sade bir yapı kullandım.

Case kapsamında ihtiyaç olmadığı için CQRS, MediatR veya Generic Repository gibi ek abstraction'lar kullanmadım.

Frontend tarafında da ihtiyaç doğmadan gereksiz component veya state management katmanları oluşturmadım.

---

## 5. Sipariş ve stok işlemlerinde veri bütünlüğünü nasıl sağladınız?

Sipariş kaydı ve ürün stoklarının azaltılması aynı EF Core transaction içerisinde gerçekleştirilir.

Tüm kontroller başarılı olduktan sonra değişiklikler kaydedilir ve transaction commit edilir.

Herhangi bir hata oluşması durumunda rollback uygulanır.

---

## 6. Cache'i nerede ve neden kullandınız?

Ürün detay endpoint'inde `IMemoryCache` kullandım.

Sık okunabilecek ancak her istekte database sorgusu gerektirmeyen ürün detaylarının kısa süreli olarak memory üzerinden dönmesini amaçladım.

---

## 7. Stok değiştiğinde cache'i nasıl yönettiniz?

Sipariş transaction'ı başarıyla commit edildikten sonra sipariş içerisindeki ürünlerin:

```text
product:{id}
```

cache key'leri silinir.

Bir sonraki ürün detay isteğinde güncel stok database üzerinden okunarak tekrar cache'e yazılır.

---

## 8. Süre nedeniyle tamamlamadığınız veya sadeleştirdiğiniz noktalar nelerdir?

Case'in çalışan ve anlaşılır çözüm beklentisine bağlı kalmak için:

- Authentication eklenmedi.
- Product CRUD geliştirilmedi.
- Redux gibi global state management kullanılmadı.
- Distributed cache kullanılmadı.
- CQRS / MediatR gibi ek mimari katmanlar eklenmedi.
- Docker zorunlu olmadığı için eklenmedi.

Bu noktaların büyük kısmı case kapsamında zaten zorunlu değildir ve çözümü gereksiz şekilde karmaşıklaştırmamak adına bilinçli olarak sade tutulmuştur.

---

## 9. Hangi AI araçlarını kullandınız?

Geliştirme sürecinde OpenAI ChatGPT'den teknik değerlendirme, alternatif yaklaşım analizi, debugging ve kod review desteği aldım.

---

## 10. AI tarafından üretilen kodları nasıl kontrol ettiniz?

AI önerilerini doğrudan kabul etmek yerine:

- Kodun mevcut mimariyle uyumunu kontrol ettim.
- Her geliştirmeden sonra build aldım.
- API endpointlerini Swagger ve Postman üzerinden manuel test ettim.
- Kritik business kuralları için otomatik testler yazdım.
- Cache ve transaction davranışlarını log ve database sonuçları üzerinden doğruladım.
- Gereksiz abstraction veya case kapsamını aşan önerileri uygulamadım.

Teslim edilen kodun tamamını anlayabilecek ve teknik kararlarını açıklayabilecek şekilde ilerledim.

---

## 11. Çalışmaya yaklaşık ne kadar zaman ayırdınız?

Aktif geliştirme süresi yaklaşık **X saat** olmuştur.

Çalışma case için verilen 48 saatlik teslim süresi göz önünde bulundurularak planlanmıştır.

---

# Developer

**Doğukan Yarar**

GitHub: `yarardogukan`

Repository:

```text
https://github.com/yarardogukan/mini-order-app
```
