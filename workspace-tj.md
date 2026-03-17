
# 📦 Задание: REST API для системы управления коворкингом WorkSpace.tj

## 🎯 Цель проекта

Разработать REST API на **C# (.NET 8+)** с использованием **Dapper** и **PostgreSQL** для системы управления бронированием рабочих мест в коворкинге.

API должно позволять:

* управлять компаниями
* управлять рабочими пространствами
* создавать бронирования
* получать статистику по бронированиям

---

# 📚 Технологический стек

* ASP.NET Core WebAPI (.NET 8+)
* Dapper
* PostgreSQL
* Swagger / OpenAPI
* Dependency Injection


Контроллеры должны возвращать:

```
ActionResult<T>
```

или

```
IActionResult
```

---

# 🏗️ Структура решения

```
WorkSpace/
├── WorkSpace.API/
│   ├── Controllers/
│   └── Program.cs
│
├── WorkSpace.Domain/
│   ├── Entities/
│   └── Dtos/
│
└── WorkSpace.Infrastructure/
    ├── Interfaces/
    └── Services/
```

---

# 📂 Структура базы данных

## 1. Таблица `companies`

| Поле       | Тип                | Описание      |
| ---------- | ------------------ | ------------- |
| id         | SERIAL PRIMARY KEY | id компании   |
| name       | VARCHAR(255)       | название      |
| phone      | VARCHAR(50)        | телефон       |
| email      | VARCHAR(255)       | email         |
| created_at | TIMESTAMP          | дата создания |

---

## 2. Таблица `rooms`

| Поле           | Тип                | Описание |
| -------------- | ------------------ | -------- |
| id             | SERIAL PRIMARY KEY |          |
| name           | VARCHAR(255)       |          |
| capacity       | INTEGER            |          |
| price_per_hour | DECIMAL(10,2)      |          |
| created_at     | TIMESTAMP          |          |

---

## 3. Таблица `workspaces`

| Поле       | Тип                | Описание |
| ---------- | ------------------ | -------- |
| id         | SERIAL PRIMARY KEY |          |
| room_id    | INTEGER            |          |
| name       | VARCHAR(100)       |          |
| type       | VARCHAR(50)        |          |
| created_at | TIMESTAMP          |          |

---

## 4. Таблица `bookings`

| Поле         | Тип                | Описание |
| ------------ | ------------------ | -------- |
| id           | SERIAL PRIMARY KEY |          |
| company_id   | INTEGER            |          |
| workspace_id | INTEGER            |          |
| booking_date | DATE               |          |
| start_time   | TIME               |          |
| end_time     | TIME               |          |
| total_price  | DECIMAL(10,2)      |          |
| status       | VARCHAR(50)        |          |
| created_at   | TIMESTAMP          |          |

---

# 🧪 Данные для тестирования (10 INSERT)

```sql
INSERT INTO companies(name, phone, email, created_at)
VALUES
('SoftClub', '+992900000001', 'softclub@mail.com', now()),
('Alif Tech', '+992900000002', 'alif@mail.com', now()),
('DC Bank', '+992900000003', 'dc@mail.com', now());


INSERT INTO rooms(name, capacity, price_per_hour, created_at)
VALUES
('Open Space', 20, 10, now()),
('Meeting Room', 8, 20, now()),
('Conference Room', 40, 50, now());


INSERT INTO workspaces(room_id, name, type, created_at)
VALUES
(1, 'Desk 1', 'desk', now()),
(1, 'Desk 2', 'desk', now()),
(2, 'Table 1', 'meeting', now()),
(2, 'Table 2', 'meeting', now());


INSERT INTO bookings(company_id, workspace_id, booking_date, start_time, end_time, total_price, status, created_at)
VALUES
(1,1,'2026-03-01','10:00','12:00',20,'confirmed',now()),
(2,3,'2026-03-01','13:00','15:00',40,'confirmed',now()),
(3,2,'2026-03-02','09:00','11:00',20,'pending',now());
```

