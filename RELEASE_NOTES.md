### 0.10.0 April 13 2021 ####
* [Add a generic cross platform serialization support](https://github.com/akkadotnet/Hyperion/pull/208)

## Cross platform serialization

You can now address any cross platform package serialization differences by providing a list of package name transformation lambda function into the `SerializerOptions` constructor. The package name will be passed into the lambda function before it is deserialized, and the result of the string transformation is used for deserialization instead of the original package name.

This short example shows how to address the change from `System.Drawing` in .NET Framework to `System.Drawing.Primitives` in .NET Core:

```
Serializer serializer;
#if NETFX
serializer = new Serializer(new SerializerOptions(
    packageNameOverrides: new List<Func<string, string>> {
        str => str.Contains("System.Drawing.Primitives") ? str.Replace(".Primitives", "") : str
    }));
#elif NETCOREAPP
serializer = new Serializer();
#endif
```

Note that only one package name transformation is allowed, any transform lambda function after the first applied transformation is ignored.