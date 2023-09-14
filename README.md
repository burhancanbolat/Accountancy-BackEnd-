# ZeusApp-Backend

## Proje Katmanları
- Domain Entities/Domain/Core Katmanı: Mimarinin merkezi katmanıdır. Tüm uygulama için olan Domain ve veritabanı entity’leri bu katmanda oluşturulur.
- Entities: ORM araçları tarafından kullanılan ve veritabanındaki tabloları temsil eden sınıflardır. *(En önemli)
- Value Object: Kimliksiz ve immutable(değişmez) olan nesnelerdir.
- Enumeration
- Exceptions: Domain için oluşturulan exception sınıflarıdır.


## Migration İşlemleri
ApplicationDBContext için migration oluşturma:  
```
add-migration MIGRATE_NAME -context ApplicationDbContext
```

IdentityContext için migration oluşturma:  
```
add-migration MIGRATE_NAME -context IdentityContext
```

IdentityContext için veritabanı güncelleme:  
```
update-database -context IdentityContext
```

ApplicationDbContext için veritabanı güncelleme:  
```
update-database -context ApplicationDbContext
```
