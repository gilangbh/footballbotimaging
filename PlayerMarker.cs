using SixLabors.Shapes;

class PlayerMarker
{
    public EllipsePolygon Marker { get; set; }
    public Player Info { get; set; }
    public int posX { get; set; }
    public int posY { get; set; }
    public int posText { get; set; }
    public bool isSide
    {
        get
        {
            if (Info != null)
                return Info.Position.Equals("RB")
                    || Info.Position.Equals("LB")
                    || Info.Position.Equals("RM")
                    || Info.Position.Equals("LM")
                    || Info.Position.Equals("RF")
                    || Info.Position.Equals("LF");
            else return false;
        }
    }
}