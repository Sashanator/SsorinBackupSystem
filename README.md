# SsorinBackupSystem (SBS)

## [ENG]
## Overview

The academic project is a tool for managing the backup process. SBS is developed using a graphical user interface developed on the WPF platform.
**Theormin:**
**Backup** - a backup copy of some data, which is made in order to be able to restore this data in the future, that is, to roll back to the moment when it was created.
**Recovery point** - a backup copy of objects created at a certain moment. Represented by the date of creation and a list of backed up objects that were backed up. There are two types of restore points - full and incremental. **Full** points contain all information about objects that have been backed up. **Incremental** points are the difference (delta) relative to the previous point, i.e. we only store changes.

#### Creation and storage algorithms
To create a backup, objects are specified - a list of files. In order to add a file, you need to click the corresponding button with the computer icon to open Windows Explorer, where you can select the path to the file.
The system implements two options for data storage:
- Single algorithm: all objects specified in the backup are added to one archive.
- Separate algorithm: files are copied to a special folder and stored there separately.

#### Point cleaning algorithms
In addition to creation, you need to control the number of stored recovery points. In order to prevent the accumulation of a large number of old and irrelevant points, it is necessary to implement mechanisms for their cleaning - they must control so that the chain of recovery points does not go beyond the acceptable limit.
**Algorithms for cleaning points:**
- By quantity - limits the length of the chain of recovery points (we store the last N points).
- By date - limits how old points will be stored (we store all points that were made no later than the specified date).
- By size - limits the total size of the backup (we store all the last points, the total size of which does not exceed the limit).
- Hybrid - the ability to combine limits. The user can specify how to combine:
- It is necessary to delete a point if it went beyond at least one set limit.
- It is necessary to delete a point if it has gone beyond all the established limits.

## Feautures

- Creating a backup with a single / separate storage algorithm.
- Adding a full / incremental restore point.
- Cleaning up old and irrelevant restore points using various algorithms.

## Technologies

- .NET Core 
- C# 
- LINQ
- IO.Compression
- WPF (Windows Presentation Foundation) – XAML
- Material Design – it is a design system created by Google to help teams build high-quality digital experiences for Android, iOS, Flutter, and the web.

## Usage
After starting the program, you will see a graphical interface. First, you need to select a storage algorithm: single or separate, by selecting the appropriate item in the lower left part of the menu. Next, add the files you need using the button with the computer icon, which will open Windows Explorer to automatically generate the path to the file of your choice. After selecting all the necessary objects for the backup, click the "Create Backup" button at the bottom of the menu. In the lower right part of the menu, you will see how yours appeared in the list of backups. In order to update the backup, add the necessary files, then right-click on the list of files and select "Update backup". Full-fledged points are named with an ordinal identifier. In addition to its own identifier, the name of the incremental point contains the identifier of the "parent" full recovery point, from which the delta was taken, through the letter I. The combined algorithm for cleaning restore points is presented in the upper right part of the menu. IsAny algorithm - deletes all restore points that meet at least one limit, IsAll - deletes all restore points that meet all limits. By clicking on the backup you need in the list of backups in the lower right part of the RMB menu, you can add a full or incremental restore point, as well as perform a separate quick cleanup of restore points using one of three algorithms.

