// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open System
open Fake
open Fake.NpmHelper

let yarn = ProcessHelper.tryFindFileOnPath "yarn"
           |> Option.get // make sure there's npm yarn is installed
           
// Directories
let buildDir  = "./build/"

// Filesets
let projects  =
      !! "src/fableconfig.json"

// Artifact packages
let packages  =
      !! "src/package.json"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "Build" (fun _ ->
    projects
    |> Seq.iter (fun s -> 
                    let dir = IO.Path.GetDirectoryName s
                    printf "Building: %s\n" dir
                    Npm (fun p ->
                        { p with
                            NpmFilePath = yarn
                            Command = Run "build"
                            WorkingDirectory = dir
                        }))
)

Target "Publish" (fun _ ->
    Npm (fun p ->
            { p with
                Command = Custom "publish"
                WorkingDirectory = "./src"
            })
)

// Build order
"Clean"
  ==> "Build"
  
// start build
RunTargetOrDefault "Build"
