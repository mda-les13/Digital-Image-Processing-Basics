# Digital-Image-Processing-Basics

## Описание

Проект представляет собой приложение на WPF, предназначенное для загрузки, обработки и сохранения изображений. Программа поддерживает различные методы обработки изображений, включая пороговую обработку и фильтрацию.

## Функциональные возможности

- Загрузка изображений: Приложение позволяет пользователю загружать изображения различных форматов (BMP, JPG, JPEG, PNG, GIF).
- Обработка изображений:
  - Пороговая обработка с использованием метода Оцу.
  - Локальная пороговая обработка (метод Ниблака).
  - Адаптивная пороговая обработка.
  - Фильтрация пониженных частот.
- Сохранение изображений: Обработанные изображения могут быть сохранены в формате PNG, JPG или BMP.
- Очистка изображений: Возможность очистки загруженных и обработанных изображений.

## Установка

Для запуска приложения вам понадобится:

- .NET Framework 4.5 или выше.
- Visual Studio (рекомендуется для разработки и отладки).

### Шаги установки:

1. Клонируйте репозиторий:
      git clone https://github.com/ваш-аккаунт/PKG_Lab3.git
   
2. Откройте решение в Visual Studio.
3. Постройте проект и запустите его.

## Использование

1. Загрузка изображения:
   - Нажмите кнопку "Load Image" (Загрузить изображение) и выберите файл изображения.
   - Загруженное изображение отобразится в окне.

2. Обработка изображения:
   - Выберите один из доступных методов обработки:
     - Local Thresholding 1 (метод Оцу)
     - Local Thresholding 2 (метод Ниблака)
     - Adaptive Threshold (адаптивная пороговая обработка)
     - Low Pass Filter (фильтрация пониженных частот)
   - Обработанное изображение будет отображено рядом с оригиналом.

3. Сохранение изображения:
   - Нажмите кнопку "Save Image" (Сохранить изображение) для сохранения обработанного изображения.

4. Очистка изображений:
   - Нажмите кнопку "Clear Images" (Очистить изображения) для сброса текущих изображений.

## Методы обработки изображений

### OtsuThreshold

Метод пороговой обработки, который автоматически вычисляет оптимальный порог для преобразования изображения в черно-белое.

### NiblackThreshold

Локальный метод пороговой обработки, основанный на максимальных и минимальных значениях яркости в окне вокруг каждого пикселя.

### AdaptiveThreshold

Метод адаптивной пороговой обработки, который использует среднее значение яркости пикселей вокруг текущего пикселя для определения порога.

### LowPassFilter

Фильтрует изображение, уменьшая высокочастотные шумы, что приводит к более гладкому изображению.

## Примеры использования

В этом разделе вы можете добавить примеры изображений до и после обработки, чтобы продемонстрировать эффективность каждого метода.

## Лицензия

Этот проект лицензирован на условиях MIT License.
