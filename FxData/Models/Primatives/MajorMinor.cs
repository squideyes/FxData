namespace SquidEyes.FxData;

public struct MajorMinor : IEquatable<MajorMinor>
{
    public MajorMinor(byte major, byte minor)
    {
        Major = major;
        Minor = minor;
    }

    public MajorMinor(int major, int minor)
    {
        if (major < 0 || major > 255)
            throw new ArgumentOutOfRangeException(nameof(major));

        if (minor < 0 || minor > 255)
            throw new ArgumentOutOfRangeException(nameof(minor));

        Major = (byte)major;
        Minor = (byte)minor;
    }

    public byte Major { get; }
    public byte Minor { get; }

    public override string ToString() => $"{Major}.{Minor}";

    public void Write(BinaryWriter writer)
    {
        writer.Write(Major);
        writer.Write(Minor);
    }

    public static MajorMinor Read(BinaryReader reader) =>
        new(reader.ReadByte(), reader.ReadByte());

    public static bool TryParse(string? value, out MajorMinor majorMinor)
    {
        majorMinor = default;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var fields = value.Split('.');

        if (fields.Length > 2)
            return false;

        if (!byte.TryParse(fields[0], out byte major))
            return false;

        byte minor = 0;

        if (fields.Length == 2 && !byte.TryParse(fields[1], out minor))
            return false;

        majorMinor = new MajorMinor(major, minor);

        return true;
    }

    public static MajorMinor Parse(string? value)
    {
        if (!TryParse(value, out MajorMinor majorMinor))
            throw new ArgumentOutOfRangeException(nameof(value));

        return majorMinor;
    }

    public static implicit operator MajorMinor(string? value) => Parse(value);

    public bool Equals(MajorMinor other) =>
        Major == other.Major && Minor == other.Minor;

    public override bool Equals(object? other)
    {
        if (other == null || other.GetType() != typeof(MajorMinor))
            return false;

        return Equals((MajorMinor)other);
    }

    public override int GetHashCode() => HashCode.Combine(Major, Minor);

    public static implicit operator MajorMinor(double value)
    {
        int major = (int)value;

        var minor = (int)(Math.Round(value - major, 3) * 1000.0);

        while (minor > 0 && (minor % 10) == 0)
            minor /= 10;

        return new MajorMinor(major, minor);
    }

    public static bool operator ==(MajorMinor left, MajorMinor right) =>
        left.Equals(right);

    public static bool operator !=(MajorMinor left, MajorMinor right) =>
        !(left == right);
}