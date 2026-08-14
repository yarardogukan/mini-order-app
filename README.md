# MiniOrder

MiniOrder, **.NET 8** ve **React + TypeScript** kullanılarak geliştirilen minimal bir full-stack e-commerce uygulamasıdır.

Proje başlangıçta teknik değerlendirme kapsamında bir sipariş yönetimi uygulaması olarak geliştirilmiş, sonraki sprintlerde ürün kataloğu, kategori ve marka altyapısı, alışveriş sepeti ve admin yönetim özellikleriyle genişletilmiştir.

Uygulama; ürün keşfi ve arama, kategori ve marka bazlı katalog yapısı, ürün detayları, alışveriş sepeti, sipariş oluşturma ve görüntüleme ile admin tarafında katalog yönetimi akışlarını içerir.

## Current Release

**v1.1.0 — Admin Foundation & Catalog Management**

Bu sürüm ile MiniOrder'ın temel admin yönetim altyapısı ve kategori/marka yönetimi tamamlanmıştır.

---

# Kullanılan Teknolojiler

## Backend

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- FluentValidation
- IMemoryCache
- Swagger / OpenAPI
- xUnit

## Frontend

- React
- TypeScript
- Vite
- React Router
- Native Fetch API
- CSS

---

# Proje Yapısı

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
├── README.md
└── SECURITY.md
```

## Backend Katmanları

### `MiniOrder.Domain`

Domain entity'lerini ve temel domain modellerini içerir.

Başlıca modeller:

- Product
- Category
- Brand
- Order
- OrderItem

### `MiniOrder.Application`

Application katmanı aşağıdaki sorumlulukları içerir:

- DTO'lar
- Request / Response modelleri
- Service interface'leri
- Validation kuralları
- Result ve Error modelleri
- Application seviyesindeki business contract'ları

### `MiniOrder.Infrastructure`

Infrastructure katmanı aşağıdaki teknik implementasyonları içerir:

- Entity Framework Core
- SQLite
- Entity configuration'ları
- Migration'lar
- Service implementasyonları
- Mapping
- Cache kullanımı
- Persistence işlemleri

### `MiniOrder.Api`

API katmanı aşağıdaki sorumlulukları içerir:

- REST Controller'ları
- Dependency Injection
- Swagger / OpenAPI
- Global Exception Handling
- CORS
- HTTP request / response yönetimi

### `MiniOrder.Tests`

Kritik business kuralları ve validation davranışlarını doğrulayan backend testlerini içerir.

---

# Mimari Yaklaşım

Proje, gereksiz mimari karmaşıklık oluşturmadan sorumlulukların ayrıldığı sade bir katmanlı mimari ile geliştirilmiştir.

Backend tarafında temel akış:

```text
API
 ↓
Application
 ↓
Infrastructure
 ↓