---

# 🔍 API Endpoints

---

# CompanyController

### 1️⃣ Получить все компании

```
GET /api/companies
```

Метод

```
Task<IEnumerable<Company>> GetAllAsync()
```

---

### 2️⃣ Получить компанию по id

```
GET /api/companies/{id}
```

Метод

```
Task<Company> GetByIdAsync(int id)
```

---

### 3️⃣ Создать компанию

```
POST /api/companies
```

Метод

```
Task<int> CreateAsync(Company request)
```

---

### 4️⃣ Обновить компанию

```
PUT /api/companies/{id}
```

Метод

```
Task<bool> UpdateAsync(int id, Company request)
```

---

### 5️⃣ Удалить компанию

```
DELETE /api/companies/{id}
```

Метод

```
Task<bool> DeleteAsync(int id)
```

---

# RoomController

---

### 1️⃣ Получить все комнаты

```
GET /api/rooms
```

---

### 2️⃣ Получить комнату по id

```
GET /api/rooms/{id}
```

---

### 3️⃣ Создать комнату

```
POST /api/rooms
```

---

### 4️⃣ Обновить комнату

```
PUT /api/rooms/{id}
```

---

### 5️⃣ Удалить комнату

```
DELETE /api/rooms/{id}
```

---

# BookingController

---

### 1️⃣ Получить бронирования компании

```
GET /api/bookings/company/{companyId}
```

Метод

```
Task<IEnumerable<Booking>> GetCompanyBookingsAsync(int companyId)
```

---

### 2️⃣ Создать бронирование

```
POST /api/bookings
```

Метод

```
Task<int> CreateBookingAsync(CreateBookingRequest request)
```

---

### 3️⃣ Обновить статус бронирования

```
PUT /api/bookings/{id}/status
```

Метод

```
Task<bool> UpdateBookingStatusAsync(int id, string status)
```

---

### 4️⃣ Получить бронирования за день

```
GET /api/bookings/daily
```

Метод

```
Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date)
```

---

# DTO для создания бронирования

```csharp
public class CreateBookingRequest
{
    public int CompanyId { get; set; }

    public int WorkspaceId { get; set; }

    public DateTime BookingDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }
}
```

---

# 📊 Дополнительная задача (SQL + Dapper)

Реализовать endpoint:

```
GET /api/bookings/statistics
```

Должен вернуть:

* количество бронирований
* общую сумму
* количество компаний

---

# 💡 Дополнительные требования

Студенты должны:

✔ использовать **Dapper**

✔ использовать **Dependency Injection**

✔ использовать **Logging (ILogger)** для логирования действий в сервисах

✔ писать **SQL-запросы вручную**

✔ использовать **DTO для запросов**

---

## Logging

Во всех сервисах необходимо использовать **ILogger** для логирования следующих действий:

* создание записей
* обновление данных
* удаление данных
* ошибки при работе с базой данных

Логи должны содержать **информативные сообщения**, например:

* создание компании
* создание бронирования
* изменение статуса заказа
* ошибки при выполнении SQL-запросов

---

## Dependency Injection

Все сервисы должны быть зарегистрированы через **Dependency Injection** и использоваться через **интерфейсы**.

Пример:

```
ICompanyService
CompanyService
```

```
IBookingService
BookingService
```

Контроллеры должны получать сервисы через **конструктор**.


##

# 📦 Вазифа: REST API барои системаи идоракунии коворкинг **WorkSpace.tj**

## 🎯 Ҳадафи лоиҳа

Таҳия кардани **REST API** бо истифода аз **C# (.NET 8+)**, **Dapper** ва **PostgreSQL** барои системаи бронкунии ҷойҳои корӣ дар коворкинг.

API бояд имконият диҳад:

