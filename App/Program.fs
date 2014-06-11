﻿open EmailParser.Utils.Mime
open EmailParser.Utils.Text
open ODonnellParser.Parser
open EmailParser.SQLitePersistence

let usage() =
    printfn "usage: EmailParser MAIL_FILE_1 [MAIL_FILE_2 ...]"
    printfn ""
    printfn "This program parses files containing emails in MIME"
    printfn "multipart format such as .eml files. The mails should"
    printfn "contain Calendar of Events information. The information"
    printfn "parsed from the files is stored in a database and then"
    printfn "an HTML report is constructed containing all the events"
    printfn "in the database from the current day onward."
    printfn ""
    printfn "The mail files passed on the command line must have specific "
    printfn "file extensions corresponding to the sender, which determines"
    printfn "the parser to be used. These extensions are:"
    printfn ""
    printfn "   .odonnell - extension for Charlie O'Donnell's mails"
    printfn ""
    printfn "Output is written into a file named Events.html in the "
    printfn "current working directory."

let loadDataFrom (filename: string) =
    // TODO Need to select which parser to use based on file extension
    let message = loadMimeMessageFrom(filename)
    //TODO Send the message to the parser. Have the parser dig this stuff out.
    let sender = senderOf message
    let sentDate = dateOf message
    let lines = textOf message |> splitIntoLines
    let parsed = parseMail sender sentDate lines
    loadMail parsed
    let eventsList = retrieveCalendarEntriesFromTodayByDate()
    ()

[<EntryPoint>]
let main argv = 
    if argv.Length = 0 then
        usage()
    else
        argv |> Array.iter loadDataFrom
    0
