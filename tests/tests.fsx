#r "../packages/NUnit/lib/dotnet/nunit.framework.dll"
#r "node_modules/fable-core/Fable.Core.dll"
#load "node_modules/fable-aether/Aether.fs"

[<AutoOpen>]
module Testing =
    type Assert = Fable.Core.Testing.Assert
    type TestAttribute = Fable.Core.Testing.TestAttribute
    type TestFixtureAttribute = Fable.Core.Testing.TestFixtureAttribute
    
    let (=!) x y = Assert.AreEqual(x,y)

open System
open Aether
open Aether.Operators

[<AutoOpen>]
module Data =
    let chars : Isomorphism<string, char[]> =
        (fun x -> x.ToCharArray ()), (fun x -> String (x))

    let rev : Isomorphism<char[], char[]> =
        Array.rev, Array.rev

[<TestFixture>]
module ``Basic Lens functions`` =
    [<Test>]
    let ``Optic.get returns correct values`` () =
        Optic.get fst_ ("Good","Bad") =! "Good"

    [<Test>]
    let ``Optic.set sets value correctly`` () =
        Optic.set fst_ "Good" ("Bad",()) =! ("Good",())

    [<Test>]
    let ``Optic.map modifies values correctly`` () =
        Optic.map fst_ (fun x -> x + x) ("Good",()) =! ("GoodGood",())

[<TestFixture>]
module ``Basic Prism functions`` =
    [<Test>]
    let ``Optic.get returns correct values for existing values`` () =
        Optic.get Choice.choice1Of2_ (Choice1Of2 "Good") =! Some "Good"

    [<Test>]
    let ``Optic.get returns correct value for missing values`` () =
        Optic.get Choice.choice2Of2_ (Choice1Of2 "Bad") =! None

    [<Test>]
    let ``Optic.set returns correct values for existing values`` () =
        Optic.set Choice.choice1Of2_ "Good" (Choice1Of2 "Bad") =! Choice1Of2 "Good"

    [<Test>]
    let ``Optic.set returns correct value for missing values`` () =
        Optic.set Choice.choice2Of2_ "Bad" (Choice1Of2 "Good") =! Choice1Of2 "Good"

    [<Test>]
    let ``Optic.map modifies values correctly for existing values`` () =
        Optic.map Choice.choice1Of2_ (fun x -> x + x) (Choice1Of2 "Good") =! Choice1Of2 "GoodGood"

    [<Test>]
    let ``Optic.map modifies values correctly for missing values`` () =
        Optic.map Choice.choice2Of2_ (fun x -> x + x) (Choice1Of2 "Good") =! Choice1Of2 "Good"

[<TestFixture>]
module ``Isomorphism composition`` =
    module ``over a Lens`` =
        [<Test>]
        let ``gets value`` () =
            Optic.get (fst_ >-> chars) ("Good",()) =! [| 'G'; 'o'; 'o'; 'd' |]

        [<Test>]
        let ``sets value`` () =
            Optic.set (fst_ >-> chars) [| 'G'; 'o'; 'o'; 'd' |] ("Bad",()) =! ("Good",())

        [<Test>]
        let ``gets value over multiple isomorphisms`` () =
            Optic.get (fst_ >-> chars >-> rev) ("dooG",()) =! [| 'G'; 'o'; 'o'; 'd' |]

        [<Test>]
        let ``sets value over multiple isomorphisms`` () =
            Optic.set (fst_ >-> chars >-> rev) [| 'd'; 'o'; 'o'; 'G' |] ("Bad",()) =! ("Good",())

    module ``over a Prism`` =
        [<Test>]
        let ``gets value when inner exists`` () =
            Optic.get (Choice.choice1Of2_ >?> chars) (Choice1Of2 "Good") =! Some [| 'G'; 'o'; 'o'; 'd' |]

        [<Test>]
        let ``gets nothing when inner does not exist`` () =
            Optic.get (Choice.choice2Of2_ >?> chars) (Choice1Of2 "Bad") =! None

        [<Test>]
        let ``sets value when inner exists`` () =
            Optic.set (Choice.choice1Of2_ >?> chars) [| 'G'; 'o'; 'o'; 'd' |] (Choice1Of2 "Bad") =! Choice1Of2 "Good"

        [<Test>]
        let ``sets nothing when inner does not exist`` () =
            Optic.set (Choice.choice2Of2_ >?> chars) [| 'B'; 'a'; 'd' |] (Choice1Of2 "Good") =! Choice1Of2 "Good"

        [<Test>]
        let ``gets value when inner exists over multiple isomorphisms`` () =
            Optic.get (Choice.choice1Of2_ >?> chars >?> rev) (Choice1Of2 "dooG") =! Some [| 'G'; 'o'; 'o'; 'd' |]

        [<Test>]
        let ``gets nothing when inner does not exist over multiple isomorphisms`` () =
            Optic.get (Choice.choice2Of2_ >?> chars >?> rev) (Choice1Of2 "daB") =! None

        [<Test>]
        let ``sets value when inner exists over multiple isomorphisms`` () =
            Optic.set (Choice.choice1Of2_ >?> chars >?> rev) [| 'd'; 'o'; 'o'; 'G' |] (Choice1Of2 "Bad") =! Choice1Of2 "Good"

        [<Test>]
        let ``sets nothing when inner does not exist over multiple isomorphisms`` () =
            Optic.set (Choice.choice2Of2_ >?> chars >?> rev) [| 'd'; 'a'; 'B' |] (Choice1Of2 "Good") =! Choice1Of2 "Good"
