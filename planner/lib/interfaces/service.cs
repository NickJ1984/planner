﻿
using lib.types;
using lib.service;

namespace lib.interfaces
{
    public interface IidentObject
    {
        void setIndex(int index);
        int getIndex();
        e_ValueType getType();
        string getID();

    }
    public interface IEntity
    {
        entityInfo getEntityInfo();
    }
}
