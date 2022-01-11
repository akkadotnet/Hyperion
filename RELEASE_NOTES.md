### 0.11.3 January 12 2022 ####

* Add deserialization whitelisting feature [#281](https://github.com/akkadotnet/Hyperion/pull/281)

We've expanded our deserialization safety check to block dangerous types from being deserialized. You can now create a custom deserialize layer type filter programmatically: 

```c#
var typeFilter = TypeFilterBuilder.Create()
    .Include<AllowedClassA>()
    .Include<AllowedClassB>()
    .Build();
var options = SerializerOptions.Default
    .WithTypeFilter(typeFilter);
var serializer = new Serializer(options);
```

For complete documentation, please read the [readme on whitelisting types.](https://github.com/akkadotnet/Hyperion#whitelisting-types-on-deserialization)

### 0.11.2 October 7 2021 ####
* Fix exception thrown during deserialization when preserve object reference was turned on 
  and a surrogate instance was inserted into a collection multiple times. [#264](https://github.com/akkadotnet/Hyperion/pull/264)
* Add support for AggregateException serialization. [#266](https://github.com/akkadotnet/Hyperion/pull/266)

### 0.11.1 August 17 2021 ####
* Add [unsafe deserialization type blacklist](https://github.com/akkadotnet/Hyperion/pull/242)
* Bump [Akka version from 1.4.21 to 1.4.23](https://github.com/akkadotnet/Hyperion/pull/246)

We've added a deserialization safety check to block dangerous types from being deserialized. 
This is done to add a layer of security from possible code injection and code execution attack.
Currently it is an all or nothing feature that can be turned on and off by using the new `DisallowUnsafeTypes` flag inside `SerializerOptions` (defaults to true).

The unsafe types that are currently blocked are:
- System.Security.Claims.ClaimsIdentity
- System.Windows.Forms.AxHost.State
- System.Windows.Data.ObjectDataProvider
- System.Management.Automation.PSObject
- System.Web.Security.RolePrincipal
- System.IdentityModel.Tokens.SessionSecurityToken
- SessionViewStateHistoryItem
- TextFormattingRunProperties
- ToolboxItemContainer
- System.Security.Principal.WindowsClaimsIdentity
- System.Security.Principal.WindowsIdentity
- System.Security.Principal.WindowsPrincipal
- System.CodeDom.Compiler.TempFileCollection
- System.IO.FileSystemInfo
- System.Activities.Presentation.WorkflowDesigner
- System.Windows.ResourceDictionary
- System.Windows.Forms.BindingSource
- Microsoft.Exchange.Management.SystemManager.WinForms.ExchangeSettingsProvider
- System.Diagnostics.Process
- System.Management.IWbemClassObjectFreeThreaded

### 0.11.0 July 8 2021 ####
* [Fix array of user defined structs serialization failure](https://github.com/akkadotnet/Hyperion/pull/235)
* [Remove dynamic keyword usage from array serializer](https://github.com/akkadotnet/Hyperion/pull/139)
* [Change field ordering to ordinal](https://github.com/akkadotnet/Hyperion/pull/236)

**Possible breaking changes**

The change to the object serializer field ordering might cause a deserialization failure of persisted objects
that are serialized using the Hyperion serializer.

Please report any serialization problem that occurs after an upgrade to this version at the
[issue tracker](https://github.com/akkadotnet/Hyperion/issues)

### 0.10.2 June 30 2021 ####
* [Update Akka version to 1.4.21](https://github.com/akkadotnet/akka.net/releases/tag/1.4.21)
* [Add exception rethrow to help with debugging](https://github.com/akkadotnet/Hyperion/pull/229)

### 0.10.1 April 20 2021 ####
* [Fix SerializerOptions constructor backward compatibility issue with Akka.NET](https://github.com/akkadotnet/Hyperion/pull/214)
