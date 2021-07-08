### 0.11.0 July 8 2021 ####
* [Fix array of user defined structs serialization failure](https://github.com/akkadotnet/Hyperion/pull/235)
* [Remove dynamic keyword usage from array serializer](https://github.com/akkadotnet/Hyperion/pull/139)
* [Change field ordering to ordinal](https://github.com/akkadotnet/Hyperion/pull/236)

#### Possible breaking changes
The change to the object serializer field ordering might cause a deserialization failure of persisted objects
that are serialized using the Hyperion serializer.

Please report any serialization problem that occurs after an upgrade to this version at the
[issue tracker](https://github.com/akkadotnet/Hyperion/issues)

### 0.10.2 June 30 2021 ####
* [Update Akka version to 1.4.21](https://github.com/akkadotnet/akka.net/releases/tag/1.4.21)
* [Add exception rethrow to help with debugging](https://github.com/akkadotnet/Hyperion/pull/229)

### 0.10.1 April 20 2021 ####
* [Fix SerializerOptions constructor backward compatibility issue with Akka.NET](https://github.com/akkadotnet/Hyperion/pull/214)
