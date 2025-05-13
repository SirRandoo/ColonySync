using ColonySync.Common.Enums;

namespace ColonySync.Common.Interfaces;

// TODO: This class uses an enum to describe assignable entity types. Maybe a generic of IIdentifiable should be considered.

// TODO: If a generic of IIdentifiable is used, an "entity wrapper" will have to be used to support in-game entities.
// TODO: This class stores a reference to a viewer's data. This should be simplified into a mini type that only stores the viewer's id and the platform they're on.
// TODO: Alternatively, a generic of Pawn could be used instead of IIdentifiable since this only pawns can ever be assigned.

// TODO: Re-evaluate this interface when the flat buffer contracts are fleshed out.

/// <summary>Interface that defines an entity capable of being assigned to a viewer.</summary>
public interface IAssignableEntity
{
    /// <summary>Gets or sets the user assigned to this entity.</summary>
    /// <value>An <see cref="IUser" /> instance representing the assigned user.</value>
    IUser AssignedUser { get; set; }

    /// <summary>Gets or sets the preview image associated with this entity.</summary>
    /// <value>A byte array representing the preview image.</value>
    byte[] PreviewImage { get; set; }

    /// <summary>Gets or sets the type of the assignable entity.</summary>
    /// <value>An <see cref="AssignableEntityType" /> enum value representing the type of the entity.</value>
    AssignableEntityType Type { get; set; }
}