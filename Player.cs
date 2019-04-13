class Player
{
    public Player(string _name, string _pos)
    {
        this.Name = _name;
        this.Position = _pos;
    }
    public string Position { get; set; }
    public string Name { get; set; }
}