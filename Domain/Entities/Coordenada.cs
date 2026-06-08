namespace OrbitGuardAI.Domain.Entities;

/// <summary>
/// Value Object imutável que representa uma coordenada geográfica (lat/long).
/// </summary>
public sealed class Coordenada
{
    public double Latitude { get; }
    public double Longitude { get; }

    public Coordenada(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude deve estar entre -90 e 90.");
        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude deve estar entre -180 e 180.");

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Calcula a distância em km até outra coordenada (fórmula de Haversine).
    /// </summary>
    public double DistanciaKm(Coordenada outra)
    {
        const double R = 6371.0;
        double dLat = ToRad(outra.Latitude - Latitude);
        double dLon = ToRad(outra.Longitude - Longitude);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRad(Latitude)) * Math.Cos(ToRad(outra.Latitude)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return 2 * R * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRad(double graus) => graus * Math.PI / 180.0;

    public override string ToString() => $"({Latitude:F4}, {Longitude:F4})";
}