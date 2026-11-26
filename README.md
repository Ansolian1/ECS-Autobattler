# ECS Autobattler

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?style=flat&logo=unity)
![Language](https://img.shields.io/badge/C%23-10.0-blue?style=flat&logo=csharp)
![Framework](https://img.shields.io/badge/Architecture-Morpeh_ECS_(DOD)-orange)
![Performance](https://img.shields.io/badge/Memory-0_B_GC_in_Combat-brightgreen)

Вертикальный срез боевой системы автобатлера (стенка на стенку), построенный на **Morpeh ECS**. 
---

## Геймплей

<img width="800" height="450" alt="ECS-ezgif com-video-to-gif-converter" src="https://github.com/user-attachments/assets/dd88deec-2e24-4769-82bf-20615b28c6cc" />

---

## Архитектура

* **Используется Morpeh ECS**

* **Разделение модели и отображения:**
  * Симуляция боя полностью изолирована и не зависит от объектов сцены.
  * ViewSyncSystem синхронизирует состояние ECS с Unity Transforms только для рендера.
  * Спрайты отрисованы под углом 45° и динамическим расчетом глубины, что даёт эффект псевдо-3Д.

* **Физика толпы:**

  * Разведение перекрывающихся тел напрямую через корректировку позиций.

---

## Системы

1. TargetAndMoveSystem - Поиск врага, расчет слота и вектора скорости
2. MovementSystem      - Интеграция позиции + физическое разведение
3. CombatSystem        - Таймеры перезарядки, расчет урона, проверка досягаемости
4. ViewSyncSystem      - Синхронизация Transforms, Z-Sorting, FlipX, Healthbars

# Компоненты

* Position       - Координата сущности в плоскости XZ
* Velocity	     - Текущий направленный вектор перемещения
* Radius         - Радиус физического тела (для коллизий)
* Health	       - Текущее и максимальное здоровье
* AttackStats	   - Базовые урон, дистанция и кулдаун
* AttackCooldown - Динамический таймер до следующего удара
* Target	       - Ссылка на Morpeh-сущность цели
* Team	         - Идентификатор стороны (0 — Люди, 1 — Нежить)
* Dead	         - Маркер гибели для исключения из расчетов
* View           - Ссылка на Unity-представление
