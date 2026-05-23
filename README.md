# Satori

**Satori** — 2D-духовная игра на C# и MonoGame (.NET 8): паломничество по лабиринтам, медитация у лотосов, сад, мини-игры и путь к просветлению.

---

## Требования

| Компонент | Версия |
|-----------|--------|
| [.NET SDK](https://dotnet.microsoft.com/download) | **8.0** или новее |
| ОС | **Windows**, **macOS** или **Linux** |
| Git | для клонирования репозитория |

Проверка:

```bash
dotnet --version
```

---

## Быстрый старт

```bash
git clone https://github.com/lovesomemommy/satori.git
cd satori
dotnet run --project src/Satori.Desktop/Satori.Desktop.csproj
```

На **macOS** и **Linux** запускайте только **Satori.Desktop** (MonoGame).  
**Satori.Windows** — WinForms-launcher **только для Windows**; на Mac он не стартует (нет `Microsoft.WindowsDesktop.App`).

**Windows** — дополнительно можно использовать WinForms-launcher с кнопками «Играть» и «Настройки»:

```bash
dotnet run --project src/Satori.Windows/Satori.Windows.csproj
```

Опция «Запускать игру сразу» сохраняется в `%APPDATA%\Satori\launcher.json`. Чтобы снова показать окно launcher: `dotnet run --project src/Satori.Windows/Satori.Windows.csproj -- --launcher`

---

## Сборка

Из корня репозитория:

```bash
dotnet restore
dotnet build Satori.sln
```

Только игра:

```bash
dotnet build src/Satori.Desktop/Satori.Desktop.csproj
```

### Release-сборка

```bash
dotnet publish src/Satori.Desktop/Satori.Desktop.csproj -c Release -o ./publish
```

Запуск:

```bash
# macOS / Linux
./publish/Satori.Desktop

# Windows
publish\Satori.Desktop.exe
```

После сборки можно запускать бинарник напрямую:

- **Windows:** `src\Satori.Desktop\bin\Debug\net8.0\Satori.Desktop.exe`
- **macOS / Linux:** `src/Satori.Desktop/bin/Debug/net8.0/Satori.Desktop`

---

## Тесты

```bash
dotnet test src/Satori.Tests/Satori.Tests.csproj
```

---

## Структура проекта

```
satori/
├── Satori.sln
└── src/
    ├── Satori.Core/       # логика, сохранения, локализация
    ├── Satori.Client/     # графика, сцены, ресурсы
    ├── Satori.Desktop/    # точка входа (macOS / Linux / Windows)
    ├── Satori.Windows/    # WinForms-launcher (только Windows)
    └── Satori.Tests/      # автотесты
```

---

## Управление

| Действие | Клавиша по умолчанию |
|----------|----------------------|
| Движение | **W** **A** **S** **D** |
| Медитация у лотоса | **Пробел** |
| Пауза | **Escape** |
| Взаимодействие | **E** |

Клавиши можно переназначить в **Настройках** (из главного меню или хаба).

В **Колесе дармы** — стрелки **↑ ↓ ← →** или клик по квадратам на экране.

---

## Сохранения

Прогресс сохраняется автоматически в `save_v1.json`:

| ОС | Папка |
|----|-------|
| Windows | `%APPDATA%\Satori\saves\` |
| macOS | `~/Library/Application Support/Satori/saves/` |
| Linux | `~/.local/share/satori/saves/` |

Сброс прогресса — **Настройки** → «Сбросить прогресс».

---

## Ресурсы и музыка

Ресурсы лежат в `src/Satori.Client/` и копируются в папку сборки автоматически.

| Папка | Назначение |
|-------|------------|
| `MenuImages/` | фон главного меню (`background.png`) |
| `HubImages/` | изображение храма в хабе |
| `PilgrimImages/` | лотос, монах, стены, портал, следы (`traces.png`) |
| `GardenImages/` | пол сада |
| `QuoteImages/` | картинки цитат (10 шт.), препятствия (монетка, жук, облако) |
| `LocalTracks/` | фоновая музыка (`.ogg`) |
| `Fonts/` | шрифты интерфейса |

### Добавить свой трек

1. Положите файл `.ogg` в `src/Satori.Client/LocalTracks/`
2. Добавьте запись в `LocalTracks/manifest.json`
3. Пересоберите проект

---

## Игровой процесс (кратко)

1. **Главное меню** → «Начать»
2. **Храм (хаб)** — сад, паломничество, мини-игры, библиотека мудрости
3. **Паломничество** — 5 троп, лотосы, медитация, цитаты
4. **Финал** — пройти паломничество, посадить **20 лотосов** в сад (15 из лабиринта + 5 из колеса), собрать **10 цитат** (5 из лабиринта + 5 из колеса), достичь просветления → **Путь к реке**

---

## Разработка в IDE

| IDE | Действие |
|-----|----------|
| Visual Studio 2022 | Открыть `Satori.sln`, стартовый проект — **Satori.Desktop** |
| JetBrains Rider | Open → `Satori.sln` |
| VS Code | Папка репозитория + расширение C# |

---

## Возможные проблемы

**`dotnet: command not found`**  
Установите [.NET 8 SDK](https://dotnet.microsoft.com/download) и перезапустите терминал.

**Ошибки при `dotnet restore`**  
Проверьте доступ к интернету и повторите: `dotnet restore --force`

**Чёрный экран на Linux**  
Установите зависимости OpenGL/SDL, например на Ubuntu:

```bash
sudo apt install libgl1-mesa-dev libsdl2-dev
```

**Сообщения `[ALSOFT]` в терминале**  
Предупреждения OpenAL при воспроизведении музыки — не критичны, на игру не влияют.

**Нет музыки**  
Убедитесь, что рядом с исполняемым файлом есть папка `LocalTracks/` с `manifest.json` и `.ogg`.

---

## Лицензия

Уточните лицензию в репозитории или добавьте файл `LICENSE`.
