open System
open System.Net.Http
open System.Text.Json
open System.Text.Json.Serialization

type Rates =
    { [<property: JsonPropertyName("USD")>]
      USD: float option
      [<property: JsonPropertyName("EUR")>]
      EUR: float option
      [<property: JsonPropertyName("CHF")>]
      CHF: float option
      [<property: JsonPropertyName("JPY")>]
      JPY: float option
      [<property: JsonPropertyName("GBP")>]
      GBP: float option
      [<property: JsonPropertyName("CAD")>]
      CAD: float option
      [<property: JsonPropertyName("RSD")>]
      RSD: float option }

type ExchangeRateResponse = { success: bool; rates: Rates }

let apiKey = "a9208706e2ff6ca440d02d5d4f5d4fd4"

let availableCurrencies = [ "USD"; "EUR"; "CHF"; "JPY"; "GBP"; "CAD"; "RSD" ]

let displayAvailableCurrencies () =
    Console.WriteLine("Verfügbare Währungen für die Umrechnung:")
    Console.WriteLine("---------------------------------------")

    for currency in availableCurrencies do
        Console.WriteLine(currency)

    Console.WriteLine("---------------------------------------\n")

let getExchangeRateForCurrency currency =
    let url = sprintf "http://api.exchangeratesapi.io/v1/latest?access_key=%s" apiKey
    let client = new HttpClient()
    let response = client.GetStringAsync(url).Result
    let data = JsonSerializer.Deserialize<ExchangeRateResponse>(response)

    if data.success then
        match currency with
        | "USD" -> data.rates.USD
        | "EUR" -> data.rates.EUR
        | "CHF" -> data.rates.CHF
        | "JPY" -> data.rates.JPY
        | "GBP" -> data.rates.GBP
        | "CAD" -> data.rates.CAD
        | "RSD" -> data.rates.RSD
        | _ -> None
    else
        None

let convertCurrency amount baseToEuroRate euroToTargetRate =
    match (baseToEuroRate, euroToTargetRate) with
    | (Some(bRate), Some(tRate)) ->
        let amountInEuros = amount / bRate
        let amountInTargetCurrency = amountInEuros * tRate
        Some amountInTargetCurrency
    | _ -> None

let displayExchangeRateForCurrency () =
    Console.WriteLine(
        "Bitte geben Sie die Währung ein, für die Sie den Wechselkurs zum Euro wissen möchten (z.B. USD):"
    )

    let currency = Console.ReadLine().ToUpper()

    match getExchangeRateForCurrency currency with
    | Some(rate) -> printfn "Der aktuelle Wechselkurs für 1 Euro zu %s ist: %f" currency rate
    | None -> printfn "Es gab ein Problem beim Abrufen des Wechselkurses oder die Währung existiert nicht."

let rec menu () =
    Console.WriteLine("\nWählen Sie eine Option:")
    Console.WriteLine("1. Währung umrechnen")
    Console.WriteLine("2. Wechselkurs abfragen")
    Console.WriteLine("3. Programm beenden")

    match Console.ReadLine() with
    | "1" ->
        displayAvailableCurrencies ()
        Console.WriteLine("Bitte geben Sie die Startwährung ein (z.B. USD):")
        let baseCurrency = Console.ReadLine().ToUpper()
        Console.WriteLine("Bitte geben Sie die Zielwährung ein (z.B. EUR):")
        let targetCurrency = Console.ReadLine().ToUpper()
        Console.WriteLine("Bitte geben Sie den Betrag ein:")
        let amount = Console.ReadLine() |> float
        let baseToEuroRate = getExchangeRateForCurrency baseCurrency
        let euroToTargetRate = getExchangeRateForCurrency targetCurrency
        let convertedAmountOption = convertCurrency amount baseToEuroRate euroToTargetRate

        match convertedAmountOption with
        | Some(convertedAmount) -> printfn "Ihr Betrag in %s ist: %.2f %s" targetCurrency convertedAmount targetCurrency
        | None -> printfn "Es gab ein Problem beim Abrufen des Wechselkurses."

        menu ()
    | "2" ->
        displayExchangeRateForCurrency ()
        menu ()
    | "3" ->
        printfn "Auf Wiedersehen!"
        exit 0
    | _ ->
        printfn "Ungültige Auswahl. Bitte erneut versuchen."
        menu ()

[<EntryPoint>]
let programStart args = menu ()
