using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field
{
    public FieldType field;
    public EntityType entity;

    public Field(FieldType field, EntityType entity = EntityType.None) {
        this.field = field;
        this.entity = entity;
    }
}

public enum FieldType {
    Wall,
    Floor,
    Target,
}

public enum EntityType {
    Player,
    Box,
    None,
}