* идора кардани ширкатҳо
* идора кардани ҷойҳои корӣ
* сохтани бронҳо (booking)
* гирифтани статистикаи бронҳо

---

# 📚 Технологияҳои истифодашаванда

* ASP.NET Core WebAPI (.NET 8+)
* Dapper
* PostgreSQL
* Swagger / OpenAPI
* Dependency Injection

Контроллерҳо бояд баргардонанд:

```
ActionResult<T>
```

ё

```
IActionResult
```

---

# 🏗️ Сохтори Solution

```
WorkSpace/
├── WorkSpace.API/
│   ├── Controllers/
│   └── Program.cs
│
├── WorkSpace.Domain/
│   ├── Entities/
│   └── Dtos/
│
└── WorkSpace.Infrastructure/
    ├── Interfaces/
    └── Services/
```

---

# 📂 Сохтори базаи додаҳо

## 1. Ҷадвали `companies`

| Майдон     | Навъ               | Тавсиф                  |
| ---------- | ------------------ | ----------------------- |
| id         | SERIAL PRIMARY KEY | идентификатори ширкат   |
| name       | VARCHAR(255)       | номи ширкат             |
| phone      | VARCHAR(50)        | рақами телефон          |
| email      | VARCHAR(255)       | почтаи электронӣ        |
| created_at | TIMESTAMP          | санаи сохта шудани сабт |

---

## 2. Ҷадвали `rooms`

| Майдон         | Навъ               | Тавсиф |
| -------------- | ------------------ | ------ |
| id             | SERIAL PRIMARY KEY |        |
| name           | VARCHAR(255)       |        |
| capacity       | INTEGER            |        |
| price_per_hour | DECIMAL(10,2)      |        |
| created_at     | TIMESTAMP          |        |

---

## 3. Ҷадвали `workspaces`

| Майдон     | Навъ               | Тавсиф |
| ---------- | ------------------ | ------ |
| id         | SERIAL PRIMARY KEY |        |
| room_id    | INTEGER            |        |
| name       | VARCHAR(100)       |        |
| type       | VARCHAR(50)        |        |
| created_at | TIMESTAMP          |        |

---

## 4. Ҷадвали `bookings`

| Майдон       | Навъ               | Тавсиф |
| ------------ | ------------------ | ------ |
| id           | SERIAL PRIMARY KEY |        |
| company_id   | INTEGER            |        |
| workspace_id | INTEGER            |        |
| booking_date | DATE               |        |
| start_time   | TIME               |        |
| end_time     | TIME               |        |
| total_price  | DECIMAL(10,2)      |        |
| status       | VARCHAR(50)        |        |
| created_at   | TIMESTAMP          |        |

---

# 🧪 Маълумот барои тест (10 INSERT)

```sql
INSERT INTO companies(name, phone, email, created_at)
VALUES
('SoftClub', '+992900000001', 'softclub@mail.com', now()),
('Alif Tech', '+992900000002', 'alif@mail.com', now()),
('DC Bank', '+992900000003', 'dc@mail.com', now());


INSERT INTO rooms(name, capacity, price_per_hour, created_at)
VALUES
('Open Space', 20, 10, now()),
('Meeting Room', 8, 20, now()),
('Conference Room', 40, 50, now());


INSERT INTO workspaces(room_id, name, type, created_at)
VALUES
(1, 'Desk 1', 'desk', now()),
(1, 'Desk 2', 'desk', now()),
(2, 'Table 1', 'meeting', now()),
(2, 'Table 2', 'meeting', now());


INSERT INTO bookings(company_id, workspace_id, booking_date, start_time, end_time, total_price, status, created_at)
VALUES
(1,1,'2026-03-01','10:00','12:00',20,'confirmed',now()),
(2,3,'2026-03-01','13:00','15:00',40,'confirmed',now()),
(3,2,'2026-03-02','09:00','11:00',20,'pending',now());
```

---

# 🔍 API Endpoints

---

# CompanyController

