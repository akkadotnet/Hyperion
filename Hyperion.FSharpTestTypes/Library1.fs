namespace Hyperion.FSharpTestTypes

#if AKKA
open Akka.Actor
#endif

type DU1 = 
| A of int
| B of string * int
and DU2 =
| C of DU1
| D of string
| E of option<DU1>

[<Struct>]
type SDU1 =
| A of a:int
| B of bString:string * bInt:int

type HubType =
    | Task of unit 
    | Chat of unit 

#if AKKA
type Connection =
  { username : string
    id : string
    hubType : HubType
    signalrAref : IActorRef }
#endif

[<CustomEquality;CustomComparison>]
type TestRecord = 
  { name : string
    aref : string option
    connections : string}

    override x.Equals(yobj) = 
        match yobj with
        | :? TestRecord as y -> (x.name = y.name)
        | _ -> false

    override x.GetHashCode() = hash x.name
    interface System.IComparable with
        member x.CompareTo yobj =
            match yobj with
            | :? TestRecord as y -> compare x.name y.name
            | _ -> invalidArg "yobj" "cannot compare values of different types"
            
[<Struct;CustomEquality;CustomComparison>]
type TestStructRecord = 
  { name : string
    aref : string option
    connections : string}
    override x.Equals(yobj) = 
        match yobj with
        | :? TestStructRecord as y -> (x.name = y.name)
        | _ -> false

    override x.GetHashCode() = hash x.name
    interface System.IComparable with
        member x.CompareTo yobj =
            match yobj with
            | :? TestStructRecord as y -> compare x.name y.name
            | _ -> invalidArg "yobj" "cannot compare values of different types"

module TestQuotations = 
    let Quotation = <@ fun (x:int) -> 
        let rec fib n = 
            match n with
            | 0 | 1 -> 1
            | _ -> fib(n-1) + fib(n-2)
        async { return fib x } @>
    

module TestMap =
    type RecordWithString = {Name:string}
    type RecordWithMap = {SomeMap: Map<int,string>}
    let createRecordWithMap = {SomeMap = Map.ofSeq [ (1, "one"); (2, "two") ] }
    