Database
```

Case ve projenin mevcut kapsamı nedeniyle aşağıdaki abstraction'lar bilinçli olarak kullanılmamıştır:

- CQRS
- MediatR
- Generic Repository
- Distributed Cache
- Gereksiz domain abstraction'ları

Frontend tarafında da ihtiyaç oluşmadan global state management veya ağır UI framework'leri eklenmemiştir.

Amaç, küçük ve orta ölçekli bir e-commerce uygulaması için okunabilir, geliştirilebilir ve sorumlulukları net bir temel oluşturmaktır.

---

# Uygulama Özellikleri

## Ürün Kataloğu

- Ürünleri listeleme
- Ürün detayını görüntüleme
- Ürün ismine göre arama
- Stok koduna göre arama
- Kategori bazlı katalog yapısı
- Marka bazlı katalog yapısı
- Fiyat görüntüleme
- Mevcut stok miktarını görüntüleme
- Responsive ürün görünümü

## Alışveriş Sepeti

- Sepete ürün ekleme
- Sepette ürün miktarını artırma
- Sepette ürün miktarını azaltma
- Sepetten ürün kaldırma
- Sepet toplamını hesaplama
- Sipariş oluşturma akışına geçiş

## Siparişler

- Müşteri adı ile sipariş oluşturma
- Birden fazla ürün seçme
- Her ürün için miktar belirleme
- Toplam tutarı görüntüleme
- Sipariş oluşturma sonucunu kullanıcıya bildirme
- Siparişleri listeleme
- Sipariş detayını görüntüleme

---

# Admin Panel

MiniOrder, storefront tarafına ek olarak katalog verilerinin yönetilebildiği ayrı bir admin alanına sahiptir.

## Admin Foundation

- Admin login ekranı
- Demo admin authentication
- Protected admin routes
- Session tabanlı admin erişimi
- Admin dashboard
- Responsive admin layout
- Desktop sidebar
- Mobile admin navigation
- Active navigation state
- Logout
- Store ↔ Admin navigation

> **Security Note:** Mevcut admin authentication yapısı portfolio/demo amacıyla hazırlanmıştır ve production-ready authentication olarak değerlendirilmemelidir.

## Admin Dashboard

Dashboard üzerinde katalog durumunu hızlı şekilde takip etmek için temel özet bilgiler bulunmaktadır:

- Total Products
- Total Categories
- Total Brands
- Active Products
- Quick Actions
- Category Management bağlantısı
- Brand Management bağlantısı
- Future Product Management CTA
- Orders placeholder

Dashboard bilinçli olarak hafif tutulmuş ve mevcut kapsam için gereksiz chart/analytics bileşenleri eklenmemiştir.

---

# Category Management

Admin panel üzerinden kategori ve alt kategori yönetimi yapılabilir.

Desteklenen işlemler:

- Kategorileri listeleme
- Root category oluşturma
- Subcategory oluşturma
- Kategori detayını görüntüleme
- Kategori güncelleme
- Parent category yönetimi
- Active / inactive durum yönetimi
- Kategori silme
- Delete confirmation
- Kategori adını yazarak destructive action doğrulaması
- Backend business error handling
- Loading state
- Empty state
- Error state
- Success feedback

Kategori yapısı parent/subcategory ilişkisini desteklemektedir.

---

# Brand Management

Admin panel üzerinden ürün markaları yönetilebilir.

Desteklenen işlemler:

- Markaları listeleme
- Marka detayını görüntüleme
- Marka oluşturma
- Marka güncelleme
- Active / inactive durum yönetimi
- Marka silme
- Delete confirmation
- Marka adını yazarak destructive action doğrulaması
- Backend business error handling
- Loading state
- Empty state
- Error state
- Success feedback

---

# Shared Admin UX

Admin panel içerisinde ortak bir kullanıcı deneyimi oluşturmak amacıyla aşağıdaki UI pattern'leri kullanılmaktadır:

- Tables
- Status badges
- Page headers
- Forms
- Modals
- Confirmation dialogs
- Alerts
- Loading buttons
- Empty states
- Error states

Admin ekranları desktop, tablet ve mobile kullanım için responsive olarak tasarlanmıştır.

---

# Business Kuralları

## Sipariş Oluşturma

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

İlişkisel veritabanı olarak **SQLite** kullanılmaktadır.

Temel domain modelleri:

```text
Product
Category
Brand
Order
OrderItem
```

`OrderItem`, `Product` ile sipariş arasındaki ilişkiyi temsil eder ve sipariş anındaki:

- Quantity
- UnitPrice
- LineTotal

bilgilerini saklar.

Bu sayede ürün fiyatı daha sonra değişse bile geçmiş sipariş kayıtları etkilenmez.

Ürün kataloğu tarafında Category ve Brand yapıları ürünlerin sınıflandırılması ve yönetilebilir katalog yapısının oluşturulması için kullanılmaktadır.

Uygulama ilk çalıştırıldığında geliştirme/demo amacıyla örnek veriler eklenebilir.

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

Bu yaklaşım sipariş ve stok verilerinin tutarlı kalmasını sağlar.

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

Request validation işlemleri backend tarafında **FluentValidation** ile yapılmaktadır.

Sipariş tarafındaki temel kontroller:

- Customer name zorunluluğu
- Customer name maksimum uzunluğu
- En az bir order item bulunması
- Duplicate product kontrolü
- ProductId değerinin pozitif olması
- Quantity değerinin pozitif olması

Admin katalog işlemlerinde ayrıca backend business kuralları uygulanmaktadır.

Frontend tarafında kullanıcı deneyimini iyileştirmek için temel form validation uygulanır.

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

Admin Category ve Brand işlemlerindeki business hataları da API üzerinden frontend'e taşınarak kullanıcıya anlamlı mesajlar halinde gösterilir.

Beklenmeyen sistem hataları merkezi `GlobalExceptionHandler` tarafından yakalanır.

Bu hatalarda ASP.NET Core `ProblemDetails` kullanılarak:

```text
500 Internal Server Error
```

response'u oluşturulur.

Teknik exception detayları ve stack trace kullanıcıya açılmaz; log içerisinde tutulur.

---

# REST API

## Products

```http
GET /api/products
GET /api/products?search={searchTerm}
GET /api/products/{id}
```

## Categories

```http
GET /api/categories
GET /api/categories/{id}
POST /api/categories
PUT /api/categories/{id}
DELETE /api/categories/{id}
```

## Brands

```http
GET /api/brands
GET /api/brands/{id}
POST /api/brands
PUT /api/brands/{id}
DELETE /api/brands/{id}
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

Başarılı resource oluşturma:

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