### 1️⃣ Гирифтани ҳамаи ширкатҳо

```
GET /api/companies
```

Метод

```
Task<IEnumerable<Company>> GetAllAsync()
```

---

### 2️⃣ Гирифтани ширкат аз рӯи id

```
GET /api/companies/{id}
```

Метод

```
Task<Company> GetByIdAsync(int id)
```

---

### 3️⃣ Сохтани ширкат

```
POST /api/companies
```

Метод

```
Task<int> CreateAsync(Company request)
```

---

### 4️⃣ Навсозии ширкат

```
PUT /api/companies/{id}
```

Метод

```
Task<bool> UpdateAsync(int id, Company request)
```

---

### 5️⃣ Ҳазфи ширкат

```
DELETE /api/companies/{id}
```

Метод

```
Task<bool> DeleteAsync(int id)
```

---

# RoomController

---

### 1️⃣ Гирифтани ҳамаи утоқҳо

```
GET /api/rooms
```

---

### 2️⃣ Гирифтани утоқ аз рӯи id

```
GET /api/rooms/{id}
```

---

### 3️⃣ Сохтани утоқ

```
POST /api/rooms
```

---

### 4️⃣ Навсозии утоқ

```
PUT /api/rooms/{id}
```

---

### 5️⃣ Ҳазфи утоқ

```
DELETE /api/rooms/{id}
```

---

# BookingController

---

### 1️⃣ Гирифтани бронҳои ширкат

```
GET /api/bookings/company/{companyId}
```

Метод

```
Task<IEnumerable<Booking>> GetCompanyBookingsAsync(int companyId)
```

---

### 2️⃣ Сохтани брон

```
POST /api/bookings
```

Метод

```
Task<int> CreateBookingAsync(CreateBookingRequest request)
```

---

### 3️⃣ Навсозии статуси брон

```
PUT /api/bookings/{id}/status
```

Метод

```
Task<bool> UpdateBookingStatusAsync(int id, string status)
```

---

### 4️⃣ Гирифтани бронҳо барои як рӯз

```
GET /api/bookings/daily
```

Метод

```
Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date)
```

---

# DTO барои сохтани брон

```csharp
public class CreateBookingRequest
{
    public int CompanyId { get; set; }

    public int WorkspaceId { get; set; }

    public DateTime BookingDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }
}
```

---

# 📊 Вазифаи иловагӣ (SQL + Dapper)

Реализатсия кардани endpoint:

```
GET /api/bookings/statistics
```

Бояд баргардонад:

* шумораи бронҳо
* маблағи умумии бронҳо
* шумораи ширкатҳо

---

# 💡 Талаботи иловагӣ

Донишҷӯён бояд:

✔ **Dapper** истифода баранд

✔ **Dependency Injection** истифода баранд

✔ **Logging (ILogger)** барои сабт кардани логҳо дар сервисҳо истифода баранд

✔ **SQL-запросҳоро дастӣ нависанд**

✔ **DTO** барои request истифода баранд

---

## Logging

Дар ҳамаи сервисҳо бояд **ILogger** истифода шавад барои лог кардани амалҳои зерин:

* сохтани сабтҳо
* навсозии маълумот
* ҳазфи маълумот
* хатогиҳо ҳангоми кор бо базаи додаҳо

Логҳо бояд **маълумоти фаҳмо** дошта бошанд, масалан:

* сохтани ширкат
* сохтани брон
* тағйир додани статуси заказ
* хатогиҳо ҳангоми иҷрои SQL-запрос

---

## Dependency Injection

Ҳамаи сервисҳо бояд тавассути **Dependency Injection** сабт (register) шаванд ва тавассути **интерфейсҳо** истифода гарданд.

Мисол:

```
ICompanyService
CompanyService
```

```
IBookingService
BookingService
```

Контроллерҳо бояд сервисҳоро тавассути **конструктор** қабул кунанд.
