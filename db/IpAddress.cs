public class IpAddress
{
    public string Address { get; set; }
    public bool Banned { get; set; }

    public override string ToString()
    {
        return Address;
    }
}