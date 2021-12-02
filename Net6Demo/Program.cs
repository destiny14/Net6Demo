// ReSharper disable All
var wasservoegel = new[]
{
    new Wasservogel("Ente", true, false, true, 1f),
    new Wasservogel("Pinguin", false, true, false, 999f),
    new Wasservogel("Möwe", false, false, true, 0.1f),
    new Wasservogel("Gans", true, true, true, 1f)
};

#region LINQ

#region TryGetNonEnumeratedCount

IEnumerable<string> entchen = new[] { "alle", " ", "meine", " ", "entchen" };
if (entchen.TryGetNonEnumeratedCount(out var count))
    Console.WriteLine(count); // Ausgabe: 5

#endregion

#region StuffBy

var niedlichkeiten = wasservoegel.DistinctBy(v => v.Niedlichkeit).Select(v => $"{v.Name}: {v.Niedlichkeit}");
Console.WriteLine(string.Join(",", niedlichkeiten));
// Ausgabe: Ente: 1, Pinguin: 999, Möwe: 0.1

var amNiedlichsten = wasservoegel.MaxBy(v => v.Niedlichkeit);
Console.WriteLine($"Der niedlichste Vogel: {amNiedlichsten?.Name}");
// Ausgabe: Der niedlichste Vogel: Pinguin

#endregion

#region Chunks

var pages = wasservoegel.Chunk(2).Select((items, index) => (items, index));
foreach (var page in pages)
{
    Console.WriteLine($"Page {page.index}");
    foreach (var item in page.items)
    {
        Console.WriteLine(item.Name);
    }
}

/* Ausgabe:
 * Page 0
 * Ente
 * Pinguin
 * Page 1
 * Möwe
 * Gans */

#endregion

#region OrDefault

var impossibleBird 
    = wasservoegel.FirstOrDefault(v => v.KannFliegen && v.Niedlichkeit > 9000,
    new Wasservogel("Überpinguin", true, true, true, float.PositiveInfinity));

#endregion

#region Zip

var wissenschaftlicheWahrheiten = new[]
{
    "Anatidae",
    "Spheniscidae",
    "Larinae",
    "Anserinae"
};

var wissenschaftlicheWasservoegel = wasservoegel.Zip(wissenschaftlicheWahrheiten, (first, second) => first + ": " + second);
foreach (var vogel in wissenschaftlicheWasservoegel)
{
    Console.WriteLine($"{vogel.First.Name}, {vogel.Second}");
}

/* Ausgabe:
 * Ente: Anatidae
 * Pinguin: Spheniscidae
 * Möwe: Larinae
 * Gans: Anserinae */

#endregion

#region Take mit Ranges

var bitteKeineGaense = wasservoegel.Take(..^1).Select(v => v.Name);
Console.WriteLine(string.Join(", ", bitteKeineGaense));
// Ausgabe: Ente, Pinguin, Möwe

#endregion

#endregion

#region Date and Time

DateOnly birthday = new(1995, 6, 14);

TimeOnly partyTime = new(13, 33, 37);
partyTime.AddHours(10, out var wrappedDays);
Console.WriteLine($"Vergangene Tage: {wrappedDays}");
// Ausgabe: Vergangene Tage: 1

#endregion

#region Kleinkram

// WaitAsync mit Timeout
Task VeryLongOperation() { return Task.Delay(100000); }
await VeryLongOperation().WaitAsync(TimeSpan.FromSeconds(1));

// ThrowIfNull
void Method(object arg)
{
    ArgumentNullException.ThrowIfNull(arg);
}

// Async Timer
PeriodicTimer timer = new(TimeSpan.FromSeconds(1));
while (await timer.WaitForNextTickAsync())
{
    Console.WriteLine("Plönk");
}

// Async foreach

var urlsToDownload = new[]
{
    "https://dotnet.microsoft.com",
    "https://www.microsoft.com",
    "https://empowersuite.com"
};

var client = new HttpClient();

await Parallel.ForEachAsync(urlsToDownload, async (url, token) =>
{
    var targetPath = Path.Combine(Path.GetTempPath(), "http_cache", url);

    HttpResponseMessage response = await client.GetAsync(url);

    if (response.IsSuccessStatusCode)
    {
        using FileStream target = File.OpenWrite(targetPath);

        await response.Content.CopyToAsync(target);
    }
});

#endregion

#region Static Abstracts in Interfaces


// Beispiel 1:
// Implementation statischer Methoden über ein Interface erzwingen
public interface IParseable<TSelf>
    where TSelf : IParseable<TSelf>
{
    static abstract TSelf Parse(string s, IFormatProvider? provider);

    static abstract bool TryParse(string? s, IFormatProvider? provider, out TSelf result);
}


// Beispiel 2:
// Einfaches Rechnen mit generischen Typen

// IAddable<T> ist eine Vereinfachung, mit .NET6 implementieren Typen
// wie int, long usw. das Interface "INumber<T>"

interface IAddable<TSelf> where TSelf : IAddable<TSelf>
{
    static abstract TSelf Zero { get; }
    static abstract TSelf operator +(TSelf a, TSelf b);
}

struct MyVeryOwnNumber : IAddable<MyVeryOwnNumber>
{
    public MyVeryOwnNumber(int number)
    {
        Number = number;
    }

    public static MyVeryOwnNumber Zero => new(0);

    public int Number { get; set; }

    public static MyVeryOwnNumber operator +(MyVeryOwnNumber a, MyVeryOwnNumber b)
    {
        return new(a.Number + b.Number);
    }
}

public class TheCalculator
{
    void DoTheCalculating()
    {
        MyVeryOwnNumber aGreatNumber = new(1337);
        MyVeryOwnNumber bGreatNumber = new(4242);
        MyVeryOwnNumber anEvenGreaterNumber = AddAll(aGreatNumber, bGreatNumber);

        int anotherNumbering = AddAll(1, 2, 3, 4); // Angenommen int würde IAddable implementieren
    }

    // ohne static abstract interface members: kompiliert nicht
    public T Add<T>(T left, T right) => left + right;

    // mit static abstract interface members: rechnen mit generics!
    T AddAll<T>(params T[] ts) where T : IAddable<T>
    {
        T result = T.Zero;
        foreach (var t in ts)
        {
            result += t;
        }
        return result;
    }

}

#endregion

public record Wasservogel(string Name, bool KannQuack, bool KannGack, bool KannFliegen, float Niedlichkeit);