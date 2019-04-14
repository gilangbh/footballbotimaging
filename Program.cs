using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        static List<PlayerMarker> playerMarkers;

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

                    RendererOptions teamOptions = new RendererOptions(formationFont, textGraphicsOptions.DpiX, textGraphicsOptions.DpiY)
                    {
                        HorizontalAlignment = textGraphicsOptions.HorizontalAlignment,
                        TabWidth = textGraphicsOptions.TabWidth,
                        VerticalAlignment = textGraphicsOptions.VerticalAlignment,
                        WrappingWidth = textGraphicsOptions.WrapTextWidth,
                        ApplyKerning = textGraphicsOptions.ApplyKerning
                    };

                    RendererOptions formationOptions = new RendererOptions(formationFont, textGraphicsOptions.DpiX, textGraphicsOptions.DpiY)
                    {
                        HorizontalAlignment = HorizontalAlignment.Right,
                        TabWidth = textGraphicsOptions.TabWidth,
                        VerticalAlignment = textGraphicsOptions.VerticalAlignment,
                        WrappingWidth = textGraphicsOptions.WrapTextWidth,
                        ApplyKerning = textGraphicsOptions.ApplyKerning
                    };
                    RendererOptions playerOptions = new RendererOptions(playerFont, textGraphicsOptions.DpiX, textGraphicsOptions.DpiY)
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TabWidth = textGraphicsOptions.TabWidth,
                        VerticalAlignment = textGraphicsOptions.VerticalAlignment,
                        WrappingWidth = textGraphicsOptions.WrapTextWidth,
                        ApplyKerning = textGraphicsOptions.ApplyKerning
                    };

                    var teamGlyph = TextBuilder.GenerateGlyphs(teamName, new PointF(20, 28), teamOptions);
                    var formationGlyph = TextBuilder.GenerateGlyphs(formation, new PointF(630, 28), formationOptions);
                    //var playerGlyph = TextBuilder.GenerateGlyphs(formation, new PointF(630, 28), playerOptions);

                    image.Mutate(ctx => ctx
                        .Fill(new Rgba32(237, 237, 237))
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
                    playerMarkers = new List<PlayerMarker>();

                    for (int i = 0; i < players.Count; i++)
                    {
                        Player p = players[i];

                        PlayerMarker pm = new PlayerMarker();
                        switch (p.Position)
                        {
                            case "LB": //.Add(p.Item1);
                                pm.posX = 130;
                                pm.posY = 148;
                                break;
                            case "RB":
                                pm.posX = 130;
                                pm.posY = 532;
                                break;
                            case "CB":
                                pm.posX = 130;
                                pm.posY = 340;
                                break;
                            case "RM":
                                pm.posX = 338;
                                pm.posY = 532;
                                break;
                            case "LM":
                                pm.posX = 338;
                                pm.posY = 148;
                                break;
                            case "DM":
                                pm.posX = 238;
                                pm.posY = 340;
                                break;
                            case "CM":
                                pm.posX = 338;
                                pm.posY = 340;
                                break;
                            case "AM":
                                pm.posX = 438;
                                pm.posY = 340;
                                break;
                            case "RF":
                                pm.posX = 538;
                                pm.posY = 532;
                                break;
                            case "LF":
                                pm.posX = 538;
                                pm.posY = 148;
                                break;
                            case "CF":
                                pm.posX = 538;
                                pm.posY = 340;
                                break;
                        }
                        pm.Info = p;
                        playerMarkers.Add(pm);
                    }

                    foreach (var pm in playerMarkers)
                    {
                        //var coordinate = GenerateCoordinate(pm);
                        //pm.posX = coordinate.Item1;
                        //pm.posY = coordinate.Item2;
                        pm.Marker = new EllipsePolygon(new PointF(pm.posX, pm.posY), 15);
                    }

                    foreach (var p in playerMarkers)
                    {
                        image.Mutate(ctx => ctx
                        .Fill(new GraphicsOptions(true), new Rgba32(237, 237, 237), p.Marker)
                        .Draw(new Rgba32(155, 186, 217), 1, p.Marker)
                        .Fill((GraphicsOptions)textGraphicsOptions, Rgba32.Black, TextBuilder.GenerateGlyphs(p.Info.Position, new PointF(p.posX, p.posY - 4), playerOptions))
                        .Fill((GraphicsOptions)textGraphicsOptions, Rgba32.Black, TextBuilder.GenerateGlyphs(p.Info.Name, new PointF(p.posX, p.posY + 20), playerOptions))
                        );
                    }

                    image.Save("output/wordart.png");
                }
            }
            finally
            {

            }
        }

        private static Tuple<int, int> GenerateCoordinate(PlayerMarker pm)
        {
            if (pm.isSide)
            {
                while (playerMarkers.Exists(x => x.posX == pm.posX))
                {
                    pm.posX += 15;
                }
            }
            else
            {

            }

            return new Tuple<int, int>(pm.posX, pm.posY);
        }
        private static void GetMidPositions(PlayerMarker pm)
        {
            List<PlayerMarker> playersinposition = playerMarkers.Where(x => x.Info.Position == pm.Info.Position).ToList();
            if (playersinposition.Count < 4)
            {
                foreach (var item in playersinposition)
                {
                    item.posY = 148 + (int)(532 - 148) * playersinposition.IndexOf(item) / (playersinposition.Count + 1);
                }
            }
            else
            {
                playersinposition[0].posY = 148 + (int)((532 - 148) / 4);
                playersinposition[1].posY = 148 + (int)((532 - 148) * 2 / 4);
                playersinposition[2].posY = 148 + (int)((532 - 148) * 3 / 4);
                for (int i = 3; i < playersinposition.Count; i++)
                {
                    playersinposition[i].posY = 148 + (int)((532 - 148) * 2 / 4);
                    playersinposition[i].posX = playersinposition[1].posX + (15 * (i - 2));
                }
            }
            for (int i = 0; i < playersinposition.Count; i++)
            {

            }
        }
        private static List<EllipsePolygon> GetMidPositions(int poscount, string _posX)
        {
            int posX = 0;
            switch (_posX)
            {
                case "CB":
                    posX = 130;
                    break;
                case "DM":
                    posX = 234;
                    break;
                case "CM":
                    posX = 338;
                    break;
                case "AM":
                    posX = 438;
                    break;
                default:
                    posX = 538;
                    break;
            }

            List<int> posY = new List<int>();
            if (poscount < 4)
            {
                switch (poscount)
                {
                    case 1:
                        posY.Add(148 + (int)((532 - 148) / 2));
                        break;
                    case 2:
                        posY.Add(148 + (int)((532 - 148) / 3));
                        posY.Add(148 + (int)((532 - 148) * 2 / 3));
                        break;
                    case 3:
                        posY.Add(148 + (int)((532 - 148) / 4));
                        posY.Add(148 + (int)((532 - 148) * 2 / 4));
                        posY.Add(148 + (int)((532 - 148) * 3 / 4));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                posY.Add(148 + (int)((532 - 148) / 4));
                posY.Add(148 + (int)((532 - 148) * 2 / 4));
                posY.Add(148 + (int)((532 - 148) * 3 / 4));
                for (int i = 0; i < poscount - 3; i++)
                {
                    posY.Add(148 + (int)((532 - 148) * 2 / 4));
                }
            }
            List<EllipsePolygon> ellipses = new List<EllipsePolygon>();
            int extraCount = 0;

            for (int i = 0; i < posY.Count; i++)
            {
                if (posY[i] == 340)
                {
                    ellipses.Add(new EllipsePolygon(new PointF(posX + (15 * extraCount), posY[i]), 15));
                    extraCount++;
                }
                else
                {
                    ellipses.Add(new EllipsePolygon(new PointF(posX, posY[i]), 15));
                }
            }
            return ellipses;
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
            players.Add(new Player("Gilang Bhagaskara", "RB"));
            players.Add(new Player("Raufan Multahada", "LB"));
            players.Add(new Player("Ken Danniswara", "LB"));
            players.Add(new Player("Julian Plufer", "CB"));
            players.Add(new Player("David Chen", "RM"));
            players.Add(new Player("Agus Triutomo", "LM"));
            players.Add(new Player("Michael Jack", "DM"));
            players.Add(new Player("Dheta Good", "AM"));
            players.Add(new Player("Luthfi Aprianto", "CF"));
            players.Add(new Player("Pena Persada", "CF"));
            return players;
        }

        private static List<EllipsePolygon> processRB(List<string> rbs)
        {
            return new List<EllipsePolygon>();
        }
    }
}
