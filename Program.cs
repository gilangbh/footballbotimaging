using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;

namespace imaging
{
    class Program
    {
        private static readonly float _formationFontSize = 15.6f;
        private static readonly float _teamNameFontSize = 15.6f;
        private static readonly float _playerNameFontSize = 13f;

        static void Main(string[] args)
        {
            string teamName = "Benfica FC";
            string formation = "4-3-1-2";

            List<Player> players = GetPlayers();
            Dictionary<string, int> posCount = new Dictionary<string, int>();

            try
            {
                using (Image<Rgba32> image = new Image<Rgba32>(650, 650))
                {
                    List<EllipsePolygon> pols = new List<EllipsePolygon>();
                    EllipsePolygon playermarker = null;

                    for (int i = 0; i < players.Count; i++)
                    {
                        var p = players[i];
                        int pos = posCount.GetValueOrDefault(p.Position);

                        if (!posCount.ContainsKey(p.Position))
                        {
                            posCount.Add(p.Position, 0);
                        }
                        posCount[p.Position]++;
                        switch (p.Position)
                        {
                            case "RB": //.Add(p.Item1);
                                playermarker = new EllipsePolygon(new PointF(130 + (pos * 15), 148), 15);
                                break;
                            case "LB":
                                playermarker = new EllipsePolygon(new PointF(130 + (pos * 15), 532), 15);
                                break;
                            case "RM":
                                playermarker = new EllipsePolygon(new PointF(338 + (pos * 15), 532), 15);
                                break;
                            case "LM":
                                playermarker = new EllipsePolygon(new PointF(338 + (pos * 15), 148), 15);
                                break;
                            case "RF":
                                playermarker = new EllipsePolygon(new PointF(445 + (pos * 15), 532), 15);
                                break;
                            case "LF":
                                playermarker = new EllipsePolygon(new PointF(445 + (pos * 15), 148), 15);
                                break;
                            default:
                                break;
                        }
                        pols.Add(playermarker);
                    }

                    List<int> cbPos = GetPosY(posCount["CB"]);
                    List<int> dmPos = GetPosY(posCount["DM"]);
                    List<int> cmPos = GetPosY(posCount["CM"]);
                    List<int> amPos = GetPosY(posCount["AM"]);
                    List<int> cfPos = GetPosY(posCount["CF"]);

                    var field = new RectangularPolygon(new PointF(0, 64), new PointF(650, 650));
                    var penaltybox = new RectangularPolygon(new PointF(0, 160), new PointF(155, 544));
                    var gkbox = new RectangularPolygon(new PointF(0, 275), new PointF(55, 430));
                    var midLine = new RectangularPolygon(new PointF(500, 64), new PointF(660, 660));
                    var midCircle = new EllipsePolygon(new PointF(500, 353), 90);
                    var penaltySpot = new EllipsePolygon(new PointF(100, 352), 2);

                    var arc = new Polygon(new CubicBezierLineSegment(new PointF[] {
                            new PointF(155,280),
                            new PointF(210,300),
                            new PointF(210,405),
                            new PointF(155,425)
                        }));

                    var textGraphicsOptions = new TextGraphicsOptions(true);

                    var formationFont = SystemFonts.CreateFont("Trebuchet MS", _formationFontSize, FontStyle.Regular);
                    var teamFont = SystemFonts.CreateFont("Trebuchet MS", _teamNameFontSize, FontStyle.Regular);
                    var playerFont = SystemFonts.CreateFont("Trebuchet MS", _playerNameFontSize, FontStyle.Regular);

                    var teamGlyph = SixLabors.Shapes.TextBuilder.GenerateGlyphs(teamName, new PointF(20, 28), new RendererOptions(teamFont, textGraphicsOptions.DpiX, textGraphicsOptions.DpiY)
                    {
                        HorizontalAlignment = textGraphicsOptions.HorizontalAlignment,
                        TabWidth = textGraphicsOptions.TabWidth,
                        VerticalAlignment = textGraphicsOptions.VerticalAlignment,
                        WrappingWidth = textGraphicsOptions.WrapTextWidth,
                        ApplyKerning = textGraphicsOptions.ApplyKerning
                    });

                    var formationGlyph = SixLabors.Shapes.TextBuilder.GenerateGlyphs(formation, new PointF(630, 28), new RendererOptions(teamFont, textGraphicsOptions.DpiX, textGraphicsOptions.DpiY)
                    {
                        HorizontalAlignment = HorizontalAlignment.Right,
                        TabWidth = textGraphicsOptions.TabWidth,
                        VerticalAlignment = textGraphicsOptions.VerticalAlignment,
                        WrappingWidth = textGraphicsOptions.WrapTextWidth,
                        ApplyKerning = textGraphicsOptions.ApplyKerning
                    });

                    image.Mutate(ctx => ctx
                        .Fill(new Rgba32(237, 237, 237)) // white background image
                        .Fill((GraphicsOptions)textGraphicsOptions, Rgba32.Black, teamGlyph)
                        .Fill((GraphicsOptions)textGraphicsOptions, Rgba32.Black, formationGlyph)
                        .Fill(new GraphicsOptions(true), new Rgba32(158, 209, 171), field)
                        .Draw(new Rgba32(180, 230, 193), 3, arc)
                        .Draw(new Rgba32(180, 230, 193), 3, penaltybox)
                        .Draw(new Rgba32(180, 230, 193), 3, gkbox)
                        .Draw(new Rgba32(180, 230, 193), 3, midLine)
                        .Draw(new Rgba32(180, 230, 193), 3, midCircle)
                        .Draw(new Rgba32(180, 230, 193), 3, penaltySpot)
                        );

                    foreach (var p in pols)
                    {
                        image.Mutate(ctx => ctx
                        .Fill(new GraphicsOptions(true), new Rgba32(237, 237, 237), p)
                        .Draw(new Rgba32(155, 186, 217), 1, p));
                    }

                    image.Save("output/wordart.png");
                }
            }
            finally
            {

            }
        }

