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

---

## 🧰 Tech Stack
![C#](https://img.shields.io/badge/C%23-512BD4?style=flat-square&logo=csharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity%20Framework%20Core-8.x-6E4C13?style=flat-square)
![SQLite](https://img.shields.io/badge/SQLite-07405E?style=flat-square&logo=sqlite&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=flat-square&logo=swagger&logoColor=black)

---

## 🧭 API Endpoints

### Inventory
- `GET /api/Inventory/{userId}` : 유저 인벤토리 조회
- `POST /api/Inventory/purchase` : 아이템 구매
- `POST /api/Inventory/use` : 아이템 사용
- `POST /api/Inventory/sell` : 아이템 판매

### Items
- `GET /api/Items` : 상점 아이템 목록 조회

### Transactions
- `GET /api/Transactions/{userId}?take=50` : 거래 로그 최신순 조회

---

## 📌 Sample Request
```json
POST /api/Inventory/purchase
{
  "userId": 1,
  "itemId": 2,
  "quantity": 1
}
```

```json
POST /api/Inventory/sell
{
  "userId": 1,
  "itemId": 1,
  "quantity": 1
}
```

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
│  ├─ Program.cs
│  └─ appsettings.json
├─ game-inventory-api.sln
└─ README.md
```

---

## 🚀 Next Step (공부중)
- JWT 인증/인가 적용
- 공통 에러 응답 포맷 통일
- 테스트 코드 확장 (xUnit)
- Redis 랭킹(리더보드) 기능 추가
