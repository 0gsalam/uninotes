# 📘 UniNotes

> Модерно уеб приложение за водене на бележки от университетски студенти.
> Modern web app for university students to take and organize notes.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat-square&logo=dotnet)
![SQLite](https://img.shields.io/badge/SQLite-3-003B57?style=flat-square&logo=sqlite)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)
![Bilingual](https://img.shields.io/badge/i18n-BG%20%7C%20EN-blue?style=flat-square)

---

## ✨ Възможности / Features

- 📝 **Пълно CRUD** за бележки, курсове и тагове
- 🌍 **Двуезичен интерфейс** — Български (по подразбиране) и Английски
- 🔍 **Умно търсене** — в заглавие, съдържание и тагове
  - Case-insensitive (главни/малки букви няма значение)
  - **Транслитерация BG ⇄ Latin** — търси с `belejka` или `бележка`, все тая
- 📌 **Закачане** на важни бележки в началото
- 🏷️ **Етикети** за организиране
- 🎨 **Тъмна тема** — приятна за очи
- 📱 **Mobile-first responsive дизайн** — работи гладко на телефон и компютър
- 🔐 **ASP.NET Identity** — регистрация, вход, изход
- 🗄️ **SQLite база данни** — без нужда от сървър
- 🚀 **.NET 9** — модерна, бърза, кросплатформена

---

## 🖼️ Screenshots

| Dashboard | Notes | Mobile |
|---|---|---|
| ![Dashboard](docs/dashboard.png) | ![Notes](docs/notes.png) | ![Mobile](docs/mobile.png) |

> 💡 Сложи свои screenshot-и в `docs/` папка и обнови имената тук.

---

## 🚀 Бърз старт / Quick Start

### Изисквания / Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) или по-нова версия
- Git (по избор)
- Браузър (Chrome, Firefox, Edge, Safari)

### Инсталация / Installation

```bash
# 1. Клонирай repo-то
git clone https://github.com/ТВОЕТО-USERNAME/uninotes.git
cd uninotes

# 2. Възстанови NuGet пакетите
dotnet restore

# 3. Стартирай приложението
dotnet run