        private static List<int> GetPosY(int poscount)
        {
            List<int> positions = new List<int>();
            if (poscount < 4)
            {
                switch (poscount)
                {
                    case 1:
                        positions.Add(148 + (int)((532 - 148) / 2));
                        break;
                    case 2:
                        positions.Add(148 + (int)((532 + 148) / 3));
                        positions.Add(148 + (int)((532 + 148) * 2 / 3));
                        break;
                    case 3:
                        positions.Add(148 + (int)((532 + 148) / 4));
                        positions.Add(148 + (int)((532 + 148) * 2 / 4));
                        positions.Add(148 + (int)((532 + 148) * 3 / 4));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                positions.Add(148 + (int)((532 + 148) / 4));
                positions.Add(148 + (int)((532 + 148) * 2 / 4));
                positions.Add(148 + (int)((532 + 148) * 3 / 4));
                for (int i = 0; i < poscount - 3; i++)
                {
                    positions.Add(148 + (int)((532 + 148) * 2 / 4));
                }
            }
            return positions;
        }

        static byte[] ImageToByteArray(Image<Rgba32> imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.SaveAsPng(ms);
                return ms.ToArray();
            }
        }

        static List<Player> GetPlayers()
        {
            List<Player> players = new List<Player>();
            players.Add(new Player("Name 1", "RB"));
            players.Add(new Player("Name 2", "CB"));
            players.Add(new Player("Name 3", "CB"));
            players.Add(new Player("Name 4", "LB"));
            players.Add(new Player("Name 5", "RM"));
            players.Add(new Player("Name 6", "LM"));
            players.Add(new Player("Name 7", "CM"));
            players.Add(new Player("Name 8", "CM"));
            players.Add(new Player("Name 9", "CF"));
            players.Add(new Player("Name 10", "CF"));
            return players;
        }

        private static List<EllipsePolygon> processRB(List<string> rbs)
        {
            return new List<EllipsePolygon>();
        }
    }
}