Frontend React + TypeScript ile geliştirilmiştir.

Ana storefront ve admin route'ları:

```text
/
└── Welcome

/products
├── Product List
└── Search / Catalog

/products/:id
└── Product Detail

/cart
└── Shopping Cart

/orders/create
└── Create Order

/orders
└── Order List

/orders/:id
└── Order Detail

/admin/login
└── Admin Login

/admin
└── Admin Dashboard

/admin/categories
└── Category Management

/admin/brands
└── Brand Management
```

Ürün aramasında gereksiz API çağrılarını azaltmak için `400 ms` debounce uygulanmıştır.

Frontend içerisinde:

- Loading state
- Empty state
- API error state
- Form validation
- Success / error feedback
- Confirmation dialogs
- Responsive storefront
- Responsive admin panel
- Protected admin navigation

bulunmaktadır.

Global state management kütüphanesine mevcut kapsamda ihtiyaç duyulmadığı için Redux veya benzeri ek bir yapı kullanılmamıştır.

---

# Admin Authentication

Admin alanı storefront'tan ayrı bir giriş akışına sahiptir.

Mevcut authentication çözümü:

- Demo login
- Session tabanlı admin state
- Protected routes
- Route guard
- Logout
- Admin → Store navigation
- Store → Admin navigation

Bu authentication mekanizması yalnızca geliştirme, portfolio ve demo amaçlıdır.

Production ortamında kullanılacak authentication sistemi için aşağıdaki konular ayrıca ele alınmalıdır:

- Server-side authentication
- Secure credential storage
- Password hashing
- Authorization
- Secure session/token management
- Refresh/revocation strategy
- Rate limiting
- Audit logging
- CSRF/XSS güvenlik kontrolleri

---

# Responsive Design

Storefront ve admin panel responsive olarak geliştirilmiştir.

Admin tarafında:

- Desktop sidebar
- Tablet layout
- Mobile admin navigation
- Responsive tables
- Responsive forms
- Responsive modals
- Responsive dashboard cards

desteklenmektedir.

---

# Testler

Backend testleri **xUnit** ile hazırlanmıştır.

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

Admin tarafında geliştirme sürecinde manuel smoke testler uygulanmıştır:

- Admin login
- Logout
- Route guard
- Category CRUD
- Subcategory CRUD
- Category business errors
- Brand CRUD
- Store ↔ Admin navigation
- Responsive davranış
- Frontend production build

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

Frontend varsayılan Vite development ortamında:

```text
http://localhost:5173
```

üzerinden açılır.

Backend'in de aynı anda çalışıyor olması gerekir.

Production frontend build kontrolü için:

```bash
npm run build
```

---

# Development Roadmap

MiniOrder iteratif sprint yaklaşımıyla geliştirilmektedir.

## Completed

### Initial Foundation

- Backend layered architecture
- SQLite persistence
- Product listing
- Product detail
- Product search
- Order creation
- Order listing
- Order detail
- Transaction management
- Cache
- Validation
- Backend tests
- React frontend

### Storefront Foundation

- Product catalog experience
- Product detail experience
- Shopping cart
- Responsive storefront

### Admin Foundation & Catalog Management — v1.1.0

- Admin login
- Protected admin routes
- Admin dashboard
- Responsive admin shell
- Category Management
- Subcategory Management
- Brand Management
- Admin CRUD feedback states
- Confirmation dialogs
- Store ↔ Admin navigation

## Next Milestone

### Admin Product Management

Planlanan sonraki geliştirme alanları:

- Product admin navigation
- Product management table
- Product create
- Product edit
- Product delete/deactivate
- Category assignment
- Brand assignment
- Price management
- Stock management
- Product status management
- Storefront ↔ Admin product integration
- Dashboard product integration

---

# Release History

## v1.1.0 — Admin Foundation & Catalog Management

MiniOrder'ın ilk kapsamlı admin yönetim sürümü.

Öne çıkan geliştirmeler:

- Admin dashboard
- Demo admin authentication
- Protected admin routes
- Responsive admin navigation
- Category CRUD
- Subcategory CRUD
- Brand CRUD
- Catalog business error handling
- Destructive action confirmation
- Shared admin UX patterns
- Responsive admin experience

---

# Security

Güvenlik açıklarının public GitHub issue üzerinden paylaşılmaması önerilir.

Güvenlik politikası ve vulnerability reporting süreci için repository içerisindeki:

```text
SECURITY.md
```

dosyasına bakabilirsiniz.

---

# Developer

**Doğukan Yarar**

GitHub: `yarardogukan`

Repository:

```text
https://github.com/yarardogukan/mini-order-app
```

---

# License & Usage

MiniOrder eğitim, teknik değerlendirme ve portfolio geliştirme amacıyla oluşturulmuştur.
