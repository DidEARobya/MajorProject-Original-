using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

[JsonConverter(typeof(StringEnumConverter))]
public enum BuildingType
{
    [EnumMember(Value = "WALL")]
    WALL,
    [EnumMember(Value = "DOOR")]
    DOOR,
    [EnumMember(Value = "FLOOR")]
    FLOOR,
    [EnumMember(Value = "BED")]
    BED,
    [EnumMember(Value = "TABLE")]
    TABLE
}
[JsonConverter(typeof(StringEnumConverter))]
public enum OreType
{
    [EnumMember(Value = "STONE_ORE")]
    STONE_ORE,
    [EnumMember(Value = "IRON_ORE")]
    IRON_ORE
}
[JsonConverter(typeof(StringEnumConverter))]
public enum PlantType
{
    NONE,
    [EnumMember(Value = "OAK_TREE")]
    OAK_TREE
}
[JsonConverter(typeof(StringEnumConverter))]
public enum ItemType
{
    [EnumMember(Value = "WOOD")]
    WOOD,
    [EnumMember(Value = "STONE")]
    STONE,
    [EnumMember(Value = "IRON")]
    IRON
}