##### Menu:
![Menu](https://github.com/Sashanator/SsorinBackupSystem/blob/main/SsorinBackupSystem/resources/menu.png)

##### Working process:
IMPORTANT: Incremental points must not be left without the point from which the delta is taken. Therefore, if you had to leave more points than planned, then an appropriate warning is displayed (as you can see in the gif below).
![Menu](https://github.com/Sashanator/SsorinBackupSystem/blob/main/SsorinBackupSystem/resources/sample.gif)

## Author
- **Alexander Ssorin** – project developer – [Sashanator](github.com/Sashanator)

## Licence
[MIT](https://choosealicense.com/licenses/mit/)

## [RUS]
## Обзор

Учебный проект представляет собой инструмент для управления процессом создания бэкапов. SBS разработан с использованием графического пользовательского интерфейса, разработанного на платформе WPF. 
**Теормин:**
**Бэкап** – резервная копия каких-то данных, которая делается для того, чтобы в дальнейшем можно было восстановить эти данные, то есть откатиться до того момента, когда она была создана.
**Точка восстановления** – резервная копия объектов, созданная в определённый момент. Представлена датой создания и списком резервных копий объектов, которые бэкапились. Есть два типа точек восстановления - полноценные и инкрементальные. **Полноценные** точки содержат всю информацию про объекты, которые забэкапились. **Инкрементальные** точки - это разница (дельта) относительно предыдущей точки, т.е. мы храним только изменения.

#### Алгоритмы создания и хранения
Для создания бэкапа указываются объекты - список файлов. Для того, чтобы добавить какой-либо файл, нужно нажать соответствующую кнопку со значком компьютера, чтобы открыть проводник Windows, где можно будет выбрать путь к файлу. 
Система реализует два варианта хранения данных:
- Единый алгоритм: все указанные в бэкапе объекты складываются в один архив.
- Раздельный алгоритм: файлы копируются в специальную папку и хранятся там раздельно.

#### Алгоритмы очистки точек
Помимо создания нужно контролировать количество хранимых точек восстановления. Чтобы не допускать накопления большого количества старых и неактуальных точек, требуется реализовать механизмы их очистки – они должны контролировать, чтобы цепочка точек восстановления не выходила за допустимый лимит. 
**Реализованные алгоритмы очистки точек:**
- По количеству – ограничивает длину цепочки точек восстановления (храним последние N точек).
- По дате – ограничивает насколько старые точки будут храниться (храним все точки, которые были сделаны не позднее указанной даты).
- По размеру – ограничивает суммарный размер, занимаемый бэкапом (храним все последние точки, суммарный размер которых не превышает лимит).
- Гибрид – возможность комбинировать лимиты. Пользователь может указывать, как комбинировать:
-- Нужно удалить точку, если вышла хотя бы за один установленный лимит.
-- Нужно удалить точку, если вышла за все установленные лимиты.

## Возможности программы

- Создание бэкапа с единым / раздельным алгоритмом хранения.
- Добавление полноценной / инкрементальной точки восстановления.
- Очистка старых и неактуальных точек восстановления по различным алгоритмам.

## Используемые технологии

- .NET Core 
- C# 
- LINQ
- IO.Compression
- WPF (Windows Presentation Foundation) – XAML
- Material Design – система, разработанная компанией Google для создания высококачественного дизайна приложения для Android, iOS, Flutter, и веб-сайтов.

## Использование
После запуска программы Вы увидите графический интерфейс. Для начала Вам нужно выбрать алгоритм хранения: единый или раздельный, выбрав соответствующий пункт в левой нижней части меню. Далее, добавьте нужные Вам файлы, воспользовавшись кнокпой со значком компьютера, которая откроет проводник Windows для автоматической генерации пути к выбранному Вами файлу. После выбора всех нужных объектов для бэкапа, нажмите кнопку "Создать Backup" в нижней части меню. В правой нижней части меню Вы увидите, как в списке бэкапов появился и Ваш. Для того, чтобы обновить бэкап, добавьте нужные файлы, после чего нажмите ПКМ по списку файлов и выберите "Обновить бэкап". Полноценные точки имеют название с порядковым идентификатором. В названии инкрементальной точки помимо собственного идентификатора через букву I указан идентификатор "родительской" полноценной точки восстановления, от которой была взята дельта. В правой верхней части меню представлен комбинированный алгоритм очистки точек восстановления. Алгоритм IsAny – удаляет все точки восстановления, удовлетворяющие хотя бы одному лимиту, IsAll – удаляет все точки восстановления, удовлетворяющие всем лимитам. Нажав на нужный Вам бэкап в списке бэкапов в правой нижней части меню ПКМ, Вы можете добавить полную или инкрементальную точку восстановления, а также провести отдельную быструю очистку точек восстановления по одному из трёх алгоритмов.

##### Меню:
![Menu](https://github.com/Sashanator/SsorinBackupSystem/blob/main/SsorinBackupSystem/resources/menu.png)

##### Рабочий процесс:
ВАЖНО: Инкрементальные точки не должны остаться без точки, от которой взята дельта. Поэтому, если пришлось оставить точек больше, чем планировалось, то выводится соответствующее предупреждение (как можно видеть на гифке ниже).
![Menu](https://github.com/Sashanator/SsorinBackupSystem/blob/main/SsorinBackupSystem/resources/sample.gif)

## Автор

- **Александр Ссорин** – разработчик проекта – [Sashanator](github.com/Sashanator)

## Лицензия
[MIT](https://choosealicense.com/licenses/mit/)
