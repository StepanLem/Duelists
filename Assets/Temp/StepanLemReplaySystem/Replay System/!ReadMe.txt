Обо мне: Система записи и воспроизведения игровых данных.

Оценка качества
{
Архитектура: Достаточно атомарная архитектура для записи сцен с большим количеством элементов.
Но на данном этапе проработки может помешать человеческий фактор во время работы с DataID.

Производительность: Работоспособно. За подробностями и возможностью улучшения смотреть: "Оптимизация производительности" в "TODO".
}

Зависимости:
Utilities => Tickers
Utilities => ListExtensions
Packages => Unity-Interface-Support https://github.com/TheDudeFromCI/Unity-Interface-Support  

Настройка в проекте:
1.Создаётся ReplayController
2.Создаются RecordingSystem и PlaybackingSystem и пробрасываются в ReplayController
3.На сцене у объекта(Пример:игрока) создаётся RecordableEntity(и/или PlaybackableEntity если нужно проигрывание(note: проигрывание может быть у другого объекта)).
4.Выставляется TargetDataID, по которому записывается объект.
(таким образом при одинаковом ID PlaybackableEntity и RecordableEntity берут и записывают данные из одной и той же ячейки)
5.Для каждого желаемого записанного значения у цели(Пример:игрока) создаётся RecorderComponent. 
и каждый RecorderComponent имеет ссылку на свою FactoryOfRecorderWithDataGetter(или сеттером, если это PlaybackerComponent)
(То есть если хотим записать PositionAndRotation, то в RecorderComponent._factoryOfRecorderWithGetter прокидываем PositionAndRotationProvidingFactory).
Все классы фабрик для каждого из значений можно создать по аналогии PositionAndRotationProvidingFactory.
6.В конце этот RecorderComponent добавляется в RecordableEntity._recorders

Работа:
У ReplayController надо вызвать метод StartRecording() =>
Надо изменить отслеживаемые значения у RecordableEntity => 
ReplayController.StopRecording() =>
ReplayController.StartPlaybacking() =>
Наблюдать за повторением изменения записанных значений PlaybackableEntity



TODO
{
Проверки на null. Пояснение: Если RecordableSystem или RecordableEntity или RecorderComponent получает null в StartRecord() - логика этого не обработана.

Оптимизация производительности:
{==
2.1 Сделать систему instanceID на словарях для экономии памяти.
2.2 Остлеживать когда значения меняются, а когда нет. И в промежутки бездействия не записывать данные.
2.3 Прекращать запись при Destroy(отгрузке или удалении) элемента
==}

Сделать возможность добавлять recordable/playbackable элементы даже после начала записи/проигрывания.(для того чтобы подгруженные во время записи объекты тоже проигрывались)

Автоподтягивание recordable/playbackable components к их Entity при добавлении их в иерархии(OnReset())

Пробрасывать RecodableTarget в RecordingSystem через инжект автоматически при появлении объекта.

Автоматическая выдача DataID, чтобы снизить человеческий фактор.(или обновляемый в ручную Enum для этого)

Добавить возможность BreakPlaybacking. Крч выйти из повтора преждевременно.

С помощью наследования и полиморфизма уменьшить количество ctrl+c, ctrl+v.

PlaybackableValue.SetValueWithEnterpolation();

Когда-нибудь дойдём до ошибки переполнения стека или листа. Это надо отслеживать.

Подсвечивние красным незаполненных полей в инспекторе.

Сериализация и десерилизация записей.
}
