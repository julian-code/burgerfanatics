using NetTopologySuite.Geometries;

namespace BurgerFanatics.Contracts;

public record LocationViewModel(Guid Id, Point Location, string Address);