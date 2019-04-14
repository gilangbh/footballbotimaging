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
        private static readonly int _baseXb = 138;
        private static readonly int _baseXdm = 238;
        private static readonly int _baseXm = 338;
        private static readonly int _baseXam = 438;
        private static readonly int _baseXf = 538;
        private static readonly int _baseYL = 148;
        private static readonly int _baseYR = 532;
        static Dictionary<string, List<PlayerMarker>> playerMarkers;

        static void Main(string[] args)
        {
            string teamName = "Benfica FC";
            string formation = "4-3-1-2";

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

                    List<Player> players = GetPlayers();
                    playerMarkers = new Dictionary<string, List<PlayerMarker>>();

                    foreach (Player p in players)
                    {
                        PlayerMarker marker = new PlayerMarker() { Info = p };
                        if (!playerMarkers.ContainsKey(p.Position))
                        {
                            List<PlayerMarker> playerInPosition = new List<PlayerMarker>();
                            playerInPosition.Add(marker);
                            playerMarkers.Add(p.Position, playerInPosition);
                        }
                        else
                        {
                            playerMarkers[p.Position].Add(marker);
                        }
                    }

                    ProcessPosition("LB", _baseXb, _baseYL, true);
                    ProcessPosition("RB", _baseXb, _baseYR, true);
                    ProcessPosition("CB", _baseXb, 0, false);
                    ProcessPosition("DM", _baseXdm, 0, false);
                    ProcessPosition("LM", _baseXm, _baseYL, true);
                    ProcessPosition("RM", _baseXm, _baseYR, true);
                    ProcessPosition("CM", _baseXm, 0, false);
                    ProcessPosition("AM", _baseXam, 0, false);
                    ProcessPosition("LF", _baseXf, _baseYL, true);
                    ProcessPosition("RF", _baseXf, _baseYR, true);
                    ProcessPosition("CF", _baseXf, 0, false);

                    Console.WriteLine(JsonConvert.SerializeObject(playerMarkers));
                    List<PlayerMarker> finalPositions = new List<PlayerMarker>();

                    foreach (var i in playerMarkers.Values)
                    {
                        foreach (var p in i)
                        {
                            p.Marker = new EllipsePolygon(new PointF(p.posX, p.posY), 15);
                            finalPositions.Add(p);
                        }
                    }

                    foreach (var p in finalPositions)
                    {
                        image.Mutate(ctx => ctx
                        .Fill(new GraphicsOptions(true), new Rgba32(237, 237, 237), p.Marker)
                        .Draw(new Rgba32(155, 186, 217), 1, p.Marker)
                        .Fill((GraphicsOptions)textGraphicsOptions, Rgba32.Black, TextBuilder.GenerateGlyphs(p.Info.Position, new PointF(p.posX, p.posY - 4), playerOptions))
                        .Fill((GraphicsOptions)textGraphicsOptions, Rgba32.Black, TextBuilder.GenerateGlyphs(p.Info.Name, new PointF(p.posX, p.posText), playerOptions))
                        );
                    }

                    image.Save("output/wordart.png");
                }
            }
            finally
            {

            }
        }

        private static void ProcessPosition(string pos, int basePosX, int basePosY, bool side)
        {
            if (playerMarkers.ContainsKey(pos)) //make sure position is not empty
            {
                //get all players on this position
                List<PlayerMarker> markers = playerMarkers[pos];
                //count them
                int totalPlayersOnThisSide = markers.Count;

                //if it is side area
                if (side)
                {
                    //then just add 15 per players
                    for (int i = 0; i < totalPlayersOnThisSide; i++)
                    {
                        markers[i].posX = basePosX + (i * 15);
                        markers[i].posY = basePosY;
                        markers[i].posText = markers[i].posY + 20 + (i * 15);
                    }
                }
                else
                {
                    basePosY = _baseYL;
                    List<int> positions = new List<int>();
                    if (totalPlayersOnThisSide < 4)
                    {
                        for (int i = 0; i < totalPlayersOnThisSide; i++)
                        {
                            int arrangedPosition = (_baseYR + _baseYL) * (i + 1) / (totalPlayersOnThisSide + 1);
                            markers[i].posX = basePosX;
                            markers[i].posY = arrangedPosition;
                            markers[i].posText = markers[i].posY + 20;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            int arrangedPosition = (_baseYR - _baseYL) * (i + 1) / 4;
                            markers[i].posX = basePosX;
                            markers[i].posY = basePosY + arrangedPosition;
                            markers[i].posText = markers[i].posY + 20;
                        }
                        for (int i = 0; i < totalPlayersOnThisSide - 3; i++)
                        {
                            markers[i + 3].posX = basePosX + (15 * (1 + i));
                            markers[i + 3].posY = basePosY + (int)((_baseYR - _baseYL) / 2);
                            markers[i + 3].posText = markers[i + 3].posY + 20 + ((i + 1) * 15);
                        }
                    }
                }
            }
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
            players.Add(new Player("Gilang Bhagaskara", "CB"));
            players.Add(new Player("Raufan Multahada", "CB"));
            players.Add(new Player("Julian Assange", "RB"));
            players.Add(new Player("David Chen", "LB"));
            players.Add(new Player("Agus Triutomo", "LM"));
            players.Add(new Player("Michael Jack", "DM"));
            players.Add(new Player("Dheta Goodwill", "CM"));
            players.Add(new Player("Luthfi Aprianto", "RM"));
            players.Add(new Player("Pena Persada", "CF"));
            players.Add(new Player("Adit Kinarang", "CF"));
            return players;
        }
    }
}
