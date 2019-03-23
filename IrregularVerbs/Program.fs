open System
open System.IO
    
type Command =
    | Delete

type Input =
    | Letter of Char
    | Command of Command


let tableBackgroundColor = ConsoleColor.White
let tableForegroundColor = ConsoleColor.DarkBlue
let searchBackgroundColor = ConsoleColor.White
let searchForegroundColor = ConsoleColor.Black
let highlightedBackgroundColor = ConsoleColor.Yellow
let highlightedForegroundColor = ConsoleColor.Black

let setTableMode() =
    Console.BackgroundColor <- tableBackgroundColor
    Console.ForegroundColor <- tableForegroundColor

let setSearchMode() =
    Console.BackgroundColor <- searchBackgroundColor
    Console.ForegroundColor <- searchForegroundColor

let setHighlightedMode() =
    Console.BackgroundColor <- searchBackgroundColor
    Console.ForegroundColor <- searchForegroundColor
    
let clearWindow() =
    setTableMode()
    Console.Clear()
    
let print (search : string) (parts : string array) =
    parts
    |> Array.iteri(fun i part ->
        Console.Write(part)
        if i < parts.Length - 1 then
            setHighlightedMode()
            Console.Write(search)
            setTableMode())
    Console.WriteLine()
        
[<EntryPoint>]
let main argv =

    let verbs = 
        File.ReadAllLines "IrregularVerbs.txt"
    
    clearWindow()

    Seq.initInfinite(fun i -> Console.ReadKey())

    |> Seq.takeWhile(fun k -> k.Key <> ConsoleKey.Escape)

    |> Seq.choose (function
        | k when Char.IsLetter k.KeyChar -> Some (Letter k.KeyChar)
        | k when k.Key = ConsoleKey.Backspace -> Some (Command Delete)
        | _ -> None)

    |> Seq.scan(fun (search:string) (input:Input) ->
        match input with
        | Letter l -> search + string(l)
        | Command Delete ->
            if search.Length > 0 then
                search.Substring(0, search.Length - 1)
            else search) ""

    |> Seq.iter(fun search ->
        clearWindow()
        
        setSearchMode()
        Console.WriteLine(search)
        setTableMode()

        verbs
        |> Array.map(fun text -> text.Split([|search|], StringSplitOptions.None))
        |> Array.filter(fun parts -> parts.Length > 1)
        |> Array.iter (print search)
        
        Console.SetCursorPosition(search.Length, 0))

    0