using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lib.types;
using lib.delegates;
using lib.service;
using lib.interfaces;
using lib.dot.iFaces;
using lib.dot.abstracts;

using System.Linq.Expressions;

namespace lib.dot.abstracts
{

  



}



/*
Общие под сущности:

    Информация
    Родственные объекты

    Установка значений

    Сравнение
    Отбор

    Свободное место
    Ограничения
    Движение


    Возможные классы:
        1. Базовый класс точки:
            - общая информация о точке
            - информация о родственниках

            - основные переменные
            - установка  значений вне ограничений

        2. Класс ограничений:
            - Степень свободы точки
            - Информация об ограничениях
            - Манипулирование ограничениями (деление на типы: ранее даты, позднее даты, точно в дату)
            - Сравнение и отбор ограничений
            - Возможно информация об объектах ограничителях
            - Возможно информация об объектах мешающих достичь исходной точки

        3. Класс перемещения:
            - Информация о возможности перемещения (свободное место, макс и мин даты)
            - Информация о том куда стремится вернутся точка (направление и исходная дата)
            - Манипуляции точкой стремления
            - Возможность движения в ту или иную сторону
            - Методы движения

        4. Класс сравнения:
            - Сравнение по датам (раньше, позже)
            - Сравнение по расстоянию (ближе к, дальше от)
            - Сравнение по ограничениям
            - Сравнение свободного пространства
            - Нахождение в/вне диапазона
            - Расстояние до точки
            - Направление движения до точки


    void getParent(); //???

    void getType();
    void getState();
    void getDate();
    void setDate();
    void getDateSource();
    void removePoint();

    void compareWhoLater(); //getMinimum
    void compareWhoEarlier(); //getMaximum
    void compareWhoCloseTo();
    void compareWhoFartherTo();
    void isEarlier();
    void getPeriodSideLocation();//С какой стороны от данной точки находится другая точка
    void getDistanceTo();
    void getCloserPoint(); //выдать ближайшую точку из группы заданных

    void getWantDirection(); //куда стремиться вернуться ограниченная точка
    void getStopLeft();
    void getStopRight();
    void setStopLeft();
    void setStopRight();
    void getFixed();
    void setFixed();
    void setFixLeft();
    void setFixRight();

    void getFreedomRate();
    void canMove();
    void canMoveLeft();
    void canMoveRight();
    void moveLeft();
    void moveRight();
    void getSpaceLeft();
    void getSpaceRight();

    void getParentReference();
    void getSecondPointReference();
    */