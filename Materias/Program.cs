using System.Net;
using HtmlAgilityPack;

(string Materia, string Nrc)[] materias = 
{
    ("I5903", "103861"),
};

var webClient = new HttpClient();
int i = 0;
bool found = false;
while (true)
{
    foreach (var materia in materias)
    {
        var liga = $"http://consulta.siiau.udg.mx/wco/sspseca.consulta_oferta?ciclop=202410&cup=D&crsep={materia.Materia}";
        string webpage = webClient.GetStringAsync(liga).GetAwaiter().GetResult();

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(webpage);

        List<List<string>> table = htmlDocument.DocumentNode.SelectSingleNode("//table").Descendants("tr").Skip(2)
            .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList()).ToList();

        foreach (var list in table)
        {
            if (list.Count < 9 || list[0] != materia.Nrc)
                continue;
            if (int.TryParse(list[6], out int disp) && disp > 0)
            {
                Console.WriteLine(
                    $"Disponible para {list[0]}, materia {list[1]} {list[2]} con {list[8].Split('\n').Select(s => s.Trim()).ToArray()[1]}");
                found = true;
                break;
            }
        }
    }

    if (!found)
    {
        Console.CursorLeft = 0;
        Console.Write((i = (i + 1) % 3) switch
        {
            0 => '-',
            1 => '\\',
            2 => '|',
            3 => '/',
            _ => '?',
        });
        //Console.WriteLine($"Sin cupos para {materia.Nrc}, materia {materia.Materia} ({table.Count(t => t.Count == 9 && int.Parse(t[6]) > 0)}/{table.Count(t => t.Count == 9)} con cupo)");
    }
    found = false;
    Thread.Sleep(10);
}