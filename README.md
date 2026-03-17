# 🎮 game-inventory-api (C# / ASP.NET Core)

C# ASP.NET Core 기반의 **게임 인벤토리 백엔드 포트폴리오** 프로젝트입니다.  
구매/판매/사용 흐름과 거래 로그를 중심으로, 게임 서비스에서 자주 나오는 핵심 로직을 간단하게 구현했습니다.

---

## ✨ Overview
- 인벤토리 조회
- 아이템 구매 (`BUY`)
- 아이템 사용 (`USE`)
- 아이템 판매 (`SELL`)
- 거래 이력 조회 (`Transaction Logs`)
- JWT 로그인/인증 기반 보호 API
- Service 계층 분리(Controller-Services-Data)

---

## 🧰 Tech Stack
![C#](https://img.shields.io/badge/C%23-512BD4?style=flat-square&logo=csharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity%20Framework%20Core-8.x-6E4C13?style=flat-square)
![SQLite](https://img.shields.io/badge/SQLite-07405E?style=flat-square&logo=sqlite&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=flat-square&logo=swagger&logoColor=black)

---

## 🧭 API Endpoints

### Auth
- `POST /api/Auth/login` : 닉네임 기반 토큰 발급(JWT)

### Inventory
- `GET /api/Inventory/{userId}` : 유저 인벤토리 조회
- `POST /api/Inventory/purchase` : 아이템 구매 (JWT 필요)
- `POST /api/Inventory/use` : 아이템 사용 (JWT 필요)
- `POST /api/Inventory/sell` : 아이템 판매 (JWT 필요)

### Items
- `GET /api/Items` : 상점 아이템 목록 조회

### Transactions
- `GET /api/Transactions/{userId}?take=50` : 거래 로그 최신순 조회

---

## 📌 Sample Request
### 0) Login (JWT 발급)
```json
POST /api/Auth/login
{
  "nickname": "Zero"
}
```

### 1) Purchase (아이템 구매)
- 의미: 유저가 상점에서 아이템을 구매합니다.
- 결과: 유저 골드가 차감되고 인벤토리 수량이 증가합니다.

```json
POST /api/Inventory/purchase
{
  "userId": 1,
  "itemId": 2,
  "quantity": 1
}
```

### 2) Sell (아이템 판매)
- 의미: 유저가 보유한 아이템을 판매합니다.
- 결과: 인벤토리 수량이 감소하고, 아이템 가격의 50%만큼 골드가 증가합니다.

```json
POST /api/Inventory/sell
{
  "userId": 1,
  "itemId": 1,
  "quantity": 1
}
```

## 📌 Sample Response
### Purchase Response
- 구매 성공 시 현재 보유 골드를 함께 반환합니다.
```json
{
  "message": "Purchase completed",
  "gold": 500
}
```

### Sell Response
- 판매 성공 시 획득 골드(`earnedGold`)와 최종 골드를 반환합니다.
```json
{
  "message": "Item sold",
  "earnedGold": 50,
  "gold": 550
}
```

### Inventory Response
- 인벤토리 조회 시 유저 정보 + 보유 아이템 목록을 한 번에 확인할 수 있습니다.
```json
{
  "id": 1,
  "nickname": "Zero",
  "gold": 550,
  "items": [
    { "itemId": 1, "name": "Potion", "quantity": 2 },
    { "itemId": 2, "name": "Sword", "quantity": 1 }
  ]
}
```

---

## 🧩 주요 코드 (핵심 로직)
### 1) Gold 차감/증가 로직
- 구매 시 골드 차감, 판매 시 골드 증가를 처리하는 핵심 부분입니다.
```csharp
// purchase
var totalPrice = item.Price * request.Quantity;
if (user.Gold < totalPrice) return BadRequest("Not enough gold");
user.Gold -= totalPrice;

// sell
var sellPrice = (inv.Item.Price / 2) * request.Quantity;
inv.User.Gold += sellPrice;
```

### 2) 거래 로그 저장
- 모든 경제 행동(BUY/USE/SELL)을 로그로 남겨 추적 가능하게 합니다.
```csharp
db.TransactionLogs.Add(new TransactionLog
{
    UserId = request.UserId,
    Type = "BUY", // USE, SELL
    ItemId = request.ItemId,
    Quantity = request.Quantity,
    GoldDelta = -totalPrice,
    CreatedAt = DateTime.UtcNow
});
```

---

## 🧱 ERD (텍스트)
```text
User (1) ───< InventoryItem >─── (1) Item
  │
  └───< TransactionLog
```

- `User`: 유저 기본 정보, 골드
- `Item`: 아이템 가격/정보
- `InventoryItem`: 유저-아이템 보유 수량
- `TransactionLog`: BUY/USE/SELL 이력

## 🔄 API Flow (요약)
1. `POST /api/Auth/login` 으로 JWT 발급
2. 구매/사용/판매 요청 시 `Authorization: Bearer {token}` 전달
3. Controller → Service → DbContext 순서로 처리
4. 성공 시 인벤토리/골드 반영 + 거래로그 저장

---

## ⚙️ Run
```bash
dotnet restore
dotnet run --project .\GameInventoryApi\GameInventoryApi.csproj
```

실행 후 Swagger:
- `https://localhost:{port}/swagger`

---

## 🗂️ Project Structure
```text
game-inventory-api/
├─ GameInventoryApi/
│  ├─ Controllers/
│  ├─ Contracts/
│  ├─ Data/
│  ├─ Models/
│  ├─ Services/
│  ├─ Program.cs
│  └─ appsettings.json
├─ GameInventoryApi.Tests/
├─ game-inventory-api.sln
└─ README.md
```

---

## ✅ Hardening & Refactor Notes
- 입력 검증(DataAnnotation) 적용
- 트랜잭션 처리로 데이터 일관성 보강
- 전역 예외 처리(ProblemDetails) 적용
- Controller → Service 계층 분리
- xUnit 테스트 프로젝트 추가

## 🚀 Next Step (공부중)
- JWT Role(Admin/User) 권한 분기
- 통합 테스트(TestServer) 추가
- Redis 랭킹(리더보드) 기능 추가
