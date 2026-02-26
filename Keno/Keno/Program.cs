using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class NapiKeno
{
    public int Ev { get; set; }
    public int Het { get; set; }
    public int Nap { get; set; }
    public string HuzasDatum { get; set; }
    public List<int> HuzottSzamok { get; set; }

    public NapiKeno(string sor)
    {
        string[] adatok = sor.Split(';');
        Ev = int.Parse(adatok[0]);
        Het = int.Parse(adatok[1]);
        Nap = int.Parse(adatok[2]);
        HuzasDatum = adatok[3];
        HuzottSzamok = new List<int>();

        for (int i = 4; i < adatok.Length; i++)
        {
            if (int.TryParse(adatok[i], out int szam))
            {
                HuzottSzamok.Add(szam);
            }
        }
    }

    public int TalalatSzam(List<int> tippek)
    {
        return tippek.Count(tipp => HuzottSzamok.Contains(tipp));
    }

    public bool Helyes
    {
        get { return HuzottSzamok.Count == 20 && HuzottSzamok.Distinct().Count() == 20; }
    }
}

public static class KenoSzamitas
{
    public static int Szorzo(NapiKeno keno, List<int> tippek)
    {
        Dictionary<string, int> nyeroParok = new Dictionary<string, int>()
        {
            {"10-10",1000000}, {"10-9",8000}, {"10-8",350}, {"10-7",30}, {"10-6",3}, {"10-5",1}, {"10-0",2},
            {"9-9",100000}, {"9-8",1200}, {"9-7",100}, {"9-6",12}, {"9-5",3}, {"9-0",1},
            {"8-8",20000}, {"8-7",350}, {"8-6",25}, {"8-5",5}, {"8-0",1},
            {"7-7",5000}, {"7-6",60}, {"7-5",6}, {"7-4",1}, {"7-0",1},
            {"6-6",500}, {"6-5",20}, {"6-4",3}, {"6-0",1},
            {"5-5",200}, {"5-4",10}, {"5-3",2},
            {"4-4",100}, {"4-3",2},
            {"3-3",15}, {"3-2",1},
            {"2-2",6},
            {"1-1",2}
        };

        int jatekTipus = tippek.Count;
        int talalatokSzama = keno.TalalatSzam(tippek);
        string kulcs = $"{jatekTipus}-{talalatokSzama}";

        if (nyeroParok.ContainsKey(kulcs))
            return nyeroParok[kulcs];
        else
            return 0;
    }
}


internal partial class Program
{
    static void Main()
    {
        // 6. feladat
        List<NapiKeno> huzasok = new List<NapiKeno>();
        StreamReader sr = new StreamReader("Huzasok.csv");
        sr.ReadLine();
        string sor;
        while ((sor = sr.ReadLine())!= null)
        {
            huzasok.Add(new NapiKeno(sor));
        }
       
        // 7. feladat
        int hibasAdatokSzama = huzasok.Count(h => !h.Helyes);
        Console.WriteLine($"7. feladat: Hibás adatok száma: {hibasAdatokSzama}");
        huzasok = huzasok.Where(h => h.Helyes).ToList();

        // 9. feladat
        Console.WriteLine("\n9. feladat: Utolsó napi nyeremény kiszámítása");
        Console.Write("Adja meg a tippjeit (1-10 szám, vesszővel elválasztva): ");
        List<int> tippek = Console.ReadLine().Split(',').Select(int.Parse).ToList();

        Console.Write("Adja meg a fogadási összegét (200-1000 Ft, 200 Ft lépésben): ");
        int fogadasOsszeg = int.Parse(Console.ReadLine());

        if (tippek.Count < 1 || tippek.Count > 10)
        {
            Console.WriteLine("Hibás tipp szám!");
            return;
        }

        if (fogadasOsszeg < 200 || fogadasOsszeg > 1000 || fogadasOsszeg % 200 != 0)
        {
            Console.WriteLine("Hibás fogadási összeg!");
            return;
        }
        int tetszoro = fogadasOsszeg / 200;
        NapiKeno utolsoNap = huzasok.Last();
        int szorzo = KenoSzamitas.Szorzo(utolsoNap, tippek);
        int nyeremeny = szorzo * tetszoro;

        Console.WriteLine($"Nyereménye: {nyeremeny} Ft");

        // 10. feladat
        Console.WriteLine("\n10. feladat: 2020-as nyeremények");
        List<int> ismerosTippek = new List<int> { 17, 28, 32, 44, 54, 63, 72, 75 };
        int tetszoro2020 = 4;
        int osszNyeremeny = 0;

        foreach (var napiKeno in huzasok.Where(h => h.Ev == 2020))
        {
            int szorzo2020 = KenoSzamitas.Szorzo(napiKeno, ismerosTippek);
            if (szorzo2020 > 0)
            {
                int nyeremeny2020 = szorzo2020 * tetszoro2020;
                Console.WriteLine($"{napiKeno.HuzasDatum}: {nyeremeny2020} Ft");
                osszNyeremeny += nyeremeny2020;
            }
        }
        Console.WriteLine($"Év végi eredménye: {osszNyeremeny} Ft");
    }
}
