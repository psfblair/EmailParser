﻿module EmailParser.HtmlGenerator

open System
open EmailParser.Types

type HtmlAccumulator = {
    Html: string;
    Date: DateTime
}

let addHeader calendarEntry accumulator =    
    let currentEntryDate = calendarEntry.EventDate

    if currentEntryDate.ToString("D") = accumulator.Date.ToString("D") then accumulator.Html
    else accumulator.Html + "<h3>" + currentEntryDate.ToString("D") + "</h3>\n\n"

let addEntryTime calendarEntry htmlSoFar = 
    htmlSoFar + "<h4>" + calendarEntry.EventDate.ToString("t").ToLower() + " - "

let addEntryTitle calendarEntry htmlSoFar = 
    htmlSoFar + calendarEntry.EventTitle + "</h4>\n\n"

let addEntryDescription calendarEntry htmlSoFar =
    htmlSoFar + "<p>" + calendarEntry.EventDescription + "</p>\n"

let addEntryLocation calendarEntry htmlSoFar =
    match calendarEntry.EventLocation with
        | Some(location) -> htmlSoFar + "<p>" + location + "</p>\n"
        | None -> htmlSoFar

let addEntryRsvp calendarEntry htmlSoFar =
    match calendarEntry.RsvpLink with
        | Some(uri) -> htmlSoFar + "<p>RSVP: <a href=" + uri.AbsoluteUri + ">" + uri.AbsoluteUri + "</a></p>\n"
        | None -> htmlSoFar

let htmlEntryFor (accumulator: HtmlAccumulator) (calendarEntry: CalendarEntry) = 
    let entryHtml = accumulator |> (addHeader calendarEntry)
                                |> (addEntryTime calendarEntry)
                                |> (addEntryTitle calendarEntry)
                                |> (addEntryDescription calendarEntry)
                                |> (addEntryLocation calendarEntry)
                                |> (addEntryRsvp calendarEntry)

    { Html = entryHtml; Date = calendarEntry.EventDate }

let rec toHtml accumulator calendarEntries =
    match calendarEntries with
        | calendarEntry :: remainingEntries -> toHtml (htmlEntryFor accumulator calendarEntry) remainingEntries    
        | [] -> { Html = accumulator.Html + "\n</body>\n</html>" ; Date = accumulator.Date }

let htmlFrom calendarEntries = 
    let initialState = { Html = "<html>\n<body>\n" ; Date = DateTime.Today.AddDays(-1.0) }
    let accumulated = toHtml initialState calendarEntries
    accumulated.Html
