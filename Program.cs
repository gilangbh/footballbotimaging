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

            List<Tuple<string,string>> players = GetPlayers();
            List<string> rb = new List<string>();

            for(int i = 0; i < players.Count; i++)
            {
                var p = players[i];
                switch(p.Item2)
                {
                    case "RB": rb.Add(p.Item1); break;
                    case "LB": break;
                    case "CB": break;
                    case "RM": break;
                    case "LM": break;
                    case "DM": break;
                    case "CM": break;
                    case "AM": break;
                    case "RF": break;
                    case "LF": break;
                    case "CF": break;
                }
            }

            try
            {
                using (Image<Rgba32> image = new Image<Rgba32>(650, 650))
                {
                    var pol = new EllipsePolygon(new PointF(200, 200), 15);
                    var rectangleField = new RectangularPolygon(new PointF(0,64),new PointF(650,650));

                    var formationFont = SystemFonts.CreateFont("Trebuchet MS", _formationFontSize, FontStyle.Regular);
                    var teamFont = SystemFonts.CreateFont("Trebuchet MS", _teamNameFontSize, FontStyle.Regular);
                    var playerFont = SystemFonts.CreateFont("Trebuchet MS", _playerNameFontSize, FontStyle.Regular);

                    var textGraphicsOptions = new TextGraphicsOptions(true);
                    
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
                        .Fill(new Rgba32(237,237,237)) // white background image
                        .Fill(new GraphicsOptions(true),new Rgba32(158,209,171),rectangleField)
                        //.Draw(Rgba32.DarkGoldenrod, 3, pol)
                        
                        .Fill((GraphicsOptions)textGraphicsOptions, Rgba32.Black, teamGlyph)
                        .Fill((GraphicsOptions)textGraphicsOptions, Rgba32.Black, formationGlyph)
                        .Fill(new GraphicsOptions(true),new Rgba32(237,237,237),pol)
                        .Draw(new Rgba32(155,186,217),1,pol)
                        )
                        ;


                    
                    image.Save("output/wordart.png");
                }
            }
            finally
            {

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

        static List<Tuple<string, string>> GetPlayers()
        {
            List<Tuple<string, string>> players = new List<Tuple<string, string>>();
            players.Add(new Tuple<string, string>("Name 1", "RB"));
            players.Add(new Tuple<string, string>("Name 2", "CB"));
            players.Add(new Tuple<string, string>("Name 3", "CB"));
            players.Add(new Tuple<string, string>("Name 4", "LB"));
            players.Add(new Tuple<string, string>("Name 5", "RM"));
            players.Add(new Tuple<string, string>("Name 6", "LM"));
            players.Add(new Tuple<string, string>("Name 7", "CM"));
            players.Add(new Tuple<string, string>("Name 8", "CM"));
            players.Add(new Tuple<string, string>("Name 9", "CF"));
            players.Add(new Tuple<string, string>("Name 10", "CF"));
            return players;
        }

        private static List<EllipsePolygon> processRB(List<string> rbs)
        {
            return new List<EllipsePolygon>();
        }
    }
}
