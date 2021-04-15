# SimpleJson

[![license](https://img.shields.io/github/license/Mzying2001/SimpleJson)](https://raw.githubusercontent.com/Mzying2001/SimpleJson/master/LICENSE)
[![nuget-version](https://img.shields.io/nuget/v/Mzying2001.SimpleJson)](https://www.nuget.org/packages/Mzying2001.SimpleJson)
[![nuget-downloads](https://img.shields.io/nuget/dt/Mzying2001.SimpleJson)](https://www.nuget.org/packages/Mzying2001.SimpleJson)

## Get Start

With SimpleJson, you can read/write json in a very simple way. This class library contains only four classes: JObject, JsonConvert, JsonReader and JsonWriter.

The JObject class represents a json object, The data types of json and C# correspond to the following table.

|String   |Number   |Boolean  |Array     |Json     |Null     |
|:-------:|:-------:|:-------:|:--------:|:-------:|:-------:|
|`string` |`double` |`bool`   |`object[]`|`JObject`|`null`   |

Of course, you can also add `int` / `byte` / `float` / `StringBuilder` etc. to `JObject` instances, which will be converted to the corresponding data type by default when saving Json data. All classes that implement the IList interface are considered arrays, and when reading Json files, all arrays are recognized as `object[]` types by default.

## JObject

The JObject class represents the json object, you can read/write the json file through the JObject class, you can also convert the json string to JObject through the `Parse` method.

```C#
var json = new JObject(); //Create an empty json object
```

If you want to initialize the content when creating a JObject, you can use the following method, which looks just like writing json in C# code.

```C#
var json = new JObject()
{
    ["sample_str"] = "this is a string",
    ["sample_int"] = 12345,
    ["sample_num"] = 0.888,

    ["sample_arr"] = new object[]
    {
        "hello",
        123
    },

    ["sample_obj"] = new JObject()
    {
        ["p1"] = "test",
        ["p2"] = 12345
    }
};
```

It represents the following json.

```Json
{
    "sample_str": "this is a string",
    "sample_int": 12345,
    "sample_num": 0.888,
    "sample_arr": [
        "hello",
        123
    ],
    "sample_obj": {
        "p1": "test",
        "p2": 12345
    }
}
```

---

Read values via indexer.

```C#
var sample_str = json["sample_str"]; //type of sample_str is object
```

The value obtained through the indexer is of type object. If you want to get the value of a certain type, use the `GetValue` method.

```C#
var sample_str = json.GetValue<string>("sample_str"); //sample_str is a string
```

If there are multiple JObjects nested, you can use an array of strings to directly index.

```C#
var obj = json["prop", "prop", "prop"];
```

If you want to read an array, use the `GetArray` method, if you make sure there is only one type of data in the array, you can get the array of that type directly, otherwise use `GetArray<object>(...)` or `GetValue<object[]>(...)` to get the array.

```C#
var arr = json.GetArray<object>("sample_arr"); //arr is object[]
```

Using the above method you have to make sure that the JObject object has the corresponding key value, if the index does not exist for the key value then an exception will be thrown. If you are not sure if the key value exists, use `TryGetValue` to get the value or use the `TryGetArray` method to get the array, they will return a bool value indicating whether the get was successful or not.

---

Modify the value by the following method.

```C#
json["sample_str"] = "change the value";
```

Using this method you can modify the value corresponding to the key, or if the key does not exist, the key is created and the value is saved.

You can also add values via the `Add` method, which has multiple overloads. You can add them directly, as a tuple, or as a KeyValuePair. You can also add multiple key values at once via the `Add` method, but you have to make sure that the added key value does not exist in the JObject, otherwise an exception will be thrown.

```C#
var json = new JObject();

//Add directly
json.Add("prop1", "test");

//Add truples
json.Add(("prop2", "test"), ("prop3", 1234));

//Add KeyValuePairs
json.Add(new Dictionary<string, object>
{
    {"prop4", true },
    {"prop5", null },
    {"prop6", 123.456 }
});
```

## JsonConvert

This is a static class, which means you cannot create instances of this class. With this class, you can serialize object instances of other classes to get a JObject without having to create one manually; you can also create an object instance by deserializing the JObject.

For example, we now have a class as follows.

```C#
class SampleClass
{
    //All public fields and properties will be serialized
    //Private fields and properties will not be serialized
    public string fieldStr;
    public string PropStr { get; set; }
    public object[] SampleArr { get; set; }
}
```

Then we can create a JObject directly from an instance object of the SampleClass class.

```C#
//Create an instance of SampleClass
var sample = new SampleClass
{
    PropStr = "sample_str",
    fieldStr = "sample_str",
    SampleArr = new object[] { 123, 0.456, "sample" }
};

//Serialize
var json = JsonConvert.Serialize(sample);
Console.WriteLine(json);
```

The output is as follows.

```Json
{
    "fieldStr": "sample_str",
    "PropStr": "sample_str",
    "SampleArr": [
        123,
        0.456,
        "sample"
    ]
}
```

---

Deserialization is also very simple.

```C#
//Deserialize
var sample2 = JsonConvert.Deserialize<SampleClass>(json);

//Output Result
Console.WriteLine(sample2.PropStr);
Console.WriteLine(sample2.fieldStr);
Console.WriteLine(string.Join(", ", sample2.SampleArr));
```

The output is as follows.

```txt
sample_str
sample_str
123, 0.456, sample
```

## JsonReader

A static class for reading Json strings or files, which contains the following methods.

|Method         |Function                                                                                 |
|---------------|-----------------------------------------------------------------------------------------|
|`Read`         |Read a string and convert it to a JObject instance.                                      |
|`ReadFile`     |Read a Json file and convert it to a JObject instance.                                   |
|`ReadArray`    |Convert a string representing a json array to `object[]`.                                |
|`ReadArrayFile`|Read a file representing the json array and convert it to an array of the specified type.|

## JsonWriter

A static class for converting a JObject to a string or saving a Json file, containing following methods.

|Method          |Function                                                         |
|----------------|-----------------------------------------------------------------|
|`Write`         |Convert JObject to Json string.                                  |
|`WriteFile`     |Converts a JObject into a Json string and writes it to a file.   |
|`WriteArray`    |Converts an array to a json array string.                        |
|`WriteArrayFile`|Converts an array to a Json array string and writes it to a file.|
