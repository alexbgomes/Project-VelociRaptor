public class Drop {
    public float Chance { get; set; }
    public PickupType PickupType { get; set; }
    public int Value { get; set; }
    public float Duration { get; set; }
    private bool rotating = true;
    public bool Rotating { get { return rotating; } set { rotating = value; } }

    public override bool Equals(object obj) {
        var drop = obj as Drop;
        if (object.ReferenceEquals(drop, null)) {
            return false;
        }
        return Chance == drop.Chance
            && PickupType == drop.PickupType
            && Value == drop.Value
            && Duration == drop.Duration
            && Rotating == drop.Rotating;
    }

    public override int GetHashCode() {
        var hc = 0;
        var p = 23;
        hc = Chance.GetHashCode();
        hc = unchecked((hc * p) ^ PickupType.GetHashCode());
        hc = unchecked((hc * p) ^ Value.GetHashCode());
        hc = unchecked((hc * p) ^ Duration.GetHashCode());
        hc = unchecked((hc * p) ^ Rotating.GetHashCode());
        return hc;
    }

    public override string ToString() {
        return $"Drop [Type={PickupType}, Chance={Chance}, Value={Value}, Duration={Duration}, Rotating={Rotating}]";
    }
}